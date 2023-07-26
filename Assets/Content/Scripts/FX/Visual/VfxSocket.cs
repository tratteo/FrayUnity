using UnityEditor;
using UnityEngine;

namespace Fray.FX
{
    /// <summary>
    ///   Represents a socket to which a <see cref="FX.VfxHandler"/> can be attached
    /// </summary>
    public class VfxSocket : MonoBehaviour
    {
        public enum ShapeType
        { Circle, Rectangle }

        [SerializeField] private ShapeType shape;
        [SerializeField] private float radius = 1F;
        [SerializeField] private Vector2 dimensions = new Vector2(1F, 1F);

        public float Width => dimensions.x;

        public float Radius => radius;

        public ShapeType Shape => shape;

        public Vector3 Scale => new Vector3(Width, Height, 0);

        public float Height => dimensions.y;

        public static implicit operator Transform(VfxSocket socket) => socket.transform;

        public static implicit operator Vector3(VfxSocket socket) => socket.transform.position;

        /// <summary>
        ///   Attach the <see cref="VfxHandler"/> to the underlying <see cref="VfxSocket"/>
        /// </summary>
        /// <param name="vfx"> </param>
        public void Attach(VfxHandler vfx)
        {
            if (!vfx.IsCached) return;
            vfx.CachedObject.transform.SetParent(transform);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            switch (shape)
            {
                case ShapeType.Circle:
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
                    break;

                case ShapeType.Rectangle:
                    Gizmos.DrawRay(transform.position - (transform.right * dimensions.x / 2F) - (transform.up * dimensions.y / 2F), transform.up * dimensions.y);
                    Gizmos.DrawRay(transform.position + (transform.right * dimensions.x / 2F) - (transform.up * dimensions.y / 2F), transform.up * dimensions.y);

                    Gizmos.DrawRay(transform.position - (transform.right * dimensions.x / 2F) + (transform.up * dimensions.y / 2F), transform.right * dimensions.x);
                    Gizmos.DrawRay(transform.position - (transform.right * dimensions.x / 2F) - (transform.up * dimensions.y / 2F), transform.right * dimensions.x);
                    break;
            }
        }

#endif
    }
}