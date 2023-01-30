using UnityEngine;

namespace Fray
{
    /// <summary>
    ///   Internally manages the application of forces
    /// </summary>
    public interface IManagedRigidbody
    {
        Vector3 AddExternalForce(Vector3 force);
    }
}