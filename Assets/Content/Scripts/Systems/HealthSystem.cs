using GibFrame;
using System;
using UnityEngine;

namespace Fray.Systems
{
    public class HealthSystem : ValueSystemBehaviour
    {
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
    }
}