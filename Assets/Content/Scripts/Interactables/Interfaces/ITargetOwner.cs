using UnityEngine;

namespace Fray
{
    /// <summary>
    ///   Has a target
    /// </summary>
    public interface ITargetOwner
    {
        Transform GetTarget();
    }
}