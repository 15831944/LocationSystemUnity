using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ControlExit : MonoBehaviour {
    public enum ExitType
    {
        /// <summary>
        /// 返回首页
        /// </summary>
        BackToMainPage,
        /// <summary>
        /// 退出程序
        /// </summary>
        ExitProgram
    }
    /// <summary>
    /// 普通图片
    /// </summary>
    public Image NormalImg;
    /// <summary>
    /// 高亮图片
    /// </summary>
    public Image HighlightImg;
    /// <summary>
    /// 不规则按钮的类型
    /// </summary>
    public ExitType CurrentExitType;
    ///// <summary>
    ///// 按钮提示
    ///// </summary>
    //public Image ToolTip;
    ///// <summary>
    ///// 提示文本框
    ///// </summary>
    //private Text ToolTipText;
	// Use this for initialization
	void Start () {
        //ToolTipText = ToolTip.GetComponentInChildren<Text>(true);
        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
        lis.onEnter += UGUI_OnEnter;
        lis.onExit += UGUI_OnExit;
        lis.onClick += UGUI_OnClick;
    }

    void UGUI_OnEnter(GameObject o)
    {
        OnMouseEnter();
    }

    void UGUI_OnExit(GameObject o)
    {
        OnMouseExit();
    }

    void UGUI_OnClick(GameObject o)
    {
        OnMouseDown();
    }

    //OnMouseDown之外，还有OnMouseEnter, OnMouseDrag和OnMouseExit
    void OnMouseEnter()
    {
        NormalImg.enabled = false;
        HighlightImg.enabled = true;
        //ToolTip.enabled = true;
        //if (CurrentExitType == ExitType.BackToMainPage) ToolTipText.text = "返回首页";
        //else if (CurrentExitType == ExitType.ExitProgram) ToolTipText.text = "退出程序";
        ShowToolTip();
    }
    void OnMouseExit()
    {       
        HighlightImg.enabled = false;
        //ToolTip.enabled = false;
        //ToolTipText.text = "";
        HideTip();
        NormalImg.enabled = true;
    }
    void OnMouseDown()
    {
        ResetImage();
        switch (CurrentExitType)
        {
            case ExitType.BackToMainPage:
                Debug.Log("BackToMainPage...");
                FullViewController.Instance.SwitchToFullView();
                break;
            case ExitType.ExitProgram:
                Debug.Log("Exit Program...");
                Application.Quit();
                break;
            default:
                Debug.Log("Type not find:"+CurrentExitType);
                break;
        }
    }
    /// <summary>
    /// 重置按钮图片
    /// </summary>
    private void ResetImage()
    {
        HighlightImg.enabled = false;
        //ToolTip.enabled = false;
        //ToolTipText.text = "";
        HideTip();
        NormalImg.enabled = true;
    }
    /// <summary>
    /// 显示ToolTip
    /// </summary>
    private void ShowToolTip()
    {
        UGUITooltip Tip = UGUITooltip.Instance;
        if (Tip&&CurrentExitType == ExitType.BackToMainPage) Tip.ShowTooltip("返回首页");
        else if (Tip&&CurrentExitType == ExitType.ExitProgram) Tip.ShowTooltip("退出程序");
    }
    /// <summary>
    /// 关闭ToolTip
    /// </summary>
    private void HideTip()
    {
        UGUITooltip Tip = UGUITooltip.Instance;
        if (Tip) Tip.Hide();
    }
}
