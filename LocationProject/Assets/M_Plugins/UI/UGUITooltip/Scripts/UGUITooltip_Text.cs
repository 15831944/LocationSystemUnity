using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.Common.Utils;

public class UGUITooltip_Text : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,IPointerDownHandler, IBeginDragHandler,IEndDragHandler,IPointerUpHandler, IDragHandler
{
    /// <summary>
    /// 鼠标移上去提示内容
    /// </summary>
    [Header("鼠标移上去提示内容")]
    public string text;
    /// <summary>
    /// 鼠标进入当前UI
    /// </summary>
    private bool IsEnter = false;
    /// <summary>
    /// 计时
    /// </summary>
    private float time = 0;
    /// <summary>
    /// 是否已经打开提示框
    /// </summary>
    private bool IsOpenTooltip = false;
    /// <summary>
    /// 是否可以打开提示框
    /// </summary>
    private bool IsCanTooltip = true;
    /// <summary>
    /// 是否拖动
    /// </summary>
    private bool IsDrag = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (IsEnter && !IsOpenTooltip && IsCanTooltip&& !IsDrag)
        {
            time += Time.deltaTime;
            if (time > UGUITooltip.Instance.delayTime)
            {
                UGUITooltip.Instance.ShowTooltip(text);
                IsOpenTooltip = true;
            }
        }
        else
        {
        }
	
	}

    private void LateUpdate()
    {
        if (ScreenHelper.IsOutOfBounder(UGUITooltip.Instance.padding))
        {
            IsEnter = false;
            IsOpenTooltip = false;
            IsCanTooltip = true;
            IsDrag = false;
            UGUITooltip.Instance.Hide();
        }
    }

    /// <summary>
    /// 重置时间
    /// </summary>
    public void ResetTime()
    {
        time = 0;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        IsCanTooltip = false;
        IsOpenTooltip = false;
        UGUITooltip.Instance.Hide();

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        IsEnter = true;
        IsCanTooltip = true;
        ResetTime();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {

    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        UGUITooltip.Instance.Hide();
        IsEnter = false;
        IsOpenTooltip = false;
        IsCanTooltip = true;
        ResetTime();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        IsDrag = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        IsDrag = false;
    }


}
