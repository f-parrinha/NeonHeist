using System.Threading;
using System.Threading.Tasks;

namespace Core.Utilities.Timing
{
    public class TickTask
    {
        public delegate void TickDelegate();

        private CancellationTokenSource cts;
        protected TickDelegate tickDelegate;
        protected TickDelegate onFinishDelegate;

        /// <summary>
        /// Wait time between ticks in <c> milliseconds </c>
        /// </summary>
        public int TickDelta { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsFinished { get; private set; }


        /// <summary>
        /// A "TickTask" represents a periodic action
        /// </summary>
        /// <param name="tickDelta"> time interval between Ticks </param>
        /// <param name="tickDelegate"> OnTick method </param>
        public TickTask(int tickDelta, TickDelegate tickDelegate)
        {
            this.tickDelegate = tickDelegate;

            TickDelta = tickDelta;
            IsFinished = true;
            IsRunning = false;
        }

        /// <summary>
        /// A "TickTask" represents a periodic action
        /// </summary>
        /// <param name="tickDelta"> time interval between Ticks </param>
        /// <param name="tickDelegate"> OnTick method </param>
        /// <param name="onFinishDelegate"> OnFinish method </param>
        public TickTask(int tickDelta, TickDelegate tickDelegate, TickDelegate onFinishDelegate)
        {
            this.tickDelegate = tickDelegate;
            this.onFinishDelegate = onFinishDelegate;

            TickDelta = tickDelta;
            IsFinished = true;
            IsRunning = false;
        }


        public bool Start()
        {
            if (!IsFinished) return false;

            IsRunning = true;
            IsFinished = false;
            cts = new CancellationTokenSource();
            _ = OnTick(cts.Token);
            return true;
        }

        public bool Stop()
        {
            if (IsFinished) return false;

            cts.Cancel();
            cts.Dispose();
            cts = null;
            IsRunning = false;
            IsFinished = true;
            RaiseOnFinish();
            return true;
        }

        public bool Resume()
        {
            if (IsRunning) return false;

            IsRunning = true;
            return true;
        }

        public bool Pause()
        {
            if (!IsRunning) return false;

            IsRunning = false;
            return true;
        }


        public void Restart()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }


            IsRunning = true;
            IsFinished = false;
            cts = new CancellationTokenSource();
            _ = OnTick(cts.Token);
        }

        public async Task OnTick(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!IsRunning) continue;

                await Task.Delay(TickDelta, token);
                
                tickDelegate?.Invoke();
            }
        }

        protected void RaiseOnFinish() => onFinishDelegate?.Invoke();
    }
}