using Fray.Systems.Weapons;
using GibFrame;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    public class LuckyBastardPassive : PassiveAbility
    {
        [SerializeField, Range(0F, 1F)] private float critChance;
        [SerializeField] private Modifier critAmount;
        private GuidDecorator<Modifier> critDecorator;
        private Sword sword;

        protected override void Initialize()
        {
            sword = ParentObj.GetComponent<IWeaponOwner>().GetWeapon() as Sword;
            critDecorator = new GuidDecorator<Modifier>(critAmount);
            sword.Triggered += CritChance;
        }

        protected override void Dispose()
        {
            sword.Triggered -= CritChance;
            sword.AttackPerformed -= RemoveCrit;
            sword.RemoveDamageModifier(critDecorator);
        }

        private void CritChance()
        {
            if (Random.value <= critChance)
            {
                sword.AddDamageModifier(critDecorator);
                sword.AttackPerformed += RemoveCrit;
            }
        }

        private void RemoveCrit(SwordAttack obj)
        {
            sword.RemoveDamageModifier(critDecorator);
            sword.AttackPerformed -= RemoveCrit;
        }
    }
}