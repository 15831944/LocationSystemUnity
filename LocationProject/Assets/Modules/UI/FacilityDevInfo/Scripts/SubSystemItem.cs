using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
public class SubSystemItem : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler {
    /// <summary>
    /// 当前选中按钮
    /// </summary>
    public static SubSystemItem CurrentSelectItem;
    /// <summary>
    /// 系统信息
    /// </summary>
    private FacilitySystem SystemInfo;

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
    // Use this for initialization
    void Start()
    {

    }
    //Todo:关闭界面的时候，取消选中
    public void Init(FacilitySystem systemInfo,Sprite normalSprite)
    {
        SystemInfo = systemInfo;
        Normal = normalSprite;
        TargetGraphic.overrideSprite = Normal;
        SubSystemText.text = systemInfo.DevName;
    }
    /// <summary>
    /// 取消选中
    /// </summary>
	public void DeselectItem(SubSystemItem current = null)
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
        if (SystemInfo != null)
        {
            //Debug.Log("ShowSubSystem:" + SystemInfo.DevName);
            FacilityInfoManage manage = FacilityInfoManage.Instance;
            if(manage &&manage.DevTree!=null)
            {
                manage.DevTree.InitTree(SystemInfo);
            }
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
