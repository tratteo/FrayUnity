using Fray.FX;
using Fray.Systems;
using Fray.Systems.Weapons;
using GibFrame;
using GibFrame.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Validatox.Meta;

namespace Fray.Weapons
{
    /// <summary>
    ///   Sword implementation of a <see cref="Weapon"/>
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Sword : Weapon, IBlocker
    {
        private readonly VfxHandler.Options parryVfxOptions = new VfxHandler.Options() { spatial = new VfxHandler.Options.Spatial() { simulationSpace = ParticleSystemSimulationSpace.World } };

        [Header("Attacks")]
        [SerializeField, Guard] private SwordAttack baseAttack;
        [Header("Parry")]
        [SerializeField, Guard] private Parry parry;
        private float parryDurationTimer;
        private float parryCooldownTimer;
        [Header("Parameters")]
        [SerializeField] private float baseDamage = 50F;
        [Header("FX")]
        [Header("Visual")]
        [SerializeField] private Optional<VfxHandler> chargeParryVfx;
        [SerializeField] private Optional<VfxHandler> attackVfx;
        [SerializeField] private Optional<VfxHandler> chargeVfx;
        [SerializeField] private Optional<VfxHandler> parryVfx;
        [Header("Sound")]
        [SerializeField] private Optional<SfxHandler> hitSfx;
        [SerializeField] private Optional<SfxHandler> missSfx;
        [SerializeField] private Optional<SfxHandler> parrySfx;
        [SerializeField, Guard] private VfxSocket vfxSocket;
        [Header("Collider")]
        [SerializeField] private int colliderQuality = 5;

        private PolygonCollider2D castCollider;
        private StaminaSystem parentStamina;
        private RaycastHit2D[] colliderCastBuffer;
        private Vector2[] colliderPoints;
        private float attackDurationTimer = 0;

        public float BaseDamage => baseDamage;

        public bool IsPreparingAttack { get; private set; }

        public bool IsParrying { get; private set; }

        public SwordAttack CachedAttack { get; private set; }

        public event Action<SwordAttack> AttackPerformed = delegate { };

        public event Action<SwordAttack> GotParried = delegate { };

        public event Action Parried = delegate { };

        public event Action<IReadOnlyCollection<GameObject>> Hit = delegate { };

        public override void Update()
        {
            base.Update();
            if (!IsPreparingAttack)
            {
                chargeVfx.Try(v => v.Stop(true));
            }
            else if (chargeVfx.TryGet(v => v.IsPlaying, out var playing) && !playing && CachedAttack.ShowVisualFx)
            {
                chargeVfx.Try(v => v.Display(this));
            }
            if (parryCooldownTimer > 0)
            {
                parryCooldownTimer -= Time.deltaTime;
                parryCooldownTimer = parryCooldownTimer < 0 ? 0 : parryCooldownTimer;
            }

            if (attackDurationTimer > 0)
            {
                attackDurationTimer -= Time.deltaTime;
                if (attackDurationTimer <= 0)
                {
                    attackDurationTimer = 0;
                    if (IsPreparingAttack)
                        PerformAttack();
                }
            }

            if (parryDurationTimer > 0)
            {
                parryDurationTimer -= Time.deltaTime;
                if (parryDurationTimer <= 0)
                {
                    parryDurationTimer = 0;
                    if (IsParrying)
                        DeactivateParry(false);
                }
            }
        }

        public override void SetOwner(GameObject owner)
        {
            base.SetOwner(owner);
            parentStamina = owner.GetComponent<StaminaSystem>();
        }

        public VfxSocket GetVfxSocket() => vfxSocket;

        public void Blocked()
        {
            DeactivateParry(true);
            parentStamina?.Increase(parry.Stamina * parry.StaminaRecoverRatio);
        }

        public bool IsBlocking() => IsParrying;

        public void EnableParry()
        {
            if (!CanParry()) return;
            parentStamina?.Decrease(parry.Stamina);

            parryDurationTimer = parry.Duration;
            IsParrying = true;

            parryDurationTimer = parry.Duration;
            IsParrying = true;
            chargeParryVfx.Try(v => v.Display(this, VfxHandler.Options.ForSocket(GetVfxSocket())));
        }

        public float GetDamage() => DamageMultiplier * baseDamage;

        protected override void TriggerBehaviour(params object[] args)
        {
            var attack = baseAttack;
            if (parentStamina != null)
            {
                if (parentStamina.Value < attack.StaminaRequired) return;
                parentStamina.Decrease(attack.StaminaRequired, gameObject);
            }

            chargeVfx.Try(v => v.Stop(true));
            IsPreparingAttack = true;
            CachedAttack = attack;
            attackDurationTimer = attack.Duration;
            if (attackDurationTimer <= 0)
            {
                attackDurationTimer = 0;
                PerformAttack();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            colliderCastBuffer = new RaycastHit2D[8];
            castCollider = GetComponent<PolygonCollider2D>();
            colliderPoints = new Vector2[colliderQuality + 1];
        }

        protected LayerMask GetActionLayerMask() => ~LayerMask.GetMask(Layers.ProjectileOf(LayerMask.LayerToName(OwnerObj.layer)));

        protected override bool CanAttack(params object[] args) => base.CanAttack(args) && !IsPreparingAttack && !IsParrying;

        private void PerformAttack()
        {
            ImpulseSource?.GenerateImpulse(new Vector3(transform.localScale.x, 0, 0) * GetDamage() / 50);
            var (hits, parried, dir) = CastCollider(CachedAttack);
            IsPreparingAttack = false;
            if (!parried) Hit?.Invoke(hits);
            if (CachedAttack.PlaySoundFx && !parried)
            {
                if (hits.Count > 0)
                    hitSfx.Try(v => v.Play(this));
                else
                    missSfx.Try(v => v.Play(this));
            }

            // Vfx
            //var target = GetTarget();
            //Vector2 dir = target ?
            //     target.position - OwnerObj.transform.position :
            //     Mathf.Sign(OwnerObj.transform.localScale.x) * OwnerObj.transform.localScale.x * OwnerObj.transform.right;

            if (CachedAttack.ShowVisualFx && !parried && attackVfx.TryGet(p => p, out var fx))
            {
                var opt = VfxHandler.Options.ForSwordAttack(CachedAttack, transform.position, dir);
                var clamped = Math.Clamp(DamageMultiplier, 1F, 3F);
                var magnitude = GibMath.Map(clamped, (1F, 3F), (0F, 1F));
                opt.appearance.color = fx.GetPrefabSystem().main.startColor.color.Redify(magnitude);
                fx.Display(this, opt);
            }

            AttackPerformed?.Invoke(CachedAttack);
            AttackRotate();
        }

        //public override List<StatisticProvider> GetStatisticProviders()
        //{
        //    var baseStats = base.GetStatisticProviders();
        //    baseStats.Add(new StatisticProvider("Damage", () => baseDamage));
        //    baseStats.Add(new StatisticProvider("Stamina usage", () => baseAttack.StaminaRequired));
        //    baseStats.Add(new StatisticProvider("Range", () => baseAttack.Range));
        //    return baseStats;
        //}

        private void Start()
        {
            chargeParryVfx.Try(v => v.Cache(this));
            attackVfx.Try(v => v.Cache(this));
            chargeVfx.Try(v => v.Cache(this));
            parryVfx.Try(v => v.Cache(this));

            hitSfx.Try(v => v.Cache(this));
            missSfx.Try(v => v.Cache(this));
            parrySfx.Try(v => v.Cache(this));
        }

        private void OnDestroy()
        {
            chargeParryVfx.Try(v => v.Dispose());
            attackVfx.Try(v => v.Dispose());
            chargeVfx.Try(v => v.Dispose());
            parryVfx.Try(v => v.Dispose());

            hitSfx.Try(v => v.Dispose());
            missSfx.Try(v => v.Dispose());
            parrySfx.Try(v => v.Dispose());
        }

        #region Parry

        private void DeactivateParry(bool parried)
        {
            if (!IsParrying) return;
            parryDurationTimer = 0F;
            parryCooldownTimer = parry.Cooldown;
            IsParrying = false;
            chargeParryVfx.Try(v => v.Stop(true));
            if (parried)
            {
                Parried?.Invoke();
                AttackRotate();
                parryVfx.Try(v => v.Display(this, parryVfxOptions));
                parrySfx.Try(v => v.Play(this));
            }
        }

        private bool CanParry() => !IsParrying && !IsPreparingAttack && parryCooldownTimer <= 0F;

        #endregion Parry

        #region Attack

        private void AttackRotate()
        {
            rendererFlipX *= -1;
            offsetRotation = rendererFlipX > 0 ? 0F : -(IdleRotation * 2F + 180F);
        }

        private (List<GameObject>, bool, Vector2) CastCollider(SwordAttack cachedAttack)
        {
            var target = GetTarget();
            bool hasTarget = target;
            var blocked = false;
            var angle = cachedAttack.ColliderAngle;
            castCollider.offset = cachedAttack.Offset;
            if (colliderPoints.Length != colliderQuality + 1)
            {
                colliderPoints = new Vector2[colliderQuality + 1];
            }
            colliderPoints[0] = Vector3.zero;
            var dir = hasTarget
                ? (target.position - OwnerObj.transform.position).normalized * cachedAttack.Range
                : cachedAttack.Range * Mathf.Sign(OwnerObj.transform.localScale.x) * OwnerObj.transform.right;
            var axis = dir;
            var stride = -angle / (colliderQuality - 1);
            axis = Quaternion.AngleAxis(angle / 2F, Vector3.forward) * axis;
            var scaleSign = GetOwnerScaleSign();
            for (var i = 1; i < colliderQuality + 1; i++)
            {
                colliderPoints[i] = axis;
                if (hasTarget)
                {
                    colliderPoints[i].x *= scaleSign;
                }
                axis = Quaternion.AngleAxis(stride, Vector3.forward) * axis;
            }
            castCollider.SetPath(0, colliderPoints);

            castCollider.enabled = true;
            var filter = new ContactFilter2D();
            filter.SetLayerMask(GetActionLayerMask());
            var amount = castCollider.Cast(Vector2.right, filter, colliderCastBuffer, 0F);
            castCollider.enabled = false;
            var hitsBuffer = new List<GameObject>();
            hitsBuffer.Clear();
            var damage = GetDamage() * cachedAttack.DamageMultiplier;
            for (var i = 0; i < amount; i++)
            {
                var hit = colliderCastBuffer[i].collider;
                if (hit.gameObject.Equals(gameObject)) continue;

                if (cachedAttack.CanBeParried)
                {
                    var blocker = hit.gameObject.GetComponentInChildren<IBlocker>();
                    if (blocker is not null && blocker.IsBlocking())
                    {
                        GotParried?.Invoke(CachedAttack);
                        parentStamina?.Decrease(cachedAttack.StaminaRequired);
                        // Notify the blocker that has blocked an attack
                        blocker.Blocked();
                        blocked = true;
                    }
                }
                if (!blocked)
                {
                    if (hit.gameObject.TryGetComponent<HealthSystem>(out var health))
                    {
                        hitsBuffer.Add(hit.gameObject);
                        health.Decrease(damage, gameObject);
                    }
                }
            }
            return (hitsBuffer, blocked, dir);
        }

        #endregion Attack
    }
}