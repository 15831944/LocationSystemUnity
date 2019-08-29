﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
using System;
using Assets.M_Plugins.Helpers.Utils;

public class FollowTargetManage : MonoBehaviour
{
    public static FollowTargetManage Instance;

    /// <summary>
    /// 建筑信息，UI预设
    /// </summary>
    public GameObject BuildingNameUIPrefab;
    /// <summary>
    /// 建筑UI集合名称
    /// </summary>
    [HideInInspector]
    public string BuildingListName = "BuildingNameUI";

    /// <summary>
    /// 摄像头漂浮UI 预设
    /// </summary>
    public GameObject CameraUIPrefab;
    /// <summary>
    /// 摄像头,漂浮UI所属组名
    /// </summary>
    [HideInInspector]
    private string CameraListName = "CameraNameUI";



    /// <summary>
    /// 设备信息漂浮UI
    /// </summary>
    public GameObject DevUIPrefab;
    /// <summary>
    /// 设备信息，漂浮UI所属组名
    /// </summary>
    private string DevListName = "DevNameUI";

    public GameObject BorderDevUIPrefab; //边界告警预设
    public GameObject FireDevUIPrefab;//消防告警预设
    private string AlarmDevUIName="AlarmDevUI";  //告警设备(边界、消防等)

    public GameObject ArchorDevUIPrefab;//基站设备
    private string ArchorDevUIName = "ArchorDevUI";

