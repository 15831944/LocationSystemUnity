using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btnAdd : MonoBehaviour {
    WebApiClient web;
    // Use this for initialization
    void Start () {
        web = gameObject.AddComponent<WebApiClient>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Text txtName;
    public Text dataPerson;


    /// <summary>
    /// 添加
    /// </summary>
    public void BtnAdd()
    {
        Debug.Log("添加");
        Personnel per = new Personnel();
        per.Name = "cs张三";
        per.Sex = "25";
        DateTime time = DateTime.Now;
        per.BirthDay = time;
        per.BirthTimeStamp =long.Parse(DateTimeToStamp(time));
        per.Enabled = true;
        web.AddPersonnel(per, callback, OnDisConnect);
    }



    private void callback(Personnel per)
    {
        Debug.Log("添加成功");
        dataPerson.text = "id:" + per.Id.ToString() + ",name:" + per.Name.ToString();
    }

    /// <summary>
    /// 连接失败
    /// </summary>
    /// <param name="errorInfo"></param>
    private void OnDisConnect(string errorInfo)
    {
        Debug.Log(errorInfo.ToString());
    }


    // 时间转时间戳
    public string DateTimeToStamp(DateTime now)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
        long timeStamp = (long)(now - startTime).TotalMilliseconds; // 相差毫秒数
        Debug.Log("\n 当前 时间戳为：" + timeStamp);
        return timeStamp.ToString();
    }

}
