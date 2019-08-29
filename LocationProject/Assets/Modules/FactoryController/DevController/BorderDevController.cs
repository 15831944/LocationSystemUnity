using HighlightingSystem;
using MonitorRange;
using RTEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderDevController : DevNode, IRTEditorEventListener
{
    /// <summary>
    /// 跟随UI的Image初始颜色
    /// </summary>
    private Color oriFollowUIColor;

    /// <summary>
    /// 渲染器
    /// </summary>
    private Renderer render;

    //名称UI
    public GameObject followNameUI;
    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, Vector3.zero);
        followNameUI = UGUIFollowManage.Instance.CreateItem(MonitorRangeManager.Instance.NameUI, targetTagObj, "BorderDevUI", null, false, false);
        Text ntxt = followNameUI.GetComponentInChildren<Text>();
        ntxt.text = Info.Name;
        oriFollowUIColor = followNameUI.GetComponentInChildren<Image>().color;
        SetRendererEnable(false);
    }
 
    /// <summary>
    /// 设置渲染器是否启用
    /// </summary>
    public void SetRendererEnable(bool isEnable,bool isAlarmOff=false)
    {
        var isSameArea = GetIsSameArea();
        if (ObjectAddListManage.IsEditMode && isAlarmOff && isSameArea)
        {
            SetFollowNameUIEnable(true);
            return;//编辑模式下，不关闭材质
        }
        GetRenderer();
        render.enabled = isEnable;
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider) collider.enabled = isEnable;
        SetFollowNameUIEnable(isEnable);
        SetSelectedUI(isEnable);
    }

    private bool GetIsSameArea()
    {
        bool isSameArea = false;
        if (ParentDepNode != null) isSameArea = ParentDepNode == FactoryDepManager.currentDep;
        return isSameArea;
    }

    /// <summary>
    /// 告警闪烁开启
    /// </summary>
    [ContextMenu("AlarmOn")]
    public void AlarmOn()
    {
        FlashingBorderOn(Color.red, 2f);
    }

    /// <summary>
    /// 告警闪烁关闭
    /// </summary>
    [ContextMenu("AlarmOff")]
    public void AlarmOff()
    {
        FlashingBorderOff();
    }
    /// <summary>
    /// 设置选中UI
    /// </summary>
    public void SetSelectedUI(bool isSelected)
    {
        if (followNameUI != null)
        {
            Image imageT = followNameUI.GetComponent<Image>();
            if (isSelected)
            {
                imageT.color = new Color(imageT.color.r, imageT.color.g, imageT.color.b, 1F);
            }
            else
            {
                imageT.color = oriFollowUIColor;
            }
        }
    }




    /// <summary>
    /// 开启闪烁
    /// </summary>
    /// <param name="flashColor"></param>
    /// <param name="frequency">Flashing frequency (times per second)</param>
    private void FlashingBorderOn(Color flashColor, float frequency)
    {
        SetRendererEnable(true);
        SetFollowNameUIEnable(false);
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        Color colorStart = flashColor;
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0);
        h.FlashingOn(colorStart, colorEnd, frequency);
    }
    /// <summary>
    /// 关闭闪烁
    /// </summary>
    private void FlashingBorderOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.FlashingOff();
        //if (ObjectAddListManage.IsEditMode &&(FactoryDepManager.currentDep != null && FactoryDepManager.currentDep.NodeID == Info.ParentId))
        //{
        //    //处于设备编辑模式，
        //    return;
        //}
        SetRendererEnable(false,true);
    }
    /// <summary>
    /// 设置跟随的名称UI是否启用
    /// </summary>
    /// <param name="isEnable"></param>
    private void SetFollowNameUIEnable(bool isEnable)
    {
        if (followNameUI)
        {
            followNameUI.SetActive(isEnable);
        }
    }
    /// <summary>
    /// 获取渲染器
    /// </summary>
    /// <returns></returns>
    private Renderer GetRenderer()
    {
        if (render == null)
        {
            render = gameObject.GetComponent<Renderer>();
        }
        return render;
    }

    public void OnSelected(ObjectSelectEventArgs selectEventArgs)
    {
        if (ObjectAddListManage.IsEditMode)
        {
            SetFollowNameUIEnable(false);
            SetSelectedUI(false);
        }
    }

    /// <summary>
    /// Called when the object has been deselected.
    /// </summary>
    public void OnDeselected(ObjectDeselectEventArgs deselectEventArgs)
    {
        var isSameArea = GetIsSameArea();
        if (ObjectAddListManage.IsEditMode&&isSameArea)
        {
            SetFollowNameUIEnable(true);
            SetSelectedUI(true);
        }
    }

    public bool OnCanBeSelected(ObjectSelectEventArgs selectEventArgs)
    {
        return true;
    }
    /// <summary>
    /// Called when the object is altered (moved, rotated or scaled) by a transform gizmo.
    /// </summary>
    /// <param name="gizmo">
    /// The transform gzimo which alters the object.
    /// </param>
    public void OnAlteredByTransformGizmo(Gizmo gizmo)
    {

    }

}
