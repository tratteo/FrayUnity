using Fray.Systems;

namespace Fray
{
    public interface IMovable
    {
        bool MovementEnabled { get; set; }

        Multiplier SpeedMultiplier { get; }
    }
}