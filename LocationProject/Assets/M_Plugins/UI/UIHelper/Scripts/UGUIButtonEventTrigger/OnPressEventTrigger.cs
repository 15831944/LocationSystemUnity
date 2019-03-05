using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// ugui长按鼠标，相当于ngui的OnPress()
/// </summary>
public class OnPressEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float interval = 0.1f; //回调触发间隔时间;

    public float delay = 0.3f;//延迟时间;

    public UnityEvent OnLongPress = new UnityEvent();


    private bool isPointDown = false;
    private float lastInvokeTime;

    private float m_Delay = 0f;

    // Use this for initialization
    void Start()
    {
        m_Delay = delay;
    }

    // Update is called once per frame
    void Update()
    {
        //print(isPointDown);
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            isPointDown = false;
            m_Delay = delay;
        }
        if (isPointDown)
        {
            if ((m_Delay -= Time.deltaTime) > 0f)
            {
                return;
            }

            if (Time.time - lastInvokeTime > interval)
            {
                //触发点击;
                OnLongPress.Invoke();
                lastInvokeTime = Time.time;
            }
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointDown = true;
        m_Delay = delay;
    }

    public void OnPointerUp(PointerEventData eventData)//当鼠标移动时会触发OnPointerUp方法
    {
        //if (Input.GetMouseButton(0)) return;

        //print("OnPointerUp");
        //    isPointDown = false;
        //    m_Delay = delay;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //isPointDown = false;
        //m_Delay = delay;
    }
}
