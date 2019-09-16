using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAnimationController : MonoBehaviour {
    public Animator Animator;

    //public string StateName = "Move";

    // Use this for initialization
    void Start()
    {
        GetAnimator();
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
        //if (isMove==false) return;
        //isMove = false;

        GetAnimator();
        if (Animator == null||Animator.runtimeAnimatorController==null) return;
        if (Animator.applyRootMotion) Animator.applyRootMotion = false;//取消模型本身的位置，统一用NavAgent移动
        Animator.SetBool("isMove", false);
    }

    private bool isMove = true;

    /// <summary>
    /// 行走动画
    /// </summary>
    [ContextMenu("DoMove")]
    public void DoMove()
    {
        //if (isMove) return;
        //isMove = true;

        GetAnimator();
        if (Animator == null || Animator.runtimeAnimatorController == null) return;
        Animator.SetBool("isMove", true);
    }

    private void GetAnimator()
    {
        if (Animator == null)
        {
            Animator = gameObject.GetComponent<Animator>();
            if (Animator == null)
            {
                Animator = gameObject.GetComponentInChildren<Animator>();
            }
        }
    }

    public void SetAnimator(bool isBool)
    {
        if (Animator == null) return;
        Animator.enabled = isBool;
    }
}
