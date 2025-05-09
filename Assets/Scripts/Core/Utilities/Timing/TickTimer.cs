namespace Core.Utilities.Timing
{
    public class TickTimer : TickTask
    {
        public TickTimer(int tickDelta, TickDelegate onFinish = null) : base(tickDelta, () => { })
        {
            onFinishDelegate = onFinish;
            tickDelegate = () =>
            {
                Stop();
            };
        }
    }
}