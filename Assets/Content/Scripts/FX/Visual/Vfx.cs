using UnityEngine;
using Validatox.Meta;

namespace Fray.FX
{
    /// <summary>
    ///   The actual object representing a VFX. Managed by <see cref="VfxHandler"/>
    /// </summary>
    [CreateAssetMenu(menuName = "Fray/FX/Visual", fileName = "visual_vfx")]
    public class Vfx : ScriptableObject
    {
        public enum DisposePolicy
        { Soft, Hard }

        public enum ReplaceAttachment
        { Unchanched, Root, None }

        [SerializeField, Guard] private GameObject effectPrefab;

        [Header("Behaviour")]
        [SerializeField, Tooltip("Soft: wait until the effect emitted particles are destroyed\nHard: instantly kill the effect")]
        private DisposePolicy disposePolicy = DisposePolicy.Soft;
        [Header("Soft")]
        [SerializeField] private ReplaceAttachment replaceAttachment = ReplaceAttachment.Unchanched;

        public GameObject Prefab => effectPrefab;

        public DisposePolicy Policy => disposePolicy;

        public ReplaceAttachment Attachment => replaceAttachment;
    }
}