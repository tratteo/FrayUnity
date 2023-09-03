using Fray.Systems.Weapons;
using GibFrame;
using GibFrame.Extensions;
using GibFrame.Meta;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    [Target(typeof(Sword), IncludeChildren = true)]
    public class PerfectStancePassive : PassiveAbility
    {
        [SerializeField] private float buffDuration = 1;
        [SerializeField] private Modifier damageBuff;
        private GuidDecorator<Modifier> damageBuffDecorator;
        private Sword sword;
        private int buffIteration = 0;

        protected override void Initialize()
        {
            damageBuffDecorator = new GuidDecorator<Modifier>(damageBuff);
            sword = ParentObj.GetComponent<IWeaponOwner>().GetWeapon() as Sword;
            sword.Parried += BuffWeapon;
        }

        protected override void Dispose()
        {
            sword.RemoveDamageModifier(damageBuffDecorator);
            sword.AttackPerformed -= AttackPerformed;
            sword.Parried -= BuffWeapon;
        }

        private void AttackPerformed(SwordAttack attack) => DebuffWeapon();

        private void BuffWeapon()
        {
            buffIteration++;
            var currentIteration = buffIteration;
            sword.AddDamageModifier(damageBuffDecorator);
            sword.AttackPerformed += AttackPerformed;
            this.ExecuteAfterDelay(buffDuration, () => TimedDebuffWeapon(currentIteration));
        }

        private void TimedDebuffWeapon(int iteration)
        {
            if (buffIteration != iteration) return;
            DebuffWeapon();
        }

        private void DebuffWeapon()
        {
            sword.RemoveDamageModifier(damageBuffDecorator);
            sword.AttackPerformed -= AttackPerformed;
        }
    }
}