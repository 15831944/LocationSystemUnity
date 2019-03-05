using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMoveController : MonoBehaviour
{

    public Animator Animator;

    public string StateName = "Move";

	// Use this for initialization
	void Start () {
	    if (Animator == null)
	    {
	        Animator = gameObject.GetComponent<Animator>();
	    }
        Animator.Play(StateName);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
