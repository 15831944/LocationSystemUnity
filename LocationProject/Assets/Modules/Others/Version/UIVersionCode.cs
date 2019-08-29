using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIVersionCode : MonoBehaviour {

    public Text versionUIText;
	// Use this for initialization
	void Start () {
        ShowVersionCode();
    }

    private void ShowVersionCode()
    {
        if(versionUIText==null)
        {
            Debug.LogError("Error:UIVersionCode.ShowVersionCode.versionUIText==null...");
            return;
        }
        if(SystemSettingHelper.instance)
        {
            versionUIText.text = SystemSettingHelper.instance.versionNum;
        }
        else
        {
            versionUIText.text = SystemSettingHelper.versionSetting.VersionNumber;
        }
    }
	
    
}
