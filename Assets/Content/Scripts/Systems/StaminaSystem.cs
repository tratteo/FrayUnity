using Fray.Extensions;
using GibFrame;
using GibFrame.Performance;
using UnityEngine;

namespace Fray.Systems
{
    public class StaminaSystem : ValueSystemBehaviour
    {
        [SerializeField] private bool exhaustInhibit = false;
        [SerializeField] private float exhaustCooldown = 3;
        [SerializeField] private float staminaPerTick = 1;
        [SerializeField] private float tickRate = 0.25F;
        private UpdateJob rechargeJob;
        private bool inhibit = false;

        public ValueSystem ValueSystem => System;

        private void Start()
        {
            rechargeJob = new UpdateJob(new Callback(Recharge), tickRate);
        }

        private void Recharge() => System.Increase(staminaPerTick);

        private void Update()
        {
            if (exhaustInhibit && System.Value <= 0 && !inhibit)
            {
                inhibit = true;
                this.ExecuteAfterDelay(exhaustCooldown, () =>
                {
                    inhibit = false;
                    System.Increase(staminaPerTick);
                });
            }
            if (!inhibit)
            {
                rechargeJob.Step(Time.deltaTime);
            }
        }
    }
}