using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ControlMenuToolTip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
    /// <summary>
    /// ToolTip内容
    /// </summary>
    public string TipContent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(UGUITooltip.Instance)
        UGUITooltip.Instance.ShowTooltip(TipContent);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (UGUITooltip.Instance)
            UGUITooltip.Instance.Hide();
    }
}
