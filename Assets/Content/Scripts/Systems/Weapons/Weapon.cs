using Cinemachine;
using GibFrame;
using System;
using UnityEngine;
using Validatox.Meta;

namespace Fray.Systems.Weapons
{
    /// <summary>
    ///   <para> Base class for a weapon </para>
    ///   This class manages the attachment of the weapon object to the owner through the <see cref="Applicator"/> module by implementing
    ///   the <see cref="IApplicable{T}"/> interface.
    /// </summary>
    public abstract class Weapon : MonoBehaviour, ITargetOwner, IDescriptable, IOwnable, ICooldownOwner, IAttackSpeedOwner, IApplicable<Weapon>
    {
        protected float rendererFlipX = 1F;
        protected float offsetRotation;
        private readonly Multiplier damageMultiplier = new Multiplier();
        [SerializeField, Guard] private Descriptor descriptor;
        [SerializeField] private float triggerCooldown;
        [Header("Transform")]
        [Range(-90F, 90F)][SerializeField] private float idleRotation;
        [SerializeField] private bool keepWeaponTransformUpVector = false;
        [Header("References")]
        [SerializeField, Guard] private Transform weaponTransform;
        [SerializeField, Guard] private Transform spriteTransform;
        private CinemachineImpulseSource cinemachineImpulseSource;
        private float currentAttackCooldown;
        private Transform target;
        private float attackTimer = 0F;

        private float targetRotation;

        public bool Enabled { get; set; } = true;

        public float TriggerCooldown => triggerCooldown;

        public float DamageMultiplier => damageMultiplier;

        public Multiplier AttackSpeedMultiplier { get; private set; } = new Multiplier();

        protected CinemachineImpulseSource ImpulseSource => cinemachineImpulseSource;

        protected float IdleRotation => idleRotation;

        protected GameObject OwnerObj { get; private set; }

        public event Action Triggered = delegate { };

        /// <summary>
        ///   Trigger the weapon
        /// </summary>
        /// <param name="args"> </param>
        public void Trigger(params object[] args)
        {
            if (!Enabled || !CanAttack(args)) return;

            currentAttackCooldown = triggerCooldown / AttackSpeedMultiplier;
            attackTimer = currentAttackCooldown;
            Triggered?.Invoke();

            TriggerBehaviour(args);
        }

        public virtual void SetOwner(GameObject owner)
        {
            OwnerObj = owner;
            AttachTo(OwnerObj);
        }

        public bool HasTarget() => target;

        public virtual void Update()
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                attackTimer = attackTimer < 0 ? 0 : attackTimer;
            }

            var flipX = 1F;
            var flipY = 1F;
            if (OwnerObj && HasTarget())
            {
                var scaleSign = GetOwnerScaleSign();
                var rot = scaleSign * Vector3.SignedAngle(Vector2.right, target.position - OwnerObj.transform.position, Vector3.forward);
                rot += (scaleSign * offsetRotation) + (scaleSign * idleRotation);
                targetRotation = rot;

                flipX = rendererFlipX * scaleSign;
            }
            else
            {
                targetRotation = offsetRotation + idleRotation;
            }
            if (keepWeaponTransformUpVector)
            {
                flipY = Mathf.Sign(weaponTransform.transform.up.y);
            }
            weaponTransform.localRotation = Quaternion.Euler(0F, 0F, targetRotation);

            spriteTransform.localScale = new Vector3(flipX, flipY, spriteTransform.localScale.z);
        }

        public void SetTarget(Transform target) => this.target = target;

        public Transform GetTarget() => target;

        public Descriptor GetDescriptor() => descriptor;

        public GameObject GetOwner() => OwnerObj;

        public float GetCooldown() => currentAttackCooldown;

        public float GetCooldownPercentage() => currentAttackCooldown <= 0F ? 0F : attackTimer / currentAttackCooldown;

        public void AddDamageModifier(GuidDecorator<Modifier> modifier) => damageMultiplier.Add(modifier.Payload, modifier.Guid);

        public void RemoveDamageModifier(Guid guid) => damageMultiplier.Remove(guid);

        protected abstract void TriggerBehaviour(params object[] args);

        protected virtual bool CanAttack(params object[] args) => GetCooldownPercentage() <= 0;

        protected float GetOwnerScaleSign() => OwnerObj ? Mathf.Sign(OwnerObj.transform.localScale.x) : 1F;

        protected virtual void Awake()
        {
            cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        #region Attachment

        public static bool TryApply<T>(T weapon, GameObject target, out T instance) where T : Weapon => Applicator.TryApply(weapon, target, out instance, wpn => wpn.SetOwner(target));

        public bool CanBeApplied(Weapon payload, GameObject target) => target.TryGetComponent<IWeaponOwner>(out var _);

        private void AttachTo(GameObject target)
        {
            gameObject.transform.SetParent(target.transform, false);
            if (target.TryGetComponent<IWeaponOwner>(out var weaponOwner))
                weaponOwner.EquipWeapon(this);
            if (target.TryGetComponent<ITargetOwner>(out var targetOwner))
                SetTarget(targetOwner.GetTarget());
        }

        #endregion Attachment
    }
}