using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDevController : DevNode {

    /// <summary>
    /// 摄像头信息
    /// </summary>
    private Dev_CameraInfo CameraInfo;

	/// <summary>
    /// 显示摄像头数据
    /// </summary>
    /// <param name="dev"></param>
    public void ShowCameraInfo(DevNode dev)
    {
        GetCameraInfo(dev.Info);
        if (CameraInfo!=null)
        {
            Debug.LogError(string.Format("OpenVideoStream-> IP:{0} UserName:{1} PassWord:{2} Port:{3} Index:{4}",CameraInfo.Ip,CameraInfo.PassWord,
                CameraInfo.Port,CameraInfo.CameraIndex));   
        }
    }
    /// <summary>
    /// 设置摄像头信息
    /// </summary>
    /// <param name="info"></param>
    public void SetCameraInfo(Dev_CameraInfo info)
    {
        CameraInfo = info;
    }
    /// <summary>
    /// 根据设备信息，获取摄像头信息
    /// </summary>
    /// <param name="dev"></param>
    public Dev_CameraInfo GetCameraInfo(DevInfo dev)
    {
        if (CameraInfo != null) return CameraInfo;
        if(dev!=null)
        {
            CommunicationObject service = CommunicationObject.Instance;
            if(service)
            {
                CameraInfo = service.GetCameraInfoByDevInfo(dev);
                if(CameraInfo==null)
                {
                    Debug.LogError(string.Format("CamerInfo not find-> DevName:{0} DevId:{1}",dev.Name,dev.Id));
                }
            }
        }
        return CameraInfo;
    }
}
