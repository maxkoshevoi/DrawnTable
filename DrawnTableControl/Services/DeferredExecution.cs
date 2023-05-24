using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawnTableControl.Services
{
    internal class DeferredExecution
    {
        private Task? worker;
        private Action? toExecute;

        public Control? InvokeControl;
        public volatile int MinDelay;
        private volatile bool isDoingWork;
        private bool waitToPush;
        private bool isPaused;

        public DeferredExecution(int minDelay, Control? invokeControl = null)
        {
            InvokeControl = invokeControl;
            MinDelay = minDelay;
            waitToPush = false;
            isPaused = false;
            isDoingWork = false;
        }

        public void Pause() => isPaused = true;

        public void Resume() => isPaused = false;

        public void Execute(Action func)
        {
            while (waitToPush) ;

            toExecute = func;
            if (worker == null || worker.IsFaulted)
            {
                worker = Task.Factory.StartNew(DoWork);
            }
        }

        private async Task DoWork()
        {
            do
            {
                try
                {
                    if (toExecute == null || isPaused)
                    {
                        isDoingWork = false;
                        await Task.Delay(1);
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

                    await Task.Delay(MinDelay);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    continue;
                }
            } while (true);
        }

        public bool IsWorking =>
            worker != null && !worker.IsFaulted && isDoingWork;

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
