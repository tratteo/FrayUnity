using UnityEngine;

namespace Fray.Weapons
{
    /// <summary>
    ///   <see cref="ScriptableObject"/> defining an attack that can be performed with a <see cref="Sword"/>
    /// </summary>
    [CreateAssetMenu(fileName = "sword_atk", menuName = "Fray/Sword attack")]
    public class SwordAttack : ScriptableObject
    {
        public const short BaseAttack = 0x0;
        public const short Parry = 0x1;

        [SerializeField] private float damageMultiplier = 1F;
        [SerializeField] private float range = 4F;
        [SerializeField] private float duration = 0.3F;
        [SerializeField] private float staminaRequired = 15F;
        [SerializeField] private bool canBeParried = true;
        [SerializeField] private Vector3 offset;
        [Header("Overrides")]
        [SerializeField] private float colliderAngle = 100;
        [Header("FX")]
        [SerializeField] private bool showVisualFx = true;
        [SerializeField] private bool playSoundFx = true;

        public float DamageMultiplier => damageMultiplier;

        public float StaminaRequired => staminaRequired;

        public float Range { get => range; set => range = value; }

        public float Duration => duration;

        public bool CanBeParried => canBeParried;

        public Vector3 Offset => offset;

        public float ColliderAngle => colliderAngle;

        public bool ShowVisualFx => showVisualFx;

        public bool PlaySoundFx => playSoundFx;

        public SwordAttack Initialize(
            float damageMultiplier,
            float range,
            float duration,
            float staminaRequired,
            bool canBeParried,
            Vector3 offset,
            float colliderAngle,
            bool showVisualFx,
            bool playSoundFx)
        {
            this.staminaRequired = staminaRequired;
            this.damageMultiplier = damageMultiplier;
            this.range = range;
            this.duration = duration;
            this.canBeParried = canBeParried;
            this.offset = offset;
            this.colliderAngle = colliderAngle;
            this.showVisualFx = showVisualFx;
            this.playSoundFx = playSoundFx;
            return this;
        }
    }
}