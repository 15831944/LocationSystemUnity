using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCtest : MonoBehaviour {

    /// <summary>
    /// NavMeshAgent
    /// </summary>
    public NavMeshAgent agent;

    // Use this for initialization
    void Start () {
        agent=GetComponent<NavMeshAgent>();

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200f))
            {
                agent.SetDestination(hit.point);
            }
        }
	}
}
