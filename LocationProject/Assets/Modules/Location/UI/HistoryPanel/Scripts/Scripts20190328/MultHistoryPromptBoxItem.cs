using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultHistoryPromptBoxItem : MonoBehaviour {

    //public Text timeText;
    public Image icon;//图标
    public Text personName;//名称
    public Text areaName;//区域名称

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="timeStr"></param>
    /// <param name="color"></param>
    /// <param name="namet"></param>
    /// <param name="areaNamet"></param>
    public void Init(string timeStr,Color color, string namet, string areaNamet)
    {
        //SetTimeText(timeStr);
        SetIconColor(color);
        SetPersonName(namet);
        SetAreaName(areaNamet);
    }

    ///// <summary>
    ///// 设置时间
    ///// </summary>
    //public void SetTimeText(string timeStr)
    //{
    //    timeText.text = timeStr;
    //}

    /// <summary>
    /// 设置图标颜色
    /// </summary>
    public void SetIconColor(Color color)
    {
        icon.color = color;
    }

    /// <summary>
    /// 设置人员名称
    /// </summary>
    public void SetPersonName(string namet)
    {
        personName.text = namet;
    }

    /// <summary>
    /// 设置区域名称
    /// </summary>
    /// <param name="areaNamet"></param>
    public void SetAreaName(string areaNamet)
    {
        areaName.text = areaNamet;
    }
}
