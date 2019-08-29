using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HistoryPlaySlider : Slider, IPointerUpHandler
{

    public Action onPointerDown;
    public Action onPointerUp;
    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    //public override void OnDrag(PointerEventData eventData)
    //{
    //    base.OnDrag(eventData);
    //    Debug.LogError("HistoryPlaySlider_OnDrag");
    //}

    //public override void OnMove(AxisEventData eventData)
    //{
    //    base.OnMove(eventData);
    //    Debug.LogError("HistoryPlaySlider_OnMove");
    //}

    //public override Selectable FindSelectableOnDown()
    //{
    //    Debug.LogError("FindSelectableOnDown");
    //    return base.FindSelectableOnDown();
    //}

    //public override Selectable FindSelectableOnLeft()
    //{
    //    Debug.LogError("FindSelectableOnLeft");
    //    return base.FindSelectableOnLeft();
    //}

    //public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    //{
    //    Debug.LogError("OnInitializePotentialDrag");
    //    base.OnInitializePotentialDrag(eventData);
    //}

    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.LogError("OnPointerDown");
        base.OnPointerDown(eventData);
        if(onPointerDown!=null)
        {
            onPointerDown();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null)
        {
            onPointerUp();
        }
        Debug.LogError("OnPointerUp");
        base.OnPointerUp(eventData);

    }
}
