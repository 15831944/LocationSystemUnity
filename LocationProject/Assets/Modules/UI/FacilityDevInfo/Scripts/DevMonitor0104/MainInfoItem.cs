using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Location.WCFServiceReferences.LocationServices;

public class MainInfoItem : MonoBehaviour//, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 当前选中按钮
    /// </summary>
    public static MainInfoItem CurrentSelectItem;
    /// <summary>
    /// 系统信息
    /// </summary>
    private DevMonitorNode SystemInfo;
    /// <summary>
    /// 名称文本框
    /// </summary>
    public Text SubSystemText;
    /// <summary>
    /// 监控值
    /// </summary>
    public Text ValueText;
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
    private int MaxTextLength=22;
    // Use this for initialization
    void Start()
    {

    }
    //Todo:关闭界面的时候，取消选中
    public void Init(DevMonitorNode systemInfo, Sprite normalSprite)
    {
        SystemInfo = systemInfo;
        Normal = normalSprite;
        TargetGraphic.overrideSprite = Normal;
        SubSystemText.text = systemInfo.Describe;
        SetDescribeInfo(systemInfo.Describe);
        ValueText.text = string.Format("{0}{1}", systemInfo.Value, systemInfo.Unit);
    }
    /// <summary>
    /// 设置设备监控信息
    /// </summary>
    /// <param name="sysDescribe"></param>
    private void SetDescribeInfo(string sysDescribe)
    {
        try
        {
            string value = FacilityStringHelper.StripTextMiddle(sysDescribe,(int)SubSystemText.rectTransform.rect.size.x,SubSystemText);
            SubSystemText.text = value;
        }catch(Exception e)
        {
            //Debug.LogError("MainInfoItem.SetDescribeInfo error:"+e.ToString());
            SubSystemText.text = sysDescribe;
        }
    }

    /// <summary>
    /// 取消选中
    /// </summary>
	public void DeselectItem(MainInfoItem current = null)
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
    }


    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (!IsSelect && TargetGraphic != null)
    //    {
    //        TargetGraphic.overrideSprite = Hover;
    //    }
    //}
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (!IsSelect && TargetGraphic != null)
    //    {
    //        TargetGraphic.overrideSprite = Normal;
    //    }
    //}
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (IsSelect) return;
    //    SelectItem();
    //}

}
