using UnityEngine;
using System.Collections;
using Base.Common;
using UnityEngine.UI;
using System;
using System.IO;
//using Assets.Modules.Setting.Scripts;
using Location.WCFServiceReferences.LocationServices;

public class SystemSettingHelper : MonoBehaviour
{
    [HideInInspector]
    public static SystemSetting systemSetting;//所有系统设置
    [HideInInspector]
    public static CinemachineSetting cinemachineSetting;
    public static CommunicationSetting communicationSetting;//通信相关设置
    public static VersionSetting versionSetting;//版本号设置
    public static RefreshSetting refreshSetting;//刷新时间间隔设置
    /// <summary>
    /// 版本号
    /// </summary>
    [HideInInspector]
    public static string versionNum = "1.0.20";

    public static SystemSettingHelper instance;

    public static string ConfigPath = "\\..\\SystemSetting.XML";

    void Awake()
    {
        GetSystemSetting();
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
        cinemachineSetting = systemSetting.CinemachineSetting;
        communicationSetting = systemSetting.CommunicationSetting;
        versionSetting = systemSetting.VersionSetting;
        refreshSetting = systemSetting.RefreshSetting;
    }

    /// <summary>
    /// 保存系统设置
    /// </summary>
    public static void SaveSystemSetting()
    {
        string path = Application.dataPath + ConfigPath;

        SerializeHelper.Save(systemSetting, path);

    }

    public static void CreateSystemSettingXml()
    {
        systemSetting = new SystemSetting();
        systemSetting.VersionSetting = new VersionSetting();
        systemSetting.VersionSetting.VersionNumber = versionNum;
        SaveSystemSetting();
    }

}
