using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAlarmInfoItem : MonoBehaviour
{

    public Sprite Fire;//宝兴摄像头火的标志
    public Sprite Safety;//宝兴摄像头安全帽的标志
    public Sprite smogInfo;//烟雾标志
    public Text message;
    public Image AlarmTypeImage;
    public Image Mask;
    public Image MaskSecond;
    public GameObject AlarmObj;
    public Text cid;
    AlarmPushInfo CurrentAlarmPushInfo;
    void Start()
    {

    }
    private DevInfo GetDevInfoByid(int id)
    {
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            return service.GetDevByid(id);
        }
        return null;
    }
    public void GetCameraAlarmData(AlarmPushInfo cameraAlarm)
    {
        if (string.IsNullOrEmpty(cameraAlarm.CameraAlarmInfor.cid_url))
        {
            CameraAlarmInfo aa = cameraAlarm.CameraAlarmInfor;
        }

        string devName = null;
        CurrentAlarmPushInfo = new global::AlarmPushInfo();
        CurrentAlarmPushInfo = cameraAlarm;
        if (cameraAlarm.CameraAlarmInfor.AlarmType == 1)
        {
            AlarmTypeImage.sprite = Safety;
            message.text = "没戴安全帽！";
        }
        else if (cameraAlarm.CameraAlarmInfor.AlarmType == 2)
        {
            AlarmTypeImage.sprite = Fire;
            message.text = "火警";

        }
        else if (cameraAlarm.CameraAlarmInfor.AlarmType == 3)
        {
            AlarmTypeImage.sprite = smogInfo;
            message.text = "烟雾";
        }
        cid.text = cameraAlarm.CameraAlarmInfor.cid.ToString();
        AlarmObj.transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (ScreenFlashesRedAndAudio.Instance != null)
            {
               
                ScreenFlashesRedAndAudio.Instance.FlashesRedTweenerStop_Click ();
            }
            int? DevID = CommunicationObject.Instance.GetCameraDevIdByURL(cameraAlarm.CameraAlarmInfor.cid_url);
            if (DevID == null)
            {
                UGUIMessageBox.Show("视频ID未找到，URL:" + cameraAlarm.CameraAlarmInfor.cid_url);
                Log.Error("Dev.ParentId is null...");
                return;
            }
            
            DevInfo Dev = GetDevInfoByid((int)DevID);
            if (Dev == null)
            {
                Debug.LogError("Dev is null...");
                return;
            }
            LocationDev(DevID.ToString(), (int)Dev.ParentId, devName);

        });
    }


    /// <summary>
    /// 通过rtspUrl获取设备ID
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private int? GetCameraInfoId(string url)
    {
        //"rtsp://admin:admin@ 192.168.1.27:554/ch1/main/h264",
        if (string.IsNullOrEmpty(url)) return null;
        string[] ips = url.Split('@');
        if (ips == null || ips.Length < 2) return null;
        string[] ipTemp = ips[1].Split(':');
        if (ipTemp == null || ipTemp.Length < 2) return null;
        string ipFinal;
        ipFinal = ipTemp[0];
        if (string.IsNullOrEmpty(ipFinal)) return null;
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            Dev_CameraInfo info = service.GetCameraInfoByIp(ipFinal);
            if (info != null)
            {
                return info.DevInfoId;
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    public void LocationDev(string devId, int DepID, string devName)
    {
      
        if (AlarmPushManage.Instance.ClickAlarmList == null)
        {
            AlarmPushManage.Instance.ClickAlarmList = new List<string>();
        }
        AlarmPushManage.Instance.ClickAlarmList.Add(cid.text );
        Debug.LogError(AlarmPushManage.Instance.ClickAlarmList.Count + " AlarmPushManage.Instance.ClickAlarmList");
        //AlarmPushManage.Instance.BaoXinDelete_ClickAlarm();
        RoomFactory.Instance.FocusDev(devId, DepID, result =>
        {
            if (!result)
            {
                string msgTitle = "找不到对应区域和设备!";
                if (!string.IsNullOrEmpty(devName)) msgTitle = string.Format("{0} : {1}", devName, msgTitle);
                UGUIMessageBox.Show(msgTitle);
            }
        });
    }
    public void MoveTween()
    {

        AlarmObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, -104, 0), 0.1f).OnComplete(() =>
        {

            Mask.GetComponent<RectTransform>().DOLocalMoveX(-300, 0.01f);
            MaskSecond.GetComponent<RectTransform>().DOLocalMoveX(-300, 0.1f).OnComplete(() =>
            {
                AlarmPushManage.Instance.SetCameraAlarmNumber(CurrentAlarmPushInfo);
            });
        });
    }
    void Update()
    {

    }
}
