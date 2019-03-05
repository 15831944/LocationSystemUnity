using MonitorRange;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 全景展示建筑分类
/// </summary>
public enum FullViewPart
{
    /// <summary>
    /// 生活区
    /// </summary>
    LivingQuarters,
    /// <summary>
    /// 主厂区
    /// </summary>
    MainBuilding,
    /// <summary>
    /// 锅炉房
    /// </summary>
    BoilerRoom,
    /// <summary>
    /// 水处理区域
    /// </summary>
    WaterTreatmentArea,
    /// <summary>
    /// 气能源区
    /// </summary>
    GasEnergyArea,
    /// <summary>
    /// 整厂
    /// </summary>
    FullFactory
}
public class FullViewController : MonoBehaviour {
    #region Field and Property
    private static FullViewController instance;
    public static FullViewController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FullViewController>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<FullViewController>();
                }
            }
            return instance;
        }
    }
    //public delegate void FullViewPartChangeDel(FullViewPart part);
    ///// <summary>
    ///// 全景展示，区域切换事件
    ///// </summary>
    //public event FullViewPartChangeDel OnFullViewPartChange;

    //public delegate void ViewChangeDel(bool isFullView);
    ///// <summary>
    ///// 进入/离开全景展示模式的事件
    ///// </summary>
    //public event ViewChangeDel OnViewChange;

    private bool isFullView=true;
    /// <summary>
    /// 是否处于全景模式
    /// </summary>
    public bool IsFullView
    {
        get { return isFullView; }
    }
    private FullViewPart currentPart;
    /// <summary>
    /// 当前所处建筑区域
    /// </summary>
    public FullViewPart CurrentPart
    {
        get { return currentPart; }
    }
    /// <summary>
    /// 全景环绕UI部分
    /// </summary>
    public FullViewCruiseUI fullViewUI;
    /// <summary>
    /// 进入电厂，转场动画
    /// </summary>
    public FVTransferAnimation TransferAnimation;
    /// <summary>
    /// 是否正在做转场动画
    /// </summary>
    private bool IsDoingAnimation;
    #endregion
    #region Private Method
    // Use this for initialization
    void Awake () {
        instance = this;
        BindingUIMethod();
	}
    /// <summary>
    /// 绑定UI触发方法
    /// </summary>
    private void BindingUIMethod()
    {
        try
        {
            fullViewUI.livingQuaterToggle.onValueChanged.AddListener(SwitchToLivingQuarters);
            fullViewUI.mainBuildingToggle.onValueChanged.AddListener(SwitchToMainBuilding);
            fullViewUI.boilerRoomToggle.onValueChanged.AddListener(SwitchToBoilerRoom);
            fullViewUI.waterTreatmentToggle.onValueChanged.AddListener(SwitchToWaterTreatmentArea);
            fullViewUI.gasEnergyToggle.onValueChanged.AddListener(SwitchToGasEnergyArea);
            fullViewUI.fullFactoryToggle.onValueChanged.AddListener(SwitchToFullFactory);
            fullViewUI.EnterFactoryButton.onClick.AddListener(ExitFullView);
            
        }catch(Exception e)
        {
            Debug.LogError("Error:FullViewController.BindingUIMethod "+e.ToString());
        }
    }
    /// <summary>
    /// 切换到整厂
    /// </summary>
    /// <param name="value"></param>
    private void SwitchToFullFactory(bool value)
    {
        ToggleTextAlphaChange(fullViewUI.fullFactoryToggle, value);
        if (value)
        {
            isFullView = true;
            currentPart = FullViewPart.FullFactory;
            SceneEvents.OnFullViewPartChange(FullViewPart.FullFactory);
        }
    }
    /// <summary>
    /// 切换至生活区
    /// </summary>
    private void SwitchToLivingQuarters(bool value)
    {
        ToggleTextAlphaChange(fullViewUI.livingQuaterToggle, value);
        if (value)
        {
            isFullView = true;
            currentPart = FullViewPart.LivingQuarters;
            SceneEvents.OnFullViewPartChange(FullViewPart.LivingQuarters);
        }
    }
    /// <summary>
    /// 切换至主区域
    /// </summary>
    private void SwitchToMainBuilding(bool value)
    {
        ToggleTextAlphaChange(fullViewUI.mainBuildingToggle, value);
        if (value)
        {
            isFullView = true;
            currentPart = FullViewPart.MainBuilding;
            SceneEvents.OnFullViewPartChange(FullViewPart.MainBuilding);
        }
    }
    /// <summary>
    /// 切换至锅炉区
    /// </summary>
    private void SwitchToBoilerRoom(bool value)
    {
        ToggleTextAlphaChange(fullViewUI.boilerRoomToggle, value);
        if (value)
        {
            isFullView = true;
            currentPart = FullViewPart.BoilerRoom;
            SceneEvents.OnFullViewPartChange(FullViewPart.BoilerRoom);
        }
    }
    /// <summary>
    /// 切换至水处理区
    /// </summary>
    private void SwitchToWaterTreatmentArea(bool value)
    {
        ToggleTextAlphaChange(fullViewUI.waterTreatmentToggle, value);
        if (value)
        {
            isFullView = true;
            currentPart = FullViewPart.WaterTreatmentArea;
            SceneEvents.OnFullViewPartChange(FullViewPart.WaterTreatmentArea);
        }
    }
    /// <summary>
    /// 切换至气能源区
    /// </summary>
    private void SwitchToGasEnergyArea(bool value)
    {
        ToggleTextAlphaChange(fullViewUI.gasEnergyToggle,value);
        if (value)
        {
            isFullView = true;
            currentPart = FullViewPart.GasEnergyArea;
            SceneEvents.OnFullViewPartChange(FullViewPart.GasEnergyArea);
        }
    }
    /// <summary>
    /// 改变toggle文本的透明度
    /// </summary>
    /// <param name="toggleUse"></param>
    /// <param name="alpha"></param>
    private void ToggleTextAlphaChange(Toggle toggleUse,bool isOn)
    {
        Text t = toggleUse.GetComponentInChildren<Text>();
        float alpha = isOn ? 1f : 0.3f;
        if (t == null) return;
        Color temp = t.color;
        temp.a = alpha;
        t.color = temp;
    }
    #endregion
    #region Public Method
    /// <summary>
    /// 进入全景模式
    /// </summary>
    public void SwitchToFullView()
    {
        isFullView = true;
        //进入视图初始化
        currentPart = FullViewPart.MainBuilding;
        SceneEvents.OnFullViewStateChange(isFullView);
    }
    /// <summary>
    /// 退出全景模式
    /// </summary>
    public void ExitFullView()
    {
        if (IsDoingAnimation)
        {
            Debug.Log("Transfer Animation not complete.");
            return;
        }
        IsDoingAnimation = true;
        TransferAnimation.DoTransferAnimation(() =>
        {
            //IsDoingAnimation = false;
            //isFullView = false;
            //SceneEvents.OnFullViewStateChange(isFullView);
            ////MonitorRangeManager.Instance.ShowRanges(SceneEvents.DepNode);
            //IsClickUGUIorNGUI.Instance.Recover();//解决鼠标右键旋转场景时，会跳一下的的问题（是IsClickUGUIorNGUI中鼠标点击检测问题）
            InvokeRepeating("WaitForTopoTree", 0, 0.1f);
        });
    }

    public void WaitForTopoTree()
    {
        if (RoomFactory.Instance.isTopoInited)//要等待这里完成
        {
            CancelInvoke("WaitForTopoTree");
            IsDoingAnimation = false;
            isFullView = false;
            SceneEvents.OnFullViewStateChange(isFullView);
            //MonitorRangeManager.Instance.ShowRanges(SceneEvents.DepNode);
            IsClickUGUIorNGUI.Instance.Recover();//解决鼠标右键旋转场景时，会跳一下的的问题（是IsClickUGUIorNGUI中鼠标点击检测问题）
        }
        else
        {
            return;
        }
    }
    /// <summary>
    /// 立刻退出首页，无需动画
    /// </summary>
    public void ExitFullViewImmediately()
    {
        isFullView = false;
        SceneEvents.OnFullViewStateChange(isFullView);
    }
    #endregion
}
