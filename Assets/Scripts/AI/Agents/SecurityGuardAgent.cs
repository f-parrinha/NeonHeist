using AI.Events;
using UnityEngine;

namespace AI.Agents
{
    public class SecurityGuardAgent : GenericEnemyAgent
    {
        private Vector3 initPos;

        [SerializeField] private float maxPatrolRadius = 5;


        public override void Initialize()
        {
            initPos = transform.position;
        }

        /// <summary>
        /// Defines random patrolling
        /// </summary>
        protected override void CalmAction()
        {
            if (goal.Evaluate())
            {
                Vector3 newPos = Movement.CreateRandomPosition(maxPatrolRadius, initPos);
                goal.Set(newPos);
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
    }
}