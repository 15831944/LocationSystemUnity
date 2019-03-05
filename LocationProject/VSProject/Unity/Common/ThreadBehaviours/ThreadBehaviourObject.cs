using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class ThreadBehaviourObject : ThreadBehaviourBase
{
    private Action<object> uiAction;

    public void Run<T>(Func<T> mainAction, Action<object> uiAction,string taskName)
    {
        BeforeRun();
        this.TaskName = taskName;
        this.uiAction = uiAction;
        ThreadPool.QueueUserWorkItem(obj =>
        {
            try
            {
                if (mainAction != null)
                {
                    Log.Info("Run MainAction", taskName);
                    Result = mainAction();
                }
            }
            catch (Exception ex)
            {
                Log.Error("ThreadBehaviour", taskName+"|"+ex.ToString());
            }
            IsFinished = true;
            //Debug.Log("Finished");
        });
    }

    public object Result;

    protected override void AfterFinished()
    {
        if (uiAction != null)
        {
            Log.Info("Run UIAction",TaskName);
            uiAction(Result);
        }
    }
}
