using System;

namespace Fray.Systems.Abilities
{
    public interface IAbilityOwner
    {
        public event Action<Ability> AbilityEquipped;

        public event Action<Ability> AbilityUnequipped;

        bool CanReceiveAbility(Ability prefabAbilityInstance);

        void EquipAbility(Ability ability);

        void UnequipAbility(Ability ability);
    }
}