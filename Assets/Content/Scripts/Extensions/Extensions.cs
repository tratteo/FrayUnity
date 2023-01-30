using System;
using System.Collections;
using UnityEngine;

namespace Fray.Extensions
{
    public static class Extensions
    {
        /// <summary>
        ///   Execute a function after the specified delay
        /// </summary>
        public static Coroutine ExecuteAfterDelay(this MonoBehaviour mono, float delay, Action callback) => mono.StartCoroutine(Delayed_C(delay, callback));

        public static void AddManagedForce2D(this Rigidbody2D rigidbody2D, Vector3 force)
        {
            IManagedRigidbody managedRigidbody;
            if ((managedRigidbody = rigidbody2D.GetComponent<IManagedRigidbody>()) != null)
            {
                if (!Mathf.Approximately(rigidbody2D.mass, 0F))
                {
                    force /= rigidbody2D.mass;
                }
                managedRigidbody.AddExternalForce(force);
            }
            else
            {
                if (!Mathf.Approximately(rigidbody2D.mass, 0F))
                {
                    force *= rigidbody2D.mass;
                }
                rigidbody2D.AddForce(force);
            }
        }

        private static IEnumerator Delayed_C(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}