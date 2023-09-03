using GibFrame;
using GibFrame.Performance;
using System;
using UnityEngine;

namespace Fray.Systems
{
    public class StaminaSystem : ValueSystemBehaviour
    {
        private readonly Multiplier increaseMultiplier = new Multiplier();
        private readonly Multiplier decreaseMultiplier = new Multiplier();
        [SerializeField] private float staminaPerTick = 1;
        [SerializeField] private float tickRate = 0.25F;
        private UpdateJob rechargeJob;

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

        private void Start()
        {
            rechargeJob = new UpdateJob(new Callback(Recharge), tickRate);
        }

        private void Recharge() => Increase(staminaPerTick);

        private void Update()
        {
            rechargeJob.Step(Time.deltaTime);
        }
    }
}