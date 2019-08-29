using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConfigButton : MonoBehaviour {
    public static ConfigButton instance;
    public Button SystemConfigButton;
    public  GameObject ConfigMainPage;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        SystemConfigButton = this.GetComponent<Button>();
        SystemConfigButton.onClick.AddListener(ShowOrHideConfigMainPage);
    }
    /// <summary>
    /// 关闭配置界面 供其他脚本使用
    /// </summary>
    public void ChoseConfigView()
    {
        ConfigMainPage.SetActive(false);
    }
    /// <summary>
    /// 显示配置主界面
    /// </summary>
	public void ShowOrHideConfigMainPage()
    {
        if(ConfigMainPage.activeInHierarchy)
            ConfigMainPage.SetActive(false);
        else
            ConfigMainPage.SetActive(true);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
