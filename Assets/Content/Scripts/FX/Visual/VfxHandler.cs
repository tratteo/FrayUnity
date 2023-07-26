using GibFrame;
using GibFrame.Extensions;
using System;
using UnityEngine;
using Validatox.Meta;

namespace Fray.FX
{
    /// <summary>
    ///   Manages a visual effect in a simple way. Use <see cref="Options"/> to specify how to display the effect.
    /// </summary>
    [Serializable]
    public partial class VfxHandler : IDisposable, IOptional
    {
        [SerializeField, Guard] private Vfx fx;
        private GameObject cachedFx;
        private ParticleSystem system;
        private ParticleSystem prefabSystem;
        private ParticleSystemRenderer systemRenderer;
        private ParticleSystem.MainModule mainModule;
        private ParticleSystem.ShapeModule shapeModule;
        private Vector3 prefabOffsetPos;
        private Vector3 prefabScale;
        private Color prefabColor;
        private bool cached = false;

        public bool IsCached => cached;

        public bool HasReference => fx;

        public GameObject CachedObject => cachedFx;

        public bool IsPlaying => cached && system && system.isPlaying;

        public ParticleSystem GetPrefabSystem()
        {
            if (!HasReference) return null;
            if (!prefabSystem)
            {
                prefabSystem = fx.Prefab.GetComponent<ParticleSystem>();
            }
            return prefabSystem;
        }

        /// <summary>
        ///   Stop the effect
        /// </summary>
        /// <param name="hardStop"> Whether to hard stop the effect (disable gameObject) </param>
        public void Stop(bool hardStop = false)
        {
            if (!cached || !system) return;
            if (hardStop)
                cachedFx?.SetActive(false);
            system?.Stop();
        }

        /// <summary>
        ///   Display the effect
        /// </summary>
        /// <param name="caster"> </param>
        public void Display(MonoBehaviour caster) => Display(caster, new Options());

        /// <summary>
        ///   Display the effect
        /// </summary>
        /// <param name="caster"> </param>
        /// <param name="options"> </param>
        public void Display(MonoBehaviour caster, Options options)
        {
            if (!Cache(caster)) return;

            // Attachment
            switch (options.attachment.parentAttachment)
            {
                case Options.Attachment.ParentAttachment.Caster:
                    cachedFx.transform.SetParent(caster.transform);
                    break;

                case Options.Attachment.ParentAttachment.Custom:
                    if (options.attachment.customParent)
                    {
                        cachedFx.transform.SetParent(options.attachment.customParent);
                    }
                    else
                    {
                        cachedFx.transform.SetParent(caster.transform);
                    }
                    break;

                case Options.Attachment.ParentAttachment.None:
                    cachedFx.transform.SetParent(null);
                    break;

                case Options.Attachment.ParentAttachment.Socket:
                    var networkSocket = options.attachment.customParent.GetComponent<VfxSocket>();
                    networkSocket.Attach(this);
                    break;
            }

            // Spatial
            var pos = new Vector3(caster.transform.localScale.x * prefabOffsetPos.x, prefabOffsetPos.y, prefabOffsetPos.z);
            cachedFx.transform.localPosition = options.spatial.position + pos;
            cachedFx.transform.localScale = (options.spatial.scale != null ? (Vector3)options.spatial.scale : prefabScale).AsPositive();
            cachedFx.transform.localRotation = Quaternion.identity;
            mainModule.simulationSpace = options.spatial.simulationSpace;
            mainModule.stopAction = options.spatial.stopAction;
            cachedFx.transform.Rotate(Vector3.forward, Vector3.SignedAngle(cachedFx.transform.right, options.spatial.direction, Vector3.forward));
            systemRenderer.flip = options.spatial.flip;

            mainModule.startColor = options.appearance.color is not null ? (ParticleSystem.MinMaxGradient)(Color)options.appearance.color : (ParticleSystem.MinMaxGradient)prefabColor;

            // Shape
            if (options.shape is not null)
            {
                shapeModule.shapeType = options.shape.shapeType;
                shapeModule.radius = options.shape.circleRadius;
                shapeModule.scale = options.shape.rectangleScale;
            }
            cachedFx.SetActive(true);
            system.Play();
        }

        /// <summary>
        ///   Clear out the effect and apply the selected <see cref="DisposePolicy"/>
        /// </summary>
        public void Dispose()
        {
            if (!cached) return;

            if (!IsPlaying)
            {
                UnityEngine.Object.Destroy(cachedFx);
                cached = false;
                return;
            }
            switch (fx.Policy)
            {
                case Vfx.DisposePolicy.Soft:
                    var attach = GetReplacementAttachment();
                    cachedFx.transform.SetParent(attach);
                    mainModule.stopAction = ParticleSystemStopAction.Destroy;
                    break;

                case Vfx.DisposePolicy.Hard:
                    UnityEngine.Object.Destroy(cachedFx);
                    cached = false;
                    break;
            }
            if (mainModule.loop)
            {
                Stop();
            }
        }

        /// <summary>
        ///   Cache (initialize) the effect and dependencies. This method is automatically called by <see cref="Display"/> each time and
        ///   cached only if not already cached
        /// </summary>
        /// <param name="owner"> </param>
        /// <returns> </returns>
        public bool Cache(MonoBehaviour owner)
        {
            if (cached) return true;
            if (!fx) return false;
            cached = true;
            cachedFx = UnityEngine.Object.Instantiate(fx.Prefab);
            system = cachedFx.GetComponent<ParticleSystem>();
            systemRenderer = cachedFx.GetComponent<ParticleSystemRenderer>();
            mainModule = system.main;
            shapeModule = system.shape;
            prefabScale = fx.Prefab.transform.localScale;
            prefabOffsetPos = fx.Prefab.transform.localPosition;
            prefabColor = mainModule.startColor.color;
            cachedFx.SetActive(false);
            return true;
        }

        public bool HasValue() => fx;

        private Transform GetReplacementAttachment()
        {
            return fx.Attachment switch
            {
                Vfx.ReplaceAttachment.None => null,
                Vfx.ReplaceAttachment.Root => cachedFx.transform.root,
                Vfx.ReplaceAttachment.Unchanched => cachedFx.transform.parent,
                _ => null,
            };
        }
    }
}