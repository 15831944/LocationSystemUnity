using Base.Common.Extensions;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGroupStateManage : MonoBehaviour {
    public static PowerGroupStateManage Instance;
    public DevNode FirstPowerGroup;
    public DevNode SecondPowerGroup;
    [Tooltip("一号机组子物体")]
    public List<GameObject> FirstGroupPart;
    [Tooltip("二号机组子物体")]
    public List<GameObject> SecondGroupPart;
    /// <summary>
    /// 一号机组是否开启（四会热电）
    /// </summary>
    public bool FirstPowerGroupOn { get { return firstGroupStateOn; } }
    /// <summary>
    /// 二号机组是否开启（四会热电）
    /// </summary>
    public bool SecondPowerGroupOn { get { return secondGroupStateOn; } }

    private bool firstGroupStateOn;
    private bool secondGroupStateOn;
    private int refreshTimeBackup=10;//防止出现0（未设置）的情况，增加一个备用刷新时间

    private string FirstGroupKKS = "1号机组负荷";
    private string SecondGroupKKS = "2号机组负荷";
    /// <summary>
    /// 是否正在连接服务器
    /// </summary>
    private bool isConnnectServer;
    void Awake()
    {
        Instance = this;    
    }
    // Use this for initialization
    void Start () {
#if !UNITY_EDITOR
        StartPowerGetRepeating();
#endif
    }
    /// <summary>
    /// 开启机组状态检测
    /// </summary>
    public void OpenPowerGroupStateChecker()
    {
        if (IsInvoking("StartKKSChecker")) CancelInvoke("StartKKSChecker");
        if (IsInvoking("TryGetGroupState")) CancelInvoke("TryGetGroupState");
        InvokeRepeating("StartKKSCheck",0,1);
    }
    /// <summary>
    /// 检测机组的KKS，是否加载完
    /// </summary>
    private void StartKKSCheck()
    {
        if (FirstPowerGroup == null || SecondGroupPart == null) return;
        if(FirstPowerGroup.Info!=null&&SecondPowerGroup.Info!=null)
        {
            Debug.Log("机组数据加载完成，开始获取机组运行状态...");
            if (IsInvoking("StartKKSCheck"))
            {
                CancelInvoke("StartKKSCheck");
            }
            StartPowerGetRepeating();
        }
    }
    /// <summary>
    /// 启动机组状态检测
    /// </summary>
    private void StartPowerGetRepeating()
    {
        Debug.Log("开始获取机组运行状态...");
        RefreshSetting refreshSetting = SystemSettingHelper.refreshSetting;
        if(IsInvoking("TryGetGroupState"))
        {
            CancelInvoke("TryGetGroupState");
        }
        int refreshTimeTemp = refreshSetting.GroupPowerState == 0 ? refreshTimeBackup : refreshSetting.GroupPowerState;
        InvokeRepeating("TryGetGroupState",0, refreshTimeTemp);
    }
    
	/// <summary>
    /// 获取机组运行状态
    /// </summary>
    private void TryGetGroupState()
    {
        if (isConnnectServer) return;
        isConnnectServer = true;
        try
        {
            DateTime timeRecord = DateTime.Now;
            TryGetPowerValueByKKS(FirstGroupKKS, a =>
            {
                //Debug.LogError("数据获取成功，1号机组运行状态：" + a);
                firstGroupStateOn = a;
                SetGroupPartState(firstGroupStateOn, FirstGroupPart);
            });
            TryGetPowerValueByKKS(SecondGroupKKS, b =>
            {
                //Debug.LogErrorFormat("数据获取成功，2号机组运行状态：{0} Time:{1}ms",b,(DateTime.Now-timeRecord).TotalMilliseconds);
                secondGroupStateOn = b;
                SetGroupPartState(secondGroupStateOn, SecondGroupPart);
                isConnnectServer = false;
            });
        }catch(Exception e)
        {
            isConnnectServer = false;
            Debug.LogError("Error:PowerGroupStateManage.TryGetGroupState->"+e.ToString());
        }
    } 
    /// <summary>
    /// 设置机组运行状态
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="objs"></param>
    private void SetGroupPartState(bool isActive,List<GameObject>objs)
    {
        if (objs == null || objs.Count == 0) return;
        foreach(var item in objs)
        {
            if(item.activeInHierarchy!=isActive)
            {
                item.SetActive(isActive);
            }
        }
    }
    /// <summary>
    /// 通过kks，获取机组运行状态
    /// </summary>
    /// <param name="kks"></param>
    /// <param name="onDataRecieved"></param>
    private void TryGetPowerValueByKKS(string kks,Action<bool>onDataRecieved=null)
    {
        Dev_Monitor monitorInfo = null;
        ThreadManager.Run(() =>
        {
            monitorInfo = CommunicationObject.Instance.GetDevMonitor(kks);
        }, () =>
        {
            bool isGroupOn = GetStateByDevMonitor(monitorInfo);
            if (onDataRecieved != null) onDataRecieved(isGroupOn);
        }, "");
    }
    /// <summary>
    /// 通过devinfo，获取机组信息
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="onDataRecieved"></param>
    private void TryGetPowerGroupValue(DevNode dev,Action<bool>onDataRecieved=null)
    {
        if (dev == null) return;
        DevInfo info = dev.Info;
        if (info == null || string.IsNullOrEmpty(info.KKSCode)) return;
        TryGetPowerValueByKKS(info.KKSCode,onDataRecieved);
    }

    private bool GetStateByDevMonitor(Dev_Monitor monitorTemp)
    {
        try
        {
            DevMonitorNode[] nodeGroup = monitorTemp.MonitorNodeList;
            if (nodeGroup == null || nodeGroup.Length == 0)
            {
                Debug.LogError("Error:PowerGroupStateManage.GetStateByDevMonitor->MonitorNode is null!");
                return false;
            }
            else
            {
                DevMonitorNode node = nodeGroup[0];
                float value = node.Value.ToFloat();
                return value > 0;
            }
        }catch(Exception e)
        {
            return false;
        }
    }
}
