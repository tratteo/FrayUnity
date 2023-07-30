using Cinemachine;
using GibFrame.Selectors;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Character
{
    public class CharacterCamera : CharacterComponent
    {
        private readonly List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        private DistanceSelector2D proximitySelector;
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
        private CinemachineTargetGroup targetGroup;
        private CinemachineTargetGroup.Target[] initialTargets;

        //public void ClientShakeCamera(ShakeStrenght strenght)
        //{
        //    var shake = GetShakeByStrenght(strenght);
        //    if (!Enabled) return;

        //    noise.m_NoiseProfile = shake.Profile;
        //    noise.m_AmplitudeGain = shake.Amplitude;
        //    noise.m_FrequencyGain = shake.Frequency;
        //    shakeDuration = shake.Duration;
        //    //Vector2 screenPos = mainCamera.WorldToViewportPoint(shake.WorldPos);
        //    //if (screenPos.x > 0F && screenPos.x < 1F && screenPos.y > 0F && screenPos.y < 1F)
        //    //{
        //    //    noise.m_AmplitudeGain = shake.Params.Amplitude;
        //    //    noise.m_FrequencyGain = shake.Params.Frequency;
        //    //    shakeDuration = shake.Params.Duration;
        //    //}
        //}

        protected override void OnEnable()
        {
            base.OnEnable();
            Cursor.visible = false;
            proximitySelector.Detected += OnDetected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Cursor.visible = true;
            proximitySelector.Detected -= OnDetected;
            ClearTargetGroup();
        }

        protected override void Awake()
        {
            base.Awake();
            proximitySelector = GetComponent<DistanceSelector2D>();
            targetGroup = cinemachineCamera.Follow.GetComponent<CinemachineTargetGroup>();
            initialTargets = targetGroup.m_Targets;
        }

        private void ClearTargetGroup()
        {
            foreach (var target in targetGroup.m_Targets)
            {
                if (target.target.Equals(transform)) continue;
                targetGroup.RemoveMember(target.target);
            }
        }

        private void OnDetected(IEnumerable<Collider2D> objs)
        {
            // Add the targets set in the editor (ourselves and others)
            targets.Clear();
            targets.AddRange(initialTargets);

            // Add all elements of interest
            foreach (var obj in objs)
            {
                if (obj.TryGetComponent<IElementOfInterest>(out var element) && element.Visible)
                {
                    targets.Add(new CinemachineTargetGroup.Target()
                    {
                        target = obj.transform,
                        weight = Mathf.Clamp01(element.Importance),
                        radius = 0
                    });
                }
            }
            // Set the targets of our CinemachineTargetGroup
            targetGroup.m_Targets = targets.ToArray();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Cursor.visible = false;
            }
        }
    }
}