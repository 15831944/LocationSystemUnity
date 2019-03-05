using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Base.Common
{
    public class Task
    {
        public List<object> moniterArgs = new List<object>();
        public Task()
        {

        }
        public Task(Action mainAction, params Action[] threadActions)
        {
            RunMulti(mainAction, threadActions);
        }

        /// <summary>
        /// threadActions运行
        /// </summary>
        /// <param name="mainAction"></param>
        /// <param name="threadActions"></param>
        public void RunMulti(Action mainAction, params Action[] threadActions)
        {
            if (mainAction == null) return;
            if (threadActions.Length == 0) return;
            for (int i = 0; i < threadActions.Length; i++)
            {
                object moniter = new object();
                moniterArgs.Add(moniter);
                Action action = threadActions[i];
                QueueUserWorkItem(action, moniter);
                //StartThread(action, moniter);
            }

            ThreadHelper.Wait(2);
            while (!IsWorking)
                ThreadHelper.Wait(2);

            for (int i = 0; i < threadActions.Length; i++)
            {
                object moniter = moniterArgs[i];
                Monitor.Enter(moniter);
            }

            mainAction();

            for (int i = threadActions.Length - 1; i >= 0; i--)
            {
                object moniter = moniterArgs[i];
                Monitor.Exit(moniter);
            }
            moniterArgs.Clear();
            IsWorking = false;
        }

        private bool IsWorking = false;

        //private void StartThread(Action action, object moniter)
        //{
        //    Thread thread = new Thread(() =>
        //    {
        //        DoAction(action, moniter);
        //    });
        //    thread.Start();
        //}

        private void QueueUserWorkItem(Action action, object moniter)
        {
#if WINDOWS_UWP
            Windows.System.Threading.ThreadPool.RunAsync(workItem =>
            {
                DoAction(action, moniter);
            });
#else
            ThreadPool.QueueUserWorkItem(obj =>
            {
                DoAction(action, moniter);
            });
#endif

        }

        private void DoAction(Action action, object moniter)
        {
            try
            {
                Monitor.Enter(moniter);
                IsWorking = true;
                DoAction(action);
            }
            catch (Exception ex)
            {
                Log.Error("Task.DoAction", ex);
            }
            finally
            {
                Monitor.Exit(moniter);
            }
        }

        private void DoAction(Action action)
        {
            if (action != null)
            {
                action();
            }
        }
    }
}
