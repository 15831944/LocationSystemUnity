using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using System;

public class ShowConfigXML : MonoBehaviour {
    public static ShowConfigXML instance;
    public Dropdown ResolutionDrop; //分辨率选择框
    public Dropdown FullScreenDrop;//是否设置成全屏   

    public Toggle ShowLeftTopoTog;
    public Toggle ShowRightDownAlarmTog;
    public Toggle ShowRightTopInfoTog;
    public Toggle ShowHomePageTog;

    //public GameObject PersonTopo_LeftTopo_1;//左侧拓补栏 
    //public GameObject PersonTopo_LeftTopo_2;//左侧拓补栏
    //public GameObject DepartTopo_LeftTopo;//左侧部门拓补栏
    //public GameObject TwoTicketTopo_LeftTopo;//左侧两票拓补栏
    //public GameObject InspectionTopo_LeftTopo;//左侧巡检拓补

    public GameObject[] LeftTopo;//左侧拓补栏
    public GameObject RightDownAlarm;//右下角告警推送
    public GameObject RightTopInfo;//右上角统计信息

    private void Awake()
    {
        instance = this;
    }
    void Start () {
        ResolutionDrop.onValueChanged.AddListener(ResolutionSetting);
        FullScreenDrop.onValueChanged.AddListener(FullScreenSetting);

        ShowLeftTopoTog.onValueChanged.AddListener(ShowLeftTopo);
        ShowRightDownAlarmTog.onValueChanged.AddListener(ShowRightDownAlarm);
        ShowRightTopInfoTog.onValueChanged.AddListener(ShowRightTopInfo);
        ShowHomePageTog.onValueChanged.AddListener(ShowHomePage);

        InitConfigUI();

    }
    /// <summary>
    /// 设置相对应xml的UI初始化设置
    /// </summary>
    public void InitConfigUI()
    {
        instance.JudgeResolution(SystemSettingHelper.systemSetting.ResolutionSetting.Width.ToString() +"*"+ SystemSettingHelper.systemSetting.ResolutionSetting.High.ToString());//从xml获取分辨率之后设置默认UI
        instance.JugdeIsFullScreen(SystemSettingHelper.systemSetting.IsFullScreen);
        

        ShowLeftTopoFun(SystemSettingHelper.systemSetting.IsShowLeftTopo);//是否显示左侧拓补栏
        ShowRightDownAlarmFun(SystemSettingHelper.systemSetting.IsShowRightDownAlarm);//是否显示右下角告警推送
        ShowRightTopInfoFun(SystemSettingHelper.systemSetting.IsShowRightTopInfo);//是否显示右上角统计信息
        ShowHomePageFun(SystemSettingHelper.systemSetting.IsShowHomePage);
    }

    private void JudgeResolution(Func<string> toString)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 选择屏幕分辨率的值
    /// </summary>
    /// <param name="value"></param>
    public void ResolutionSetting(int value)
    {
        switch(value)
        {
            case 0:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1920;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 1200;
                break;
            case 1:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1920;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 1080;
                break;
            case 2:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1680;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 1050;
                break;
            case 3:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1600;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 1200;
                break;
            case 4:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1600;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 900;
                break;
            case 5:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1440;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 1050;
                break;
            case 6:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1440;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 900;
                break;
            case 7:
                SystemSettingHelper.systemSetting.ResolutionSetting.Width = 1366;
                SystemSettingHelper.systemSetting.ResolutionSetting.High = 768;
                break;
        }
        SetWindow(SystemSettingHelper.systemSetting.ResolutionSetting.Width, SystemSettingHelper.systemSetting.ResolutionSetting.High);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 判断分辨率的宽度值来设定弹框的初始UI
    /// </summary>
    public void JudgeResolution(string value)
    {
        switch (value)
        {
            case "1920*1200":
                ResolutionDrop.value = 0;
                break;
            case "1920*1080":
                ResolutionDrop.value = 1;
                break;
            case "1680*1050":
                ResolutionDrop.value = 0;
                break;
            case "1600*1200":
                ResolutionDrop.value = 0;
                break;
            case "1600*900":
                ResolutionDrop.value = 0;
                break;
            case "1440*1050":
                ResolutionDrop.value = 0;
                break;
            case "1440*900":
                ResolutionDrop.value = 0;
                break;
            case "1366*768":
                ResolutionDrop.value = 0;
                break;

        }
    }
    /// <summary>
    /// 是否设置成全屏
    /// </summary>
    public void FullScreenSetting(int value)
    {
        switch (value)
        {
            case 0:
                SystemSettingHelper.systemSetting.IsFullScreen = true;
                break;
            case 1:
                SystemSettingHelper.systemSetting.IsFullScreen = false;
                break;
        }
        SetFullScreen(SystemSettingHelper.systemSetting.IsFullScreen);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 判断是否全屏来设置弹框的初始UI
    /// </summary>
    public void JugdeIsFullScreen(bool value)
    {
        if (value)
            FullScreenDrop.value = 0;
        else
            FullScreenDrop.value = 1;
    }
    
    /// <summary>
    /// 是否显示左侧拓补栏
    /// </summary>
    public void ShowLeftTopo(bool value)
    {
        SystemSettingHelper.systemSetting.IsShowLeftTopo = value;
        ShowLeftTopoFun(value);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 带Fun的都是UI组件的相应初始化 相对应UI对象的显示
    /// </summary>
    /// <param name="IsShow"></param>
    public void ShowLeftTopoFun(bool IsShow)
    {
        foreach(var item in LeftTopo)
        {
            item.SetActive(IsShow);
        }
        //PersonTopo_LeftTopo_1.SetActive(IsShow);
        //PersonTopo_LeftTopo_2.SetActive(IsShow);
        ShowLeftTopoTog.isOn = IsShow;
    }

    /// <summary>
    /// 是否显示右下角告警推送
    /// </summary>
    public void ShowRightDownAlarm(bool value)
    {
        SystemSettingHelper.systemSetting.IsShowRightDownAlarm = value;
        ShowRightDownAlarmFun(value);
        SystemSettingHelper.SaveSystemSetting();
    }
    public void ShowRightDownAlarmFun(bool IsShow)
    {
        RightDownAlarm.SetActive(IsShow);
        ShowRightDownAlarmTog.isOn = IsShow;
    }

    /// <summary>
    /// 是否显示右上角统计信息
    /// </summary>
    public void ShowRightTopInfo(bool value)
    {
        SystemSettingHelper.systemSetting.IsShowRightTopInfo = value;
        ShowRightTopInfoFun(value);
        SystemSettingHelper.SaveSystemSetting();
    }
    public void ShowRightTopInfoFun(bool IsShow)
    {
        RightTopInfo.SetActive(IsShow);
        ShowRightTopInfoTog.isOn = IsShow;
    }

    /// <summary>
    /// 是否显示首页
    /// </summary>
    /// <param name="value"></param>
    public void ShowHomePage(bool value)
    {
        SystemSettingHelper.systemSetting.IsShowHomePage = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    public void ShowHomePageFun(bool IsShow)
    {
        ShowHomePageTog.isOn = IsShow;
    }
    /// <summary>
    /// 设置屏幕分辨率
    /// </summary>
    public void SetWindow(int width, int high)
    {
        Screen.SetResolution(width, high, SystemSettingHelper.systemSetting.IsFullScreen);
    }
    /// <summary>
    /// 是否设置全屏
    /// </summary>
    /// <param name="value"></param>
    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }
	void Update () {
		
	}
}