    /// <summary>
    /// UI跟随管理脚本
    /// </summary>
    private UGUIFollowManage UIFollowManager;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SceneEvents.FullViewStateChange += OnFullViewChange;
        SceneEvents.DepNodeChanged += OnDepTypeChange;
    }

    /// <summary>
    /// 进入/退出首页
    /// </summary>
    /// <param name="isFullView"></param>
    private void OnFullViewChange(bool isFullView)
    {
        if (UIFollowManager == null)
        {
            UIFollowManager = UGUIFollowManage.Instance;
            if (UIFollowManager == null)
            {
                Debug.LogError("UGUIFollowManage.Instance is null.");
                return;
            }
        }
        SetAlarmDevUIState(isFullView);
        if (isFullView)
        {
            UGUIFollowManage.Instance.SetCommonFollowUIsActive(false);
            HideAllFollowUI();
        }
        else
        {
            UGUIFollowManage.Instance.SetCommonFollowUIsActive(true);
            ShowAllFollowUI();
        }
    }
    private void OnDepTypeChange(DepNode oldDep, DepNode currentDep)
    {
        if (UIFollowManager == null)
        {
            UIFollowManager = UGUIFollowManage.Instance;
            if (UIFollowManager == null)
            {
                Debug.LogError("UGUIFollowManage.Instance is null.");
                return;
            }
        }
        HideAllFollowUI(oldDep);
        ShowAllFollowUI(currentDep);
        bool isSameFloor = (oldDep is RoomController && oldDep.ParentNode == currentDep) || (currentDep is RoomController && currentDep.ParentNode == oldDep);
        if (isSameFloor)
        {
            SetAlarmDevUIState(true,currentDep);
        }
        //SetAlarmDevUIState(false,oldDep);
        //SetAlarmDevUIState(true,currentDep);
    }
    /// <summary>
    /// 关闭所有漂浮UI （告警除外）
    /// </summary>
    /// <param name="dep"></param>
    public void HideAllFollowUI(DepNode dep=null)
    {
        HideBuildingUI();
        CloseDepCameraUI();
        HideCameraUI(dep);
        HideDevInfoUI(dep);
        HideArchorInfoUI(dep);
        SetAlarmDevUIState(false, dep);
    }
    /// <summary>
    /// 显示所有漂浮UI 
    /// </summary>
    /// <param name="dep"></param>
    public void ShowAllFollowUI(DepNode dep=null)
    {
        ShowBuidingUI();
        ShowCameraUI(dep);
        ShowDevInfoUI(dep);
        ShowArchorInfoUI(dep);
        SetAlarmDevUIState(true, dep);
    }
    /// <summary>
    /// 获取主相机
    /// </summary>
    /// <returns></returns>
    private Camera GetMainCamera()
    {
        CameraSceneManager manager = CameraSceneManager.Instance;
        if (manager && manager.MainCamera != null)
        {
            return manager.MainCamera;
        }
        else
        {
            Debug.LogError("CameraSceneManager.MainCamera is null!");
            return null;
        }
    }
    #region 建筑信息
    /// <summary>
    /// 显示建筑漂浮信息
    /// </summary>
    public void ShowBuidingUI()
    {
        DepNode node = FactoryDepManager.currentDep;
        bool isRoomState = node == null || node as BuildingController || node as FloorController || node as RoomController;
        if (FunctionSwitchBarManage.Instance == null) return;
        if (FullViewController.Instance.IsFullView||!FunctionSwitchBarManage.Instance.BuildingToggle.ison|| isRoomState||ObjectAddListManage.IsEditMode) return;
        UIFollowManager.SetGroupUIbyName(BuildingListName, true);
    }
    /// <summary>
    /// 隐藏建筑漂浮信息
    /// </summary>
    public void HideBuildingUI()
    {
        UIFollowManager.SetGroupUIbyName(BuildingListName, false);
    }
    #endregion
    #region 摄像头创建UI部分
    /// <summary>
    /// 当前区域打开的UI
    /// </summary>
    private List<CameraMonitorFollowUI> depOpenUI = new List<CameraMonitorFollowUI>();

    public void SaveDepOpenCameraUI(CameraMonitorFollowUI followUI)
    {
        if (depOpenUI == null) depOpenUI = new List<CameraMonitorFollowUI>();
        if(!depOpenUI.Contains(followUI))
        {
            depOpenUI.Add(followUI);
        }
    }
    public void RemoveOpenCameraUI(CameraMonitorFollowUI followUI)
    {
        if (depOpenUI == null) return;
        if (depOpenUI.Contains(followUI))
        {
            depOpenUI.Remove(followUI);
        }
    }
    /// <summary>
    /// 关闭当前Toggle为True的UI
    /// </summary>
    public void CloseDepCameraUI()
    {
        if (depOpenUI == null||depOpenUI.Count==0) return;
        try
        {
            List<CameraMonitorFollowUI> followTemp = new List<CameraMonitorFollowUI>();
            followTemp.AddRange(depOpenUI);
            foreach (var item in followTemp)
            {
                if (item == null) continue;
                if (item.BgToggle.isOn) item.BgToggle.isOn = false;
            }
        }catch(Exception e)
        {
            Log.Error("FollowTargetManage.CloseDepCameraUI.Exception:"+e.ToString());
        }
        depOpenUI.Clear();
    }

    public void CreateCameraUI(GameObject cameraDev,DepNode devDep,DevNode info)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(cameraDev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return;
        string cameraDepName = GetDepNodeId(devDep) + CameraListName;
        DisposeFollowTarget dispostTarget = targetTagObj.AddMissingComponent<DisposeFollowTarget>();
        dispostTarget.SetInfo(cameraDepName);

        //if (!CameraDepNameList.Contains(cameraDepName)) CameraDepNameList.Add(cameraDepName);
        GameObject name = UGUIFollowManage.Instance.CreateItem(CameraUIPrefab, targetTagObj, cameraDepName, mainCamera);
        CameraMonitorFollowUI cameraInfo = name.GetComponent<CameraMonitorFollowUI>();
        if (cameraInfo != null)
        {
            cameraInfo.SetInfo(info);
        }
        if (DevSubsystemManage.IsRoamState || !FunctionSwitchBarManage.Instance.CameraToggle.ison||ObjectAddListManage.IsEditMode)
        {
            UGUIFollowManage.Instance.SetGroupUIbyName(cameraDepName, false);
        }
    }
   
    /// <summary>
    /// 隐藏摄像机漂浮UI
    /// </summary>
    public void HideCameraUI(DepNode dep=null)
    {
        try
        {
            Log.Info("HideCameraUI");
            if (FactoryDepManager.currentDep == null || FunctionSwitchBarManage.Instance == null) return;
            if (dep == null && FunctionSwitchBarManage.Instance.CameraToggle.ison) return;
            DepNode depToShow = dep == null||dep is DepController ? FactoryDepManager.currentDep : dep;
            depToShow = depToShow is DepController ? FactoryDepManager.Instance : depToShow;//区域也默认关闭整厂摄像头
            string cameraDepName = GetDepNodeId(depToShow) + CameraListName;
            UGUIFollowManage.Instance.SetGroupUIbyName(cameraDepName, false);
            CloseDepCameraUI();//关闭当前区域打开的Toggle
        }catch(Exception e)
        {
            Log.Error("FollowTargetManage.HideCameraUI.Exception:" + e.ToString());
        }
    }
    /// <summary>
    /// 显示摄像机漂浮UI
    /// </summary>
    public void ShowCameraUI(DepNode dep = null)
    {
        Log.Info("ShowCameraUI");
        if (FunctionSwitchBarManage.Instance == null) return;
        if (!FunctionSwitchBarManage.Instance.CameraToggle.ison||FactoryDepManager.currentDep==null||ObjectAddListManage.IsEditMode) return;      
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        depToShow = depToShow is DepController ? FactoryDepManager.Instance : depToShow;//区域也默认显示整厂摄像头
        string cameraDepName = GetDepNodeId(depToShow) + CameraListName;
        UGUIFollowManage.Instance.SetGroupUIbyName(cameraDepName, true);
    }
    #endregion

    #region 创建设备漂浮UI
    /// <summary>
    /// 创建设备漂浮UI
    /// </summary>
    /// <param name="sisDev"></param>
    /// <param name="info"></param>
    /// <param name="isShow">是否显示</param>
    public void CreateDevFollowUI(GameObject sisDev,DepNode devDep, DevNode info,Action<DeviceFollowUI>onCreateFinished=null)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(sisDev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return;
        string devDepName = GetDepNodeId(devDep) + DevListName;
        DisposeFollowTarget dispostTarget = targetTagObj.AddMissingComponent<DisposeFollowTarget>();
        dispostTarget.SetInfo(devDepName);

        //if (!DevDepNameList.Contains(devDepName)) DevDepNameList.Add(devDepName);
        GameObject name = UGUIFollowManage.Instance.CreateItem(DevUIPrefab, targetTagObj, devDepName, mainCamera);
        UGUIFollowTarget followTarget = name.GetComponent<UGUIFollowTarget>();
        //followTarget.SetEnableDistace(true,60);
        DeviceFollowUI cameraInfo = name.GetComponent<DeviceFollowUI>();
        if (cameraInfo != null)
        {
            cameraInfo.SetInfo(info);
            if (onCreateFinished != null) onCreateFinished(cameraInfo);
        }
        //if (DevSubsystemManage.IsRoamState||!FunctionSwitchBarManage.Instance.DevInfoToggle.ison)
        //{
        //    UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, false);
        //}        
    }
    /// <summary>
    /// 隐藏设备漂浮UI
    /// </summary>
    public void HideDevInfoUI(DepNode dep=null)
    {
        if (FactoryDepManager.currentDep == null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        //string devDepName = GetDepNodeId(depToShow) + DevListName;
        //UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, false);
        if (DeviceFollowUI.CurrentMonitor != null) DeviceFollowUI.CurrentMonitor.CloseUI();
    }
    /// <summary>
    /// 显示设备漂浮UI
    /// </summary>
    public void ShowDevInfoUI(DepNode dep=null)
    {
        if (FunctionSwitchBarManage.Instance == null) return;
        if (!FunctionSwitchBarManage.Instance.DevInfoToggle.ison|| FactoryDepManager.currentDep == null || ObjectAddListManage.IsEditMode) return;
        //DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        //string devDepName = GetDepNodeId(depToShow) + DevListName;
        //UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, true);
    }
    #endregion
    #region 创建设备告警漂浮UI

    /// <summary>
    /// 设置告警跟随UI的信息
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="alarmInfo"></param>
    public void SetAlarmFollowUI(DevNode dev, DeviceAlarm alarmInfo)
    {
        string typeCode = dev.Info.TypeCode.ToString();
        if (TypeCodeHelper.IsAlarmDev(typeCode))
        {
            if (dev.alarmUi == null)
            {
                dev.alarmUi = CreateFireDevFollowUI(dev.gameObject, dev.ParentDepNode, alarmInfo);
            }
            else
            {
                dev.alarmUi.InitInfo(alarmInfo);
            }
        }
        else
        {
            if (dev.alarmUi == null)
            {
                dev.alarmUi = CreateAlarmDevFollowUI(dev.gameObject, dev.ParentDepNode, alarmInfo);
            }
            else
            {
                dev.alarmUi.InitInfo(alarmInfo);
            }
        }
    }

    /// <summary>
    /// 创建设备漂浮UI
    /// </summary>
    /// <param name="sisDev"></param>
    /// <param name="info"></param>
    /// <param name="isShow">是否显示</param>
    private BorderDevFollowUI CreateAlarmDevFollowUI(GameObject borderDev,DepNode dep ,DeviceAlarm alarmInfo)
    {
        try
        {
            string depNodeId = dep is RoomController ? dep.ParentNode.NodeID.ToString() : dep.NodeID.ToString();
            string groupName = string.Format("{0}{1}", AlarmDevUIName, depNodeId);
            GameObject followObj = CreateFollowTarget(BorderDevUIPrefab, borderDev, groupName);
            if (followObj == null) return null;
            BorderDevFollowUI followUI = followObj.GetComponent<BorderDevFollowUI>();
            //if (followUI != null)
            //{
            //    followUI.InitInfo(alarmInfo);
            //}
            return followUI;
        }
        catch (Exception ex)
        {
            Log.Error("FollowTargetManage.CreateAlarmDevFollowUI", ex.ToString());
            return null;
        }
    
    }
    public FireDevFollowUI CreateFireDevFollowUI(GameObject fireDev,DepNode dep,DeviceAlarm alarmInfo)
    {
        //string depNodeId = dep is RoomController ? dep.ParentNode.NodeID.ToString():dep.NodeID.ToString();
        string groupName = string.Format("{0}{1}", AlarmDevUIName, dep.NodeID);
        GameObject followObj = CreateFollowTarget(FireDevUIPrefab, fireDev, groupName);
        if (followObj == null) return null;
        FireDevFollowUI followUI = followObj.GetComponent<FireDevFollowUI>();
        //if (followUI != null)
        //{
        //    followUI.InitInfo(alarmInfo);
        //}
        return followUI;
    }
    /// <summary>
    /// 移除设备漂浮UI
    /// </summary>
    /// <param name="dev"></param>
    public void RemoveAlarmDevFollowUI(DevNode dev)
    {
        if (dev.ParentDepNode == null) return;
        string groupName = string.Format("{0}{1}", AlarmDevUIName, dev.ParentDepNode.NodeID);
        Transform titleTag = dev.gameObject.transform.Find("TitleTag");
        if(titleTag!=null)
        {
            UGUIFollowManage.Instance.RemoveUIbyTarget(groupName, titleTag.gameObject);
        }       
    }
    public void RemoveAlarmDevFloowUI(DepNode depNode,GameObject alarmObj)
    {
        if (depNode == null) return;
        string groupName = string.Format("{0}{1}", AlarmDevUIName, depNode.NodeID);
        Transform titleTag = alarmObj.gameObject.transform.Find("TitleTag");
        if (titleTag != null)
        {
            UGUIFollowManage.Instance.RemoveUIbyTarget(groupName, titleTag.gameObject);
        }
    }
    /// <summary>
    /// 设置告警漂浮UI的状态（显示/关闭）
    /// </summary>
    /// <param name="isShow"></param>
    public void SetAlarmDevUIState(bool isShow,DepNode dep=null)
    {
        if (FactoryDepManager.currentDep == null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string groupName = string.Format("{0}{1}", AlarmDevUIName, depToShow.NodeID);
        if (isShow)
        {
            if (ObjectAddListManage.IsEditMode) return;
            UGUIFollowManage.Instance.SetGroupUIbyName(groupName, true);
        }
        else
        {
            UGUIFollowManage.Instance.SetGroupUIbyName(groupName, false);
        }
    }
    #endregion
    #region 创建基站漂浮UI

    /// <summary>
    /// 创建设备漂浮UI
    /// </summary>
    /// <param name="sisDev"></param>
    /// <param name="info"></param>
    /// <param name="isShow">是否显示</param>
    public void CreateArchorFollowUI(GameObject archorDev, DepNode devDep, DevNode info)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(archorDev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return;
        string devDepName = GetDepNodeId(devDep) + ArchorDevUIName;
        DisposeFollowTarget dispostTarget = targetTagObj.AddMissingComponent<DisposeFollowTarget>();
        dispostTarget.SetInfo(devDepName);

        //if (!DevDepNameList.Contains(devDepName)) DevDepNameList.Add(devDepName);
        GameObject name = UGUIFollowManage.Instance.CreateItem(ArchorDevUIPrefab, targetTagObj, devDepName, mainCamera);
        UGUIFollowTarget followTarget = name.GetComponent<UGUIFollowTarget>();
        BaseStationFollowUI archorFollow = name.GetComponent<BaseStationFollowUI>();
        if (archorFollow != null)
        {
            archorFollow.InitInfo(info);
        }
        if (DevSubsystemManage.IsRoamState || !FunctionSwitchBarManage.Instance.ArchorInfoToggle.ison)
        {
            UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, false);
        }
    }
    /// <summary>
    /// 隐藏设备漂浮UI
    /// </summary>
    public void HideArchorInfoUI(DepNode dep = null)
    {
        if (FactoryDepManager.currentDep == null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string devDepName = GetDepNodeId(depToShow) + ArchorDevUIName;
        UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, false);
    }
    /// <summary>
    /// 显示设备漂浮UI
    /// </summary>
    public void ShowArchorInfoUI(DepNode dep = null)
    {
        if (FunctionSwitchBarManage.Instance == null) return;
        if (!FunctionSwitchBarManage.Instance.ArchorInfoToggle.ison || FactoryDepManager.currentDep == null || ObjectAddListManage.IsEditMode) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string devDepName = GetDepNodeId(depToShow) + ArchorDevUIName;
        UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, true);
    }
    #endregion

    /// <summary>
    /// 创建漂浮物体
    /// </summary>
    /// <param name="followPrefab"></param>
    /// <param name="dev"></param>
    /// <param name="UIGroupName"></param>
    /// <returns></returns>
    private GameObject CreateFollowTarget(GameObject followPrefab,GameObject dev,string UIGroupName)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(dev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return null;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return null;
        GameObject obj = UGUIFollowManage.Instance.CreateItem(followPrefab, targetTagObj, UIGroupName, mainCamera);
        return obj;
    }
    /// <summary>
    /// 获取区域Id
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private string GetDepNodeId(DepNode dep)
    {
        if (dep == null)
        {
            Log.Error("FollowTargetManage.GetDepNodeId", "dep==null");
            return "";
        }
        if (dep as RoomController) return dep.ParentNode.NodeID.ToString();
        return dep.NodeID.ToString();
    }
    void OnDestroy()
    {
        SceneEvents.DepNodeChanged -= OnDepTypeChange;
        SceneEvents.FullViewStateChange -= OnFullViewChange;
    }
}
