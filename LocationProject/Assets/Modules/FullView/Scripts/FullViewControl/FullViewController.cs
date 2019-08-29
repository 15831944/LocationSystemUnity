using DG.Tweening;
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
   // public SystemModeTweenManage systemModeTweenManage;
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
            Log.Error("Error:FullViewController.BindingUIMethod "+e.ToString());
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
        Log.Error("FullViewController->返回首页!");
        if (SystemModeTweenManage.Instance !=null&&SystemModeTweenManage.Instance .IsStartTween==false)
        {
            Log.Info("FullViewController.SystemModeTweenManage.TweenSequence rewind start.");
            SystemModeTweenManage.Instance.TweenSequence.OnRewind(() =>
            {
                Log.Info("FullViewController.SystemModeTweenManage.TweenSequence rewind end.");
                SystemModeTweenManage.Instance. KillModdleTween();
                Log.Info("FullViewController.SwitchToFullView.TransferAnimation start.");
                TransferAnimation.DoTransferAnimation(()=> 
                {
                    Log.Info("FullViewController.SwitchToFullView.TransferAnimation end.");
                    currentPart = FullViewPart.MainBuilding;
                    if (FullViewCameraPath.Instance) FullViewCameraPath.Instance.OnViewChange(true);
                    Log.Info("FullViewController.SceneEvents.OnFullViewStateChange:"+IsFullView);
                    SceneEvents.OnFullViewStateChange(isFullView);
                });               
            }).PlayBackwards();

            //进入视图初始化
        }
        else
        {
            Log.Info("FullViewController.SwitchToFullView. without tween.");
            currentPart = FullViewPart.MainBuilding;
            if (FullViewCameraPath.Instance) FullViewCameraPath.Instance.OnViewChange(true);
            SceneEvents.OnFullViewStateChange(isFullView);
        }

        if (SceneAssetManager.Instance)
        {
            SceneAssetManager.Instance.SetEnableLoadBuilding(false);
        }

    }
    /// <summary>
    /// 退出全景模式
    /// </summary>
    public void ExitFullView()
    {
        Log.Error("FullViewController->进入电厂!");
        if (IsDoingAnimation)
        {
            Log.Info("FullViewController.Transfer Animation not complete.");
            return;
        }
        IsDoingAnimation = true;
        Log.Info("FullViewController.Transfer Animation start...");
        TransferAnimation.DoTransferAnimation(() =>
        {
            Log.Info("FullViewController.Transfer Animation complete...");
            if (FullViewCameraPath.Instance) FullViewCameraPath.Instance.OnViewChange(false);
            InitTopTreeBefroeFactory();
            if (SceneAssetManager.Instance)
            {
                SceneAssetManager.Instance.SetEnableLoadBuilding(true);
            }
        });
    }


    /// <summary>
    /// 进入电厂前，初始化拓扑树数据
    /// </summary>
    public void InitTopTreeBefroeFactory()
    {
        if (!RoomFactory.Instance.isTopoInited)//要等待这里完成
        {
            Log.Info("FullViewController.Roomfactory.Init...");
            RoomFactory.Instance.Init(()=> 
            {
                Log.Info("FullViewController.Roomfactory.Init complete!");
                Log.Info("FullViewController.InitPeronnelTree start!");
                if (PersonnelTreeManage.Instance) PersonnelTreeManage.Instance.InitTree();
                Log.Info("FullViewController.InitPeronnelTree end!");
                EnterFactory();
            });           
        }
        else
        {
            Log.Info("FullViewController.enter factory while topoTree inited!");
            EnterFactory();
        }
    }
    /// <summary>
    /// 进入电厂
    /// </summary>
    private void EnterFactory()
    {
        try
        {
            Log.Info("FullViewController.enter facotry start...");
            SystemModeTweenManage.Instance.SetStartTween();
           
            Log.Info("FullViewController.tween sequence start...");
            SystemModeTweenManage.Instance.TweenSequence.OnComplete(() =>
            {
                ParkInformationManage.Instance.ShowParkInfoUI(true);
                Log.Info("FullViewController.tween sequence end...");
                SystemModeTweenManage.Instance.IsStartTween = false;
                SystemModeTweenManage.Instance.ModdleTween();
            }).Restart();
            if(AlarmPushManage.Instance)AlarmPushManage.Instance.ShowIsShow();
            FactoryDepManager dep = FactoryDepManager.Instance;
            if (dep)
            {
                Log.Info("FullViewController.start creat factory dev...");
                dep.CreateFactoryDev();
                Log.Info("FullViewController.creat factory dev complete.");
            }
            IsDoingAnimation = false;
            isFullView = false;
            Log.Info("FullViewController.enter factory event start...");
            SceneEvents.OnFullViewStateChange(isFullView);
            //MonitorRangeManager.Instance.ShowRanges(SceneEvents.DepNode);
            IsClickUGUIorNGUI.Instance.Recover();//解决鼠标右键旋转场景时，会跳一下的的问题（是IsClickUGUIorNGUI中鼠标点击检测问题）
        }catch(Exception e)
        {
            Log.Error("Error:FullViewController enter factory:"+e.ToString());
        }
    }
    /// <summary>
    /// 立刻退出首页，无需动画
    /// </summary>
    public void ExitFullViewImmediately()
    {
        if (FullViewCameraPath.Instance) FullViewCameraPath.Instance.OnViewChange(false);
        InitTopTreeBefroeFactory();
        if (SceneAssetManager.Instance)
        {
            SceneAssetManager.Instance.SetEnableLoadBuilding(true);
        }
    }
     void OnDestroy()
    {
        //Log.Info("FullViewController.OnDestory->Application Quit.");
    }
    #endregion
}
