using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;

public class CameraNormalFollowUI : MonoBehaviour
{

    /// <summary>
    /// 视频监控按钮
    /// </summary>
    public Button VideoMonitorButton;
    ///// <summary>
    ///// 打开界面按钮
    ///// </summary>
    //public Toggle BgToggle;

    /// <summary>
    /// 弹窗
    /// </summary>
    public GameObject InfoBg;
    /// <summary>
    /// 标题文本
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 信息文本
    /// </summary>
    public Text InfoText;
    /// <summary>
    /// 摄像头信息
    /// </summary>
    private DevNode CameraDev;
    void Start()
    {
        VideoMonitorButton.onClick.AddListener(ShowMonitor);

    }

    public void ShowNormalCameraFollowUI()
    {
        ShowInfo();
        CameraDev.HighlightOn();
        InfoBg.SetActive(true);
    }
    public void CloseCurrentWindow()
    {
        InfoBg.SetActive(false);
    }
    /// <summary>
    /// 显示摄像头信息
    /// </summary>
    private void ShowInfo()
    {
        if (CameraDev == null)
        {
            UGUIMessageBox.Show("Camera.Devinfo is null...");
            return;
        }
        DevInfo devInfo = CameraDev.Info;
        TitleText.text = devInfo.Name;
        string info = "";
        if (devInfo.ParentId != null)
        {
            DepNode node = RoomFactory.Instance.GetDepNodeById((int)devInfo.ParentId);
            if (node != null) info = node.NodeName + "/";
        }
        CameraDevController cameraInfo = CameraDev as CameraDevController;
        Dev_CameraInfo camInfo = cameraInfo.GetCameraInfo(CameraDev.Info);
        if (camInfo != null) info += camInfo.Ip;
        InfoText.text = info;
    }
    /// <summary>
    /// 显示监控信息
    /// </summary>
    private void ShowMonitor()
    {
        //RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin
        if (CameraDev != null && CameraDev is CameraDevController)
        {
            if (CameraVideoManage.Instance)
            {
                if(DevSubsystemManage.IsRoamState)
                {
                    Transform parent = RoamFollowMange.Instance==null?transform: RoamFollowMange.Instance.gameObject.transform;
                    CameraVideoManage.Instance.SetNewParent(parent);
                }
                CameraVideoManage.Instance.Show(CameraDev as CameraDevController);
            }
        }
        else
        {
            //Todo:提示错误信息
            Debug.LogError("VideoMonitor devInfo is null...");
        }
    }
    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void SetInfo(DevNode devNode)
    {
        CameraDev = devNode;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
