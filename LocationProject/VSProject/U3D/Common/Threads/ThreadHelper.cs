using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Base.Common
{
    /// <summary>
    /// 线程帮助类
    /// </summary>
    public static class ThreadHelper
    {
        /// <summary>
        /// 启动一个线程
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        public static void Run(Action action,int delay=0)
        {
#if WINDOWS_UWP
            Windows.System.Threading.ThreadPool.RunAsync((workItem =>
            {
                RunAction(action, delay);
            }));
#else
            ThreadPool.QueueUserWorkItem((obj =>
            {
               RunAction(action, delay);
            }));
            //thread.Start();
#endif
        }

        /// <summary>
        /// 线程等待
        /// </summary>
        /// <param name="ms"></param>
        public static void Wait(int ms)
        {
#if WINDOWS_UWP
            System.Threading.Tasks.Task.Delay(ms).Wait();
#else
            Thread.Sleep(ms);
#endif
        }

        private static void RunAction(Action action, int delay)
        {
            try
            {
                if (action != null)
                {
                    if (delay > 0)
                    {
                        Wait(delay);
                    }
                    action();
                }
            }
            catch (Exception ex)
            {
                Log.Error("ThreadHelper.RunAction", ex);
            }
        }

        //public static List<object> moniterArgs = new List<object>();

        //public static void RunMulti(params Action[] threadActions)
        //{
        //    RunMulti(() => { }, threadActions);
        //}

       /// <summary>
       /// 测试运行多个线程
       /// </summary>
       /// <param name="mainAction">主线程行为</param>
       /// <param name="threadActions">子线程行为</param>
        public static void RunMultiTest(Action mainAction, params Action[] threadActions)
        {
            if (mainAction == null) return;
            if (threadActions.Length == 0) return;
            for (int i = 0; i < threadActions.Length; i++)
            {
                Action action = threadActions[i];
                DoAction(action);
            }
            mainAction();
        }

        /// <summary>
        /// threadActions运行
        /// </summary>
        /// <param name="mainAction"></param>
        /// <param name="threadActions"></param>
        public static void RunMulti(Action mainAction, params Action[] threadActions)
        {
            //if (mainAction == null) return;
            //if (threadActions.Length == 0) return;
            //for (int i = 0; i < threadActions.Length; i++)
            //{
            //    object moniter = new object();
            //    moniterArgs.Add(moniter);
            //    Action action = threadActions[i];
            //    QueueUserWorkItem(action, moniter);
            //    //StartThread(action, moniter);
            //}

            //Thread.Sleep(2);
            //while (!IsWorking)
            //    Thread.Sleep(2);

            //for (int i = 0; i < threadActions.Length; i++)
            //{
            //    object moniter = moniterArgs[i];
            //    Monitor.Enter(moniter);
            //}

            //mainAction();

            //for (int i = threadActions.Length - 1; i >= 0; i--)
            //{
            //    object moniter = moniterArgs[i];
            //    Monitor.Exit(moniter);
            //}
            //moniterArgs.Clear();
            //IsWorking = false;

            Task task = new Task(mainAction, threadActions);
        }

        //private static bool IsWorking = false;

        //private static void StartThread(Action action, object moniter)
        //{
        //    Thread thread = new Thread(() =>
        //    {
        //        DoAction(action, moniter);
        //    });
        //    thread.Start();
        //}

        //private static void QueueUserWorkItem(Action action, object moniter)
        //{
        //    ThreadPool.QueueUserWorkItem(obj =>
        //    {
        //        DoAction(action, moniter);
        //    });
        //}

        //private static void DoAction(Action action, object moniter)
        //{
        //    try
        //    {
        //        Monitor.Enter(moniter);
        //        IsWorking = true;
        //        DoAction(action);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("ThreadHelper.RunAction", ex);
        //    }
        //    finally
        //    {
        //        Monitor.Exit(moniter);
        //    }
        //}

        private static void DoAction(Action action)
        {
            if (action != null)
            {
                action();
            }
        }
    }
}
