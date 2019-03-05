using System;

namespace Base.Common
{
    /// <summary>
    /// 异步调用接口
    /// </summary>
    public interface IAsyncTask
    {
        /// <summary>
        /// 执行异步任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainAction">数据获取行为</param>
        /// <param name="uiAction">UI界面行为</param>
        /// <param name="taskName">名称</param>
        void RunTask<T>(Func<T> mainAction, Action<T> uiAction, string taskName);
    }

    /// <summary>
    /// 异步调用接口
    /// </summary>
    public static class AsyncTaskHelper
    {
        /// <summary>
        /// 上层传入具体实现类，在Unity中是AsyncTaskManager
        /// </summary>
        public static IAsyncTask Instance;

        /// <summary>
        /// 执行异步任务，Instance为null时正常同步顺序执行。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainAction">数据获取行为</param>
        /// <param name="uiAction">UI界面行为</param>
        /// <param name="taskName">名称</param>
        public static void RunTask<T>(Func<T> mainAction, Action<T> uiAction, string taskName)
        {
            string mode = "";
            if (Instance != null)
            {
                mode="AsyncRun";
            }
            else
            {
                mode = "SyncRun";
            }
            Log.Info("AsyncTaskHelper.RunTask",string.Format("taskName:{0},mode:{1}",taskName,mode));
            if (Instance!=null)
            {
                Instance.RunTask<T>(mainAction, uiAction, taskName);
            }
            else
            {
                T result = mainAction();
                uiAction(result);
            }
        }
    }

}
