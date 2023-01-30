using UnityEngine;

namespace Fray
{
    public class ElementOfInterest : MonoBehaviour, IElementOfInterest
    {
        [SerializeField] private bool visible = true;
        [SerializeField, Range(0F, 1F)] private float importance;

        public bool Visible => visible;

        public float Importance => importance;
    }
}