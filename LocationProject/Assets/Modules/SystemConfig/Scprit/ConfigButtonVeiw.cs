using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConfigButtonVeiw : MonoBehaviour {

    public Button Bt_Close; //关闭按钮
    public Button SystemConfigButton;
    public Toggle[] togs; //按钮

    public GameObject[] ConfigView; // 配置子界面
    public GameObject MainConfigPage;//配置主界面
    void Start () {
        Bt_Close.onClick.AddListener(CloseMainConfigPage); //隐藏所有子配置界面
	    foreach(var item in togs) //所有按钮和子配置界面的绑定事件
        {
            item.onValueChanged.AddListener((bool isOn) => { ChangeImageOrChangePrefab(item, isOn); });
        }
	}
    /// <summary>
    /// 显示按钮初始化图片为高亮ui
    /// 点击配置菜单按钮切换相应的页面
    /// </summary>
    public void ChangeImageOrChangePrefab(Toggle TogChild, bool value)
    {
        HideAllConfigPage();
        switch (TogChild.name)
        {
            case "Show_Toggle":
                ConfigView[0].SetActive(true);
                break;
            case "ModelConfig_Toggle":
                ConfigView[1].SetActive(true);
                break;
            case "HotKey_Toggle":
                ConfigView[2].SetActive(true);
                break;
            case "Socket_Toggle":
                ConfigView[3].SetActive(true);
                break;
            case "Position_Toggle":
                ConfigView[4].SetActive(true);
                break;
            case "Debug_Toggle":
                ConfigView[5].SetActive(true);
                break;
            case "Software_Toggle":
                ConfigView[6].SetActive(true);
                break;
        }
    }
    /// <summary>
    /// 隐藏所有配置界面
    /// </summary>
    public void HideAllConfigPage()
    {
        foreach(var item in ConfigView)
        {
            item.SetActive(false);
        }
    }
    /// <summary>
    /// 隐藏配置主界面
    /// </summary>
	public void HideMainConfigPage()
    {
        //HideAllConfigPage();//关闭配置界面时隐藏所有的配置子界面
        //ConfigView[0].SetActive(true);
        //MainConfigPage.SetActive(false);
        Debug.Log("设置按钮是否被冻结："+ ConfigButton.instance.SystemConfigButton.enabled);
    }
    /// <summary>
    /// 关闭配置主界面
    /// </summary>
    public void CloseMainConfigPage()
    {
        HideAllConfigPage();//关闭配置界面时隐藏所有的配置子界面
        ConfigView[0].SetActive(true);
        MainConfigPage.SetActive(false);
    }
	void Update () {
		
	}
}
