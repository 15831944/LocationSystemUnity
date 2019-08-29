using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentTarget : MonoBehaviour
{
    public bool useNavAgent = true;

    /// <summary>
    /// 当前的
    /// </summary>
    public NavAgentControllerBase navAgent;

    /// <summary>
    /// 跟随的
    /// </summary>
    public NavAgentFollowPerson navAgentFollow;
}