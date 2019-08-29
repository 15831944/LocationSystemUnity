using System;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;

public class CameraVideoManage : MonoBehaviour
{
    public static CameraVideoManage Instance;
    /// <summary>
    /// 霍尼韦尔视频
    /// </summary>
    public HoneyWellVideoPart honeyWellVideo;
    /// <summary>
    /// Rtsp取视频
    /// </summary>
    public RtspVideo rtspVideo;

    private Transform NormalParent;//非漫游状态的父物体
    // Use this for initialization
    void Start()
    {
        Instance = this;
        NormalParent = transform.parent;
    }
    /// <summary>
    /// 恢复父物体
    /// </summary>
    public void RecoverParent()
    {
        if (NormalParent == null) return;
        gameObject.transform.parent = NormalParent;
    }
    /// <summary>
    /// 设置新的父物体
    /// </summary>
    /// <param name="newParent"></param>
    public void SetNewParent(Transform newParent)
    {
        gameObject.transform.parent = newParent;
    }
    /// <summary>
    /// 关闭视频
    /// </summary>
    public void Close()
    {
        if (!SystemSettingHelper.honeyWellSetting.EnableHoneyWell)
        {
            rtspVideo.Close();
        }
    }
    public void Show(CameraDevController devController)
    {
        if (devController == null || devController.Info == null) return;
        if (SystemSettingHelper.honeyWellSetting.EnableHoneyWell)
        {
            if (honeyWellVideo) honeyWellVideo.ShowVideo(devController.Info.Abutment_DevID);
        }
        else
        {
            Dev_CameraInfo camInfo = devController.GetCameraInfo(devController.Info);
            if(camInfo!=null)rtspVideo.ShowVideo(camInfo.RtspUrl,devController.Info);
        }
    }
    /// <summary>
    /// 显示Rtsp视频
    /// </summary>
    /// <param name="rtspUrl"></param>
    public void ShowRtspVideo(string rtspUrl)
    {
        rtspVideo.ShowVideo(rtspUrl, null);
    }

    private DateTime recordTime;
    /// <summary>
    /// 显示摄像头视频
    /// </summary>
    /// <param name="cameraInfo"></param>
    public void Show(DevInfo cameraInfo)
    {
        try
        {
            if (SystemSettingHelper.honeyWellSetting.EnableHoneyWell)
            {
                if (honeyWellVideo) honeyWellVideo.ShowVideo(cameraInfo.Abutment_DevID);
            }
            else
            {
                if (rtspVideo)
                {
                    recordTime = DateTime.Now;
                    Dev_CameraInfo info = CommunicationObject.Instance.GetCameraInfoByDevInfo(cameraInfo);
                    Debug.LogErrorFormat("GetCameraByDevInfo,Cost Time:{0}ms",(DateTime.Now-recordTime).TotalMilliseconds);
                    recordTime = DateTime.Now;
                    if (info != null)
                    {
                        rtspVideo.ShowVideo(info.RtspUrl,cameraInfo);
                        Debug.LogErrorFormat("rtspVideo.ShowVideo,Cost Time:{0}ms", (DateTime.Now - recordTime).TotalMilliseconds);
                    }
                    else
                    {
                        Debug.LogError("Error:CameraVideoManage.Show->Dev_Camerainfo is null,ID:" + cameraInfo == null ? "null" : cameraInfo.Id.ToString());
                    }
                }
            }
        }catch(Exception e)
        {
            Debug.LogError("Error:CameraVideoManage.Show->"+e.ToString());
        }       
    }
}
