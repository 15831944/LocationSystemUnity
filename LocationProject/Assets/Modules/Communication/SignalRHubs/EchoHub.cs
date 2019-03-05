using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoHub : Hub
{

    public EchoHub()
        : base("echoHub")
    {
        // Setup server-called functions     
        base.On("Message", Message);
    }
    private void Message(Hub hub, MethodCallMessage methodCall)
    {
        string arg0 = methodCall.Arguments[0].ToString();
        //ItemValueModel item
        Debug.Log("EchoMessage:" + arg0);
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg"></param>
    public void Send(string msg)
    {
        base.Call("Broadcast", msg);
    }
}
