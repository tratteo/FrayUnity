using System;

namespace Fray.Systems.Animation
{
    internal interface IAnimationDataSource
    {
        public event Action<AnimatorDriverData> AnimatorDriverDataEvent;

        public void OnTimelineEvent(TimelineEvent ev);
    }
}