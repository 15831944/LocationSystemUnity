using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DefaultSetting : MonoBehaviour {

    private Button DefaultXMLData;
	void Start () {
        DefaultXMLData = this.GetComponent<Button>();
        DefaultXMLData.onClick.AddListener(DefaultXMLFun);
    }
	/// <summary>
    /// 初始化XML默认数据
    /// </summary>
    public void DefaultXMLFun()
    {
            InitShowConfigXML();
            InitModelConfigXML();
            InitPositionConfigXML();
            InitSocketConfigXML();
            InitDebugConfigXML();
    }
    /// <summary>
    /// 初始化默认显示配置页面XML
    /// </summary>
    public void InitShowConfigXML()
    {
        SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1920;
        SystemSettingHelper.systemSetting.ResolutionSetting.High = 1080;
        SystemSettingHelper.systemSetting.IsShowLeftTopo = true;
        SystemSettingHelper.systemSetting.IsShowRightDownAlarm = true;
        SystemSettingHelper.systemSetting.IsShowRightTopInfo = true;
        SystemSettingHelper.systemSetting.IsShowHomePage = true;

        SystemSettingHelper.SaveSystemSetting();//保存XML设置
        ShowConfigXML.instance.InitConfigUI(); //初始化显示配置界面的UI
       
    }
    /// <summary>
    /// 初始化默认模型配置页面XML
    /// </summary>
    public void InitModelConfigXML()
    {
        SystemSettingHelper.assetLoadSetting.DeviceResolution = 2048;
        SystemSettingHelper.systemSetting.AssetLoadSetting.BuildingFromFile = true;
        SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceFromFile = true;
        SystemSettingHelper.systemSetting.AssetLoadSetting.LoadDeviceAsset = true;
        SystemSettingHelper.systemSetting.AssetLoadSetting.CacheCount = 2;
        SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceCacheCount = 15;
        SystemSettingHelper.SaveSystemSetting();
        ModelConfigXML.instance.InitConfigUI();

    }
    /// <summary>
    /// 初始化默认定位配置页面XML
    /// </summary>
    public void InitPositionConfigXML()
    {
        SystemSettingHelper.systemSetting.LocationSetting.EnableNavMesh = true;
        SystemSettingHelper.systemSetting.LocationSetting.NavMode = 0;
        SystemSettingHelper.systemSetting.LocationSetting.hideTimeHours = 72;
        SystemSettingHelper.systemSetting.LocationSetting.HistoryIntervalTime = 10;
        SystemSettingHelper.systemSetting.HistoryPathSetting.StartHour = 8;
        SystemSettingHelper.systemSetting.HistoryPathSetting.StartMinute = 3;
        SystemSettingHelper.systemSetting.HistoryPathSetting.Duration = 8;
        SystemSettingHelper.SaveSystemSetting();
        PositionConfigXML.instance.InitConfigUI();
    }
    /// <summary>
    /// 初始化默认通信配置页面XML
    /// </summary>
    public void InitSocketConfigXML()
    {
        SystemSettingHelper.systemSetting.HoneyWellSetting.EnableHoneyWell = false;
        SystemSettingHelper.systemSetting.CommunicationSetting.Ip1 = "172.16.100.26";
        SystemSettingHelper.systemSetting.HoneyWellSetting.Ip = "192.168.1.3";
        SystemSettingHelper.systemSetting.HoneyWellSetting.UserName = "sdk3D";
        SystemSettingHelper.systemSetting.HoneyWellSetting.PassWord = "123456";
        SystemSettingHelper.SaveSystemSetting();
        SocketConfigXML.instance.InitConfigUI(SystemSettingHelper.systemSetting.HoneyWellSetting.EnableHoneyWell);
    }
    /// <summary>
    /// 初始化默认调试配置页面XML
    /// </summary>
    public void InitDebugConfigXML()
    {
        SystemSettingHelper.systemSetting.IsDebug = true;
        SystemSettingHelper.systemSetting.DebugSetting.IsCloseCommunication = false;
        SystemSettingHelper.systemSetting.DebugSetting.IsRemoteMode = false;

        SystemSettingHelper.systemSetting.DeviceSetting.LoadParkDevWhenEnter = true ;
        SystemSettingHelper.systemSetting.DeviceSetting.LoadAnchorDev = true;
        SystemSettingHelper.systemSetting.DeviceSetting.ShowDevInTree = false;
        SystemSettingHelper.systemSetting.DeviceSetting.NotLoadAllDev = false;

        SystemSettingHelper.systemSetting.RefreshSetting.TagPos = 0.35f;
        SystemSettingHelper.systemSetting.RefreshSetting.PersonTree = 5;
        SystemSettingHelper.systemSetting.RefreshSetting.AreaStatistics = 1;
        SystemSettingHelper.systemSetting.RefreshSetting.DepartmentTree = 60;
        SystemSettingHelper.systemSetting.RefreshSetting.NearCamera = 5;
        SystemSettingHelper.systemSetting.RefreshSetting.ScreenShot = 6;
        SystemSettingHelper.SaveSystemSetting();
        DebugConfigXML.instance.InitConfigUI();
    }
    void Update () {
		
	}
}
