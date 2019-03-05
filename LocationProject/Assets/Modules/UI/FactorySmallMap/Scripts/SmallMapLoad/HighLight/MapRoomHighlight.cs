using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MapRoomHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    /// <summary>
    /// 房间名称
    /// </summary>
    public string RoomName;
    /// <summary>
    /// 房间图片
    /// </summary>
    public Image roomImage;
	// Use this for initialization
	void Start () {
		
	}
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowToolTip(true);
        SetImageAlpha(1);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ShowToolTip(false);
        SetImageAlpha(0.3f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ShowToolTip(false);
        SetImageAlpha(0.3f);
    }
    /// <summary>
    /// 设置图片颜色
    /// </summary>
    /// <param name="alpha"></param>
    private void SetImageAlpha(float alpha)
    {
        if (roomImage == null) return;
        Color tempColor = roomImage.color;
        tempColor.a = alpha;
        roomImage.color = tempColor;
    }
    /// <summary>
    /// 是否显示ToolTip
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowToolTip(bool isShow)
    {
        if (string.IsNullOrEmpty(RoomName)) return;
        if(isShow)
        {
            if (UGUITooltip.Instance)
                UGUITooltip.Instance.ShowTooltip(RoomName);
        }
        else
        {
            if (UGUITooltip.Instance)
                UGUITooltip.Instance.Hide();
        }
    }

}
