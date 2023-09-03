using GibFrame;
using System;
using UnityEngine;

namespace Fray.Systems
{
    public class HealthSystem : ValueSystemBehaviour
    {
        private readonly Multiplier increaseMultiplier = new Multiplier();
        private readonly Multiplier decreaseMultiplier = new Multiplier();

        public event Action<float, GameObject> OnIncrease = delegate { };

        public event Action<float, GameObject> OnDecrease = delegate { };

        public void Decrease(float value, GameObject subject = null)
        {
            value *= decreaseMultiplier;
            System.Decrease(value);
            OnDecrease(value, subject);
        }

        public void Increase(float value, GameObject subject = null)
        {
            value *= increaseMultiplier;
            System.Increase(value);
            OnIncrease(value, subject);
        }

        public bool AddIncreaseModifier(GuidDecorator<Modifier> modifier) => increaseMultiplier.Add(modifier.Payload, modifier.Guid);

        public bool AddDecreaseModifier(GuidDecorator<Modifier> modifier) => decreaseMultiplier.Add(modifier.Payload, modifier.Guid);

        public bool RemoveDecreaseModifier(GuidDecorator<Modifier> modifier) => decreaseMultiplier.Remove(modifier.Guid);

        public bool RemoveIncreaseModifier(GuidDecorator<Modifier> modifier) => increaseMultiplier.Remove(modifier.Guid);
    }
}