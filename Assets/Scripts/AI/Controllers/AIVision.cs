using AI.Events;
using Core.Common.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System;
using UnityEditor;
using UnityEngine;

namespace AI.Controllers
{
    public class AIVision : MonoBehaviour, ICleanable
    {

        private TickTask findTargetTask;
        private Collider[] targets;

        private event EventHandler<OnTargetFoundArgs> onTargetFound;
        private event EventHandler<OnTargetLostArgs> onTargetLost;

        [SerializeField][Range(100, 2000)] private int refreshRate = 500;
        [SerializeField] private float visionRange = 10f;
        [SerializeField] private float fieldOfView = 130f;
        [SerializeField] private float soundRange = 1f;

        public Collider[] Targets => (Collider[])targets.Clone();
        public Collider ClosestTarget { get; private set; }
        public bool HasTarget => Targets.Length > 0;


        private void Start()
        {
            findTargetTask = new TickTask(refreshRate, () => Scan());
            findTargetTask.Start();
            targets = new Collider[0];
        }


        public void Scan()
        {
            targets = Physics.OverlapSphere(transform.position, visionRange, LayerUtils.PLAYER);

            if (targets.Length == 0)
            {
                if (ClosestTarget != null)
                {
                    RaiseOnTargetLost(ClosestTarget, ClosestTarget.transform.position);
                }

                ClosestTarget = null;
                return;
            }

            // Found something in range...
            ClosestTarget = GetClosestTarget(targets, out var minDistance);
            var dir = (ClosestTarget.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(dir, transform.forward);

            // Check for field of view and minimum range (due to sound?)
            if (angle > fieldOfView && minDistance > soundRange) return;

            // Found new target! (new closest)
            if (Physics.Raycast(transform.position, dir, minDistance))
            {
                RaiseOnTargetFound(ClosestTarget, ClosestTarget.transform.position, minDistance);
            }
        }

        private Collider GetClosestTarget(Collider[] targets, out float minDistance)
        {
            minDistance = 0f;
            if (targets.Length == 0) return null;

            Collider closest = targets[0];
            minDistance = Vector3.Distance(transform.position, closest.transform.position);
            foreach (Collider target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < minDistance)
                {
                    closest = target;
                    minDistance = distance;
                }
            }

            return closest;
        }


        public void AddOnTargetFoundHandler(EventHandler<OnTargetFoundArgs> handler) => onTargetFound += handler;
        public void AddOnTargetLostHandler(EventHandler<OnTargetLostArgs> handler) => onTargetLost += handler;
        public void RemoveOnTargetFoundHandler(EventHandler<OnTargetFoundArgs> handler) => onTargetFound -= handler;
        public void RemoveOnTargetLostHandler(EventHandler<OnTargetLostArgs> handler) => onTargetLost -= handler;
        protected void RaiseOnTargetFound(Collider target, Vector3 targetPos, float distance) => onTargetFound?.Invoke(this, new OnTargetFoundArgs
        {
            Target = target,
            TargetPosition = targetPos,
            Distance = distance
        });
        protected void RaiseOnTargetLost(Collider target, Vector3 targetPos) => onTargetLost?.Invoke(this, new OnTargetLostArgs
        {
            Target = target,
            TargetPosition = targetPos
        });

        public void CleanUp()
        {
            onTargetFound = null;
            onTargetLost = null;

            findTargetTask.Stop();
            findTargetTask = null;
        }
    }
}