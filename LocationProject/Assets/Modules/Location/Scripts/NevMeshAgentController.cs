using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NevMeshAgentController : MonoBehaviour {
    private NavMeshAgent agent;


    void Start()
    {
        //获取组件
        agent = GetComponent<NavMeshAgent>();
    }


    //   // Update is called once per frame
    //   void Update () {

    //}

    public void SetPosition(Vector3 point)
    {

        agent.SetDestination(point);
    }
}
