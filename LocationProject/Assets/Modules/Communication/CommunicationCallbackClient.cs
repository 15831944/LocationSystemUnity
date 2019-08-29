using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.JsonEncoders;
using BestHTTP.SignalR.Messages;
using LitJson;
//using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Common.Utils;
using UnityEngine;

public class CommunicationCallbackClient : MonoSingletonBase<CommunicationCallbackClient>
{  
    /// <summary>
    /// IP地址
    /// </summary>
    private string Ip;
    /// <summary>
    /// 端口
    /// </summary>
    private string Port;
    /// <summary>
    /// SignalR服务地址
    /// </summary>
    Uri URI;

    /// <summary>
    /// The SignalR connection instance
    /// </summary>
    Connection signalRConnection;

    /// <summary>
    /// 告警Hub
    /// </summary>
    public AlarmHub alarmHub = new AlarmHub();

    /// <summary>
    /// EchoHub
    /// </summary>
    public EchoHub echoHub=new EchoHub();

    /// <summary>
    /// 门禁信息Hub
    /// </summary>
    public DoorAccessHub doorAccessHub = new DoorAccessHub();

    /// <summary>
    /// 移动巡检Hub
    /// </summary>
    public InspectionTrackHub inspectionTrackHub = new InspectionTrackHub();

    /// <summary>
    /// 告警Hub
    /// </summary>
    public CameraAlarmHub cameraAlarmHub = new CameraAlarmHub();

    // Use this for initialization
    void Start () {
        Login(CommunicationObject.Instance.ip);
        Debug.Log("CommunicationCallbackClient_ip:" + CommunicationObject.Instance.ip);
        SceneEvents.ConnectStateChange += ReConnectByHeartBeat;
    }
    /// <summary>
    /// 连接SignalR服务端
    /// </summary>
    /// <param name="IpTemp"></param>
    /// <param name="portTemp"></param>
    public void Login(string IpTemp,string portTemp= "8735")
    {
        if(string.IsNullOrEmpty(IpTemp))
        {
            Debug.LogError("SignalR.Login->Ip is null...");
            return;
        }
        //string PortT = Port.ToString();

        //#if !UNITY_EDITOR
        //        Ip = SystemSettingHelper.communicationSetting.Ip2;
        //        PortT = SystemSettingHelper.communicationSetting.Port2;
        //#endif
        portTemp = "8735";
        RegisterImporter();
        InitIpArgs(IpTemp,portTemp);
        URI = new Uri(string.Format("http://{0}:{1}/realtime", IpTemp, portTemp));
        // Create the SignalR connection, passing all the three hubs to it
        signalRConnection = new Connection(URI, alarmHub, echoHub, doorAccessHub,inspectionTrackHub, cameraAlarmHub);//Hub要加到这个列表中，才能收到消息。
        signalRConnection.JsonEncoder = new LitJsonEncoder();
        signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
        signalRConnection.OnError += signalRConnection_OnError;
        signalRConnection.OnNonHubMessage += signaleRConnection_OnNoHubMsg;
        signalRConnection.OnConnected += (connection) =>
        {
            // Call the demo functions
            echoHub.Send("Start Connect...");
        };

        // Start opening the signalR connection
        signalRConnection.Open();        
    }

    /// <summary>
    /// 通过心跳检测，重连SignalR
    /// </summary>
    public void ReConnectByHeartBeat(SceneEvents.ServerConnectState state)
    {
        if(state==SceneEvents.ServerConnectState.reConnect)
        {
            if (!IsInvoking("ReConnect")) InvokeRepeating("ReConnect", 0, 1);
        }
    }
    /// <summary>
    /// 重连
    /// </summary>
    private void ReConnect()
    {
        if(signalRConnection.State==ConnectionStates.Closed)
        {
            signalRConnection.Open();
            Debug.Log("Reconnect Singnal...");
        }
        else if(signalRConnection.State==ConnectionStates.Connected)
        {
            if(IsInvoking("ReConnect"))
            {
                CancelInvoke("ReConnect");
                Debug.Log("Reconnect Singnal success...");
            }
        }
    }

    /// <summary>
    /// 初始化参数
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void InitIpArgs(string ip,string port)
    {
        Ip = ip;
        Port = port;
    }
    /// <summary>
    /// 注册类型转换器
    /// </summary>
    private void RegisterImporter()
    {
        JsonMapper.RegisterImporter<int, long>((int value) =>
        {
            return (long)value;
        });
        JsonMapper.RegisterImporter<double, float>((double value) =>
        {
            return (float)value;
        });
    }

    /// <summary>
    /// Display state changes
    /// </summary>
    void signalRConnection_OnStateChanged(Connection connection, ConnectionStates oldState, ConnectionStates newState)
    {
        Debug.Log(string.Format("[State Change] {0} => {1}", oldState, newState));
    }

    /// <summary>
    /// Display errors.
    /// </summary>
    void signalRConnection_OnError(Connection connection, string error)
    {
        Debug.Log("[Error] " + error);
    }
    void signaleRConnection_OnNoHubMsg(Connection connection, object data)
    {
        string arg0 = JsonMapper.ToJson(data);
        Debug.Log("NoHub Msg:"+arg0);
    }
    void OnDestroy()
    {
        // Close the connection when we are closing this sample
        if (signalRConnection != null)
        {
            signalRConnection.Close();
        }
    }
   
}
