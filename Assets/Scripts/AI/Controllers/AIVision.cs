using AI.Agents;
using AI.Common;
using AI.Enums;
using AI.Events;
using Codice.Client.BaseCommands;
using Core.Common.Interfaces;
using Core.Utilities;
using Core.Utilities.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Controllers
{
    public class AIVision : MonoBehaviour, ICleanable
    {
        private const float DEFAULT_LOOK_AT_DEPTH = 3;
        private readonly Faction[] DEFAULT_SCAN_FACTION_FILTER = (Faction[]) Enum.GetValues(typeof(Faction));

        private TickTask scanTask;
        private Faction[] scanFactionFilter;
        private Dictionary<string, ScannedTarget> targets;
        private Transform lookAtTarget;

        [Header("General Settings")]
        [SerializeField] private Transform head;
        [SerializeField][Range(100, 2000)] private int refreshRate = 500;
        [SerializeField] private float visionRange = 10f;
        [SerializeField] private float fieldOfView = 130f;
        [Header("Look At Settings")]
        [SerializeField] private Transform lookAtAim;
        [SerializeField] private float lookAtSpeed = 3f;
        [SerializeField] private float lookAtHeight = 1f;

        private event EventHandler<OnTargetsUpdateArgs> onTargetsUpdate;
        private event EventHandler<OnTargetFoundArgs> onTargetFound;
        private event EventHandler<OnTargetLostArgs> onTargetLost;

        public List<ScannedTarget> Targets => targets.Values.ToList();
        public float VisionRange => visionRange;
        public bool HasTargets => targets.Count > 0;
        public Faction[] ScanFactionFilter
        {
            get => scanFactionFilter;
            set => scanFactionFilter = value;
        }

        private void Awake()
        {
            ScanFactionFilter = DEFAULT_SCAN_FACTION_FILTER;
        }

        private void Start()
        {
            // Setup state
            targets = new Dictionary<string, ScannedTarget>();
            scanTask = new TickTask(refreshRate, () => Scan());
            scanTask.Start();

            ResetLookAtTarget();
        }

        private void Update()
        {
            RefreshLookAt();
        }

        /// <summary>
        /// Scans the environment and populates the Target list
        /// <para> Only validates objects within the field of view and if not obstructed </para>
        /// <para> Called upon every Scan Task tick </para>
        /// <para> Checks if new targets were added or if targets got lost, launching an event upon that action </para>
        /// <param name="tags"> tags to inlcude in the scan. If null, use default tags list </param> 
        /// </summary>
        public void Scan()
        {
            var lostTargets = new List<ScannedTarget>(Targets);
            var newTargets = new List<ScannedTarget>();
            var allTargets = Physics.OverlapSphere(transform.position, visionRange, LayerMask.GetMask("Player", "Agent"));

            // Refresh already scanned targets and add new ones
            foreach (var target in allTargets)
            {
                Vector3 dir = (target.transform.position - head.position).normalized;
                float distance = Vector3.Distance(head.position, target.transform.position);
                float angle = Vector3.Angle(head.forward, dir);

                // Extract SimAgent and faction
                Debug.DrawRay(head.position, dir * distance, Color.red, 2f);
                bool canSee = Physics.Raycast(head.position, dir, out var hit, distance);
                bool isSimAgent = hit.collider.TryGetComponent(out SimulationAgent agent);
                if (!canSee || !isSimAgent || angle >= fieldOfView || !scanFactionFilter.Contains(agent.Faction)) continue;

                // Aldready scanned the target...
                if (targets.ContainsKey(agent.ID))
                {
                    ScannedTarget existingTarget = targets[agent.ID];
                    existingTarget.Refresh(distance);
                    lostTargets.Remove(existingTarget);
                    continue;
                }

                // Scan new target
                var newTarget = new ScannedTarget(distance, agent);
                targets.Add(newTarget.Agent.ID, newTarget);
                newTargets.Add(newTarget);
            }

            // Remove targets lost
            foreach (var target in lostTargets)
            {
                targets.Remove(target.Agent.ID);
            }

            // Raise event for the discovery/removal of targets
            if (newTargets.Count > 0 || lostTargets.Count > 0)
            {
                RaiseOnTargetsUpdate(newTargets, lostTargets);
            }
        }

        /// <summary>
        /// Returns the closest target given an array of tags. If no tags are given, uses the components defined scan tags
        /// </summary>
        /// <param name="tags"> tags to compare to </param>
        /// <returns> the closest target with a tag belonging to the given array. If no array is given, just the closest </returns>
        public ScannedTarget GetClosestTarget(params Faction[] factions)
        {
            if (targets.Count == 0) return null;
            if (factions.Length == 0)
            {
                factions = scanFactionFilter;
            }

            ScannedTarget closestTarget = Targets[0];
            float minDistance = float.MaxValue;
            foreach (var (_, target) in targets)
            {
                if (target.Distance < minDistance && factions.Contains(closestTarget.Agent.Faction))
                {
                    minDistance = target.Distance;
                    closestTarget = target;
                }
            }
            return closestTarget;
        }

        /// <summary>
        /// Sets a new look at target. If set, the AI continuously looks at that place
        /// </summary>
        /// <param name="lookAtTarget"> new lookAt target </param>
        public void SetLookAtTarget(Transform lookAtTarget)
        {
            this.lookAtTarget = lookAtTarget;
        }

        /// <summary>
        /// Sets the look at target to null. Makes the AI look forward
        /// </summary>
        public void ResetLookAtTarget()
        {
            lookAtTarget = null;
        }

        /// <summary>
        /// Creates the look at animation, by changing the look at aim's position
        /// </summary>
        private void RefreshLookAt()
        {
            // Get the default looAt target (in front of the eyes)
            var defaultLookAt = transform.position + 
                transform.forward * DEFAULT_LOOK_AT_DEPTH + 
                transform.up * lookAtHeight;

            // Defines the position where the AI should look at
            var lookAtPos = lookAtTarget == null ?  defaultLookAt : lookAtTarget.position;

            // Update the lookAt aim
            lookAtAim.position = MathUtils.VectorLerp(lookAtAim.position, lookAtPos, lookAtSpeed, Time.deltaTime);
        }


        /* ---------- EVENT METHODS ---------- */

        public void AddOnTargetFoundHandler(EventHandler<OnTargetFoundArgs> handler) => onTargetFound += handler;
        public void AddOnTargetLostHandler(EventHandler<OnTargetLostArgs> handler) => onTargetLost += handler;
        public void AddOnTargetsUpdateHandler(EventHandler<OnTargetsUpdateArgs> handler) => onTargetsUpdate += handler;
        public void RemoveOnTargetFoundHandler(EventHandler<OnTargetFoundArgs> handler) => onTargetFound -= handler;
        public void RemoveOnTargetLostHandler(EventHandler<OnTargetLostArgs> handler) => onTargetLost -= handler;
        public void RemoveOnTargetsUpdateHandler(EventHandler<OnTargetsUpdateArgs> handler) => onTargetsUpdate -= handler;
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
        protected void RaiseOnTargetsUpdate(List<ScannedTarget> newTargets, List<ScannedTarget> lostTargets) => onTargetsUpdate?.Invoke(this, new OnTargetsUpdateArgs
        {
            NewTargets = newTargets,
            LostTargets = lostTargets
        });

        public void CleanUp()
        {
            onTargetFound = null;
            onTargetLost = null;

            scanTask.Stop();
            scanTask = null;
        }
    }
}