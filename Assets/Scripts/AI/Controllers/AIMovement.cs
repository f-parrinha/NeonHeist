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
        public const int POINT_SEARCH_RETRY = 10;

        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float runSpeed = 3.5f;

        private NavMeshAgent navMeshAgent;

        private event EventHandler<OnArriveArgs> onArrive;


        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public Vector3 CreateRandomPosition(float range)
        {
            Vector3 origin = transform.position;

            for (int i = 0; i < POINT_SEARCH_RETRY; i++)
            {
                Vector3 randomPoint = origin + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, range, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return origin;
        }

        public void MoveTo(Vector3 pos, AIMoveState moveState)
        {
            if (navMeshAgent.isActiveAndEnabled)
            {
                navMeshAgent.speed = SpeedByMoveState(moveState);
                navMeshAgent.SetDestination(pos);
            }
        }


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