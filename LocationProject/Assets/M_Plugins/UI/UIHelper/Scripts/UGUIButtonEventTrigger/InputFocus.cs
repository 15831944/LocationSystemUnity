using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class InputFocus : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    /// <summary>
    /// InputFiled是否获取输入焦点
    /// </summary>
    public static bool IsInputFocus=false;
    /// <summary>
    /// 选中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        IsInputFocus = true;
        Debug.Log("OnSelect:"+transform.name);
    }
    /// <summary>
    /// 未选中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        IsInputFocus = false;
        Debug.Log("OnDeselect:" + transform.name);
    }
    /// <summary>
    /// 关闭
    /// </summary>
    public void OnDisable()
    {
        IsInputFocus = false;
        Debug.Log("OnDisable:" + transform.name);
    }
    /// <summary>
    /// 销毁
    /// </summary>
    private void OnDestroy()
    {
        IsInputFocus = false;
        Debug.Log("OnDestroy:" + transform.name);
    }
}
