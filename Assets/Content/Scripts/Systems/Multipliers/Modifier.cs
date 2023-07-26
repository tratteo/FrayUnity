using System;
using UnityEngine;

namespace Fray.Systems
{
    /// <summary>
    ///   Represents a modification that can be applied to a <see cref="Multiplier"/>
    /// </summary>
    [Serializable]
    public class Modifier
    {
        public enum ActionType
        { Additive, Percentage }

        [SerializeField] private ActionType type;
        [SerializeField] private float value;

        public ActionType Type => type;

        public float Value => value;

        public Modifier(ActionType type, float value)
        {
            this.type = type;
            this.value = value;
        }

        public float GetAction(float multiplier)
        {
            var action = 0F;
            switch (type)
            {
                case ActionType.Percentage:
                    action = (value + 1F) * multiplier - multiplier;
                    break;

                case ActionType.Additive:
                    action = value;
                    break;
            }
            return action;
        }
    }
}