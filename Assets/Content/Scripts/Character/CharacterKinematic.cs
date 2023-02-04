using Fray.Character.Input;
using Fray.Extensions;
using Fray.Systems;
using GibFrame;
using GibFrame.Extensions;
using GibFrame.Performance;
using UnityEngine;

namespace Fray.Character
{
    public class CharacterKinematic : CharacterComponent, IManagedRigidbody, IResourceCooldownOwner
    {
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

        private Vector2 traslation;
        private Vector3 externalVelocity = Vector2.zero;

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
        }

        private bool CanDodge() => !IsDodging && dodgesCharges >= 1 && Stamina.ValueSystem.Value >= staminaRequired;

        private void Dodge(Vector2 direction)
        {
            if (!CanDodge()) return;
            Stamina.ValueSystem.Decrease(staminaRequired);
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