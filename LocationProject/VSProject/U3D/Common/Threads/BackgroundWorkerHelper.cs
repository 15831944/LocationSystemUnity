using System;
using System.ComponentModel;
using System.Threading;

namespace Base.Common
{
    public static class BackgroundWorkerHelper
    {
        public static void Start(Action action)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                DoAction(action);
            };
            
            worker.RunWorkerAsync();
        }

        private static void DoAction(Action action)
        {
            try
            {
                if (action != null)
                    action();
            }
            catch (Exception ex)
            {
                Log.Error("BackgroundWorkerHelper.DoAction", ex);
            }
        }

        private static T DoAction<T>(Func<T> action)
        {
            try
            {
                if (action != null)
                {
                    return action();
                }
            }
            catch (Exception ex)
            {
                Log.Error("BackgroundWorkerHelper.DoAction<T>","Exception:"+ex,st);
            }
            return default (T);
        }

        private static void DoAction<T>(Action<T> action,T arg)
        {
            try
            {
                if (action != null)
                    action(arg);
            }
            catch (Exception ex)
            {
                Log.Error("BackgroundWorkerHelper.DoAction<T>", "Arg:" + arg+"|Exception:"+ex,st);
            }
        }

        public static void StartDelay(Action action, int delayTime)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                ThreadHelper.Wait(delayTime);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                DoAction(action);
            };
            worker.RunWorkerAsync();
        }

        public static void Start(Action action,Action afterAction)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                DoAction(action);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                DoAction(afterAction);
            };
            worker.RunWorkerAsync();
        }

        public static System.Diagnostics.StackTrace st;

        public static BackgroundWorker Start<T>(Func<T> action, Action<T> afterAction)
        {
            st = new System.Diagnostics.StackTrace();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result= DoAction(action);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                DoAction(afterAction,(T)e.Result);
                //DisposeSender(sender);
            };
            worker.RunWorkerAsync();
            return worker;
        }

#if !UNITY_WSA

        private static void DisposeSender(object sender)
        {
            BackgroundWorker w = sender as BackgroundWorker;
            if (w != null)
            {
                w.Dispose();
            }
        }
#endif

        public static void Start<T>(Action<T> action,T arg)
        {
            st = new System.Diagnostics.StackTrace();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                DoAction(action, arg);
            };
            worker.RunWorkerAsync();
        }
    }
}
