using AI.Enums;
using AI.Events;
using AI.Goals;
using Core.Utilities;
using UnityEngine;

namespace AI.Agents
{
    public class SecurityGuardAgent : GenericEnemyAgent
    {
        private const float CLOSE_DISTANCE_POINT = 3f;

        private Vector3 initPos;

        // When the AI decides do to go to a close distance point, it does not wait in that position
        private float lastGoalPosDistance;
        private Vector3 lastGoalPos;

        [SerializeField] private float maxPatrolRadius = 5;
        [SerializeField] private float maxWaitTime = 5f;
        [SerializeField] private float minWaitTime = 2f;


        public override void Initialize()
        {
            initPos = transform.position;
            lastGoalPos = initPos;
            lastGoalPosDistance = 0;
        }

        /// <summary>
        /// Defines random patrolling
        /// </summary>
        protected override void CalmAction()
        {
            if (goal.Evaluate())
            {
                switch (goal)
                {
                    case PositionGoal _:
                        UponPositionGoalFinished();
                        break;
                    case TimeGoal _:
                        UponTimeGoalFinished();
                        break;
                    default:
                        Log.Warning(this, "CalmAction", "An Undefined goal was given");
                        break;
                }
            }
        }

        /// <summary>
        /// Defines moving to player and trying to attack him
        /// </summary>
        protected override void DangerAction()
        {

        }

        
        protected override void OnTargetFoundHandler(object sender, OnTargetFoundArgs args)
        {
            // TODO
        }

        protected override void OnTargetLostHandler(object sender, OnTargetLostArgs args)
        {
            // TODO
        }

        /// <summary>
        /// Sets a new TimeGoal if the last point was far enough, making the AI wait a bit. Otherwise just set a new position goal to keep it moving
        /// </summary>
        private void UponPositionGoalFinished()
        {
            if (lastGoalPosDistance < CLOSE_DISTANCE_POINT)
            {
                UponTimeGoalFinished();
                return;
            }

            float waitTime = Random.Range(maxWaitTime, minWaitTime);
            goal = new TimeGoal(this, waitTime);
        }


        /// <summary>
        /// Creates a new position goal 
        /// </summary>
        private void UponTimeGoalFinished()
        {
            Vector3 newPos = Movement.CreateRandomPosition(maxPatrolRadius, initPos);
            lastGoalPosDistance = Vector3.Distance(lastGoalPos, newPos);
            lastGoalPos = newPos;

            // Move and assign goal
            Movement.MoveTo(newPos, AIMoveState.Walk);
            goal = new PositionGoal(this, newPos);
        }
    }
}