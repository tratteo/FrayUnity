using Fray.Extensions;
using UnityEngine;

namespace Fray
{
    public class Pusher : MonoBehaviour
    {
        [SerializeField] private float pushForce = 1000;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.AddManagedForce2D(transform.up * pushForce);
            }
        }
    }
}