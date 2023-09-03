using GibFrame;
using GibFrame.Meta;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    [Target(typeof(IMovable))]
    public class TheFastPassive : PassiveAbility
    {
        [SerializeField] private Modifier movementBuff;
        private GuidDecorator<Modifier> movementBuffDecorator;
        private IMovable movable;

        protected override void Initialize()
        {
            movementBuffDecorator = new GuidDecorator<Modifier>(movementBuff);
            movable = ParentObj.GetComponent<IMovable>();
            movable.SpeedMultiplier.Add(movementBuffDecorator.Payload, movementBuffDecorator.Guid);
        }

        protected override void Dispose()
        {
            movable.SpeedMultiplier.Remove(movementBuffDecorator);
        }
    }
}