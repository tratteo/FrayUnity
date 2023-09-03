using System;
using UnityEngine;

namespace Fray
{
    /// <summary>
    ///   An object of a specified type that can be applied to a <see cref="GameObject"/>. See <see cref="Abilities.Ability"/> or <see cref="StatusEffects.StatusEffect"/>
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public interface IApplicable<T> : IApplicable
    {
        bool IApplicable.CanBeApplied(object payload, GameObject target) => payload is T payloadType && CanBeApplied(payloadType, target);

        /// <summary>
        ///   Define whether the payload can be applied to the target <see cref="GameObject"/>. Used by <see cref="Stitcher.TryStitch{T}(T,
        ///   GameObject, out T, Action{T})"/>
        /// </summary>
        /// <param name="payload"> </param>
        /// <param name="target"> </param>
        /// <returns> </returns>
        bool CanBeApplied(T payload, GameObject target);
    }

    /// <summary>
    ///   An object that can be applied to a <see cref="GameObject"/>. Do not directly implement this type, use <see cref="IApplicable{T}"/> instead.
    /// </summary>
    public interface IApplicable
    {
        bool CanBeApplied(object payload, GameObject target);
    }

    /// <summary>
    ///   Collection of utility methods to stitch features at runtime to game objects
    /// </summary>
    public static class Stitcher
    {
        /// <summary>
        ///   Try to stitch (attach to a specific <see cref="GameObject"/>) the prefab.
        /// </summary>
        /// <returns> </returns>
        public static bool TryStitch<T>(T payload, GameObject target, out T obj, Action<T> factory = null) where T : MonoBehaviour
        {
            obj = default;
            if (payload is IApplicable applicable && !applicable.CanBeApplied(payload, target)) return false;

            var instance = UnityEngine.Object.Instantiate(payload);
            factory?.Invoke(instance);
            obj = instance;
            return true;
        }
    }
}