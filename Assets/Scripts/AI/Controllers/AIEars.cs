using AI.Common;
using Core.Utilities;
using Core.Utilities.Timing;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Controllers
{
    public class AIEars : MonoBehaviour
    {
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private int scanDeltaTime = 500;

        // Keep this value at around the radius of the head

        private Collider[] targets;
        private TickTask scanTask;

        public IEnumerable<Collider> TargetsEnumerator => targets;
        public Collider ClosestTarget { get; private set; }
        public bool HasTargets => targets.Length > 0;

        private void Start()
        {
            scanTask = new TickTask(scanDeltaTime, Scan);
            scanTask.Start();

            targets = new Collider[0];
            ClosestTarget = null;
        }

        private void Scan()
        {
            targets = Physics.OverlapSphere(headCollider.transform.position, headCollider.radius, LayerMask.GetMask(LayerNames.SOUND));

            // Get closest target
            if (HasTargets) {

                ClosestTarget = targets[0];
                float minDistance = Vector3.Distance(headCollider.transform.position, ClosestTarget.transform.position);
                foreach (Collider target in targets)
                {
                    float distance = Vector3.Distance(headCollider.transform.position, target.transform.position);
                    if (distance < minDistance)
                    {
                        ClosestTarget = target;
                        minDistance = distance;
                    }
                }
            } else
            {
                ClosestTarget = null;
            }
        }
    }
}