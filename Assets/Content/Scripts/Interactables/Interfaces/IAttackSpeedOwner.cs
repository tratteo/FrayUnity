using Fray.Systems;

namespace Fray
{    /// <summary>
     /// Has attack speed </summary>
    public interface IAttackSpeedOwner
    {
        public Multiplier AttackSpeedMultiplier { get; }
    }
}