using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentTarget : MonoBehaviour
{
    public bool useNavAgent = true;

    /// <summary>
    /// ��ǰ��
    /// </summary>
    public NavAgentControllerBase navAgent;

    /// <summary>
    /// �����
    /// </summary>
    public NavAgentFollowPerson navAgentFollow;
}