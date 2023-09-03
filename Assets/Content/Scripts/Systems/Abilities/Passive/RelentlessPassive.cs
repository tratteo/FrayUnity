using GibFrame;
using GibFrame.Meta;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    [Target(typeof(StaminaSystem))]
    public class RelentlessPassive : PassiveAbility
    {
        [SerializeField] private Modifier staminaUsageModifier;
        private GuidDecorator<Modifier> staminaUsageModifierDecorator;
        private StaminaSystem stamina;

        protected override void Dispose()
        {
            stamina.RemoveDecreaseModifier(staminaUsageModifierDecorator);
        }

        protected override void Initialize()
        {
            staminaUsageModifierDecorator = new GuidDecorator<Modifier>(staminaUsageModifier);
            stamina = ParentObj.GetComponent<StaminaSystem>();
            stamina.AddDecreaseModifier(staminaUsageModifierDecorator);
        }
    }
}