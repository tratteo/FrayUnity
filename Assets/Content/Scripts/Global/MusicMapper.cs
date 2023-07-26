using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Validatox.Serializable;

namespace Fray
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicMapper : MonoBehaviour
    {
        [SerializeField] private bool enableMusic = true;
        [SerializeField] private List<Map> scenesMusics;
        private AudioSource musicSource;

        private void Awake() => musicSource = GetComponent<AudioSource>();

        private void Start() => PlayMappedMusic(SceneManager.GetActiveScene());

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void PlayMappedMusic(Scene scene)
        {
            if (!enableMusic) return;
            var index = scenesMusics.FindIndex(m => m.Scene.Equals(scene.path));
            if (index == -1) return;
            var map = scenesMusics[index];
            musicSource.clip = map.Clip;
            musicSource.volume = map.Volume;
            musicSource.Play();
        }

        private void StopMappedMusic(Scene scene)
        {
            var index = scenesMusics.FindIndex(m => m.Scene.Equals(scene.path));
            if (index == -1) return;
            var map = scenesMusics[index];
            musicSource.clip = map.Clip;
            musicSource.volume = map.Volume;
            musicSource.Stop();
        }

        private void OnSceneUnloaded(Scene scene) => StopMappedMusic(scene);

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => PlayMappedMusic(scene);

        [Serializable]
        public class Map
        {
            [SerializeField] private SceneReference scene;
            [SerializeField, Range(0F, 1F)] private float volume = 1F;
            [SerializeField] private AudioClip clip;

            public string Scene => scene.Path;

            public float Volume => volume;

            public AudioClip Clip => clip;
        }
    }
}