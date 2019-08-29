using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelConfigXML : MonoBehaviour {
    public static ModelConfigXML instance;

    public Dropdown EquipTex;

    public Toggle StaticEquipModel;
    public Toggle DynamicEquipModel;
    public Toggle LoadEquip;

    public Slider BuildModelCountLoadSetting;
    public Slider MainFactoryModelCountLoadSetting;

    public Text BuildModelCount;
    public Text MainFactoryModelCount;

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        InitConfigUI(); //初始化页面所有UI
        EquipTex.onValueChanged.AddListener(SelectEquipTex);//设置生产设备的贴图
        
        StaticEquipModel.onValueChanged.AddListener(IsLoadStaticEquip);//是否从服务端下载静态模型
        DynamicEquipModel.onValueChanged.AddListener(IsLoadDynamicEquip);//是否从服务端下载动态模型
        LoadEquip.onValueChanged.AddListener(LoadEquipFun);//是否下载生产设备模型

        BuildModelCountLoadSetting.onValueChanged.AddListener(BuildModelCountFun);
        MainFactoryModelCountLoadSetting.onValueChanged.AddListener(MainFactoryModelCountFun);
    }

    /// <summary>
    /// 设置生产设备贴图
    /// </summary>
    public void SelectEquipTex(int value)
    {
        switch(value)
        {
            case 0:
                SystemSettingHelper.assetLoadSetting.DeviceResolution = 8192;
                break;
            case 1:
                SystemSettingHelper.assetLoadSetting.DeviceResolution = 4096;
                break;
            case 2:
                SystemSettingHelper.assetLoadSetting.DeviceResolution = 2048;
                break;
            case 3:
                SystemSettingHelper.assetLoadSetting.DeviceResolution = 1024;
                break;
            case 4:
                SystemSettingHelper.assetLoadSetting.DeviceResolution = 512;
                break;
        }
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 初始化模型配置界面所有UI
    /// </summary>
    public void InitConfigUI()
    {
        EquipTexDropIndex(SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceResolution);//通过xml分辨率设置drop初始的UI
        InitBuildSlider();//读取xml数据初始   化Buildslider
        InitMainFactorySlider();//读取xml数据初始化MainFactorySlider 
        StaticEquipModel.isOn = SystemSettingHelper.systemSetting.AssetLoadSetting.BuildingFromFile;
        DynamicEquipModel.isOn = SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceFromFile;
        LoadEquip.isOn = SystemSettingHelper.systemSetting.AssetLoadSetting.LoadDeviceAsset;
    }
    /// <summary>
    /// 设置初始化drop的UI
    /// </summary>
    public void EquipTexDropIndex(int value)
    {
        switch(value)
        {
            case 8192:
                EquipTex.value = 0;
                break;
            case 4096:
                EquipTex.value = 1;
                break;
            case 2048:
                EquipTex.value = 2;
                break;
            case 1024:
                EquipTex.value = 3;
                break;
            case 512:
                EquipTex.value = 4;
                break;

        }
    }
    /// <summary>
    /// 是否从服务端下载静态模型
    /// </summary>
    /// <param name="value"></param>
    public void IsLoadStaticEquip(bool value)
    {
        SystemSettingHelper.systemSetting.AssetLoadSetting.BuildingFromFile = value;
        StaticEquipModel.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否从服务端下载动态模型
    /// </summary>
    /// <param name="value"></param>
    public void IsLoadDynamicEquip(bool value)
    {
        SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceFromFile = value;
        DynamicEquipModel.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 是否加载生产设备
    /// </summary>
    /// <param name="value"></param>
    public void LoadEquipFun(bool value)
    {
        SystemSettingHelper.systemSetting.AssetLoadSetting.LoadDeviceAsset = value;
        LoadEquip.isOn = value;
        SystemSettingHelper.SaveSystemSetting();
    }



    /// <summary>
    /// 设置建筑模型滑动条UI相对应的值
    /// </summary>
    /// <param name="value"></param>
	public void BuildModelCountFun(float value)
    {
        BuildModelCount.text = (int)(value * 25) + "/25";
        SystemSettingHelper.systemSetting.AssetLoadSetting.CacheCount = (int)(value * 25);
        SystemSettingHelper.SaveSystemSetting();
        Debug.Log(BuildModelCount.text);
    }
    /// <summary>
    /// 读取xml数值初始化建筑模型slider以及对应的text
    /// </summary>
    public void InitBuildSlider()
    {
        BuildModelCountLoadSetting.value = (float)SystemSettingHelper.systemSetting.AssetLoadSetting.CacheCount / 25;
        BuildModelCount.text = SystemSettingHelper.systemSetting.AssetLoadSetting.CacheCount + "/25";
        Debug.Log("建筑模型：" + BuildModelCount.text);
    }
    /// <summary>
    /// 设置主厂区模型滑动条UI相对应的值
    /// </summary>
    /// <param name="value"></param>
    public void MainFactoryModelCountFun(float value)
    {
        MainFactoryModelCount.text = (int)(value * 25) + "/25";
        SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceCacheCount = (int)(value * 25);
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 读取xml数值初始化主厂区模型slider以及对应的text
    /// </summary>
    public void InitMainFactorySlider()
    {
        MainFactoryModelCount.text = SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceCacheCount + "/25";
        MainFactoryModelCountLoadSetting.value = (float)SystemSettingHelper.systemSetting.AssetLoadSetting.DeviceCacheCount/25;
        Debug.Log("主厂区模型：" + MainFactoryModelCount.text);
    }

void Update () {
		
	}
}
