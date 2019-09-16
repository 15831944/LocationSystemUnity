using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentControllerBase : MonoBehaviour {

    /// <summary>
    /// NavMeshAgent
    /// </summary>
    public NavMeshAgent agent;

    public float speedPower = 0.5f;

    public float distance = 0;

    public float MaxDistance = 3;

    public bool enableJump = true;

    //public bool useWrap = true;

    public double timespan = 1;

    public bool isPosInfoSet;//位置信息是否设置
    public bool isOnEnableWrap;//是否在激活时wrap（处理：人从一楼激活，慢慢走到二楼的情况）

    private float MaxSpeed = 3f;//NavMeshAgent移动最大速度
    private float MinSpeed = 0.5f;//NavMeshAgent移动最大速度
    //public bool printDistance = true;

    protected void SetSpeed(float dis)
    {
        if(timespan<0)timespan = Mathf.Abs((float)timespan);//防止出现负数，导致人物停止
        double speed = dis * speedPower * rate / timespan;//根据距离调整速度
        if(speed>MaxSpeed)
        {
            speed = MaxSpeed;
        }
        if (speed < MinSpeed)
        {
            speed = MinSpeed;
        }
        agent.speed = (float)speed;
    }

    protected void SetSpeedByDistance()
    {
        if (targetPos == Vector3.zero) return;//总不会那么巧，目标位置就是原点吧。
        //bool r = agent.SetDestination(followTarget.position);
        if (!isPosInfoSet) return;
        distance = Vector3.Distance(targetPos, transform.position);
        //if(printDistance)
        //    Debug.Log("distance:" + distance);
        //Log.Info("NavAgentControllerBase.SetSpeedByDistance",string.Format("{0}->{1},distance:{2}", transform.position,targetPos,distance));
        SetSpeed(distance);

        if (enableJump)
        {
            if (distance > MaxDistance||isOnEnableWrap)//距离太远了，直接飞过去
            {

                //if (useWrap)
                //{
                //Vector3 destination = GetDestination(targetPos);
                ////Vector3 destination = NavMeshHelper.GetClosetPointEx(targetPos, agent);
                //if (destination == Vector3.zero)
                //{

                //}
                //else
                //{
                //    var r=agent.Warp(destination);//要用这个
                //    Log.Info("NavAgentControllerBase.SetSpeedByDistance", string.Format("Jump(Warp) distance:{0}|name:{1}|destination:{2}|wrap:{3}|isOnEnableWrap:{4}", 
                //        distance , this.name , destination , r, isOnEnableWrap));
                //}

                NavMeshHelper.GetClosetPointAsync(targetPos, this.name, agent, (destination, o) =>
                {
                    var r = agent.Warp(destination);//要用这个
                    Log.Info("NavAgentControllerBase.SetSpeedByDistance", string.Format("Jump(Warp) distance:{0}|name:{1}|destination:{2}|wrap:{3}|isOnEnableWrap:{4}|MaxDistance:{5}",
                        distance , this.name , destination , r, isOnEnableWrap, MaxDistance));
                });

                isOnEnableWrap = false;
            }
        }
    }

    protected virtual void UpdateSpeed()
    {
        if (gameObject.activeInHierarchy)
        {
            SetSpeedByDistance();
        }
    }

    protected virtual void InitNavMeshAgent()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                agent.radius = 0.2f;
                agent.height = 0.8f;
            }
            agent.updateRotation = true;
        }
        
    }

    public Vector3 samplePos;

    public Vector3 edgePos;

    public Vector3 castPos;

    /// <summary>
    /// 获取有效目标点
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    protected Vector3 GetDestination(Vector3 targetPos)
    {
        Vector3 destination = targetPos;
        InitNavMeshAgent();
        return NavMeshHelper.GetClosetPoint(destination,agent);
    }

    private Dictionary<string,GameObject> posBuffer=new Dictionary<string, GameObject>();

    private void ShowPoint(string posName,Vector3 pos,Color color)
    {
        try
        {
            if (!posBuffer.ContainsKey(posName))
            {
                GameObject posObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                posObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                posObj.GetComponent<Renderer>().material.color = color;
                posObj.SetTransparent(0.5f);
                posObj.name = posName;
                GameObject.DestroyImmediate(posObj.GetComponent<Collider>());
                posBuffer.Add(posName, posObj);
            }

            GameObject obj = posBuffer[posName];
            obj.transform.position = pos;
            obj.HighlightOn();
        }
        catch (System.Exception ex)
        {
            Log.Error("NavAgentControllerBase.ShowDestination", ex.ToString());
        }
       
    }

    public Vector3 targetPos = Vector3.zero;

    public float rate = 1;

    //public virtual void SetDestination(Vector3 targetPos, float rateT)
    //{
    //    if (gameObject.activeInHierarchy == false) return;
    //    this.rate = rateT;
    //    this.targetPos = targetPos;
    //    Vector3 destination = GetDestination(targetPos);
    //    agent.SetDestination(destination);
    //}

    private PosInfo posInfo;

    public void SetDestination(PosInfo posInfo, float rateT)
    {
        //Log.Info("SetDestination","posInfo:"+posInfo.TargetPos+",rateT:"+rateT);
        if (gameObject.activeInHierarchy == false)
        {
            //Log.Info("SetDestination", "gameObject.activeInHierarchy == false");
            return;
        }
        this.rate = rateT;
        this.posInfo = posInfo;
        //这里设置坐标信息，然后到Update->UpdatePosition 执行移动坐标
    }
    public void DoMovePerson()
    {
        var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoMove();
    }
    public void DoStopPerson()
    {
        var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoStop();
    }



    public float minDistanceToTarget = 0.2f;//经测试0.2f差不多

    public bool IsBusyUpdatePosition = false;

    //public bool UseShowPos = false;

    public void UpdatePosition(Action callback)
    {
        if (posInfo == null)
        {
            //if (callback != null)
            //{
            //    callback();
            //}
            return;
        }
        DateTime start = DateTime.Now;

        var posNew = posInfo.TargetPos; //实际的点
        if (LocationManager.Instance.UseShowPos)//宝信用UseShowPos等于true效果好很多，不会偏到上一层，因为原来的代码已经对高度做了一下限制。
        {
            posNew = posInfo.ShowPos; //原来的算法的人移动的点
        }

        //Log.Info("NavAgentControllerBase.UpdatePosition");
        if (this.targetPos == posNew)//用TargetPos，可以优化性能，用ShowPos，这里就没什么用了
        {
            //Log.Info("NavAgentControllerBase.UpdatePosition", "this.targetPos == posNew : " + targetPos);
            isPosInfoSet = true;
            return;
        }

        Log.Info("UpdatePosition", string.Format("{0},{1},{2},{3}", this.name, posInfo.TargetPos, posInfo.ShowPos, LocationManager.Instance.UseShowPos));

        this.targetPos = posNew;
        isPosInfoSet = true;
        //1.实际的点，独立往目标点移动的效果。
        //  待机能看出来，人物会在原地不动。
        //  这个确定效果可以的话，原来的人就可以删掉了。
        //  目标点距离较远时移动速度会加快。
        //  结合后面的lastPos == pos会提高性能，不用一直计算。
        //  但是，点与点之间切换时能看出一点“切换”的迹象来。

        //this.targetPos = posInfo.ShowPos;
        //2.原来的算法的人移动的点,跟随原来的人物的效果。
        //  会被柜子挡住，NavMesh无法移动到柜子后面的目的地。

        InitNavMeshAgent();
        SetDestination(targetPos, r =>
         {
             if (r)
             {
                 PrintTimeInfo();
             }


             if (callback != null)
             {
                 callback();
             }
         });
    }

    private void PrintTimeInfo()
    {
        HisPosInfo hisPosInfo = posInfo as HisPosInfo; //假如是历史数据
        if (hisPosInfo != null)
        {
            var current = hisPosInfo.CurrentPosInfo;
            var next = current.Next;

            if (next != null)
            {
                TimeSpan time = next.Time - current.Time; //下一个点的时间
                float distance = Vector3.Distance(next.Vec, current.Vec); //下一个点的距离

                Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("{0}=>{1},time:{2},distance:{3}", current.Vec, next.Vec, time, distance));

                MultHistoryTimeStamp timeStamp = LocationHistoryUITool.GetTimeStamp();
                double timesum = timeStamp.timeSum;
                DateTime showPointTime = timeStamp.showPointTime;
                float currentSpeedT = timeStamp.currentSpeed;

                Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("t1:{0},t2:{1},t3{2}", current.Time.ToString("HH:mm:ss.f"), next.Time.ToString("HH:mm:ss.f"), showPointTime.ToString("HH:mm:ss.f")));
            }

        }
    }

    protected Vector3 lastPos = Vector3.zero;

    protected bool EnableUpdate = false;

    protected void SetDestination(Vector3 pos,Action<bool> callback)
    {
        Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}->{1},obj:{2}", transform.position, pos, this));
        if (lastPos == pos)//坐标不变，不用计算处理。
        {
            if (callback != null)
            {
                callback(false);
            }
            return;
        }
        //if (lastPos == Vector3.zero)//说明是第一次，直接跳过去吧
        //{
        //    lastPos = pos;
        //    Vector3 posNew=NavMeshHelper.GetClosetPointEx(pos, agent);
        //    if (agent != null)
        //    {
        //        if (agent.gameObject.activeInHierarchy)
        //        {
        //           
        //        }
        //        else
        //        {
        //            transform.position = posNew;
        //        }
        //    }
        //    else
        //    {
        //        transform.position = posNew;
        //    }
        //            if (callback != null)
        //    {
        //        callback(true);
        //    }
        //    return;
        //}
        lastPos = pos;

        //Log.Info("NavAgentControllerBase.SetDestination", string.Format("pos:{0},obj:{1}",pos,this));
        if (IsBusyUpdatePosition)
        {
            //Log.Info("NavAgentControllerBase.UpdatePosition", "IsBusyUpdatePosition");
            if (callback != null)
            {
                callback(false);
            }
            return;
        }
        DateTime start = DateTime.Now;
        IsBusyUpdatePosition = true;
        NavMeshHelper.GetClosetPointAsync(pos,this.name,agent, (destination,p) => //多线程异步方式计算,避免影响性能
        {
            try
            {
                //Log.Info("NavAgentControllerBase.SetDestination", string.Format("{0}=>{1},{2}", pos,destination, this));
                if (SystemSettingHelper.IsDebug())//调试模式下，查看接下来的移动方向
                {
                    if (ArcManager.Instance != null)
                    {
                        if (p != null)
                            ArcManager.Instance.SetLine(gameObject.transform, p.transform);
                    }
                }

                if (agent != null)
                {
                    if (agent.gameObject.activeInHierarchy)
                    {
                        if (EnableUpdate == false)//代表第一次
                        {
                            Log.Info("NavAgentControllerBase.SetDestination", string.Format("First Wrap!! pos:{0},obj:{1}", pos, this));
                            agent.Warp(destination);
                        }
                        else
                        {
                            bool r = agent.SetDestination(destination);//Agent被关闭或者被销毁，调用这个方法会报错
                            if (r == false)//人物物体不在NavMesh上，立刻跳到目标位置
                            {
                                Log.Info("NavAgentControllerBase.SetDestination", string.Format("Wrap pos:{0},obj:{1}", pos, this));
                                //this.transform.position = destination;
                                agent.Warp(destination);//要用这个,用上面那句话，"不在NavMesh上"的问题会一直出现，要用Warp才会重新计算
                            }
                            else
                            {
                                HisPosInfo hisPosInfo = posInfo as HisPosInfo; //假如是历史数据
                                if (hisPosInfo != null)
                                {
                                    var current = hisPosInfo.CurrentPosInfo;
                                    var next = current.Next;

                                    if (next != null)
                                    {
                                        TimeSpan t = next.Time - current.Time; //下一个点的时间
                                                                               //float distance = Vector3.Distance(next.Vec, current.Vec); //下一个点的距离

                                        //Log.Info("NavAgentControllerBase.UpdatePosition", string.Format("{0}=>{1},time:{2},distance:{3}", current.Vec, next.Vec, time, distance));
                                        this.timespan = t.TotalSeconds;//时间越长，走的越慢
                                    }

                                }
                            }
                        }

                        EnableUpdate = true;
                    }
                    else
                    {
                        Log.Error("NavAgentControllerBase.SetDestination", "agent.gameObject.activeInHierarchy==false:" + this);
                    }
                }
                else
                {
                    Log.Error("NavAgentControllerBase.SetDestination", "agent==null:" + this);
                }
            }
            catch (Exception ex)
            {
                Log.Error("NavAgentControllerBase.SetDestination", "Exception:" + ex);
            }
            finally
            {
                IsBusyUpdatePosition = false;
                TimeSpan time = DateTime.Now - start;

                if (callback != null)
                {
                    callback(false);
                }

            }
            
            

            //Log.Info("NavAgentControllerBase.UpdatePosition", NavMeshHelper.Log);
            //Log.Info("NavAgentControllerBase.SetDestination", "UpdatePosition End time:" +time.TotalMilliseconds+"ms");
        });

        //IsBusyUpdatePosition = true;
        //var destination = NavMeshHelper.GetClosetPoint(pos); //  => //多线程异步方式计算,避免影响性能
        //if (agent != null && agent.gameObject.activeInHierarchy)
        //{
        //    bool r = agent.SetDestination(destination); //Agent被关闭或者被销毁，调用这个方法会报错
        //    Log.Info("NavAgentControllerBase.UpdatePosition", "r:" + r);
        //    if (r == false) //人物物体不在NavMesh上，立刻跳到目标位置
        //    {
        //        //this.transform.position = destination;
        //        agent.Warp(destination);//要用这个,用上面那句话，"不在NavMesh上"的问题会一直出现，要用Warp才会重新计算
        //    }
        //}

        //IsBusyUpdatePosition = false;
        //TimeSpan time = DateTime.Now - start;
        ////Log.Info("NavAgentControllerBase.UpdatePosition", NavMeshHelper.Log);
        //Log.Info("NavAgentControllerBase.UpdatePosition", "UpdatePosition End time:" + time.TotalMilliseconds + "ms");

        //return true;
    }

    void OnDestroy()
    {
        foreach (GameObject o in posBuffer.Values)
        {
            GameObject.Destroy(o);
        }
    }



    public void MovePerson()
    {
        var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoMove();
    }

    public void StopPerson()
    {
        var personAnimationController = gameObject.GetComponent<PersonAnimationController>();
        if (personAnimationController)
            personAnimationController.DoStop();
    }
}
