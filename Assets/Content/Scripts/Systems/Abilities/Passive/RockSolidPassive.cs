using GibFrame;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    public class RockSolidPassive : PassiveAbility
    {
        [SerializeField] private Modifier armorModifier;
        private HealthSystem healthSystem;
        private GuidDecorator<Modifier> armorDecorator;

        protected override void Initialize()
        {
            healthSystem = ParentObj.GetComponent<HealthSystem>();
            armorDecorator = new GuidDecorator<Modifier>(armorModifier);
            healthSystem.AddDecreaseModifier(armorDecorator);
        }

        protected override void Dispose()
        {
            healthSystem.RemoveDecreaseModifier(armorDecorator);
        }
    }
}