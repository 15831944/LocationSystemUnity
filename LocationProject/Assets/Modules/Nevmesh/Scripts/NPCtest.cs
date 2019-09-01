using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCtest : MonoBehaviour {

    /// <summary>
    /// NavMeshAgent
    /// </summary>
    public NavMeshAgent agent;

    public Transform followTarget;

    // Use this for initialization
    void Start () {
        NavMeshHelper.IsDebug = true;

        agent =GetComponent<NavMeshAgent>();
        GetClosetPoint(this.transform.position,agent,p=>
        {
            agent.Warp(p);
        });
       
        if (followTarget && EnableFollow)
        {
            GetClosetPoint(followTarget.position, agent,p2=>
            {
                agent.SetDestination(p2);
            });
            
        }
    }

    public static void GetClosetPoint(Vector3 target, NavMeshAgent agent,Action<Vector3> callback)
    {
        NavMeshHelper.GetClosetPointAsync(target, "NPCTest",agent,(p,o)=>
        {
            if (callback != null)
            {
                callback(p);
            }
        });
    }

    public bool EnableFollow;

    [ContextMenu("Follow")]
    public void Follow()
    {
        GetClosetPoint(followTarget.position, agent, p2 =>
        {
            agent.SetDestination(p2);
        });
    }

    [ContextMenu("GetClosetPoint")]
    public void GetClosetPoint()
    {
        GetClosetPoint(followTarget.position, agent, p2 =>
        {
            //agent.SetDestination(p2);
        });
    }

    public List<GameObject> testPoints = new List<GameObject>();

    [ContextMenu("GetDownPoint")]
    public void GetDownPoint()
    {
        //NavMeshHelper.downCount = downSampleCount;
        //NavMeshHelper.downStep = downStep;
        // showDownTest = true;
        var targetPos = followTarget.position;
        var down= NavMeshHelper.GetDownSample(targetPos, downSampleCount, downStep);
        //NavMeshHelper.CreatePoint(down, "Down", 0.1f, Color.green);
    }

    public int downSampleCount = 5;

    public float downStep = 0.1f;

    public bool showDownTest;

    NavMeshPath navMeshPath;

    [ContextMenu("CalculatePath")]
    public void CalculatePath()
    {
        foreach (var item in testPoints)
        {
            GameObject.Destroy(item);
        }

        var targetPos = followTarget.position;
        GetClosetPoint(targetPos, agent,p=>
        {
            targetPos = p;

            navMeshPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, targetPos, -1, navMeshPath);
            Debug.Log(string.Format("{0},{1}", navMeshPath.status, navMeshPath.corners.Length));
            //foreach (var item in navMeshPath.corners)
            //{
            //    GameObject tp = NavMeshHelper.CreatePoint(item, "" + item, 0.1f, Color.black);
            //    testPoints.Add(tp);
            //}
            float sum = 0;
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
            {
                var p1 = navMeshPath.corners[i];
                var p2 = navMeshPath.corners[i + 1];
                var dis = Vector3.Distance(p1, p2);
                sum += dis;
            }
            Debug.Log(string.Format("distance:{0}", sum));
        });

       
    }
	
	// Update is called once per frame
	void Update () {

        if (showDownTest)
        {
            GetDownPoint();
        }

        if (navMeshPath!=null && navMeshPath.corners.Length > 0)
        {
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
                Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1], Color.red);
        }

        //else
        //{
            if (Input.GetMouseButtonDown(1))
            {
                Camera camera = Camera.main;
                if (camera == null)
                {
                    camera = GameObject.FindObjectOfType<Camera>();
                }
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                //NavMesh.Raycast(ray.origin,)
                {
                    Debug.Log("SetDestination:" + hit.collider.gameObject);
                    Debug.Log("SetDestination:"+ hit.point);
                    if (agent.SetDestination(hit.point) == false)
                    {
                        agent.Warp(hit.point);
                    }

                    CreateTestPoint(hit.point, hit.point + "");
                }
                else
                {
                    Debug.Log("No Hit");
                }
            }
        else
        {
            if (followTarget && followTarget.gameObject.activeInHierarchy)
            {
                
                if (EnableFollow)
                {
                    if (lastTargetPos == followTarget.position) return;
                    lastTargetPos = followTarget.position;
                     GetClosetPoint(lastTargetPos, agent, p2=>
                     {
                         var dis = Vector3.Distance(p2, transform.position);
                         if (dis < 0.01f) return;

                         agent.SetDestination(p2);
                     });
                }
            }
        }
        //}
	}

    public Vector3 lastTargetPos;

    private void CreateTestPoint(Vector3 v, string n)
    {
        GameObject o=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.transform.position = v;
        o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        o.name = n;
    }
}
