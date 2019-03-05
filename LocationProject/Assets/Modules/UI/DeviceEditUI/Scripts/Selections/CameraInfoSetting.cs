using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraInfoSetting : MonoBehaviour {
    public InputField IP;
    public InputField UserName;
    public InputField PassWord;
    public InputField Port;
    public InputField CameraIndex;//通道号

    public Toggle SettingToggle;
    public GameObject SettingWindow;//设置窗体
    private CameraDevController CameraTemp;//参数缓存
	// Use this for initialization
	void Start () {
        SettingToggle.onValueChanged.AddListener(OnToggleValueChanged);
        IP.onValueChanged.AddListener(OnCameraInfoValueChanged);
        UserName.onValueChanged.AddListener(OnCameraInfoValueChanged);
        PassWord.onValueChanged.AddListener(OnCameraInfoValueChanged);
        Port.onValueChanged.AddListener(OnCameraInfoValueChanged);
        CameraIndex.onValueChanged.AddListener(OnCameraInfoValueChanged);

    }
    private void OnToggleValueChanged(bool isOn)
    {
        SettingWindow.SetActive(isOn);
    }
    /// <summary>
    /// 输入框信息改变
    /// </summary>
    /// <param name="value"></param>
    private void OnCameraInfoValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value)|| CameraTemp == null) return;
        if(IsValueChanged())
        {
            SaveCameraInfo();
            CommunicationObject service = CommunicationObject.Instance;
            if(service)
            {
                Dev_CameraInfo info = CameraTemp.GetCameraInfo(CameraTemp.Info);
                bool isSave = service.ModifyCameraInfo(info);
                Debug.Log("Save CameraInfo:"+isSave);
            }
        }
        else
        {
            Debug.Log("CameraInfo not change...");
        }
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        SettingToggle.isOn = false;
        SettingToggle.gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="camDev"></param>
    public void Show(CameraDevController camDev)
    {
        SettingToggle.gameObject.SetActive(true);
        if (camDev==null||camDev.Info==null)
        {
            Debug.LogError("CameraDev==null||camDev.Info==null");
            return;
        }
        CameraTemp = camDev;
        Dev_CameraInfo info = CameraTemp.GetCameraInfo(camDev.Info);
        if (info!=null)
        {
            SetInputFiledValue(info.Ip,info.UserName,info.PassWord,info.Port.ToString(),info.CameraIndex.ToString());
        }
        else
        {
            SetInputFiledValue("","","","","");
        }
    }
    /// <summary>
    /// 设置输入框的值
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    /// <param name="port"></param>
    /// <param name="cameraIndex"></param>
    private void SetInputFiledValue(string ip,string userName,string passWord,string port,string cameraIndex)
    {
        IP.text = ip;
        UserName.text = userName;
        PassWord.text = passWord;
        Port.text = port;
        CameraIndex.text = cameraIndex;
    }
    /// <summary>
    /// 参数是否变化
    /// </summary>
    /// <returns></returns>
    private bool IsValueChanged()
    {
        Dev_CameraInfo info = CameraTemp.GetCameraInfo(CameraTemp.Info);
        if (info == null) return false;
        if (info.Ip != IP.text) return true;
        else if (info.UserName != UserName.text) return true;
        else if (info.PassWord != PassWord.text) return true;
        else if (info.Port.ToString() != Port.text) return true;
        else if (info.CameraIndex.ToString() != CameraIndex.text) return true;
        else return false;
    }

    /// <summary>
    /// 更新摄像机参数
    /// </summary>
    private void SaveCameraInfo()
    {
        Dev_CameraInfo info = CameraTemp.GetCameraInfo(CameraTemp.Info);
        info.Ip = IP.text;
        info.UserName = UserName.text;
        info.PassWord = PassWord.text;
        info.Port = TryParseInt(Port.text);
        info.CameraIndex = TryParseInt(CameraIndex.text);
    }

    /// <summary>
    /// 字符转int
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int TryParseInt(string num)
    {
        try
        {
            int value = int.Parse(num);
            return value;
        }catch(Exception e)
        {
            return 0;
        }
    }
}
