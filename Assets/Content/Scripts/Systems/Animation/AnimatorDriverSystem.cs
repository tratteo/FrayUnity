using System;
using System.Text;
using UnityEngine;

namespace Fray.Systems.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorDriverSystem : MonoBehaviour
    {
        public const string Run = "run";
        public const string Idle = "idle";
        public const string DashForward = "dash_f";
        public const string DashBackward = "dash_b";
        private Animator animator;

        [SerializeField, Range(0, 10), Tooltip("The layer in which the animation clips are placed")]
        private int animationLayerIndex = 0;
        [SerializeField, Tooltip("Animations can be mirrored in order to show different sides. This string represents the suffix that will be added to the animation name in order to look for the mirrored one")]
        private string rotatedSuffix = "_mirror";

        private int speedHash;
        private string currentPlaying;
        private StringBuilder animIdBuilder;
        private IAnimationDataSource dataSource;

        public void DriveAnimation(AnimatorDriverData data)
        {
            var animId = data.id;
            animIdBuilder.Clear();
            animIdBuilder.Append(animId);
            if (data.mirrored)
            {
                animIdBuilder.Append(rotatedSuffix);
            }

            animId = animIdBuilder.ToString();
            if (data.id is Run)
            {
                var speed = data.GetArgAs<float>();
                animator.SetFloat(speedHash, speed);
            }
            if (currentPlaying == animId) return;
            animator.Play(animId, animationLayerIndex, 0);
            //animator.CrossFade(animId, 1F, animationLayerIndex);
            currentPlaying = animId;
        }

        public void Footstep()
        {
            if (currentPlaying != Run) return;
            dataSource.OnTimelineEvent(TimelineEvent.Footsteps);
        }

        private void Awake()
        {
            animIdBuilder = new StringBuilder();
            animator = GetComponent<Animator>();
            dataSource = GetComponent<IAnimationDataSource>();
            speedHash = Animator.StringToHash("Speed");
        }

        private void Start()
        {
            dataSource.AnimatorDriverDataEvent += DriveAnimation;
        }
    }
}