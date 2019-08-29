using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DebugConfigXML : MonoBehaviour {

    public static DebugConfigXML instance;

    public Toggle IsDebug;//是否开启调式模式
    public Toggle KeepSocket;//是否保持通信开启状态
    public Toggle IsRemoteDebug;//是否开启远程调试

    public Toggle EnterFactoryLoadEquip;//进入电厂时加载园区设备
    public Toggle LoadRecordEquip;//加载记载设备
    public Toggle ShowEquipInTree;//在树中显示设备
    public Toggle NotLoadAllEquip;//不加载任何设备

    public InputField PositionRefreshTime;//位置信息刷新间隔
    public InputField PersonTreeRefreshTime;//人员树节点刷新时间间隔
    public InputField AreaInfoRefreshTime;//区域统计信息刷新时间间隔
    public InputField DepartTreeRefreshTime;//部门树刷新时间间隔
    public InputField NearVideoRefreshTime;//附近摄像头刷新时间间隔
    public InputField ScreenshotSaveRefreshTime;//截图保存刷新间隔刷新间隔

    private void Awake()
    {
        instance = this;
    }
    void Start () {
        InitConfigUI();//初始化界面的UI
        IsDebug.onValueChanged.AddListener(IsDebugFun);
        KeepSocket.onValueChanged.AddListener(KeepSocketFun);
        IsRemoteDebug.onValueChanged.AddListener(IsRemoteDebugFun);
        EnterFactoryLoadEquip.onValueChanged.AddListener(EnterFactoryLoadEquipFun);
        LoadRecordEquip.onValueChanged.AddListener(LoadRecordEquipFun);
        ShowEquipInTree.onValueChanged.AddListener(ShowEquipInTreeFun);
        NotLoadAllEquip.onValueChanged.AddListener(NotLoadAllEquipFun);

        PositionRefreshTime.onEndEdit.AddListener(PositionRefreshTimeFun);
        PersonTreeRefreshTime.onEndEdit.AddListener(PersonTreeRefreshTimeFun);
        AreaInfoRefreshTime.onEndEdit.AddListener(AreaInfoRefreshTimeFun);
        DepartTreeRefreshTime.onEndEdit.AddListener(DepartTreeRefreshTimeFun);
        NearVideoRefreshTime.onEndEdit.AddListener(NearVideoRefreshTimeFun);
        ScreenshotSaveRefreshTime.onEndEdit.AddListener(ScreenshotSaveRefreshTimeFun);

    }
    /// <summary>
    /// 读取XMl数据初始化配置界面UI
    /// </summary>
	public void InitConfigUI()
    {
        IsDebug.isOn = SystemSettingHelper.systemSetting.IsDebug;
        KeepSocket.isOn = SystemSettingHelper.systemSetting.DebugSetting.IsCloseCommunication;
        IsRemoteDebug.isOn = SystemSettingHelper.systemSetting.DebugSetting.IsRemoteMode;

        EnterFactoryLoadEquip.isOn = SystemSettingHelper.systemSetting.DeviceSetting.LoadParkDevWhenEnter;
        LoadRecordEquip.isOn = SystemSettingHelper.systemSetting.DeviceSetting.LoadAnchorDev;
        ShowEquipInTree.isOn = SystemSettingHelper.systemSetting.DeviceSetting.ShowDevInTree;
        NotLoadAllEquip.isOn = SystemSettingHelper.systemSetting.DeviceSetting.NotLoadAllDev;

        PositionRefreshTime.text = SystemSettingHelper.systemSetting.RefreshSetting.TagPos.ToString();
        PersonTreeRefreshTime.text = SystemSettingHelper.systemSetting.RefreshSetting.PersonTree.ToString();
        AreaInfoRefreshTime.text = SystemSettingHelper.systemSetting.RefreshSetting.AreaStatistics.ToString();
        DepartTreeRefreshTime.text = SystemSettingHelper.systemSetting.RefreshSetting.DepartmentTree.ToString();
        NearVideoRefreshTime.text = SystemSettingHelper.systemSetting.RefreshSetting.NearCamera.ToString();
        ScreenshotSaveRefreshTime.text = SystemSettingHelper.systemSetting.RefreshSetting.ScreenShot.ToString();
    }
    /// <summary>
    /// 是否开启调试
    /// </summary>
    public void IsDebugFun(bool value)
    {
        SystemSettingHelper.systemSetting.IsDebug = value;
        IsDebug.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否保持通信开启状态
    /// </summary>
    public void KeepSocketFun(bool value)
    {
        SystemSettingHelper.systemSetting.DebugSetting.IsCloseCommunication = value;
        KeepSocket.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否开启远程调试
    /// </summary>
    public void IsRemoteDebugFun(bool value)
    {
        SystemSettingHelper.systemSetting.DebugSetting.IsRemoteMode = value;
        IsRemoteDebug.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 进入电厂时加载园区设备
    /// </summary>
    public void EnterFactoryLoadEquipFun(bool value)
    {
        SystemSettingHelper.systemSetting.DeviceSetting.LoadParkDevWhenEnter = value;
        EnterFactoryLoadEquip.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 加载记载设备
    /// </summary>
    public void LoadRecordEquipFun(bool value)
    {
        SystemSettingHelper.systemSetting.DeviceSetting.LoadAnchorDev = value;
        LoadRecordEquip.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 在树中显示设备
    /// </summary>
    public void ShowEquipInTreeFun(bool value)
    {
        SystemSettingHelper.systemSetting.DeviceSetting.ShowDevInTree = value;
        ShowEquipInTree.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 不加载任何设备
    /// </summary>
    public void NotLoadAllEquipFun(bool value)
    {
        SystemSettingHelper.systemSetting.DeviceSetting.NotLoadAllDev = value;
        NotLoadAllEquip.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 位置信息刷新间隔
    /// </summary>
    /// <param name="text"></param>
    public void PositionRefreshTimeFun(string text)
    {
        SystemSettingHelper.systemSetting.RefreshSetting.TagPos = float.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 人员树节点刷新时间间隔
    /// </summary>
    /// <param name="text"></param>
    public void PersonTreeRefreshTimeFun(string text)
    {
        SystemSettingHelper.systemSetting.RefreshSetting.PersonTree= int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 区域统计信息刷新时间间隔
    /// </summary>
    /// <param name="text"></param>
    public void AreaInfoRefreshTimeFun(string text)
    {
        SystemSettingHelper.systemSetting.RefreshSetting.AreaStatistics = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 部门树刷新时间间隔
    /// </summary>
    /// <param name="text"></param>
    public void DepartTreeRefreshTimeFun(string text)
    {
        SystemSettingHelper.systemSetting.RefreshSetting.DepartmentTree = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 附近摄像头刷新时间间隔
    /// </summary>
    /// <param name="text"></param>
    public void NearVideoRefreshTimeFun(string text)
    {
        SystemSettingHelper.systemSetting.RefreshSetting.NearCamera = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 截图保存刷新间隔刷新间隔
    /// </summary>
    /// <param name="text"></param>
    public void ScreenshotSaveRefreshTimeFun(string text)
    {
        SystemSettingHelper.systemSetting.RefreshSetting.ScreenShot = int.Parse(text);
        SystemSettingHelper.SaveSystemSetting();
    }

    void Update () {
		
	}
}
