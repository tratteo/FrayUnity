using Fray.Character.Input;
using Fray.Extensions;
using Fray.FX;
using Fray.Systems.Weapons;
using GibFrame;
using UnityEngine;
using Validatox.Meta;

namespace Fray.Character
{
    /// <summary>
    ///   Handles weapon equipment and combat system
    /// </summary>
    public class CharacterCombat : CharacterComponent, IStunnable, IWeaponOwner, ITargetOwner
    {
        [SerializeField, Guard] private Weapon startingWeapon;
        [SerializeField, Guard] private Transform crosshair;
        [Header("UI")]
        [SerializeField] private string[] parryPhrases;
        [Header("SFX")]
        [SerializeField, Range(0F, 1F)] private float triggerSoundChance = 0.5F;
        [SerializeField] private Optional<SfxHandler> triggerSfx;
        [SerializeField, Range(0F, 1F)] private float hurtSoundChance = 0.5F;
        [SerializeField] private Optional<SfxHandler> hurtSfx;

        private Sword sword;

        private CharacterCamera cameraService;

        public bool IsStunned { get; private set; }

        //public ActiveAbility ActiveAbility { get; private set; }

        //public PassiveAbility PassiveAbility { get; private set; }

        //public event Action<Ability> AbilityEquipped = delegate { };

        //public event Action<Ability> AbilityUnequipped = delegate { };

        public void Stun(float duration)
        {
            IsStunned = true;
            Input.Enabled = false;
        }

        //public void EquipAbility(Ability ability)
        //{
        //    if (ability is ActiveAbility active)
        //    {
        //        if (ActiveAbility)
        //        {
        //            ActiveAbility.DetachFrom(this);
        //            UnequipAbility(ActiveAbility);
        //        }
        //        ActiveAbility = active;
        //    }
        //    else if (ability is PassiveAbility passive)
        //    {
        //        if (PassiveAbility)
        //        {
        //            PassiveAbility.DetachFrom(this);
        //            UnequipAbility(PassiveAbility);
        //        }
        //        PassiveAbility = passive;
        //    }
        //    AbilityEquipped?.Invoke(ability);
        //}
        public void OnHit(float force, GameObject dealer)
        {
            if (UnityEngine.Random.value < hurtSoundChance) hurtSfx.Try(s => s.Play(this));
            if (dealer)
            {
                var axis = (transform.position - dealer.transform.position).normalized;
                axis = Quaternion.AngleAxis(UnityEngine.Random.Range(-40F, 40F), Vector3.forward) * axis;
                Rigidbody.AddManagedForce2D(30F * force * axis);
            }
        }

        //public bool CanReceiveAbility(Ability prefabAbilityInstance) => true;
        public Weapon GetWeapon() => sword;

        public Transform GetTarget() => crosshair;

        //public void UnequipAbility(Ability ability)
        //{
        //    if (ability is ActiveAbility active)
        //    {
        //        if (!ability || ability.Equals(ActiveAbility))
        //        {
        //            AbilityUnequipped?.Invoke(ability);
        //            ActiveAbility = null;
        //        }
        //    }
        //    else if (ability is PassiveAbility passive)
        //    {
        //        if (!ability || ability.Equals(PassiveAbility))
        //        {
        //            AbilityUnequipped?.Invoke(ability);
        //            PassiveAbility = null;
        //        }
        //    }
        //}
        public void Unstun()
        {
            IsStunned = false;
            Input.Enabled = true;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (this.sword || weapon is not Sword sword) return;
            this.sword = sword;
            sword.AttackPerformed += (attack) =>
            {
                //cameraService.ClientShakeCamera(CharacterCamera.ShakeStrenght.Small);
                if (UnityEngine.Random.value < triggerSoundChance)
                {
                    triggerSfx.Try(s => s.Play(this));
                }
            };
            sword.Parried += () =>
            {
                if (parryPhrases.Length <= 0) return;
                //GameUI.WorldText(
                //    parryPhrases[UnityEngine.Random.Range(0, parryPhrases.Length)],
                //    (sword.transform.position + (2F * Vector3.up)).Perturbate(Vector2.right, 1.5F));
            };
            //sword.GotParried += (attack) => cameraService.ClientShakeCamera(CharacterCamera.ShakeStrenght.Medium);
        }

        protected override void Awake()
        {
            base.Awake();
            cameraService = GetComponent<CharacterCamera>();
        }

        protected override void OnInput(CharacterInputData data)
        {
            if (IsStunned) return;
            switch (data.action)
            {
                case CharacterAction.Parry:
                    sword?.EnableParry();
                    break;

                case CharacterAction.Attack:
                    sword?.Trigger();
                    break;

                    //case CharacterAction.Ability:
                    //    if (ActiveAbility)
                    //        ActiveAbility.Perform();
                    //    break;

                    //case CharacterAction.Focus:
                    //    var pos = (Vector2)data.payload;
                    //    crosshair.localPosition = pos;
                    //    var currScale = transform.localScale.AsPositive();
                    //    currScale.x = crosshair.position.x > transform.position.x ? currScale.x * 1F : currScale.x * -1F;
                    //    transform.localScale = currScale;
                    //    break;
            }
        }

        private void Start()
        {
            crosshair.SetParent(null);
            crosshair.gameObject.SetActive(true);
            Weapon.TryApply(startingWeapon, gameObject, out var _);
            Health.OnDecrease += OnHit;
        }
    }
}