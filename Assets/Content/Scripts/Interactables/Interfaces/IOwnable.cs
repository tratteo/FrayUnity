using UnityEngine;

namespace Fray
{
    /// <summary>
    ///   Is ownable by a <see cref="GameObject"/>
    /// </summary>
    public interface IOwnable
    {
        GameObject GetOwner();

        void SetOwner(GameObject owner);
    }
}