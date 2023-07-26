using GibFrame.Data;
using UnityEngine;

namespace Fray.FX
{
    [CreateAssetMenu(menuName = "FX/Sound", fileName = "sound_fx")]
    public class SoundFx : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private RandomizedFloat volume;
        [SerializeField] private RandomizedFloat pitch;

        public AudioClip[] Clips => clips;

        public AudioClip RandomClip => clips[Random.Range(0, clips.Length)];

        public RandomizedFloat Volume => volume;

        public RandomizedFloat Pitch => pitch;
    }
}