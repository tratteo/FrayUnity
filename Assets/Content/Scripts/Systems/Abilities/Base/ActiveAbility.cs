using System;
using System.Collections;
using UnityEngine;

namespace Fray.Systems.Abilities
{
    /// <summary>
    ///   <para> An <see cref="Ability"/> that can be activated and used </para>
    ///   Implements a system to easily manage abilities <see cref="Phase"/>. Phases can be defined in the editor and describe different
    ///   steps of the ability. Override the desired methods to implement functionalities.
    /// </summary>
    public abstract class ActiveAbility : Ability, IResourceCooldownOwner
    {
        [Header("Active")]
        [SerializeField] private float cooldown;
        [SerializeField, Min(1)] private int charges;
        [SerializeField] private Phase[] phases;
        private int usedCharges = 0;
        private float cooldownTimer;
        private Coroutine coroutine;
        private int currentExecutingPhase = -1;

        private float executingTimer;

        public bool IsPerforming { get; private set; } = false;

        /// <summary>
        ///   Can the ability be performed
        /// </summary>
        /// <returns> </returns>
        public virtual bool CanPerform() => cooldownTimer <= 0F && !IsPerforming;

        public float GetCooldown() => cooldown;

        public float GetCooldownPercentage() => 1F - (cooldownTimer / cooldown);

        public bool Perform()
        {
            if (CanPerform())
            {
                IsPerforming = true;
                coroutine = StartCoroutine(Execute_C());
                cooldownTimer = cooldown;
                return true;
            }
            return false;
        }

        public void Update()
        {
            if (cooldownTimer > 0 && !IsPerforming)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer < 0) cooldownTimer = 0;
            }
        }

        public int GetResourcesAmount() => charges - usedCharges;

        public override string ToString() => GetType().Name;

        protected override void Dispose()
        {
            Interrupt();
        }

        protected Phase GetPhase(int index) => phases[index];

        /// <summary>
        ///   Called at the start of a new <see cref="Phase"/>
        /// </summary>
        /// <param name="phase"> </param>
        /// <param name="index"> </param>
        protected abstract void Begin(Phase phase, int index);

        /// <summary>
        ///   Called every frame during the action of the current <see cref="Phase"/>
        /// </summary>
        /// <param name="phase"> </param>
        /// <param name="index"> </param>
        /// <param name="deltaTime"> </param>
        protected abstract void Tick(Phase phase, int index, float deltaTime);

        /// <summary>
        ///   Called at the end of the current <see cref="Phase"/>
        /// </summary>
        /// <param name="phase"> </param>
        /// <param name="index"> </param>
        protected abstract void End(Phase phase, int index);

        protected void Interrupt()
        {
            if (!IsPerforming) return;
            if (coroutine is not null)
            {
                StopCoroutine(coroutine);
            }
            if (currentExecutingPhase >= 0 && currentExecutingPhase < phases.Length)
            {
                for (; currentExecutingPhase < phases.Length; currentExecutingPhase++)
                {
                    End(phases[currentExecutingPhase], currentExecutingPhase);
                }
            }
            Complete();
        }

        protected void EndCurrentPhase()
        {
            if (IsPerforming) executingTimer = 0;
        }

        protected IEnumerator Execute_C()
        {
            if (TryUseCharge())
            {
                var wait = new WaitForFixedUpdate();
                for (currentExecutingPhase = 0; currentExecutingPhase < phases.Length; currentExecutingPhase++)
                {
                    var current = phases[currentExecutingPhase];
                    executingTimer = current.Duration;
                    Begin(current, currentExecutingPhase);
                    while ((executingTimer -= Time.fixedDeltaTime) > 0)
                    {
                        yield return wait;
                        Tick(current, currentExecutingPhase, Time.fixedDeltaTime);
                    }
                    End(current, currentExecutingPhase);
                }
            }
            Complete();
        }

        private void Complete()
        {
            currentExecutingPhase = -1;
            coroutine = null;
            IsPerforming = false;
            if (GetResourcesAmount() <= 0)
            {
                Parent.UnequipAbility(this);
                DetachFrom(Parent);
            }
        }

        private bool TryUseCharge() => usedCharges++ < charges;

        [Serializable]
        public struct Phase
        {
            [SerializeField] private string name;
            [SerializeField] private float duration;

            public string Name => name;

            public float Duration => duration;
        }
    }
}