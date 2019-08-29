using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoftwareConfigXML : MonoBehaviour {
    public Text VersionText;
	// Use this for initialization
	void Start () {
        InitConfigUI();

    }

    /// <summary>
    /// 获取XML信息初始化的信息
    /// </summary>
    public void InitConfigUI()
    {
        try
        {
            string num= SystemSettingHelper.systemSetting.VersionSetting.VersionNumber;
            if(!num.ToLower().Contains("v"))
            {
                num = "V" + num;
            }
            VersionText.text = num;
        }catch(Exception e)
        {
            VersionText.text = "1.0.0";
            Debug.LogError("Error:SoftwareConfigXML->"+e.ToString());
        }        
    }
}
