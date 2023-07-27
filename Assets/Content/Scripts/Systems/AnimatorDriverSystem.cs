using System;
using System.Text;
using UnityEngine;

namespace Fray.Systems
{
    public readonly struct AnimatorDriverData
    {
        public readonly string id;

        public readonly object arg;

        public readonly bool mirrored;

        public AnimatorDriverData(string id, bool horizontalRotated = false, object args = null)
        {
            this.mirrored = horizontalRotated;
            this.id = id;
            arg = args;
        }

        public T GetArgAs<T>() => (T)arg;
    }

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

        public event Action FootstepEvent = delegate { };

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
            FootstepEvent?.Invoke();
        }

        private void Awake()
        {
            animIdBuilder = new StringBuilder();
            animator = GetComponent<Animator>();
            speedHash = Animator.StringToHash("Speed");
        }
    }
}