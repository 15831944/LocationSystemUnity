using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavClosetTest : MonoBehaviour {

    public int downCount = 10;
    public float downStep = 0.1f;

	// Use this for initialization
	void Start () {
		
	}

    public UnityEngine.AI.NavMeshAgent agent;
	
	// Update is called once per frame
	void Update () {
        if (NavMeshHelper.currentTarget == this.transform.position) return;
        GetClosetPoint();
    }

    [ContextMenu("GetClosetPoint")]
    public void GetClosetPoint()
    {
        NavMeshHelper.GetClosetPointAsync(this.transform.position, "Test", agent, null);
    }
}
