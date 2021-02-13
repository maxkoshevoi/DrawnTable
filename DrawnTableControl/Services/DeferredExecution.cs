using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawnTableControl.Services
{
    public class DeferredExecution
    {
        Task worker;
        Action toExecute;

        public Control InvokeControl;
        public volatile int MinDelay;
        volatile bool isDoingWork;
        bool waitToPush;
        bool isPaused;

        public DeferredExecution(int minDelay, Control invokeControl = null)
        {
            InvokeControl = invokeControl;
            MinDelay = minDelay;
            waitToPush = false;
            isPaused = false;
            isDoingWork = false;
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Resume()
        {
            isPaused = false;
        }

        public void Execute(Action func)
        {
            while (waitToPush) ;

            toExecute = func;
            if (worker == null || worker.IsFaulted)
            {
                worker = Task.Factory.StartNew(DoWork);
            }
        }

        void DoWork()
        {
            try
            {
                do
                {
                    if (toExecute == null || isPaused)
                    {
                        isDoingWork = false;
                        Thread.Sleep(1);
                        continue;
                    }

                    isDoingWork = true;
                    waitToPush = true;
                    Action action = toExecute;
                    toExecute = null;
                    waitToPush = false;
                    if (InvokeControl?.InvokeRequired == true)
                    {
                        InvokeControl.Invoke(action);
                    }
                    else
                    {
                        action.Invoke();
                    }

                    Thread.Sleep(MinDelay);
                } while (true);
            }
#pragma warning disable CS0168, IDE0059
            catch (Exception ex)
            {
                return;
            }
#pragma warning restore CS0168
        }

        public bool IsWorking => worker != null && !worker.IsFaulted && isDoingWork;

        public void Join()
        {
            if (!IsWorking)
            {
                return;
            }

            if (InvokeControl?.InvokeRequired == false)
            {
                throw new InvalidOperationException($"Such action will freeze {nameof(InvokeControl)}'s thread");
            }
            while (IsWorking)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }
    }
}
