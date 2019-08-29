using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocketConfigXML : MonoBehaviour {

    public static SocketConfigXML instance;
    public Toggle IsOpenHoney;//是否开启霍尼韦尔视频

    public InputField DefaultLoginIP;//软件默认登入IP
    public InputField VideoSystemIP;//视频监控系统IP设置
    public InputField VideoSystemUserName;//视屏监控系统用户名
    public InputField VideoSystemPassword;//视频监控系统登入密码
    public InputField VideoMaxConnectTime;//视频最大重连次数
    private void Awake()
    {
        instance = this;
    }
    void Start () {
        InitConfigUI(SystemSettingHelper.systemSetting.HoneyWellSetting.EnableHoneyWell);//初始化页面UI

        IsOpenHoney.onValueChanged.AddListener(OpenHoney);//是否开启霍尼韦尔视频

        VideoSystemIP.onEndEdit.AddListener(ReviseVideoSystemIP);
        VideoSystemUserName.onEndEdit.AddListener(ReviseVideoSystemUserName);
        VideoSystemPassword.onEndEdit.AddListener(ReviseVideoSystemPassword);
        if (VideoMaxConnectTime != null) VideoMaxConnectTime.onEndEdit.AddListener(ReviseVideoReConnectTime);
    }
    /// <summary>
    /// 设置视频最大重连次数
    /// </summary>
    /// <param name="text"></param>
    private void ReviseVideoReConnectTime(string text)
    {
        SystemSettingHelper.systemSetting.HoneyWellSetting.MaxConnectTime = text;
        SystemSettingHelper.SaveSystemSetting();
    }

    /// <summary>
    /// 加载激活状态的sprite
    /// </summary>
    public void LoadAbleSprite()
    {
        VideoSystemIP.image.overrideSprite = Resources.Load("ChangeUI/textbox 1", typeof(Sprite)) as Sprite;
        VideoSystemUserName.image.overrideSprite = Resources.Load("ChangeUI/textbox 1", typeof(Sprite)) as Sprite;
        VideoSystemPassword.image.overrideSprite = Resources.Load("ChangeUI/textbox 1", typeof(Sprite)) as Sprite;
    }
    /// <summary>
    /// 加载未激活状态下的sprite
    /// </summary>
    public void LoadUnableSprite()
    {
        VideoSystemIP.image.overrideSprite = Resources.Load("ChangeUI/textbox_unable 1", typeof(Sprite)) as Sprite;
        VideoSystemUserName.image.overrideSprite = Resources.Load("ChangeUI/textbox_unable 1", typeof(Sprite)) as Sprite;
        VideoSystemPassword.image.overrideSprite = Resources.Load("ChangeUI/textbox_unable 1", typeof(Sprite)) as Sprite;
    }
	/// <summary>
    /// 开启霍尼韦尔视频
    /// </summary>
    public void OpenHoney(bool value)
    {
        VideoSystemIP.enabled = value;
        VideoSystemUserName.enabled = value;
        VideoSystemPassword.enabled = value;
        CloseHoney(value); //处理填充信息
        if (value)
        {
            LoadAbleSprite();
        }
        else
        {
            LoadUnableSprite();
        }
        
        SystemSettingHelper.systemSetting.HoneyWellSetting.EnableHoneyWell = value;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 开启或者关闭霍尼韦尔时，textbox内文字内容的填充和清理
    /// </summary>
    public void CloseHoney(bool value)
    {
        if(value)
        {
            VideoSystemIP.text = SystemSettingHelper.systemSetting.HoneyWellSetting.Ip;
            VideoSystemUserName.text = SystemSettingHelper.systemSetting.HoneyWellSetting.UserName;
            VideoSystemPassword.text = SystemSettingHelper.systemSetting.HoneyWellSetting.PassWord;
        }
        else
        {
            VideoSystemIP.text = null;
            VideoSystemUserName.text = null;
            VideoSystemPassword.text = null;
        }
    }
    /// <summary>
    /// 获取XML信息初始化TextBox内的信息
    /// </summary>
    public void InitConfigUI(bool value)
    {        
        IsOpenHoney.isOn = SystemSettingHelper.systemSetting.HoneyWellSetting.EnableHoneyWell;
        DefaultLoginIP.text = SystemSettingHelper.systemSetting.CommunicationSetting.Ip1;
        OpenHoney(value);
        if (VideoMaxConnectTime != null)
        {
            VideoMaxConnectTime.text = SystemSettingHelper.systemSetting.HoneyWellSetting.MaxConnectTime.ToString();
        }
    }
    /// <summary>
    /// 修改视频监控IP并保存XML
    /// </summary>
    public void ReviseVideoSystemIP(string text)
    {
        SystemSettingHelper.systemSetting.HoneyWellSetting.Ip = text;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 修改视频监控登入名并保存XML
    /// </summary>
    public void ReviseVideoSystemUserName(string text)
    {
        SystemSettingHelper.systemSetting.HoneyWellSetting.UserName = text;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    ///修改视频监控密码并保存XML
    /// </summary>
    public void ReviseVideoSystemPassword(string text)
    {
        SystemSettingHelper.systemSetting.HoneyWellSetting.PassWord = text;
        SystemSettingHelper.SaveSystemSetting();
    }
    void Update () {
		
	}
}
