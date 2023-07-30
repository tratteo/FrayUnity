using Fray.Character.Input;
using Fray.Extensions;
using Fray.FX;
using Fray.Systems.Animation;
using GibFrame;
using GibFrame.Extensions;
using GibFrame.Performance;
using System;
using UnityEngine;

namespace Fray.Character
{
    [RequireComponent(typeof(IManagedRigidbody))]
    public class CharacterKinematic : CharacterComponent, IResourceCooldownOwner, IAnimationDataSource
    {
        private readonly Lazy<VfxHandler.Options> footstepsVfxOpt = new Lazy<VfxHandler.Options>(() => new VfxHandler.Options()
        {
            spatial = new VfxHandler.Options.Spatial() { simulationSpace = ParticleSystemSimulationSpace.World },
            attachment = new VfxHandler.Options.Attachment() { parentAttachment = VfxHandler.Options.Attachment.ParentAttachment.None }
        });
        private readonly Lazy<VfxHandler.Options> dashVfxOpt = new Lazy<VfxHandler.Options>(() => new VfxHandler.Options()
        {
            spatial = new VfxHandler.Options.Spatial() { simulationSpace = ParticleSystemSimulationSpace.World },
            attachment = new VfxHandler.Options.Attachment() { parentAttachment = VfxHandler.Options.Attachment.ParentAttachment.None }
        });
        [Header("Physics")]
        [SerializeField] private float movementSpeed = 5F;
        [Header("Dodge")]
        [SerializeField] private float staminaRequired = 10;
        [SerializeField] private float dodgeForce = 250F;
        [SerializeField] private float dodgeDuration = 0.275F;
        [SerializeField] private int dodgesCount = 3;
        [SerializeField] private float dodgeCooldown = 4F;
        [Header("Look")]
        [SerializeField] private Transform crosshair;
        [Header("FX")]
        [SerializeField] private Optional<VfxHandler> dashVfx;
        [SerializeField] private Optional<SfxHandler> dashSfx;
        [SerializeField] private Optional<VfxHandler> footstepsVfx;
        [SerializeField] private Optional<SfxHandler> footstepsSfx;

        private float distanceTraveled = 0F;
        private float lastUpdateStepDistance = 0F;
        private Vector3 lastPos = Vector3.zero;

        private int dodgesCharges = 0;
        private UpdateJob rechargeDodgesJob;
        private ManagedRigidbody managedRb;

        private float dodgeTimer = 0F;

        public bool IsDodging => dodgeTimer > 0F;

        public event Action<AnimatorDriverData> AnimatorDriverDataEvent = delegate { };

        public void Move(Vector2 direction) => managedRb.Traslation = movementSpeed * direction.normalized;

        /// <summary>
        ///   +1 if facing in the same direction of the vector, -1 otherwise
        /// </summary>
        public float GetRelativeTraslationSign(Vector3 vector) => Mathf.Sign(transform.localScale.x * vector.x);

        public int GetResourcesAmount() => dodgesCharges;

        public float GetCooldown() => dodgeCooldown;

        public float GetCooldownPercentage() => dodgesCharges >= dodgesCount ? 1 : rechargeDodgesJob.GetUpdateProgress();

        public System.Numerics.Vector2 GetTraslation() => throw new NotImplementedException();

        public void OnTimelineEvent(TimelineEvent ev)
        {
            switch (ev)
            {
                case TimelineEvent.Footsteps:
                    footstepsSfx.Try(s => s.Play(this));
                    break;
            }
        }

        protected override void OnInput(CharacterInputData data)
        {
            if (!Enabled) return;
            Vector2 direction;
            switch (data.action)
            {
                case CharacterAction.Move:
                    direction = data.CastPayload<Vector2>();
                    Move(direction);
                    break;

                case CharacterAction.Look:
                    var pos = data.CastPayload<Vector2>();
                    crosshair.localPosition = pos;
                    var currScale = transform.localScale.AsPositive();
                    currScale.x = crosshair.position.x > transform.position.x ? currScale.x * 1F : currScale.x * -1F;
                    transform.localScale = currScale;
                    break;

                case CharacterAction.Dodge:

                    direction = data.CastPayload<Vector2>();
                    if (!CanDodge() || Mathf.Approximately(direction.magnitude, 0F)) return;

                    Dodge(direction);
                    dashVfxOpt.Value.spatial.position = transform.position;
                    dashVfxOpt.Value.spatial.direction = direction;
                    dashVfx.Try(vfx => vfx.Display(this, dashVfxOpt.Value));
                    dashVfx.Try(vfx => vfx.Display(this, dashVfxOpt.Value));
                    dashSfx.Try(sfx => sfx.Play(this));
                    if (GetRelativeTraslationSign(direction) > 0F)
                    {
                        AnimatorDriverDataEvent?.Invoke(new AnimatorDriverData(AnimatorDriverSystem.DashForward));
                    }
                    else
                    {
                        AnimatorDriverDataEvent?.Invoke(new AnimatorDriverData(AnimatorDriverSystem.DashBackward));
                    }
                    break;
            }
        }

        protected void Update()
        {
            // Dodge
            rechargeDodgesJob.Step(Time.deltaTime);
            if (IsDodging)
            {
                dodgeTimer -= Time.deltaTime;
                if (dodgeTimer <= 0)
                {
                    dodgeTimer = 0F;
                }
            }
            else if (Mathf.Approximately(managedRb.Traslation.magnitude, 0F) || !Enabled)
            {
                AnimatorDriverDataEvent?.Invoke(new AnimatorDriverData(AnimatorDriverSystem.Idle));
            }
            else
            {
                AnimatorDriverDataEvent?.Invoke(new AnimatorDriverData(AnimatorDriverSystem.Run, false, GetRelativeTraslationSign(managedRb.Traslation)));
            }

            if (lastUpdateStepDistance > (movementSpeed * Time.deltaTime / 1.75F))
            {
                if (UnityEngine.Random.value < 0.01)
                {
                    footstepsVfxOpt.Value.spatial.position = transform.position;
                    footstepsVfxOpt.Value.spatial.direction = Rigidbody.velocity;
                    footstepsVfxOpt.Value.spatial.flip = Rigidbody.velocity.x < 0 ? Vector3.zero : Vector3.right;
                    footstepsVfx.Try(v => v.Display(this, footstepsVfxOpt.Value));
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            rechargeDodgesJob = new UpdateJob(new Callback(RechargeDodge), dodgeCooldown);
            managedRb = GetComponent<ManagedRigidbody>();
        }

        protected void Start()
        {
            crosshair.SetParent(null);
            dodgesCharges = dodgesCount;
        }

        private bool CanDodge() => !IsDodging && dodgesCharges >= 1 && Stamina.Value >= staminaRequired;

        private void Dodge(Vector2 direction)
        {
            if (!CanDodge()) return;
            Stamina.Decrease(staminaRequired);
            Rigidbody.AddManagedForce2D(direction * dodgeForce);
            dodgeTimer = dodgeDuration;
            dodgesCharges--;
        }

        private void RechargeDodge()
        {
            if (dodgesCharges < dodgesCount)
            {
                dodgesCharges++;
            }
        }

        private void FixedUpdate()
        {
            lastUpdateStepDistance = Vector3.Distance(transform.position, lastPos);
            distanceTraveled += lastUpdateStepDistance;
            lastPos = transform.position;

            managedRb.EnableTraslation = Enabled && !IsDodging;
        }
    }
}