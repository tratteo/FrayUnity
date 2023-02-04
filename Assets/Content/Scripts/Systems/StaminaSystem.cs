using GibFrame;
using GibFrame.Performance;
using UnityEngine;

namespace Fray.Systems
{
    public class StaminaSystem : ValueSystemBehaviour
    {
        [SerializeField] private float staminaPerTick = 1;
        [SerializeField] private float tickRate = 0.25F;
        private UpdateJob rechargeJob;

        public ValueSystem ValueSystem => System;

        private void Start()
        {
            rechargeJob = new UpdateJob(new Callback(Recharge), tickRate);
        }

        private void Recharge() => System.Increase(staminaPerTick);

        private void Update()
        {
            rechargeJob.Step(Time.deltaTime);
        }
    }
}