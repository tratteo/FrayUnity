using Fray.Systems;
using Fray.Systems.Animation;
using Fray.Systems.Weapons;
using GibFrame;
using GibFrame.Data;
using GibFrame.Performance;
using GibFrame.Selectors;
using UnityEngine;

namespace Fray.Npc
{
    public class TrainingDummy : MonoBehaviour, IStunnable, IWeaponOwner
    {
        [Header("Attack")]
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private RandomizedFloat attackCooldown;
        private HealthSystem health;
        private float attackTimer;
        private DistanceSelector2D selector;
        private float healCd;
        private UpdateJob healJob;
        private Weapon weapon;
        private Optional<AnimationDriver> animatorDriver;
        private float lookSign = 1F;
        private float speed = 0F;
        private Vector3 lastPos;
        private Vector3 traslation;

        public bool MovementEnabled { get; set; } = true;

        public bool IsStunned { get; private set; }

        public Multiplier SpeedMultiplier { get; private set; } = new Multiplier(false);

        public bool Visible { get; }

        public float Importance { get; }

        protected DistanceSelector2D Selector => selector;

        public void Stun(float duration) => IsStunned = true;

        public Weapon GetWeapon() => weapon;

        public float GetRelativeTraslationSign(Vector3 vector) => Mathf.Sign(transform.localScale.x * vector.x);

        public void Unstun() => IsStunned = false;

        public virtual void EquipWeapon(Weapon weapon) => this.weapon = weapon;

        protected void ResetAttackTimer() => attackTimer = attackCooldown;

        protected virtual void TriggerAttack() => ResetAttackTimer();

        protected virtual void Update()
        {
            traslation = transform.position - lastPos;
            speed = traslation.magnitude;
            lastPos = transform.position;

            healJob.Step(Time.deltaTime);
            if (health != null && healCd > 0 && health.ValuePercentage < 1F)
            {
                healCd -= Time.deltaTime;
                if (healCd <= 0)
                {
                    healCd = 0;
                    healJob.Resume();
                }
            }
            lookSign = selector && selector.SelectedObj
                ? selector.SelectedObj.transform.position.x > transform.position.x ? 1 : -1
                : Mathf.Sign(traslation.x);

            transform.localScale = new Vector3(lookSign, transform.localScale.y, transform.localScale.z);
            DriveAnimations();
            if (IsStunned) return;
            AttackLoop();
        }

        private void Start()
        {
            if (health != null)
            {
                healJob = new UpdateJob(new Callback(() => health.Increase(25, gameObject)), 0.25F);
                health.OnDecrease += OnHealthChanged;
            }
            if (weaponPrefab)
            {
                var res = Weapon.TryApply(weaponPrefab.GetComponent<Weapon>(), gameObject, out var _);
            }
        }

        private void Awake()
        {
            animatorDriver = new Optional<AnimationDriver>(() => GetComponent<AnimationDriver>());
            selector = GetComponent<DistanceSelector2D>();
            health = GetComponent<HealthSystem>();
            lastPos = transform.position;
        }

        private void OnHealthChanged(float val, GameObject obj)
        {
            healCd = 1.5F;
            healJob.Suspend();
        }

        private void AttackLoop()
        {
            if (selector && weapon)
            {
                if (attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0)
                    {
                        attackTimer = 0F;
                        TriggerAttack();
                    }
                }
                if (selector.SelectedObj)
                {
                    weapon.SetTarget(selector.SelectedObj.transform);
                }
                else
                {
                    weapon.SetTarget(null);
                    attackTimer = attackCooldown;
                }
            }
        }

        private void DriveAnimations()
        {
            animatorDriver.Try((driver) =>
            {
                if (Mathf.Approximately(speed, 0F))
                {
                    driver.DriveAnimation(new AnimatorDriverData(Animations.Idle));
                }
                else
                {
                    driver.DriveAnimation(new AnimatorDriverData(Animations.Run, transform.localScale.x < 1, SpeedMultiplier.Value * GetRelativeTraslationSign(traslation)));
                }
            });
        }
    }
}