using MonitorRange;
using RTEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 区域名称UI控制
/// </summary>
public class AreaNameUIController : MonoBehaviour
{

    public MonitorRangeObject monitorRangeObject;//对应区域
    public Image image;//UI

    // Use this for initialization
    void Start()
    {
        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
        lis.onClick = On_Click;
        image=GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 设置区域
    /// </summary>
    /// <param name="obj"></param>
    public void SetMonitorRangeObject(MonitorRangeObject obj)
    {
        monitorRangeObject = obj;
    }

    /// <summary>
    /// 点击触发
    /// </summary>
    /// <param name="o"></param>
    public void On_Click(GameObject o)
    {
        if (monitorRangeObject == null) return;
        if (!MonitorRangeManager.Instance.IsEditState) return;

        //选中区域
        EditorObjectSelection.Instance.ClearSelection(false);
        EditorObjectSelection.Instance.SetSelectedObjects(new List<GameObject>() { monitorRangeObject.gameObject }, false);
        RangeEditWindow.Instance.Show(monitorRangeObject);
    }

    /// <summary>
    /// 设置UI是否可以进行交互
    /// </summary>
    /// <param name="isbool"></param>
    public void SetImageRaycast(bool isbool)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        image.raycastTarget = isbool;
    }
}
