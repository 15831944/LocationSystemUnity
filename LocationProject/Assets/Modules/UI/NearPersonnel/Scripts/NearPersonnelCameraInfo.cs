using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NearPersonnelCameraInfo : MonoBehaviour {

    public Text cameraName;
    public Text cameraId;
    public Text Distance;
    public Toggle camTog;
    int num;
    int m;
    public NearPerCameraRotation nearPerCameraRotation;
    void Start () {
        camTog.onValueChanged.AddListener(Click_Toggle);

    }
    public void showNearPersonnelCamInfo(NearbyDev devList ,int total,int i)
    {
        num = total;
        m = i + 1;
        if (devList.TypeName==null)
        {
            cameraName.text = "";
        }
       else
        {
            cameraName.text = devList.TypeName.ToString();
        }
           
            cameraId.text  = devList.id.ToString();

        Distance.text = "距离人员" + string.Format("{0:F}", devList.Distance);
        
    }

    public void Click_Toggle(bool ison)
    {
        if (ison)
        {
            nearPerCameraRotation.CameraPiontClick();
            nearPerCameraRotation.CaneraPoint.isOn = true;
            ChangeScrollbarValue();
        }
        else
        {
            nearPerCameraRotation.CameraPointExit();
            nearPerCameraRotation.CaneraPoint.isOn = false;
        }
    }
    /// <summary>
    /// 改便滑动条的数值
    /// </summary>
    public void ChangeScrollbarValue()
    {
        float n = (float )m/num;
           
        NearPersonnelCameraManage.Instance.vertical.value = 1-n;

    }
    void Update () {
		
	}
}
