using AI.Agents;

namespace AI.Goals
{

    public class StartGoal : TimeGoal
    {
        public StartGoal(GenericEnemyAgent agent) : base(agent, 0f)
        {
        }

        public override bool Evaluate()
        {
            return true;
        }
    }
}