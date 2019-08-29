using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PositionConfigXML : MonoBehaviour {
    public static PositionConfigXML instance; 
    public InputField PersonMaxShowTime;//人员最大显示时间
    public InputField HistoryTrack;//历史轨迹中断时间设置

    public Toggle UseNavMesh;//使用NavMesh
    public Toggle SingleMode; //简单模式
    public Toggle UseNavMeshSingleFun; //使用寻路算法
    public Toggle UseNavMeshAStarFun;//使用A星寻路算法

    public InputField HistoryTrackDefaultStartTime;//历史路劲默认开始时间
    public InputField DefaultStartTime;
    public InputField DefaultDuring;// 默认的播放时长

    private void Awake()
    {
        instance = this;
    }
    void Start () {
        InitConfigUI();//初始化所有UI组件

        PersonMaxShowTime.onEndEdit.AddListener(RevisePersonMaxShowTime);
        HistoryTrack.onEndEdit.AddListener(ReviseHistoryTrack);

        UseNavMesh.onValueChanged.AddListener(IsOpenNavMesh);
        SingleMode.onValueChanged.AddListener(IsOpenSingleMode);
        UseNavMeshSingleFun.onValueChanged.AddListener(IsOpenPathfinding);
        UseNavMeshAStarFun.onValueChanged.AddListener(IsOpenAStar);

        HistoryTrack.onEndEdit.AddListener(SetStartHour);
        DefaultStartTime.onEndEdit.AddListener(SetStartMin);
        DefaultDuring.onEndEdit.AddListener(SetDuring);
    }
	
    /// <summary>
    /// 从XML读取数据初始化所有UI
    /// </summary>
    public void InitConfigUI()
    {
        UseNavMesh.isOn = SystemSettingHelper.systemSetting.LocationSetting.EnableNavMesh;
        if(UseNavMesh.isOn)
        {
            SingleMode.isOn = SystemSettingHelper.systemSetting.LocationSetting.NavMode == 0;
            UseNavMeshSingleFun.isOn = SystemSettingHelper.systemSetting.LocationSetting.NavMode == 1;
            UseNavMeshAStarFun.isOn = SystemSettingHelper.systemSetting.LocationSetting.NavMode == 2;
        }
        else
        {
            SingleMode.isOn = false;
            UseNavMeshSingleFun.isOn = false;
            UseNavMeshAStarFun.isOn = false;
        }
       

        PersonMaxShowTime.text = SystemSettingHelper.systemSetting.LocationSetting.hideTimeHours.ToString();
        HistoryTrack.text = SystemSettingHelper.systemSetting.LocationSetting.HistoryIntervalTime.ToString();

        HistoryTrackDefaultStartTime.text = SystemSettingHelper.systemSetting.HistoryPathSetting.StartHour.ToString();
        DefaultStartTime.text = SystemSettingHelper.systemSetting.HistoryPathSetting.StartMinute.ToString();
        DefaultDuring.text = SystemSettingHelper.systemSetting.HistoryPathSetting.Duration.ToString();
    }
    /// <summary>
    /// 修改人员最大显示时间并保存XML
    /// </summary>
    public void  RevisePersonMaxShowTime(string text)
    {
        SystemSettingHelper.systemSetting.LocationSetting.hideTimeHours = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 修改历史轨迹中断时间设置
    /// </summary>
    public void ReviseHistoryTrack(string text)
    {
        SystemSettingHelper.systemSetting.LocationSetting.HistoryIntervalTime = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否开启子toggle
    /// </summary>
    public void IsOpenChildeToggle(bool value)
    {
        SingleMode.enabled = value; //禁用或者开启子toggle
        UseNavMeshSingleFun.enabled = value;
        UseNavMeshAStarFun.enabled = value;
        if(value)
        {
            SingleMode.isOn = SystemSettingHelper.systemSetting.LocationSetting.NavMode == 0;
            UseNavMeshSingleFun.isOn = SystemSettingHelper.systemSetting.LocationSetting.NavMode == 1;
            UseNavMeshAStarFun.isOn = SystemSettingHelper.systemSetting.LocationSetting.NavMode == 2;
        }
        else
        {
            SingleMode.isOn = value;
            UseNavMeshSingleFun.isOn = value;
            UseNavMeshAStarFun.isOn = value;
        }
        //SingleMode.isOn = value;
        //UseNavMeshSingleFun.isOn = value;
        //UseNavMeshAStarFun.isOn = value;
    }
    /// <summary>
    /// 是否启用NavMesh
    /// </summary>
    public void IsOpenNavMesh(bool value)
    {
        UseNavMesh.isOn = value;
        IsOpenChildeToggle(value);//禁用或者开启子toggle
        SystemSettingHelper.systemSetting.LocationSetting.EnableNavMesh = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否启用NavMesh简单模式
    /// </summary>
    public void IsOpenSingleMode(bool value)
    {
        SystemSettingHelper.systemSetting.LocationSetting.NavMode = 0;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否启用NavMesh寻路模式
    /// </summary>
    public void IsOpenPathfinding(bool value)
    {
        SystemSettingHelper.systemSetting.LocationSetting.NavMode = 1;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否启用NavMeshA星寻路模式
    /// </summary>
    public void IsOpenAStar(bool value)
    {
        SystemSettingHelper.systemSetting.LocationSetting.NavMode = 2;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 设定历史路径默认开始时间小时
    /// </summary>
    public void SetStartHour(string text)
    {
        SystemSettingHelper.systemSetting.HistoryPathSetting.StartHour = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 设定默认开始时间分钟,一个单位代表10分钟
    /// </summary>
    public void SetStartMin(string text)
    {
        SystemSettingHelper.systemSetting.HistoryPathSetting.StartMinute = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 设定默认播放时长，小时为单位
    /// </summary>
    public void SetDuring(string text)
    {
        SystemSettingHelper.systemSetting.HistoryPathSetting.Duration = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }



    void Update () {
		
	}
}
