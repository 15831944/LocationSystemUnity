using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Jacovone.AssetBundleMagic;

public class CameraSceneManager : MonoBehaviour
{

    #region Field and Property
    public static CameraSceneManager Instance;
    /// <summary>
    /// 场景主相机
    /// </summary>
    public Camera MainCamera;
    /// <summary>
    /// 相机控制脚本
    /// </summary>
    public AroundAlignCamera alignCamera;

    /// <summary>
    ///  Camera default align.
    /// </summary>
    protected AlignTarget defaultAlign;
    /// <summary>
    /// TranslatePro Default
    /// </summary>
    protected MouseTranslatePro defaultTranslatePro;
    /// <summary>
    /// 相机是否处于聚焦状态
    /// </summary>
    public bool IsFocus;
    /// <summary>
    /// 鼠标拖动
    /// </summary>
    public MouseTranslatePro mouseTranslate;
    /// <summary>
    /// 第三人称跟随
    /// </summary>
    public TheThirdPersonCamera theThirdPersonCamera;


    #endregion

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        if (alignCamera == null)
            alignCamera = GameObject.FindObjectOfType<AroundAlignCamera>();
        RecordDefaultAlign();
        if (alignCamera != null)
        {
            alignCamera.OnAlignEnd += OnCameraAlignEnd;
            alignCamera.OnAlignStart += AlignCamera_OnAlignStart;
        }
    }

    private void AlignCamera_OnAlignStart()
    {
        if (SceneAssetManager.Instance)
        {
            SceneAssetManager.Instance.SaveEnableLoadBuilding(false);
        }
        if(LoadingIndicatorScript.Instance)
        {
            LoadingIndicatorScript.Instance.SetMaskImangeState(true);
        }
    }

    void Update()
    {
        SetPointSensitivity();
    }

    /// <summary>
    /// 获取默认的AlignTarget
    /// </summary>
    public AlignTarget GetDefaultAlign()
    {
        return defaultAlign;
    }
    public void SetDefaultAlign(AlignTarget alignTragetTemp)
    {
        defaultAlign = alignTragetTemp;
    }
    /// <summary>
    /// 设置鼠标灵敏度
    /// </summary>
    private void SetPointSensitivity()
    {
        if (alignCamera == null || mouseTranslate == null) return;
        float disToCamera = alignCamera.CurrentDistance;
        if (disToCamera > 70)
        {
            SetSensitivity(40, 2f);
        }
        else if (disToCamera > 35 && disToCamera <= 70)
        {
            SetSensitivity(20, 1f);
        }
        else if (disToCamera > 10 && disToCamera <= 35)
        {
            SetSensitivity(8, 0.5f);
        }
        else
        {
            SetSensitivity(4, 0.1f);
        }
    }
    /// <summary>
    /// 设置灵敏度
    /// </summary>
    /// <param name="wheelT"></param>
    /// <param name="areaT"></param>
    private void SetSensitivity(float wheelT, float areaT)
    {
        alignCamera.mouseSettings.wheelSensitivity = wheelT;
        mouseTranslate.mouseSettings.pointerSensitivity = areaT;
    }
    /// <summary>
    /// 记录初始相机参数
    /// </summary>
	private void RecordDefaultAlign()
    {
        //defaultAlign = new AlignTarget(alignCamera.target, new Vector2(40,0),
        //                        230, new Range(5,90), new Range(1,250));
        defaultAlign = new AlignTarget(mouseTranslate.areaSettings.center, new Vector2(40, 0),
                                270, new Range(0, 90), new Range(1, 300));
        SetDefaultAlign();
        defaultTranslatePro = new MouseTranslatePro();
        defaultTranslatePro.areaSettings = mouseTranslate.areaSettings;
        defaultTranslatePro.mouseSettings = mouseTranslate.mouseSettings;
        defaultTranslatePro.targetCamera = mouseTranslate.targetCamera;
    }
    /// <summary>
    /// 设置默认聚焦参数
    /// </summary>
    public void SetDefaultAlign()
    {
        RoomFactory roomFactory = RoomFactory.Instance;
        if (roomFactory)
        {
            roomFactory = GameObject.FindObjectOfType<RoomFactory>();
            if (roomFactory == null) return;
        }
        FactoryTypeEnum factory = roomFactory.FactoryType;
        switch (factory)
        {
            case FactoryTypeEnum.SiHui:
                defaultAlign = new AlignTarget(mouseTranslate.areaSettings.center, new Vector2(40, 0),
                                300, new Range(0, 90), new Range(1, 400));
                break;
            case FactoryTypeEnum.BaoXin:
                defaultAlign = new AlignTarget(mouseTranslate.areaSettings.center, new Vector2(40, 0),
                                160, new Range(0, 90), new Range(1, 300));
                break;
            case FactoryTypeEnum.ZiBo:
                defaultAlign = new AlignTarget(mouseTranslate.areaSettings.center, new Vector2(40, 0),
                    800, new Range(0, 90), new Range(1, 1000));
                break;
        }
    }

    /// <summary>
    /// 恢复到初始视角
    /// </summary>
    public void ReturnToDefaultAlign(Action onComplete = null, Action onBreakAction = null)
    {
        OnAlignEndAction = onComplete;
        if (OnBreakAction != null) OnBreakAction();
        OnBreakAction = onBreakAction;
        IsFocus = false;
        IsMouseTranslateSet = true;
        mouseTranslate.ResetTranslateOffset();
        AreaSize = new Vector2(defaultTranslatePro.areaSettings.length, defaultTranslatePro.areaSettings.width);
        mouseTranslate.areaSettings = defaultTranslatePro.areaSettings;
        mouseTranslate.mouseSettings = defaultTranslatePro.mouseSettings;
        mouseTranslate.targetCamera = defaultTranslatePro.targetCamera;
        //mouseTranslate.SetTranslatePosition(mouseTranslate.areaSettings.center.position);
        alignCamera.AlignVeiwToTarget(defaultAlign);
    }
    /// <summary>
    /// 获取聚焦的目标和信息
    /// </summary>
    /// <returns></returns>
    public AlignTarget GetCurrentAlignTarget()
    {
        AlignTarget alignTargetTemp = new AlignTarget(alignCamera.GetTarget(), alignCamera.CurrentAngles,
                                alignCamera.CurrentDistance, alignCamera.angleRange, alignCamera.distanceRange);
        return alignTargetTemp;
    }
    /// <summary>
    /// 获取区域移动数据
    /// </summary>
    /// <returns></returns>
    public MouseTranslatePro GetCurrentTranslateInfo()
    {
        MouseTranslatePro TranslateProInfo = new MouseTranslatePro();
        TranslateProInfo.areaSettings = mouseTranslate.areaSettings;
        TranslateProInfo.mouseSettings = mouseTranslate.mouseSettings;
        TranslateProInfo.targetCamera = mouseTranslate.targetCamera;
        return TranslateProInfo;
    }
    /// <summary>
    /// 聚焦到目标物体
    /// </summary>
    /// <param name="targetObj"></param>
    public void FocusTarget(AlignTarget targetObj, Action onFocusComplete = null, Action onBreakAction = null)
    {
        //if (targetObj==null) return;
        OnAlignEndAction = onFocusComplete;
        if (OnBreakAction != null) OnBreakAction();
        OnBreakAction = onBreakAction;
        IsMouseTranslateSet = false;
        alignCamera.AlignVeiwToTarget(targetObj);
    }

    public void FocusTarget(Transform target)
    {
        alignCamera.AlignVeiwToTarget(target);
    }

    private float minDistanceOrg = -50f; //正交最远距离
    /// <summary>
    /// 更换当前聚焦角度和距离
    /// </summary>
    /// <param name="angle">角度</param>
    /// <param name="distance">距离</param>
    /// <param name="onFocusComplete">聚焦完成回调</param>
    /// <param name="onBreakAction">被其他聚焦中断的回调</param>
    public void ChangeFocusAngle(Vector2 angle, Action onFocusComplete = null, Action onBreakAction = null)
    {
        OnAlignEndAction = onFocusComplete;
        if (OnBreakAction != null) OnBreakAction();
        OnBreakAction = onBreakAction;
        AlignTarget target = new AlignTarget();
        //target.center = alignCamera.target;
        target.SetCenter(alignCamera.GetTargetPos());
        target.angleRange = new Range(0, 90);

        target.distanceRange = alignCamera.distanceRange;
        target.distance = alignCamera.CurrentDistance;
        target.angles = angle;
        alignCamera.AlignVeiwToTarget(target);
    }
    /// <summary>
    /// 更换当前聚焦角度和距离
    /// </summary>
    /// <param name="target"></param>
    /// <param name="onFocusComplete"></param>
    /// <param name="onBreakAction"></param>
    public void ChangeFocusAngle(AlignTarget target, Action onFocusComplete = null, Action onBreakAction = null)
    {
        OnAlignEndAction = onFocusComplete;
        if (OnBreakAction != null) OnBreakAction();
        OnBreakAction = onBreakAction;
        alignCamera.AlignVeiwToTarget(target);
    }

    /// <summary>
    /// 获取当前默认参数
    /// </summary>
    /// <returns></returns>
    public AlignTarget GetCurrentAlign()
    {
        AlignTarget target = new AlignTarget();
        //target.center = alignCamera.target;
        target.SetCenter(alignCamera.GetTargetPos());
        target.angleRange = alignCamera.angleRange;
        target.distanceRange = alignCamera.distanceRange;
        target.distance = alignCamera.CurrentDistance;
        target.angles = alignCamera.CurrentAngles;
        return target;
    }
    /// <summary>
    /// 聚焦设备，可以拖拽
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="size"></param>
    /// <param name="action"></param>
    public void FocusTargetWithTranslate(AlignTarget targetObj, Vector2 size, Action action = null, Action onBreakAction = null)
    {
        OnAlignEndAction = action;
        if (OnBreakAction != null) OnBreakAction();
        OnBreakAction = onBreakAction;
        IsMouseTranslateSet = true;
        AreaSize = size;
        mouseTranslate.ResetTranslateOffset();
        //mouseTranslate.areaSettings.length = size.x;
        //mouseTranslate.areaSettings.width = size.y;
        //Transform targetCenter = targetObj.center;
        //targetObj.center = mouseTranslate.gameObject.transform;
        //mouseTranslate.areaSettings.center = targetCenter;
        //mouseTranslate.SetTranslatePosition(targetCenter.position);
        //if(Mathf.Abs(targetObj.angles.x-alignCamera.CurrentAngles.x)<5f)
        //{
        //    Vector2 angle = alignCamera.CurrentAngles - new Vector2(0,30);
        //    alignCamera.SetCurrentAngle(angle);
        //}
        alignCamera.AlignVeiwToTarget(targetObj);
    }


    /// <summary>
    /// 聚焦设备，可以拖拽
    /// </summary>
    /// <param name="targetObj"></param>
    /// <param name="size"></param>
    /// <param name="action"></param>
    public void FocusTargetWithTranslate(AlignTarget targetObj, Action action = null, Action onBreakAction = null)
    {
        OnAlignEndAction = action;
        if (OnBreakAction != null) OnBreakAction();
        OnBreakAction = onBreakAction;
        IsMouseTranslateSet = true;
        mouseTranslate.ResetTranslateOffset();
        alignCamera.AlignVeiwToTarget(targetObj);
    }

    /// <summary>
    /// 摄像机移动完成后的回调
    /// </summary>
    private Action OnAlignEndAction;
    /// <summary>
    /// 摄像头移动被中止的回调
    /// </summary>
    private Action OnBreakAction;
    private Vector2 AreaSize;
    /// <summary>
    /// 是否可以区域拖拽
    /// </summary>
    private bool IsMouseTranslateSet;
    private void SetMouseTranslate()
    {
        if (IsMouseTranslateSet)
        {
            //Debug.LogError(string.Format("设置拖动范围: ({0},{1})",AreaSize.x,AreaSize.y));
            mouseTranslate.areaSettings.length = AreaSize.x;
            mouseTranslate.areaSettings.width = AreaSize.y;
            //mouseTranslate.areaSettings.center = alignCamera.target;
            mouseTranslate.areaSettings.SetCenter(alignCamera.GetTargetPos());
            mouseTranslate.SetTranslatePosition(alignCamera.GetTargetPosition());
            //alignCamera.target = mouseTranslate.transform;
            alignCamera.SetTarget(mouseTranslate.transform);
            IsMouseTranslateSet = false;
        }
    }
    /// <summary>
    /// 摄像机移动完成后回调
    /// </summary>
    private void OnCameraAlignEnd()
    {
        if (SceneAssetManager.Instance)
        {
            SceneAssetManager.Instance.RecoverEnableLoadBuilding();
        }
        if (LoadingIndicatorScript.Instance)
        {
            LoadingIndicatorScript.Instance.SetMaskImangeState(false);
        }
        Action actionT = OnAlignEndAction;
        OnAlignEndAction = null;
        OnBreakAction = null;
        SetMouseTranslate();
        if (actionT != null) actionT();
    }

    void Destroy()
    {
        if (alignCamera != null)
        {
            alignCamera.OnAlignEnd -= OnCameraAlignEnd;
            alignCamera.OnAlignStart -= AlignCamera_OnAlignStart;
        }
    }

    /// <summary>
    /// 设置第三人称跟随
    /// </summary>
    public void SetTheThirdPersonCamera(bool isEnable)
    {
        Log.Info("CameraSceneManager.SetTheThirdPersonCamera", "isEnable:"+ isEnable);
        if (isEnable)
        {
            Transform followTarget = null;
            Transform lookAtTarget = null;
            if (LocationManager.Instance.IsFocus)//实时对焦
            {
                followTarget = LocationManager.Instance.currentLocationFocusObj.titleTag;
            }
            else if (LocationHistoryManager.Instance.IsFocus)//历史对焦
            {
                var historyManController = LocationHistoryManager.Instance.CurrentFocusController;
                followTarget = historyManController.followTarget;
                lookAtTarget = historyManController.followTitle;
            }

            alignCamera.enabled = false;
            if (followTarget != null)
            {
                Log.Info("CameraSceneManager.SetTheThirdPersonCamera", "followTarget:" + followTarget);
                if (CinemachineController.Instance)//切换到Cinemachine摄像机控制系统
                {
                    CinemachineController.Instance.SetFollow(followTarget, lookAtTarget);
                }
                if (theThirdPersonCamera != null)
                {
                    theThirdPersonCamera.SetFollow(followTarget);
                    theThirdPersonCamera.enabled = isEnable;
                }

                alignCamera.SetTarget(followTarget);
            }
           
        }
        else
        {
            if (CinemachineController.Instance)
            {
                CinemachineController.Instance.Disable();
            }
            if (theThirdPersonCamera != null)
            {
                theThirdPersonCamera.enabled = false;
            }
            if (alignCamera != null)
            {
                alignCamera.enabled = true;
                
            }
        }
    }


    /// <summary>
    /// 设置启用第三人称跟随
    /// </summary>
    [ContextMenu("SetTheThirdPersonCameraTrue")]
    public void SetTheThirdPersonCameraTrue()
    {
        SetTheThirdPersonCamera(true);
    }

    /// <summary>
    /// 设置关闭第三人称跟随
    /// </summary>
    [ContextMenu("SetTheThirdPersonCameraFalse")]
    public void SetTheThirdPersonCameraFalse()
    {
        SetTheThirdPersonCamera(false);
    }
}
