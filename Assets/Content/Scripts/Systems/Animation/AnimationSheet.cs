using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Systems.Animation
{
    [Serializable]
    public class AnimationEvent
    {
        [SerializeField] private string id;
        [SerializeField] private int[] frameTriggers;

        public string Id => id;

        public int[] FrameTriggers => frameTriggers;
    }

    [Serializable]
    public class Animation
    {
        [SerializeField] private string name;
        [SerializeField] private int length;
        [SerializeField] private int fps = 8;
        [SerializeField] private bool loop = true;
        [SerializeField] private AnimationEvent[] events;

        public int Length => length;

        public bool Loop => loop;

        public AnimationEvent[] Events => events;

        public int Fps => fps;

        public string Name => name;
    }

    [CreateAssetMenu(fileName = "animation_sheet", menuName = "Fray/Animation/Sheet")]
    public class AnimationSheet : ScriptableObject
    {
        [SerializeField] private Texture2D sheet;
        [SerializeField] private int blockSize = 32;
        [SerializeField] private int pixelPerUnit = 12;
        [SerializeField] private string rotatedSuffix = "_mirror";
        [SerializeField] private List<Animation> animations;
        [SerializeField] private Dictionary<string, float> test;

        public string RotatedSuffix => rotatedSuffix;

        public bool GetFrame(string name, int count, out Sprite frame, out Animation anim)
        {
            frame = null;
            anim = null;
            var index = animations.FindIndex(a => a.Name == name);
            if (index >= 0)
            {
                anim = animations[index];
                frame = Sprite.Create(sheet, new Rect(count * blockSize, sheet.height - (index + 1) * blockSize, blockSize, blockSize), new Vector2(0.5F, 0.5F), pixelPerUnit);
                return true;
            }
            return false;
        }
    }
}