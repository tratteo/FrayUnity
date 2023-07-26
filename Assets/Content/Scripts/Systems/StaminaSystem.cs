using GibFrame;
using GibFrame.Performance;
using System;
using UnityEngine;

namespace Fray.Systems
{
    public class StaminaSystem : ValueSystemBehaviour
    {
        [SerializeField] private float staminaPerTick = 1;
        [SerializeField] private float tickRate = 0.25F;
        private UpdateJob rechargeJob;

        public event Action<float, GameObject> OnIncrease = delegate { };

        public event Action<float, GameObject> OnDecrease = delegate { };

        public void Decrease(float value, GameObject subject = null)
        {
            System.Decrease(value);
            OnDecrease(value, subject);
        }

        public void Increase(float value, GameObject subject = null)
        {
            System.Increase(value);
            OnIncrease(value, subject);
        }

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