using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 判断鼠标是否是拖动ui
/// </summary>
public class IsUIMouseDrag : MonoBehaviour {
    /// <summary>
    /// 鼠标开始拖动事件
    /// </summary>
    public static Action dragStart;
    /// <summary>
    /// 鼠标拖动事件
    /// </summary>
    public static Action drag;
    /// <summary>
    /// 鼠标拖动结束事件
    /// </summary>
    public static Action dragEnd;
    /// <summary>
    /// 起始在UI的鼠标是否拖动了
    /// </summary>
    private static bool isUIDragging = false;
    /// <summary>
    /// 位置起始
    /// </summary>
    private Vector3 posstart;
    /// <summary>
    /// 0.02秒的位置改变量，
    /// </summary>
    private Vector3 delta;
    /// <summary>
    /// 鼠标点下是在UI上
    /// </summary>
    private bool IsMouseDownStartOnUI = false;
    /// <summary>
    /// 判断鼠标是否是拖动ui
    /// </summary
    public static bool IsUIDragging
    {
        get
        {
            return isUIDragging;
        }

        set
        {

            isUIDragging = value;
        }
    }


    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// 添加拖动开始监听事件
    /// </summary>
    public static void AddDragStartListener(Action action)
    {
        dragStart -= action;
        dragStart += action;
    }

    /// <summary>
    /// 添加拖动过程监听事件
    /// </summary>
    public static void AddDragListener(Action action)
    {
        drag -= action;
        drag += action;
    }

    /// <summary>
    /// 添加拖动结束监听事件
    /// </summary>
    public static void AddDragEndListener(Action action)
    {
        dragEnd -= action;
        dragEnd += action;
    }



    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            if (IsClickUGUIorNGUI.Instance.isOverUI)
            {
                IsMouseDownStartOnUI = true;
            }
            posstart = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            if (!IsMouseDownStartOnUI) return;
            delta = Input.mousePosition - posstart;
            posstart = Input.mousePosition;
            if (delta != Vector3.zero && IsUIDragging == false)
            {
                IsUIDragging = true;
                On_BeginDrag();
            }
            if (IsUIDragging)
            {
                On_Drag();
            }

            //print(delta);
        }
        //if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        //{
        //    if (!IsMouseDownStartOnUI)
        //    {
        //        if (IsDragging)
        //        {
        //            IsDragging = false;
        //            delta = Vector3.zero;
        //            On_EndDrag();
        //        }
        //    }
        //    else
        //    {
        //        IsMouseDownStartOnUI = false;
        //    }
        //}
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            if (IsMouseDownStartOnUI)
            {
                if (IsUIDragging)
                {
                    IsUIDragging = false;
                    delta = Vector3.zero;
                    On_EndDrag();
                }
            }
            //else
            //{
            //    IsMouseDownStartOnUI = false;
            //}
            IsMouseDownStartOnUI = false;
        }
    }

    public void On_BeginDrag()
    {
        IsUIDragging = true;
        if (dragStart != null)
        {
            dragStart();
        }
        print("On_BeginDrag");
    }

    public void On_Drag()
    {
        //print("On_Drag");
        if (drag != null)
        {
            drag();
        }
    }

    public void On_EndDrag()
    {
        if (dragEnd != null)
        {
            dragEnd();
        }
        IsUIDragging = false;
        print("On_EndDrag");
    }
}
