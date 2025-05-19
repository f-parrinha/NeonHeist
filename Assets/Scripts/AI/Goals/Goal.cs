using AI.Agents;

namespace AI.Goals
{
    public abstract class Goal {
        protected GenericEnemyAgent agent;

        public Goal(GenericEnemyAgent agent)
        {
            this.agent = agent;
        }

        public abstract bool Evaluate();
    }
}