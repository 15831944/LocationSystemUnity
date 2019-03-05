using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseStationFollowUI : MonoBehaviour {

    public static BaseStationFollowUI currentFollowUI;//当前打开的基站信息

    public Toggle StateToggle;
    public Text Title;
    public Text CodeIp;
    public Text CADInfo;

    public GameObject window;//窗体显示部分

    private DevNode dev;//基站Id
    private Archor currentArchor;//基站信息
	// Use this for initialization
	void Start () {
        StateToggle.onValueChanged.AddListener(SetWindowState);
    }
    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <param name="_devId"></param>
    public void InitInfo(DevNode _dev)
    {
        dev = _dev;
    }

    private void SetWindowState(bool value)
    {
        if(value)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    /// <summary>
    /// 显示基站信息
    /// </summary>
    public void Open()
    {
        if (currentFollowUI != null&&currentFollowUI!=this) currentFollowUI.StateToggle.isOn = false;
        GetArchorDevInfo(value=> 
        {
            window.SetActive(true);
            currentFollowUI = this;
            if (!value)
            {
                Title.text = dev.Info.Name;
                CodeIp.text = string.Format("<color=#6DECFEFF>Code:</color>{0}  <color=#6DECFEFF>IP:</color>{1}", "", "");
                CADInfo.text = string.Format("<color=#6DECFEFF>X:</color>{0} <color=#6DECFEFF>Y:</color>{1} <color=#6DECFEFF>Z:</color>{2}", "", "", "");
            }      
        });
        
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        window.SetActive(false);
    }
    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <param name="callBack"></param>
    private void GetArchorDevInfo(Action<bool> callBack)
    {
        try
        {
            if (currentArchor != null)
            {
                if (callBack != null) callBack(true);
                return;
            }
            ThreadManager.Run(() =>
            {
                currentArchor = CommunicationObject.Instance.GetArchorByDevId(dev.Info.Id);
            }, () =>
            {
                if (currentArchor != null)
                {
                    Title.text = currentArchor.Name;
                    CodeIp.text = string.Format("<color=#6DECFEFF>Code:</color>{0}  <color=#6DECFEFF>IP:</color>{1}", currentArchor.Code, currentArchor.Ip);
                    CADInfo.text = string.Format("<color=#6DECFEFF>X:</color>{0}  <color=#6DECFEFF>Y:</color>{1}  <color=#6DECFEFF>Z:</color>{2}", currentArchor.X.ToString("f2"), currentArchor.Y.ToString("f2"), currentArchor.Z.ToString("f2"));
                    if (callBack != null) callBack(true);
                }
                else
                {
                    if (callBack != null) callBack(false);
                }
            }, "");
        }catch(Exception e)
        {
            Debug.LogError(string.Format("BaseStationFollowUI.GetDevInfo Error:{0}",e.ToString()));
            if (callBack != null) callBack(false);
        }
        
    }
}
