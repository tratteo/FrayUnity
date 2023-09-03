using Fray.Systems.Weapons;
using GibFrame;
using UnityEngine;

namespace Fray.Systems.Abilities

{
    public class DoubleEdgedSwordPassive : PassiveAbility
    {
        [SerializeField] private Modifier dmgDealtModifier;
        [SerializeField] private Modifier dmgTakenModifier;
        private GuidDecorator<Modifier> dmgDealtDecorator;
        private GuidDecorator<Modifier> dmgTakenDecorator;
        private HealthSystem healthSystem;
        private Sword sword;

        protected override void Initialize()
        {
            dmgDealtDecorator = new GuidDecorator<Modifier>(dmgDealtModifier);
            dmgTakenDecorator = new GuidDecorator<Modifier>(dmgTakenModifier);
            healthSystem = ParentObj.GetComponent<HealthSystem>();
            sword = ParentObj.GetComponent<IWeaponOwner>().GetWeapon() as Sword;
            sword.AddDamageModifier(dmgDealtDecorator);
            healthSystem.AddDecreaseModifier(dmgTakenDecorator);
        }

        protected override void Dispose()
        {
            healthSystem.RemoveDecreaseModifier(dmgTakenDecorator);
            sword.RemoveDamageModifier(dmgDealtDecorator);
        }
    }
}