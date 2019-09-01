using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Set();
    }
	
	// Update is called once per frame
	void Update () {
        Set();

    }

    public GameObject navMeshObj;

    [ContextMenu("ShowNevMesh")]
    public void ShowNevMesh()
    {
        NavMeshHelper.ShowNavMesh(navMeshObj);
    }

    public float meshDisOff = 0.25f;

    public float floorOffsetY = 0.1f;

    public Vector3 globalOffset = new Vector3(0, -0.1f, 0);

   

    [ContextMenu("Set")]
    public void Set()
    {
        NavPointList.meshDisOff = meshDisOff;
        NavPointList.floorOffsetY = floorOffsetY;
        NavMeshHelper.globalOffset = globalOffset;
    }

    [ContextMenu("Get")]
    public void Get()
    {
        meshDisOff = NavPointList.meshDisOff;
        floorOffsetY = NavPointList.floorOffsetY;
        globalOffset = NavMeshHelper.globalOffset;
    }
}
