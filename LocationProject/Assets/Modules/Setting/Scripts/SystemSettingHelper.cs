using UnityEngine;
using System.Collections;
using Base.Common;
using UnityEngine.UI;
using System;
using System.IO;
//using Assets.Modules.Setting.Scripts;
//using Location.WCFServiceReferences.LocationServices;

public class SystemSettingHelper : MonoBehaviour
{
    [HideInInspector]
    public static SystemSetting systemSetting;//所有系统设置
    [HideInInspector]
    public static ResolutionSetting resolutionSetting;
    public static CinemachineSetting cinemachineSetting;
    public static CommunicationSetting communicationSetting;//通信相关设置
    public static VersionSetting versionSetting;//版本号设置
    public static RefreshSetting refreshSetting;//刷新时间间隔设置
    public static AssetLoadSetting assetLoadSetting;//加载asset设置
    public static HoneyWellSetting honeyWellSetting;//霍尼韦尔监控设置
    public static DeviceSetting deviceSetting;
    public static HistoryPathSetting historyPathSetting;
    public static LocationSetting locationSetting;
    public static DebugSetting debugSetting;
    public static AlarmSetting alarmSetting;
    /// <summary>
    /// 版本号
    /// </summary>
    public string versionNum = "1.0.20";
    public static SystemSettingHelper instance;

    public static string ConfigPath = "\\..\\SystemSetting.XML";

    void Awake()
    {
        SetInstance();
        //instance = this;
        //GetSystemSetting();
    }

    /// <summary>
    /// 设置单例
    /// </summary>
    private void SetInstance()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            GetSystemSetting();
        }
        else
        {
            Debug.LogError("SystemSettingHelper is exist...");
            gameObject.RemoveComponent<SystemSettingHelper>();
        }
    }

    // Use this for initialization
    void Start()
    {
        SetSetting();//将相关参数设置到相应的脚本中
       
    }
    /// <summary>
    /// 相关的界面xml初始化配置信息
    /// </summary>
    public void ConfigSetting()
    {
        //try
        //{
        //    if(ShowConfigXML.instance==null)
        //    {
        //        Log.Error("SystemSettingHelper.ConfigSetting.ShowConfigXML is null");
        //    }
        //    if(systemSetting==null)
        //    {
        //        Log.Error("SystemSettingHelper.systemSetting is null");
        //    }
        //    Log.Error("SystemSettingHelper.ConfigSetting" + systemSetting.IsShowLeftTopo);
            //ShowConfigXML.instance.ShowLeftTopoFun(systemSetting.IsShowLeftTopo);//是否显示左侧拓补栏
            //ShowConfigXML.instance.ShowRightDownAlarmFun(systemSetting.IsShowRightDownAlarm);//是否显示右下角告警推送
            //ShowConfigXML.instance.ShowRightTopInfoFun(systemSetting.IsShowRightTopInfo);//是否显示右上角统计信息
            //ShowConfigXML.instance.ShowHomePageFun(systemSetting.IsShowHomePage);
        //}catch(Exception e)
        //{
        //    Log.Error("SystemSettingHelper.ConfigSetting.Error:"+e.ToString());
        //}
        
    }
    /// <summary>
    /// 将相关参数设置到相应的脚本中
    /// </summary>
    public static void SetSetting()
    {
        try
        {
            Screen.SetResolution(systemSetting.ResolutionSetting.Width, systemSetting.ResolutionSetting.High, systemSetting.IsFullScreen);
            Screen.fullScreen = systemSetting.IsFullScreen;

            //if (systemSetting.IsFullScreen)
            //{
            //    Screen.SetResolution(1920, 1080, true);
            //}
            //else
            //{
            //    Screen.fullScreen = false;
            //}
            //Instance相关的设置都要放到Start里面

            if (SceneAssetManager.Instance)
            {
                SceneAssetManager.Instance.SetSetting(assetLoadSetting);
            }

            if (AssetbundleGet.Instance)
            {
                AssetbundleGet.Instance.DeviceFromFile = assetLoadSetting.DeviceFromFile;
                AssetbundleGet.Instance.InitAssetPath();//这边必须加上，重新设置一下
            }

            if (LocationHistoryManager.Instance)
            {
                LocationHistoryManager.Instance.IntervalTime = locationSetting.HistoryIntervalTime;
            }

            NavMeshHelper.IsDebug = IsDebug();
        }
        catch (Exception ex)
        {
            Debug.LogError("SystemSettingHelper.SetSetting:"+ex);
        }
        
    }


    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        Screen.fullScreen = !Screen.fullScreen;
    //    }
    //}

    /// <summary>
    /// 获取系统设置
    /// </summary>
    public static void GetSystemSetting()
    {
        string path = Application.dataPath + ConfigPath;

        if (!File.Exists(path))
        {
            CreateSystemSettingXml();
        }
        else
        {
            systemSetting = SerializeHelper.DeserializeFromFile<SystemSetting>(path);
        }
        Log.Error("SystemSettingHelper.GetSystemSetting" + systemSetting.IsShowLeftTopo);
        resolutionSetting = systemSetting.ResolutionSetting;
        cinemachineSetting = systemSetting.CinemachineSetting;
        communicationSetting = systemSetting.CommunicationSetting;
        versionSetting = systemSetting.VersionSetting;
        refreshSetting = systemSetting.RefreshSetting;
        assetLoadSetting = systemSetting.AssetLoadSetting;
        honeyWellSetting = systemSetting.HoneyWellSetting;
        deviceSetting = systemSetting.DeviceSetting;
        historyPathSetting = systemSetting.HistoryPathSetting;
        alarmSetting = systemSetting.AlarmSetting;
        ///如果版本号不一致，自动更新
        if (versionSetting.VersionNumber != SystemSettingHelper.instance.versionNum)
        {
            versionSetting.VersionNumber = SystemSettingHelper.instance.versionNum;
            SaveSystemSetting();
        }

        locationSetting = systemSetting.LocationSetting;

        debugSetting = systemSetting.DebugSetting;


    }


    /// <summary>
    /// 保存系统设置
    /// </summary>
    public static void SaveSystemSetting()
    {
        string path = Application.dataPath + ConfigPath;
        try
        {
            SerializeHelper.Save(systemSetting, path);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

    }

    public static void CreateSystemSettingXml()
    {
        systemSetting = new SystemSetting();
        systemSetting.VersionSetting = new VersionSetting();
        systemSetting.VersionSetting.VersionNumber = SystemSettingHelper.instance.versionNum;
        SaveSystemSetting();
    }

    public static bool IsDebug()
    {
#if UNITY_EDITOR
        return true;
#endif
        return systemSetting.IsDebug;
    }

}
