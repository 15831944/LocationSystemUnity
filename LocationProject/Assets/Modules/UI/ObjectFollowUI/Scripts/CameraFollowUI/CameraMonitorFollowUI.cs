using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;
public class CameraMonitorFollowUI : MonoBehaviour
{
    /// <summary>
    /// 当前展开弹窗的UI
    /// </summary>
    public static CameraMonitorFollowUI CurrentMonitor;

    /// <summary>
    /// 打开界面按钮
    /// </summary>
    public Toggle BgToggle;
    /// <summary>
    /// 摄像头信息
    /// </summary>
  //  private DevNode CameraDev;
    public CameraNormalFollowUI NormalFollowUI;
    public CameraAlarmFollowUI AlarmFollowUI;
    public VideoFollowItem videoFollowUI;//普通漂浮视频
    // Use this for initialization
    void Start ()
	{
	    //VideoMonitorButton.onClick.AddListener(ShowMonitor);
        BgToggle.onValueChanged.AddListener(ShowBg);
	}
    /// <summary>
    /// 显示/关闭 窗体
    /// </summary>
    private void ShowBg(bool isOn)
    {
        SetLayerUp(isOn);
        if (isOn) Show();
        else Hide();
    }
    /// <summary>
    /// 显示背景
    /// </summary>
    public void Show()
    {
        if (FollowTargetManage.Instance) FollowTargetManage.Instance.SaveDepOpenCameraUI(this);
        if (RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin)
        {
            AlarmFollowUI.StaetOpenWindowShowInfo(this.gameObject  );
        }
        else if(videoFollowUI!=null)
        {
            if (CurrentMonitor != null && CurrentMonitor != this) CurrentMonitor.BgToggle.isOn = false;
            CurrentMonitor = this;
            videoFollowUI.Show();
        }
        else
        {
            if (CurrentMonitor != null && CurrentMonitor != this) CurrentMonitor.BgToggle.isOn = false;
            CurrentMonitor = this;
            NormalFollowUI.ShowNormalCameraFollowUI();
        }
       
        //ShowInfo();
        //CameraDev.HighlightOn();
        //InfoBg.SetActive(true);
    }
    /// <summary>
    /// 关闭背景
    /// </summary>
    public void Hide()
    {
        if (FollowTargetManage.Instance) FollowTargetManage.Instance.RemoveOpenCameraUI(this);
        if (RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin)
        {
            AlarmFollowUI.ShowFollowUI(false );
            AlarmFollowUI. RecoverFollowParentPos();
        }
        else if(videoFollowUI!=null)
        {
            CurrentMonitor = null;
            videoFollowUI.Close();
        }
        else
        {
            CurrentMonitor = null;        
            NormalFollowUI.CloseCurrentWindow();
        }
     
    }
    /// <summary>
    /// 设置界面是否显示在最上层
    /// </summary>
    /// <param name="isUp"></param>
    private void SetLayerUp(bool isUp)
    {
        UGUIFollowTarget followTarget = transform.GetComponent<UGUIFollowTarget>();
        if (followTarget) followTarget.SetIsUp(isUp);
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void SetInfo(DevNode devNode)
    {
        if (RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin)
        {
            AlarmFollowUI .SetInfo(devNode, BgToggle); 
        }
        else if(videoFollowUI!=null)
        {
            videoFollowUI.SetInfo(devNode,BgToggle);
        }
        else
        {
            NormalFollowUI.SetInfo(devNode);
        }
        //CameraDev = devNode;        
    }
    ///// <summary>
    ///// 显示摄像头信息
    ///// </summary>
    //private void ShowInfo()
    //{
    //    if(CameraDev==null)
    //    {
    //        UGUIMessageBox.Show("Camera.Devinfo is null...");
    //        return;
    //    }
    //    DevInfo devInfo = CameraDev.Info;
    //    TitleText.text = devInfo.Name;
    //    string info = "";
    //    if (devInfo.ParentId != null)
    //    {
    //        DepNode node = RoomFactory.Instance.GetDepNodeById((int)devInfo.ParentId);
    //        if (node != null) info = node.NodeName + "/";
    //    }
    //    CameraDevController cameraInfo = CameraDev as CameraDevController;
    //    Dev_CameraInfo camInfo = cameraInfo.GetCameraInfo(CameraDev.Info);
    //    if(cameraInfo!=null)info += camInfo.Ip;
    //    InfoText.text = info;
    //}
    ///// <summary>
    ///// 显示监控信息
    ///// </summary>
    //private void ShowMonitor()
    //{
    //    //RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin
    //    if (CameraDev != null&&CameraDev is CameraDevController)
    //    {
    //        if (RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin)
    //        {
    //            if (CameraVideoManage.Instance) CameraAlarmManage.Instance.ShowCurrentCameraDev(CameraDev as CameraDevController); 
    //        }
    //        else
    //        {
    //            if (CameraVideoManage.Instance) CameraVideoManage.Instance.Show(CameraDev as CameraDevController);
    //        }
            
    //    }
    //    else
    //    {
    //        //Todo:提示错误信息
    //        Debug.LogError("VideoMonitor devInfo is null...");
    //    }
    //}
}
