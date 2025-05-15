using AI.Agents;
using UnityEngine;

namespace AI.Goals
{
    public class PositionGoal : Goal
    {
        public const float MIN_DISTANCE_TO_GOAL = 2f;

        public Vector3 Position { get; private set; }
        public bool Arrived { get; private set; }
        public float Distance => Vector3.Distance(Position, agent.transform.position);


        public PositionGoal(GenericEnemyAgent agent, Vector3 position) : base(agent)
        {
            Position = position;
        }

        public override bool Evaluate()
        {
            return Arrived = Distance < MIN_DISTANCE_TO_GOAL;
        }
    }
}