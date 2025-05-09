using AI.Agents;
using UnityEngine;

namespace AI.Common
{
    public class Goal
    {
        public const float MIN_DISTANCE_TO_GOAL = 1f;

        private AIAgent agent;

        public Vector3 Position { get; private set; }
        public bool Arrived { get; private set; }
        public float Distance => Vector3.Distance(Position, agent.transform.position);


        public Goal(AIAgent agent)
        {
            this.agent = agent;
            Position = agent.transform.position;
        }

        public void Set(Vector3 newPos)
        {
            Position = newPos;
            Arrived = false;
        }

        public bool Evaluate()
        {
            return Arrived = Distance < MIN_DISTANCE_TO_GOAL;
        }
    }
}