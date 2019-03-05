using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamRaycastCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ShowDevInfo();
    }
    /// <summary>
    /// 清除信息，退出射线检测模式
    /// </summary>
    public void Clear()
    {
        if (lastDev != null)
        {
            lastDev.SetMouseState(false);
            lastDev = null;
        }
    }
    private FacilityDevController lastDev;//上一个设备

    private float maxDis = 50f;
    /// <summary>
    /// 显示设备信息
    /// </summary>
    private void ShowDevInfo()
    {
        Vector2 screenMiddlePos = new Vector2(Screen.width/2,Screen.height/2);
        Ray ray = Camera.main.ScreenPointToRay(screenMiddlePos);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo, maxDis))
        {           
            FacilityDevController dev = hitInfo.transform.GetComponentInParent<FacilityDevController>();
            if(dev!=null)
            {
                if(dev!=lastDev)
                {
                    if(lastDev!=null)lastDev.SetMouseState(false);
                    dev.SetMouseState(true);
                    lastDev = dev;
                }
            }else if(lastDev!=null)
            {
                lastDev.SetMouseState(false);
                lastDev = null;
            }
        }
    }
}
