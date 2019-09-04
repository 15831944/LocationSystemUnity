using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamFollowMange : MonoBehaviour {
    public static RoamFollowMange Instance;
    /// <summary>
    /// 预设存放处
    /// </summary>
    public GameObject CameraUIContainer;
    /// <summary>
    /// 摄像头漂浮UI 预设
    /// </summary>
    public GameObject CameraUIPrefab;
    /// <summary>
    /// 漫游漂浮UI
    /// </summary>
    private List<CameraMonitorFollowUI> RoamCameraList = new List<CameraMonitorFollowUI>();
    [HideInInspector]
    public bool isCameraUIShow;
    private float MaxUIDis = 50;
    public Camera RoamCamera;

    public GameObject FixedFollowContainer;
    
    // Use this for initialization
    void Start () {
        Instance = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 显示漫游摄像头UI
    /// </summary>
    public void ShowRoamCameraUI()
    {
        isCameraUIShow = true;
        CameraUIContainer.SetActive(true);
    }
    /// <summary>
    /// 关闭摄像头UI
    /// </summary>
    public void CloseCameraUI()
    {
        CameraUIContainer.SetActive(false);
        isCameraUIShow = false;
    }
    /// <summary>
    /// 开启摄像头UI
    /// </summary>
    /// <param name="cameraDev"></param>
    /// <param name="devDep"></param>
    /// <param name="info"></param>
    public void CreateCameraUI(GameObject cameraDev, DepNode devDep, DevNode info)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(cameraDev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return;
        }
        if (RoamCamera == null) return;
        GameObject ui = Instantiate(CameraUIPrefab);
        ui.transform.parent = CameraUIContainer.transform;
        ui.transform.localScale = Vector3.one;
        ui.transform.localEulerAngles = Vector3.zero;
        ui.SetActive(true);
        UGUIFollowTarget followTarget = UGUIFollowTarget.AddUGUIFollowTarget(ui, targetTagObj, RoamCamera, true, -1);
        followTarget.SetIsRayCheckCollision(true);
        followTarget.SetEnableDistace(true, MaxUIDis);
        CameraMonitorFollowUI cameraInfo = ui.GetComponent<CameraMonitorFollowUI>();
        if (cameraInfo != null)
        {
            if (!RoamCameraList.Contains(cameraInfo)) RoamCameraList.Add(cameraInfo);
            cameraInfo.SetInfo(info);
            //cameraInfo.Show();
        }
    }
   
}
