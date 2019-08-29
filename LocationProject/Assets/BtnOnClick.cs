using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;
using System;
using Newtonsoft.Json;

public class BtnOnClick : MonoBehaviour {
    WebApiClient web;
	// Use this for initialization
	void Start () {
        web = gameObject.AddComponent<WebApiClient>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public InputField name;
    public Text txtName;
    public Text dataPerson;

    public void BtnSelect()
    {
        Debug.Log("获取列表");
        string personname = name.text;
        web.getListPersonls(personname, callback, OnDisConnect);
    }



  
    /// <summary>
    /// 返回参数 
    /// </summary>
    /// <param name="obj"></param>
    private void callback(Personnel[] obj)
    {
        string aa = "";
        for (int i = 0; i < obj.Length; i++)
        {
           aa+= "{id:" + obj[i].Id.ToString() + ",name:" + obj[i].Name.ToString()+"},";
        } 
        dataPerson.text = aa;
    }

    /// <summary>
    /// 连接失败
    /// </summary>
    /// <param name="errorInfo"></param>
    private void OnDisConnect(string errorInfo)
    {
        Debug.Log(errorInfo.ToString());
    }






    
}
