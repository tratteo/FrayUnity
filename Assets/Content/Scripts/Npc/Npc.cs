using Fray.Systems;
using Fray.Systems.Animation;
using GibFrame;
using GibFrame.Performance;
using GibFrame.Selectors;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Npc
{
    public abstract class Npc : MonoBehaviour, IAnimationDataSource, ITargetOwner
    {
        [SerializeField] private float movementSpeed = 5F;
        [SerializeField] private float nextWaypointDistance = 3F;
        [Header("Advanced")]
        [SerializeField] private float pathUpdateInterval = 0.5F;
        private UpdateJob computePathJob;

        private Seeker seeker;
        private Path currentPath;
        private int currentWaypointIndex = 0;

        protected ManagedRigidbody ManagedRb { get; private set; }

        protected HealthSystem Health { get; private set; }

        protected DistanceSelector2D TargetSelector { get; private set; }

        protected Rigidbody2D Rigidbody { get; private set; }

        protected GameObject Target { get; set; }

        public event Action<AnimatorDriverData> AnimatorDriverDataEvent = delegate { };

        public float GetRelativeTraslationSign(Vector3 vector) => Mathf.Sign(transform.localScale.x * vector.x);

        public virtual void OnTimelineEvent(string ev)
        {
        }

        public Transform GetTarget() => Target ? Target.transform : null;

        protected virtual void Start()
        {
            TargetSelector.Detected += OnDetected;
            computePathJob = new UpdateJob(new Callback(ComputePath), pathUpdateInterval, true);
        }

        protected virtual void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                currentPath = p;
                currentWaypointIndex = 0;
            }
        }

        protected void MoveTowards(Vector3 pos)
        {
            seeker.StartPath(transform.position, pos, OnPathComplete);
        }

        protected virtual void Update()
        {
            transform.localScale = !Target
                ? Rigidbody.velocityX < Mathf.Epsilon
                    ? new Vector3(-1F * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z)
                    : new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z)
                : Target.transform.position.x < transform.position.x
               ? new Vector3(-1F * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z)
               : new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            if (Mathf.Approximately(ManagedRb.Traslation.magnitude, 0F))
            {
                AnimatorDriverDataEvent?.Invoke(new AnimatorDriverData(Animations.Idle));
            }
            else
            {
                AnimatorDriverDataEvent?.Invoke(new AnimatorDriverData(Animations.Run, false, GetRelativeTraslationSign(ManagedRb.Traslation)));
            }
            computePathJob.Step(Time.deltaTime);
            if (currentPath != null)
            {
                if (currentWaypointIndex < currentPath.vectorPath.Count)
                {
                    ManagedRb.Traslation = movementSpeed * ((Vector2)currentPath.vectorPath[currentWaypointIndex] - Rigidbody.position).normalized;
                    float distance = Vector2.Distance(Rigidbody.position, currentPath.vectorPath[currentWaypointIndex]);
                    if (distance < nextWaypointDistance)
                    {
                        currentWaypointIndex++;
                    }
                }
            }
        }

        protected virtual void Awake()
        {
            seeker = GetComponent<Seeker>();
            TargetSelector = GetComponent<DistanceSelector2D>();
            ManagedRb = GetComponent<ManagedRigidbody>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        protected void DriveAnimation(AnimatorDriverData data) => AnimatorDriverDataEvent?.Invoke(data);

        protected virtual void OnDetected(IEnumerable<Collider2D> enumerable)
        {
        }

        protected virtual Vector3 GetTargetPosition() => Target.transform.position;

        private void ComputePath()
        {
            if (Target)
            {
                seeker.StartPath(transform.position, GetTargetPosition(), OnPathComplete);
            }
            computePathJob.EditUpdateTime(pathUpdateInterval);
        }
    }
}