using Fray.Npc.Pathfinding;
using Fray.Systems;
using Fray.Systems.Animation;
using GibFrame;
using GibFrame.Performance;
using GibFrame.Selectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fray.Npc
{
    public abstract class Npc : MonoBehaviour, IAnimationDataSource, ITargetOwner
    {
        [SerializeField] private float movementSpeed = 5F;
        [SerializeField] private float nextWaypointDistance = 0.2F;
        [Header("Advanced")]
        [SerializeField] private float pathUpdateInterval = 0.75F;
        private UpdateJob computePathJob;

        private Vector2[] currentPath;

        private int currentWaypointIndex = 0;

        protected Pathfinder Pathfinder { get; private set; }

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

        protected virtual void OnPathComplete(Vector2[] points, bool success)
        {
            if (success)
            {
                currentPath = points;
                currentWaypointIndex = 0;
            }
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
                if (currentWaypointIndex < currentPath.Length)
                {
                    ManagedRb.Traslation = movementSpeed * (currentPath[currentWaypointIndex] - Rigidbody.position).normalized;
                    float distance = Vector2.Distance(Rigidbody.position, currentPath[currentWaypointIndex]);
                    if (distance < nextWaypointDistance) currentWaypointIndex++;
                    if (currentWaypointIndex >= currentPath.Length) ManagedRb.Traslation = Vector2.zero;
                }
            }
        }

        protected virtual void Awake()
        {
            Pathfinder = GetComponent<Pathfinder>();
            TargetSelector = GetComponent<DistanceSelector2D>();
            ManagedRb = GetComponent<ManagedRigidbody>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        protected void DriveAnimation(AnimatorDriverData data) => AnimatorDriverDataEvent?.Invoke(data);

        protected virtual void OnDetected(IEnumerable<Collider2D> enumerable)
        {
        }

        protected virtual Vector3 GetTargetPosition() => Pathfinder.GetClosestCellTo(Target.transform.position).WorldPos;

        private void OnDrawGizmos()
        {
            if (currentPath != null && currentPath.Length > 0)
            {
                for (int i = currentWaypointIndex; i < currentPath.Length; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawCube(currentPath[i], Vector3.one * .5F);

                    if (i == currentWaypointIndex) Gizmos.DrawLine(transform.position, currentPath[i]);
                    else Gizmos.DrawLine(currentPath[i - 1], currentPath[i]);
                }
            }
        }

        private void ComputePath()
        {
            Debug.Log("Computing path");
            if (Target && Pathfinder)
            {
                var cell = Pathfinder.GetClosestCellTo(transform.position);
                Pathfinder.Pathfind(new PathRequest(cell.WorldPos, GetTargetPosition(), OnPathComplete));
            }
            computePathJob.EditUpdateTime(pathUpdateInterval);
        }
    }
}