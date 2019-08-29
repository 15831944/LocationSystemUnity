using UnityEngine;
using System.Collections;

public class TransformInfo : MonoBehaviour
{

    public Vector3 Pos;

    public Vector3 LocalPos;

    // Use this for initialization
    void Start ()
	{
	    Pos = transform.position;
        LocalPos = transform.localPosition;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
