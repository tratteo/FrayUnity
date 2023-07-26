using GibFrame;
using System;
using UnityEngine;
using Validatox.Meta;

namespace Fray.FX
{
    [Serializable]
    public class SfxHandler : IDisposable, IOptional
    {
        [SerializeField, Guard] private SoundFx fx;
        private AudioSource source;
        private bool cached = false;

        public bool IsPlaying => source && source.isPlaying;

        public void PlayAt(Vector3 position)
        {
            if (fx)
            {
                AudioSource.PlayClipAtPoint(fx.RandomClip, position, fx.Volume);
            }
        }

        public void Play(MonoBehaviour owner)
        {
            if (Cache(owner))
            {
                source.clip = fx.RandomClip;
                source.volume = fx.Volume;
                source.pitch = fx.Pitch;
                source.Play();
            }
        }

        public bool Cache(MonoBehaviour owner)
        {
            if (cached) return true;
            source = owner.gameObject.AddComponent<AudioSource>();
            cached = true;
            return true;
        }

        public void Dispose()
        {
            if (!cached) return;
            UnityEngine.Object.Destroy(source);
            cached = false;
        }

        public bool HasValue() => fx;
    }
}