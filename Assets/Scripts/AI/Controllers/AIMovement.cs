using AI.Enums;
using AI.Events;
using Core.Common.Interfaces;
using Core.Utilities;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI.Controllers
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class AIMovement : MonoBehaviour, ICleanable
    {
        /// <summary>
        /// The ammount of retries for searching a new random point
        /// </summary>
        public const int POINT_SEARCH_RETRY = 10;

        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float runSpeed = 3.5f;

        private float samplePosMaxDistance;     // This is always 2 * navMeshAgent.height, as recommended in the documentation
        private Vector3 lastPos;
        private NavMeshAgent navMeshAgent;

        private event EventHandler<OnArriveArgs> onArrive;

        public float CurrentSpeed { get; private set; }

        private void Start()
        {
            // Get components
            navMeshAgent = GetComponent<NavMeshAgent>();

            // Define state
            samplePosMaxDistance = 2 * navMeshAgent.height;
            lastPos = transform.position;
        }

        private void Update()
        {
            // Calculate and update current speed stat
            CurrentSpeed = (transform.position - lastPos).magnitude / Time.deltaTime;

            // Keep at last!
            lastPos = transform.position;
        }

        /// <summary>
        /// Creates a new random position given an origin point and a search radius. Only points that are in the NavMesh are valid. 
        /// <para> Retries POINT_SEARCH_RETRY times to improve the probability of finding a new valid random point. </para>
        /// </summary>
        /// <param name="range"> max range/radius for the new point </param>
        /// <param name="origin"> anchor point for the search circle </param>
        /// <returns> a new random point if it is in the NavMesh, 'origin' if it is not </returns>
        public Vector3 CreateRandomPosition(float range, Vector3 origin)
        {
            for (int i = 0; i < POINT_SEARCH_RETRY; i++)
            {
                Vector3 randomPoint = origin + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, samplePosMaxDistance, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return origin;
        }

        /// <summary>
        /// Create a new random position given a range, with the center being on the current GameObject's position.
        ///     Only points that are in the NavMesh are valid. 
        /// <para> Retries POINT_SEARCH_RETRY times to improve the probability of finding a new valid random point. </para>
        /// </summary>
        /// <param name="range">max range for the new poinst</param>
        /// <returns> new point if it's in the NavMesh, GameObject's position if it is not in the NavMesh </returns>
        public Vector3 CreateRandomPosition(float range)
        {
            return CreateRandomPosition(range, transform.position);
        }


        /// <summary>
        /// Moves the AI agent to a given point 
        /// </summary>
        /// <param name="pos"> position to move to </param>
        /// <param name="moveState"> state of the current AI agent </param>
        public void MoveTo(Vector3 pos, AIMoveState moveState)
        {
            if (!navMeshAgent.isActiveAndEnabled)
            {
                Log.Warning(this, "MoveTo", "NavMeshAgent is not active or enabled");
                return;
            }

            navMeshAgent.speed = SpeedByMoveState(moveState);
            if (!navMeshAgent.SetDestination(pos))
            {
                Log.Warning(this, "MoveTo", "SetDestination was called for an impossible position!");
            }
        }

        /// <summary>
        /// Returnst the correct speed given an AI Move State
        /// </summary>
        /// <param name="state"> the state to select the speed from </param>
        /// <returns> speed for the given state </returns>
        public float SpeedByMoveState(AIMoveState state)
        {
            switch (state)
            {
                case AIMoveState.Walk:
                    return walkSpeed;
                case AIMoveState.Run:
                    return runSpeed;
                default:
                    Log.Warning(this, "SpeedByMoveState", "Unkown AIMoveState");
                    return walkSpeed;
            }
        } 

        /* Event stuff */
        public void AddOnArriveHandler(EventHandler<OnArriveArgs> handler) => onArrive += handler;
        protected void RaiseOnArrive(Vector3 position) => onArrive?.Invoke(this, new OnArriveArgs { Position = position });

        public void CleanUp()
        {
            onArrive = null;
        }
    }
}