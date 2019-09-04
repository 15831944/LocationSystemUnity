using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAlarmFollowUILocation : MonoBehaviour {

    public static CameraAlarmFollowUILocation Instance;
    public GameObject CameraAlarmFollowUILocationWindow;
    private void Awake()
    {
        Instance = this;
    } 
    void Start () {
		
	}
	
	public GameObject GetCameraFollowUIWindow()
    {
        if(DevSubsystemManage.IsRoamState&&RoamFollowMange.Instance!=null&&RoamFollowMange.Instance.FixedFollowContainer!=null)
        {
            return RoamFollowMange.Instance.FixedFollowContainer;
        }
        else
        {
            return CameraAlarmFollowUILocationWindow;
        }
    }
}
