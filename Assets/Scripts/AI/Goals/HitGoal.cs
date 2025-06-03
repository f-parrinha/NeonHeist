using AI.Agents;
using AI.Controllers;
using UnityEngine;

namespace AI.Goals
{
    public class HitGoal : Goal
    {
        AIAnimations animations;

        public HitGoal(GenericEnemyAgent agent) : base(agent)
        {
        }

        public override bool Evaluate()
        {
            throw new System.NotImplementedException();
        }
    }
}