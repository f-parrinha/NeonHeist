using AI.Agents;
using Core.Utilities.Timing;

namespace AI.Goals
{
    public class TimeGoal : Goal
    {
        private TickTimer timer;

        public TimeGoal(GenericEnemyAgent agent, float time) : base(agent)
        {
            timer = new TickTimer(TimeUtils.FracToMilli(time));
            timer.Restart();
        }

        public override bool Evaluate()
        {
            return timer.IsFinished;
        }
    }
}