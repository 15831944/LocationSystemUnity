using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentFollowPerson : NavAgentControllerBase
{

    public Transform followTarget;

    void Start()
    {
        MovePerson();
        InitNavMeshAgent();

        //InvokeRepeating("UpdatePosition",0,0.3f);//这里这样写的话，走的路径就有问题，奇怪
    }
    private void OnEnable()
    {
        isOnEnableWrap = true;
        MovePerson();
    }

    private void OnDisable()
    {
        isPosInfoSet = false;
    }

    protected override void InitNavMeshAgent()
    {
        if (agent == null)//这个必须加上，不如会一直进来的
        {
            base.InitNavMeshAgent();
            //if (followTarget)
            //{
            //    //agent.SetDestination(followTarget.position);
            //    SetDestination(followTarget.position);
            //}
        }

    }

    public bool FirstUpdate = true;

    void Update()
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        if (FirstUpdate)
        {

            FirstUpdate = false;
        }

        UpdatePosition(() =>
        {
            UpdateSpeed();
        });
    }

    protected override void UpdateSpeed()
    {
        base.UpdateSpeed();
        //if (followTarget && followTarget.gameObject.activeInHierarchy && gameObject.activeInHierarchy)
        //{
        //    //bool r = agent.SetDestination(followTarget.position);
        //    distance = Vector3.Distance(followTarget.position, transform.position);
        //    //Debug.Log("distance:" + distance);
        //    SetSpeed(distance);//根据距离调整速度
        //    if (enableJump && EnableUpdate)
        //    {
        //        if (distance > MaxDistance)//距离太远了，直接飞过去
        //        {
        //            if (useWrap)
        //            {
        //                Vector3 destination = NavMeshHelper.GetSamplePosition(followTarget.position);
        //                bool r = agent.Warp(destination);//要用这个
        //                NavMeshHelper.currentName = this.name;
        //                NavMeshHelper.CreatePoint(destination, "Warp", 0.1f, Color.black);
        //                Debug.Log("Jump(Warp) distance:" + distance + "|" + this + "|" + destination + "|" + r);
        //            }
        //            else
        //            {
        //                transform.position = followTarget.position;//这个会导致人飞到一个位置后，又按原来路径去走回去。
        //                Debug.Log("Jump distance:" + distance);
        //            }
        //        }
        //    }
        //}
    }

    public void SetFollowTarget(Transform target)
    {
        Debug.Log("SetDestination First");
        followTarget = target;
        //transform.position = target.position;
        //Debug.LogError("NavAgentFollowPerson:"+transform.name+" Pos:"+transform.position+" TargetPos:"+target.transform.position);
        //transform.parent = target.parent;
        gameObject.SetActive(true);
    }
}
