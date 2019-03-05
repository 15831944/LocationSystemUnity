using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onHover;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onDeselect;
    public VoidDelegate onUpdateSelect;

    private bool isHover;//鼠标是否放在UI上

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    void Update()
    {
        if (isHover)
        {
            OnPointerHover();
        }
    }

    void OnDisable()
    {
        isHover = false;
        //Debug.Log("OnDisable!");
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (onClick != null) onClick(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (onDown != null) onDown(gameObject);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (onEnter != null) onEnter(gameObject);
        isHover = true;
        //Debug.Log("OnPointerEnter!");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        base.OnPointerExit(eventData);
        if (onExit != null) onExit(gameObject);
        //Debug.Log("OnPointerExit!");
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (onUp != null) onUp(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (onDeselect != null) onDeselect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        base.OnUpdateSelected(eventData);
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }

    public void OnPointerHover()
    {
        if (onHover != null) onHover(gameObject);
        //Debug.Log("OnPointerHover!");
    }
}

