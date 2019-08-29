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

    public Transform followTarget;

    // Use this for initialization
    void Start () {
        agent=GetComponent<NavMeshAgent>();
        if (followTarget)
        {
            agent.SetDestination(followTarget.position);
        }

    }
	
	// Update is called once per frame
	void Update () {
        if(followTarget&& followTarget.gameObject.activeInHierarchy)
        {
            
            agent.SetDestination(followTarget.position);
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                //NavMesh.Raycast(ray.origin,)
                {
                    Debug.Log("SetDestination:" + hit.collider.gameObject);
                    Debug.Log("SetDestination:"+ hit.point);
                    agent.SetDestination(hit.point);

                    CreateTestPoint(hit.point, hit.point + "");
                }
                else
                {
                    Debug.Log("No Hit");
                }
            }
        }
	}

    private void CreateTestPoint(Vector3 v, string n)
    {
        GameObject o=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.transform.position = v;
        o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        o.name = n;
    }
}
