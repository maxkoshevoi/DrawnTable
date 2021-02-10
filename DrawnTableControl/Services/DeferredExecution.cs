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
                    else
                    {
                        isDoingWork = true;
                    }

                    waitToPush = true;
                    Action action = toExecute;
                    toExecute = null;
                    waitToPush = false;
                    if (InvokeControl != null)
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
#pragma warning disable CS0168
            catch (Exception ex)
            {
                return;
            }
#pragma warning restore CS0168
        }

        public bool IsWorking { get { return worker != null && !worker.IsFaulted && isDoingWork; } }

        public void Join()
        {
            if (IsWorking)
            {
                if (InvokeControl != null && Thread.CurrentThread.ManagedThreadId == GetControlThreadId(InvokeControl))
                {
                    throw new InvalidOperationException($"Such action will frozen {nameof(InvokeControl)}'s thread");
                }
                while (IsWorking)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
        }

        private int GetControlThreadId(Control control)
        {
            int threadId = -1;
            control.Invoke(new Action(() =>
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
            }));
            return threadId;
        }
    }
}
