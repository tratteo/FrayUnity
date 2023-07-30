using UnityEngine;

namespace Fray
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ManagedRigidbody : MonoBehaviour, IManagedRigidbody
    {
        [SerializeField, Range(0F, 1F)] private float forceDampening = 0.1F;
        private Vector3 externalVelocity = Vector2.zero;

        public Vector2 Traslation { get; set; }

        public Rigidbody2D Rigidbody { get; private set; }

        public bool EnableTraslation { get; set; } = true;

        public Vector3 AddExternalForce(Vector3 force) => externalVelocity += force;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            externalVelocity = Vector2.Lerp(externalVelocity, Vector2.zero, forceDampening);
            Rigidbody.velocity = externalVelocity;

            if (EnableTraslation)
            {
                Rigidbody.velocity += Traslation;
            }
        }
    }
}