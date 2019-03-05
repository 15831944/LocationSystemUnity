using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickEventTrigger_UGUI : MonoBehaviour,IPointerDownHandler {

    public delegate void VoidDelegate(GameObject o);
    public VoidDelegate onClick;
    public VoidDelegate onDoubleClick;

    public delegate void VoidDelegate2();
    public VoidDelegate2 on_Click;
    public VoidDelegate2 on_DoubleClick;

    public static GameObject ClickedObject;
    public static GameObject DoubleClickedObject;

    protected float TimeInterval = 500;//单双击检测时间范围
    private DateTime startTime;
    private int clickTimes = 0;//点击次数

    static public DoubleClickEventTrigger_UGUI Get(GameObject go)
    {
        DoubleClickEventTrigger_UGUI listener = go.GetComponent<DoubleClickEventTrigger_UGUI>();
        if (listener == null) listener = go.AddComponent<DoubleClickEventTrigger_UGUI>();
        return listener;
    }

    //public 

    void Update()
    {
        if (clickTimes == 1)
        {
            bool result = CheckTime();
            if (!result)//时间超出时间范围触发单击事件
            {
                ClearFlag();
                OnClick_u();
            }
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //if (!ispressed) return;
        clickTimes++;
        //print (DateTime.Now+" mousedown:"+transform.parent+"\\"+gameObject);
        if (clickTimes == 1)
        {
            startTime = DateTime.Now;
            ClickedObject = gameObject;
        }
        else if (clickTimes == 2)
        {
            ClearFlag();
            bool result = CheckTime();
            if (!result) return;
            //print("DoubleClickEvent1:" + time + " < " + limit + "=" + result);
            OnDoubleClick_u();
            DoubleClickedObject = gameObject;
        }
    }
    //void OnPress(bool ispressed)
    //{
    //    if (!ispressed) return;
    //    clickTimes++;
    //    //print (DateTime.Now+" mousedown:"+transform.parent+"\\"+gameObject);
    //    if (clickTimes == 1)
    //    {
    //        startTime = DateTime.Now;
    //        ClickedObject = gameObject;
    //    }
    //    else if (clickTimes == 2)
    //    {
    //        bool result = CheckTime();
    //        if (!result) return;
    //        //print("DoubleClickEvent1:" + time + " < " + limit + "=" + result);
    //        OnDoubleClick_u();
    //        DoubleClickedObject = gameObject;

    //        ClearFlag();
    //    }
    //}

    private bool CheckTime()
    {
        TimeSpan time = DateTime.Now - startTime;
        TimeSpan limit = new TimeSpan(0, 0, 0, 0, (int)TimeInterval);
        bool result = time < limit;
        return result;
    }

    protected void ClearFlag()
    {
        clickTimes = 0;
        print("ClearFlag_clickTimes");
    }

    protected void OnClick_u()
    {
        if (onClick != null) onClick(gameObject);
        if (on_Click != null) on_Click();
        print("OnClick_u");

    }

    protected void OnDoubleClick_u()
    {
        if (onDoubleClick != null) onDoubleClick(gameObject);
        if (on_DoubleClick != null) on_DoubleClick();
        
        print("OnDoubleClick_u");
    }

    public void RemoveAllEvent()
    {
        onClick = null;
        onDoubleClick = null;
        on_Click = null;
        on_DoubleClick = null;
    }
}
