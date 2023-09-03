using UnityEngine;

namespace Fray.Extensions
{
    public static class Extensions
    {
        public static Color WithOpacity(this Color c, float a)
        {
            c.a = a;
            return c;
        }

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
    }
}