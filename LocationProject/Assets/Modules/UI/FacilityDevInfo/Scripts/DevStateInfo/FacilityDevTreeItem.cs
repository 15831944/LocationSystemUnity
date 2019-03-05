using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
public class FacilityDevTreeItem : MonoBehaviour {

    /// <summary>
    /// 状态
    /// </summary>
    public Text StatusText;
    /// <summary>
    /// 值
    /// </summary>
    public Text ValueText;
	// Use this for initialization
	void Start () {

    }
    public void SetValue(FacilitySystem SystemInfo)
    {
        if(string.IsNullOrEmpty(SystemInfo.Value))
        {
            ValueText.text = "/";
        }
        else
        {
            ValueText.text = SystemInfo.Value;
        }
        StatusText.text = SystemInfo.Status;
    }

    public void Init(object treeNodeTag,Text describe)
    {
        //Todo:确定数据中是否有告警状态
        if(treeNodeTag is Dev_Monitor)
        {
            Dev_Monitor dev = treeNodeTag as Dev_Monitor;
            ValueText.text = "/";
            StatusText.text = "";
        }else if(treeNodeTag is DevMonitorNode)
        {
            describe.text = string.Format("<color=#6DECFEFF>{0}</color>", describe.text);
            DevMonitorNode node = treeNodeTag as DevMonitorNode;
            if (string.IsNullOrEmpty(node.Value))
            {
                ValueText.text = "<color=#6DECFEFF>/</color>";
            }
            else
            {
                ValueText.text = string.Format("<color=#6DECFEFF>{0}{1}</color>",node.Value,node.Unit);
            }
            StatusText.text ="";
        }
        else
        {
            ValueText.text = "";
            StatusText.text = "";
        }
    }
}
