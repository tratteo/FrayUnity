using UnityEngine;

namespace Fray.Weapons
{
    /// <summary>
    ///   <see cref="ScriptableObject"/> defining parry statistics for a <see cref="Sword"/>
    /// </summary>
    [CreateAssetMenu(fileName = "parry", menuName = "Fray/Sword parry")]
    public class Parry : ScriptableObject
    {
        [SerializeField] private float stamina = 15F;
        [SerializeField] private float duration = 0.25F;
        [SerializeField] private float cooldown = 0.5F;
        [SerializeField, Range(0F, 2F)] private float staminaRecoverRatio = 0.75F;

        public float Stamina => stamina;

        public float Duration => duration;

        public float Cooldown => cooldown;

        public float StaminaRecoverRatio => staminaRecoverRatio;
    }
}