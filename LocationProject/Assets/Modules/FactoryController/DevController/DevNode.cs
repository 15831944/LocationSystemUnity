using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;
using Assets.M_Plugins.Helpers.Utils;
using Mogoson.CameraExtension;
using RTEditor;

public class DevNode : MonoBehaviour {

    /// <summary>
    /// 设备Id
    /// </summary>
    public string DevId;
    /// <summary>
    /// 设备信息
    /// </summary>
    public DevInfo Info;
    /// <summary>
    /// 设备所在区域
    /// </summary>
    public DepNode ParentDepNode;
    /// <summary>
    /// 设备是否被聚焦
    /// </summary>
    public bool IsFocus;
    /// <summary>
    /// 当前聚焦设备
    /// </summary>
    public static DevNode CurrentFocusDev;
    /// <summary>
    /// 设备是否告警
    /// </summary>
    public bool isAlarm;

    public virtual void Start()
    {
        DevId = Info.DevID;
        CreateFollowUI();
    }
    /// <summary>
    /// 创建设备漂浮UI
    /// </summary>
    private void CreateFollowUI()
    {
        if (Info != null && ParentDepNode != null)
        {
            string typeCode = Info.TypeCode.ToString();
            if (TypeCodeHelper.IsDoorAccess(Info.ModelName)) return;
            if (TypeCodeHelper.IsCamera(typeCode))
            {
                FollowTargetManage.Instance.CreateCameraUI(gameObject,ParentDepNode, this);
            }else if(TypeCodeHelper.IsStaticDev(typeCode))
            {
                FollowTargetManage.Instance.CreateDevFollowUI(gameObject,ParentDepNode, this);
            }else if(TypeCodeHelper.IsLocationDev(typeCode))
            {
                FollowTargetManage.Instance.CreateArchorFollowUI(gameObject,ParentDepNode,this);
            }
        }
    }
    #region 设备高亮 设备聚焦
    /// <summary>
    /// 高亮设备
    /// </summary>
    public virtual void HighlightOn()
    {    
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        Color colorConstant = Color.green;
        //h.ConstantOn(colorConstant);
        h.ConstantOnImmediate(colorConstant);
        h.seeThrough = false;
        HighlightManage manager = HighlightManage.Instance;
        if (manager)
        {
            manager.SetHightLightDev(this);
        }
    }
    /// <summary>
    /// 取消高亮
    /// </summary>
    public virtual void HighLightOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //h.ConstantOff();
        h.ConstantOffImmediate();
    }

    /// <summary>
    /// 高亮闪烁设备
    /// </summary>
    public virtual void FlashingOn()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //Color colorConstant = Color.green;
        h.FlashingOn(new Color(Color.green.r, Color.green.g, Color.green.b, 0), Color.green);
    }
    /// <summary>
    /// 取消高亮闪烁设备
    /// </summary>
    public virtual void FlashingOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //h.ConstantOff();
        h.FlashingOff();
    }

    /// <summary>
    /// 聚焦设备
    /// </summary>
    public void FocusOn()
    {
        bool sameArea = IsSameArea();
        if (CurrentFocusDev != null) CurrentFocusDev.FocusOff(false);
        IsFocus = true;       
        CameraSceneManager manager = CameraSceneManager.Instance;
        if (manager)
        {
            if(sameArea)
            {
                AlignTarget target = GetTargetInfo(gameObject);
                manager.FocusTarget(target, () =>
                {
                    ChangeBackButtonState(true);
                });
                HighlightOn();
            }
            else
            {
                RoomFactory.Instance.FocusNode(ParentDepNode, () =>
                {
                    AlignTarget target = GetTargetInfo(gameObject);
                    manager.FocusTarget(target,()=> 
                    {
                        ChangeBackButtonState(true);
                    });
                    HighlightOn();
                });               
            }
            CurrentFocusDev = this;
        }
    }
    /// <summary>
    /// 取消聚焦
    /// </summary>
    /// <param name="isCameraBack">摄像机是否返回区域视角</param>
    public void FocusOff(bool isCameraBack=true)
    {
        IsFocus = false;
        CurrentFocusDev = null;
        HighLightOff();
        ChangeBackButtonState(false);
        if (isCameraBack)
        {
            CameraSceneManager manager = CameraSceneManager.Instance;
            DepNode currnetDep = FactoryDepManager.currentDep;
            if (manager&& currnetDep != null)
            {               
                currnetDep.FocusOn();
            }
        }
    }
    /// <summary>
    /// 是否属于同一区域
    /// </summary>
    /// <returns></returns>
    private bool IsSameArea()
    {
        if (ParentDepNode == FactoryDepManager.currentDep) return true;
        if (CurrentFocusDev == null)
        {
            bool isSameFloor = ParentDepNode as RoomController &&ParentDepNode.ParentNode==FactoryDepManager.currentDep;
            if (isSameFloor) return true;
            else return false;
        }
        else
        {           
            bool isRoom = CurrentFocusDev.ParentDepNode as RoomController && ParentDepNode as RoomController;
            if (isRoom)
            {
                bool value = CurrentFocusDev.ParentDepNode.ParentNode == ParentDepNode.ParentNode;
                return value;
            }
            else
            {
                return false;
            }
        }       
    }
    /// <summary>
    /// 显示/关闭返回按钮
    /// </summary>
    /// <param name="isShow"></param>
    private void ChangeBackButtonState(bool isShow)
    {
        if(isShow)
        {
            StartOutManage.Instance.SetUpperStoryButtonActive(false);
            StartOutManage.Instance.ShowBackButton(()
             =>
            {
                if (CurrentFocusDev != null) CurrentFocusDev.FocusOff();
            });
            ParkInformationManage.Instance.ShowParkInfoUI(false );
            AlarmPushManage.Instance.ShowAlarmPushWindow(false);
        }
        else
        {
            AlarmPushManage.Instance.ShowAlarmPushWindow(true );
            if (FactoryDepManager.currentDep.depType!=DepType.Factory)
            {
                StartOutManage.Instance.SetUpperStoryButtonActive(true);
            }
            StartOutManage.Instance.HideBackButton();
            if (PersonSubsystemManage.Instance .IsHistorical==false)
            {
                ParkInformationManage.Instance.ShowParkInfoUI(true);
            }
           
           
        }
    }

    private Vector2 angleFocus = new Vector2(30, 0);
    private float camDistance = 1.5f;
    private Range angleRange = new Range(5, 90);
    private Range disRange = new Range(0.5f, 3);
    /// <summary>
    /// 获取相机聚焦物体的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private AlignTarget GetTargetInfo(GameObject obj)
    {
        angleFocus = new Vector2(15, transform.eulerAngles.y);
        AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                               camDistance, angleRange, disRange);
        return alignTargetTemp;
    }
    #endregion
}
