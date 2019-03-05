using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Location.WCFServiceReferences.LocationServices;
public class DevSubSystemItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    /// <summary>
    /// 当前选中按钮
    /// </summary>
    public static DevSubSystemItem CurrentSelectItem;
    /// <summary>
    /// 系统信息
    /// </summary>
    private Dev_Monitor SystemInfo;
    /// <summary>
    /// 名称文本框
    /// </summary>
    public Text SubSystemText;
    /// <summary>
    /// 目标图片
    /// </summary>
    public Image TargetGraphic;
    /// <summary>
    /// 正常图
    /// </summary>
    [HideInInspector]
    public Sprite Normal;
    /// <summary>
    /// Hover图
    /// </summary>
    public Sprite Hover;
    /// <summary>
    /// 选中图
    /// </summary>
    public Sprite Select;

    /// <summary>
    /// 是否选中
    /// </summary>
    private bool IsSelect;

    /// <summary>
    /// 最大显示字符数
    /// </summary>
    private int MaxTextLength = 28;
    // Use this for initialization
    void Start()
    {

    }
    //Todo:关闭界面的时候，取消选中
    public void Init(Dev_Monitor systemInfo, Sprite normalSprite)
    {
        SystemInfo = systemInfo;
        Normal = normalSprite;
        TargetGraphic.overrideSprite = Normal;
        SetDescribeInfo(systemInfo.Name,GetEffectiveNodeList(systemInfo));
    }
    /// <summary>
    /// 获取有效监控节点
    /// </summary>
    /// <param name="node"></param>
    /// <param name="nodeList"></param>
    /// <returns></returns>
    private int GetEffectiveNodeList(Dev_Monitor dev)
    {
        int value = 0;
        if (dev.MonitorNodeList != null)
        {
            value += dev.MonitorNodeList.Length;
        }
        if(dev.ChildrenList!=null)
        {
            foreach (var subDev in dev.ChildrenList)
            {
                value += GetEffectiveNodeList(subDev);
            }
        }       
        return value;
    }
    /// <summary>
    /// 设置设备监控信息
    /// </summary>
    /// <param name="sysDescribe"></param>
    private void SetDescribeInfo(string sysDescribe,int effectiveNum)
    {
        try
        {
            sysDescribe = string.Format("{0} ({1})",sysDescribe,effectiveNum);
            string value = FacilityStringHelper.StripTextMiddle(sysDescribe, (int)SubSystemText.rectTransform.rect.size.x, SubSystemText);
            SubSystemText.text = value;
        }
        catch (Exception e)
        {
            //Debug.LogError("MainInfoItem.SetDescribeInfo error:" + e.ToString());
            SubSystemText.text = sysDescribe;
        }
    }
    /// <summary>
    /// 取消选中
    /// </summary>
	public void DeselectItem(DevSubSystemItem current = null)
    {
        if (current != null && current == this) return;
        else
        {
            if (TargetGraphic != null) TargetGraphic.overrideSprite = Normal;
            IsSelect = false;
            CurrentSelectItem = null;
        }
    }
    /// <summary>
    /// 选中目标
    /// </summary>
    public void SelectItem()
    {
        if (IsSelect) return;
        if (CurrentSelectItem != null)
        {
            CurrentSelectItem.DeselectItem(this);
        }

        IsSelect = true;
        CurrentSelectItem = this;
        if (TargetGraphic != null) TargetGraphic.overrideSprite = Select;
        if(FacilityDevManage.Instance&&SystemInfo!=null)
        {
            FacilityDevManage.Instance.SubSytemTree.InitTree(SystemInfo);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelect && TargetGraphic != null)
        {
            TargetGraphic.overrideSprite = Hover;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelect && TargetGraphic != null)
        {
            TargetGraphic.overrideSprite = Normal;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsSelect) return;
        SelectItem();
    }
}
