using GibFrame;
using GibFrame.Meta;
using UnityEngine;
using Validatox.Meta;

namespace Fray.Systems.Abilities
{
    /// <summary>
    ///   <para> Base class for abilities </para>
    ///   This class manages the attachment of the ability object to the owner through the <see cref="Server"/> module by implementing the
    ///   <see cref="IApplicable{T}"/> interface.
    /// </summary>
    public abstract class Ability : MonoBehaviour, IApplicable<Ability>, IDescriptable
    {
        [SerializeField, Guard] private Descriptor descriptor;
        private bool awaitingDestruction = false;

        public Descriptor Descriptor => descriptor;

        public IAbilityOwner Parent { get; private set; }

        public Collider2D ParentCollider { get; private set; }

        public GameObject ParentObj { get; private set; }

        public static bool TryApply(Ability ability, GameObject target) => Stitcher.TryStitch(ability, target, out var _, ab => ab.ServerAttachedTo(target));

        public bool CanBeApplied(Ability ability, GameObject target) => this.SatisfiesTargetAttribute(target) && target.TryGetComponent<IAbilityOwner>(out var owner) && owner.CanReceiveAbility(ability);

        public Descriptor GetDescriptor() => descriptor;

        public void DetachFrom(IAbilityOwner parent) => Destroy();

        /// <summary>
        ///   Called after instantiation and initialization
        /// </summary>
        protected virtual void Initialize()
        { }

        /// <summary>
        ///   Called just before destruction
        /// </summary>
        protected virtual void Dispose()
        { }

        private void OnDestroy()
        {
            Dispose();
            Parent.UnequipAbility(this);
        }

        private void Destroy()
        {
            if (awaitingDestruction) return;
            awaitingDestruction = true;
            Destroy(gameObject);
        }

        private void ServerAttachedTo(GameObject parent)
        {
            AttachTo(parent);
            Initialize();
            Parent.EquipAbility(this);
        }

        private void AttachTo(GameObject parent)
        {
            Parent = parent.GetComponent<IAbilityOwner>();
            ParentObj = parent;
            ParentCollider = parent.GetComponent<Collider2D>();
            Transform holder;
            if (!(holder = ParentObj.transform.Find("AbilitiesHolder")))
            {
                var holderObj = new GameObject
                {
                    name = "AbilitiesHolder"
                };
                holderObj.transform.SetParent(ParentObj.transform);
                holderObj.transform.localPosition = Vector3.zero;
                holder = holderObj.transform;
            }
            transform.SetParent(holder);
            transform.localPosition = Vector3.zero;
        }
    }
}