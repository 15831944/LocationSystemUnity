using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentController : NavAgentControllerBase
{
    void Start()
    {
        InitNavMeshAgent();
    }

    void Update()
    {
        UpdateSpeed();
    }
}
