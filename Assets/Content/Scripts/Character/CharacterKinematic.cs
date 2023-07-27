using Fray.Character.Input;
using Fray.Extensions;
using Fray.FX;
using Fray.Systems;
using GibFrame;
using GibFrame.Extensions;
using GibFrame.Performance;
using System;
using UnityEngine;

namespace Fray.Character
{
    public class CharacterKinematic : CharacterComponent, IManagedRigidbody, IResourceCooldownOwner
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
        [Range(0F, 1F), SerializeField] private float forceDampening = 0.1F;
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

        private Vector2 traslation;
        private Vector3 externalVelocity = Vector2.zero;
        private float distanceTraveled = 0F;
        private float lastUpdateStepDistance = 0F;
        private Vector3 lastPos = Vector3.zero;

        private int dodgesCharges = 0;
        private UpdateJob rechargeDodgesJob;

        private float dodgeTimer = 0F;

        public bool IsDodging => dodgeTimer > 0F;

        public void Move(Vector2 direction) => traslation = movementSpeed * direction.normalized;

        /// <summary>
        ///   +1 if facing in the same direction of the vector, -1 otherwise
        /// </summary>
        public float GetRelativeTraslationSign(Vector3 vector) => Mathf.Sign(transform.localScale.x * vector.x);

        public Vector3 AddExternalForce(Vector3 force) => externalVelocity += force;

        public int GetResourcesAmount() => dodgesCharges;

        public float GetCooldown() => dodgeCooldown;

        public float GetCooldownPercentage() => dodgesCharges >= dodgesCount ? 1 : rechargeDodgesJob.GetUpdateProgress();

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
                    dashVfxOpt.Value.spatial.direction = externalVelocity;
                    dashVfx.Try(vfx => vfx.Display(this, dashVfxOpt.Value));
                    dashVfx.Try(vfx => vfx.Display(this, dashVfxOpt.Value));
                    dashSfx.Try(sfx => sfx.Play(this));
                    if (GetRelativeTraslationSign(direction) > 0F)
                    {
                        Animator?.DriveAnimation(new AnimatorDriverData(AnimatorDriverSystem.DashForward));
                    }
                    else
                    {
                        Animator?.DriveAnimation(new AnimatorDriverData(AnimatorDriverSystem.DashBackward));
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

            // Kinematic animations
            else if (Mathf.Approximately(traslation.magnitude, 0F) || !Enabled)
            {
                Animator?.DriveAnimation(new AnimatorDriverData(AnimatorDriverSystem.Idle));
            }
            else
            {
                Animator?.DriveAnimation(new AnimatorDriverData(AnimatorDriverSystem.Run, false, GetRelativeTraslationSign(traslation)));
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
        }

        protected void Start()
        {
            crosshair.SetParent(null);
            dodgesCharges = dodgesCount;
            if (Animator)
            {
                Animator.FootstepEvent += () => footstepsSfx.Try(s => s.Play(this));
            }
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
            // Slowly reduce the external force
            externalVelocity = Vector2.Lerp(externalVelocity, Vector2.zero, forceDampening);
            Rigidbody.velocity = externalVelocity;

            // Prevent direction change during dodge
            if (Enabled && !IsDodging)
            {
                Rigidbody.velocity += traslation;
            }
        }
    }
}