using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAnimationController : MonoBehaviour {
    public Animator Animator;

    //public string StateName = "Move";

    // Use this for initialization
    void Start()
    {
        if (Animator == null)
        {
            Animator = gameObject.GetComponent<Animator>();
        }
        //Animator.Play(StateName);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    DoMove();
        //}
        //else if (Input.GetMouseButtonDown(1))
        //{
        //    DoStop();
        //}

    }
    /// <summary>
    /// 静止动画
    /// </summary>
    [ContextMenu("DoStop")]
    public void DoStop()
    {
        if (Animator == null)
        {
            Animator = gameObject.GetComponent<Animator>();
        }
        if (Animator == null) return;
        Animator.SetBool("isMove", false);
    }

    /// <summary>
    /// 行走动画
    /// </summary>
    [ContextMenu("DoMove")]
    public void DoMove()
    {
        if (Animator == null)
        {
            Animator = gameObject.GetComponent<Animator>();
        }
        if (Animator == null) return;
        Animator.SetBool("isMove", true);
    }

    public void SetAnimator(bool isBool)
    {
        if (Animator == null) return;
        Animator.enabled = isBool;
    }
}
