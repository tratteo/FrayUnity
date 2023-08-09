using GibFrame;
using GibFrame.Performance;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fray.Systems.Animation
{
    public static class Animations
    {
        public const string Idle = "idle";
        public const string Run = "run";
        public const string DashForward = "dash_f";
        public const string DashBackward = "dash_b";
    }

    public class AnimationDriver : MonoBehaviour
    {
        [SerializeField] private AnimationSheet sheet;
        private SpriteRenderer spriteRenderer;

        private int currentIndex = 0;
        private UpdateJob frameJob;
        private string currentAnim = "idle";
        private StringBuilder animIdBuilder;
        private IAnimationDataSource dataSource;

        public void DriveAnimation(AnimatorDriverData data)
        {
            var animId = data.id;
            animIdBuilder.Clear();
            animIdBuilder.Append(animId);
            if (data.mirrored)
            {
                animIdBuilder.Append(sheet.RotatedSuffix);
            }

            animId = animIdBuilder.ToString();
            //if (data.id is Run)
            //{
            //    var speed = data.GetArgAs<float>();
            //    animator.SetFloat(speedHash, speed);
            //}
            if (currentAnim == animId) return;
            currentAnim = animId;
            currentIndex = 0;
        }

        private void Awake()
        {
            animIdBuilder = new StringBuilder();
            spriteRenderer = GetComponent<SpriteRenderer>();
            dataSource = GetComponent<IAnimationDataSource>();
            frameJob = new UpdateJob(new Callback(FrameJob), 1 / 8);
        }

        private void FrameJob()
        {
            if (sheet.GetFrame(currentAnim, currentIndex, out var frame, out var anim))
            {
                spriteRenderer.sprite = frame;
                currentIndex += 1;
                if (anim.Loop)
                {
                    currentIndex %= anim.Length;
                }
                else
                {
                    currentIndex = Mathf.Clamp(currentIndex, 0, anim.Length - 1);
                }
                foreach (var ev in anim.Events)
                {
                    if (ev.FrameTriggers.Contains(currentIndex))
                    {
                        dataSource.OnTimelineEvent(ev.Id);
                    }
                }

                frameJob.EditUpdateTime(1F / anim.Fps);
            }
        }

        private void Start()
        {
            dataSource.AnimatorDriverDataEvent += DriveAnimation;
        }

        private void Update()
        {
            frameJob.Step(Time.deltaTime);
        }
    }
}