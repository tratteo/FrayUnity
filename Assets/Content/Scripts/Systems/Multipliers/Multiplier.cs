using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Systems
{
    /// <summary>
    ///   Class that manages a multiplier (float). The multiplier can be edited by the application of <see cref="Modifier"/>
    /// </summary>

    [Serializable]
    public class Multiplier
    {
        private readonly Dictionary<Guid, ModifierEffect> modifiersDict;

        [SerializeField] private bool allowNegative = false;

        private float multiplier = 1F;

        public float RawMultiplier => multiplier;

        public bool AllowNegative => allowNegative;

        public float Value => allowNegative ? multiplier : (multiplier < 0F ? 0F : multiplier);

        public Multiplier(bool allowNegative = false)
        {
            this.allowNegative = allowNegative;
            modifiersDict = new Dictionary<Guid, ModifierEffect>();
        }

        public Multiplier(float multiplier, bool allowNegative) : this(allowNegative)
        {
            this.multiplier = multiplier;
        }

        public event Action<float> MultiplierChanged = delegate { };

        public static implicit operator float(Multiplier multiplier) => multiplier.Value;

        public void Reset()
        {
            multiplier = 1F;
            modifiersDict.Clear();
        }

        /// <summary>
        ///   Add the modifier and bind it to the provided <see cref="Guid"/>
        /// </summary>
        /// <param name="mod"> </param>
        /// <param name="guid"> </param>
        /// <returns> </returns>
        public bool Add(Modifier mod, Guid guid)
        {
            if (!modifiersDict.ContainsKey(guid))
            {
                var action = mod.GetAction(multiplier);
                multiplier += action;
                modifiersDict.Add(guid, new ModifierEffect(mod, action));
                MultiplierChanged?.Invoke(this);
                //Debug.Log("Actuating mod[" + guid.ToString() + "]: " + multiplier + ": " + action);
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Remove the modifier associated to the provided <see cref="Guid"/>
        /// </summary>
        /// <param name="guid"> </param>
        /// <returns> </returns>
        public bool Remove(Guid guid)
        {
            if (modifiersDict.TryGetValue(guid, out var mod))
            {
                multiplier -= mod.action;
                modifiersDict.Remove(guid);
                //Debug.Log("Removing mod[" + guid.ToString() + "]: " + multiplier);
                MultiplierChanged?.Invoke(this);
                return true;
            }
            return false;
        }

        private readonly struct ModifierEffect
        {
            public readonly Modifier modifier;
            public readonly float action;

            public ModifierEffect(Modifier modifier, float action)
            {
                this.modifier = modifier;

                this.action = action;
            }
        }
    }
}