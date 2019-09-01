using Base.Common.Extensions;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMP;
public class CameraAlarmFollowUI : MonoBehaviour
{

    public Toggle VideoTog;
    public Toggle AlarmTog;
    public Toggle PictureTog;
    public GameObject FollowUI;
    public GameObject CameraVedioWindow;
    public GameObject CameraAlarmWindow;
    public Text TitleText;
    public string CurrentCameraDevID;
    List<CameraAlarmInfo> CurrentCameraAlarmList;
    List<CameraAlarmInfo> RefreshCameraAlarmListInfo;
    CameraDevController CurrentCameraDev;
    public GameObject LineExample;
    public GameObject grid;
    public Button MaxBut;
    DevNode CameraDev;
    UniversalMediaPlayer player;
    public GameObject connectBg;//连接时提示的背景
    bool isRefresh = false;
    public GameObject SmallPictureWindow;
    int width = 400;
    int height = 225;
    public Text PictureInfo;
    public Image Picture;//告警图片
    public GameObject NewstAlarmTag;//有新的告警过来，图片按钮的小红点提示
    List<CameraAlarmInfo> CurrentCameraAlarmInfo;
    public GameObject VedioPrefabs;
    public GameObject PlayParent;
    public Toggle FixedTog;
    public GameObject FollowParentObj;
    public Vector3 FollowParentPos;
    public Text VedioPrompt;
    AlarmSearchArg CameraAlarmSearchArg;
    List<CameraAlarmInfo> CameraHistoryAlarm;
    int ReshTime = 20;
    void Start()
    {
        VideoTog.onValueChanged.AddListener(ShowCameraVedioWindow);
        AlarmTog.onValueChanged.AddListener(ShowCameraAlarmWindow);
        MaxBut.onClick.AddListener(Max_ButClick);
        //  AddEncounteredEvent();
        PictureTog.onValueChanged.AddListener(ShowSmallPictureWindow);
        FixedTog.onValueChanged.AddListener(FixedTog_Click);

        uis.Add(this);
    }

    public static List<CameraAlarmFollowUI> uis = new List<CameraAlarmFollowUI>();

    private void OnDestroy()
    {
        uis.Remove(this);
    }

    public static void RefreshAll()
    {
        foreach (var item in uis)
        {
            if (item == null) continue;
            item.RefreshCameraAlarmInfo();
        }
    }

    /// <summary>
    /// 打开当前设备的监控视频
    /// </summary>
    /// <param name="cameraDev"></param>
    public void ShowCurrentCameraVedio(CameraDevController cameraDev)
    {
        if (CameraVideoManage.Instance)
        {
            Dev_CameraInfo camInfo = cameraDev.GetCameraInfo(cameraDev.Info);
            //   if (camInfo != null) CameraVideoManage.Instance.rtspVideo.ShowVideo(camInfo.RtspUrl, cameraDev.Info);
            if (IsMinPrompt)
            {
                if (string.IsNullOrEmpty(camInfo.RtspUrl))
                {
                    Debug.LogError("Error : RtspVideo.ShowVideo-> rtspURL is null!");
                    UGUIMessageBox.Show("Rtsp地址未输入!");
                    if (VedioPrompt)
                        VedioPrompt.text = "Rtsp地址未输入!";
                    if(CameraAlarmManage.Instance.VedioPrompt)
                        CameraAlarmManage.Instance.VedioPrompt.text = "Rtsp地址未输入!";
                    IsMinPrompt = true;
                    return;
                }
                if (!IsEffectiveURL(camInfo.RtspUrl))
                {
                    UGUIMessageBox.Show("Rtsp地址无效!");
                    if(VedioPrompt)
                        VedioPrompt.text = "Rtsp地址无效!";
                    IsMinPrompt = true;
                    return;
                }
            }


            GameObject Obj = Instantiate(VedioPrefabs, PlayParent.transform) as GameObject;
            Obj.transform.SetAsFirstSibling();
            Obj.SetActive(true);
            VedioPrefabs.SetActive(false);
            player = Obj.GetComponent<UniversalMediaPlayer>();
            AddEncounteredEvent(player, () =>
             {
                 ShowVideo(camInfo.RtspUrl, player);
             });
        }
    }

    private bool IsEffectiveURL(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            Debug.Log(url);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    //public void RefreshCameraAlarmList()
    //{
    //    if (isRefresh) return;
    //    isRefresh = true;
    //    Debug.LogError("min刷新摄像机告警信息");
    //    CurrentCameraAlarmList = new List<CameraAlarmInfo>();
    //    if (AlarmPushManage.Instance.AllCameraAlarmPush!=null)
    //    {
    //        CurrentCameraAlarmList.AddRange(AlarmPushManage.Instance.AllCameraAlarmPush);
    //    }

    //    DelectChildItem();
    //    ShowCurrentCameraAlarm();
    //}
    /// <summary>
    /// 在打开摄像头漂浮UI的情况下，没有打开摄像头图片界面，刷新看有没有新告警产生
    /// </summary>
    public void RefreshCameraAlarmInfo()
    {
        Debug.LogError("刷新摄像机告警信息Picture CurrentCameraDevID:" + CurrentCameraDevID);
        try
        {
            var refreshCameraAlarmListInfo = new List<CameraAlarmInfo>();
            //  RefreshCameraAlarmListInfo.AddRange(AlarmPushManage.Instance.CameraAlarmPushList);
            var alarmPushList = AlarmPushManage.Instance.AllCameraAlarmPush;
            if (alarmPushList != null)
            {
                
                //  RefreshCameraAlarmListInfo.AddRange(AlarmPushManage.Instance.NewestCameraAlarmPush);
                for (int i = 0; i < alarmPushList.Count; i++)
                {
                    var alarm = alarmPushList[i];
                    int DevID = alarm.DevID;
                    if (DevID == 0)
                    {
                        int? DevID2 = CommunicationObject.Instance.GetCameraDevIdByURL(alarm.cid_url);
                        Log.Info("RefreshCameraAlarmInfo 1", string.Format("url:{0},dev:{1}", alarm.cid_url, DevID2));
                        if (DevID2 != null)
                        {
                            alarm.DevID = (int)DevID2;
                            DevID = alarm.DevID;
                        }
                    }
                    else
                    {
                        Log.Info("RefreshCameraAlarmInfo 2", "DevID:" + DevID);
                    }
                    
                    if (CurrentCameraDevID == DevID.ToString())
                    {
                        refreshCameraAlarmListInfo.Add(alarm);
                    }
                }
            }
            else
            {
                Log.Info("RefreshCameraAlarmInfo", "AlarmPushManage.Instance.AllCameraAlarmPush==null");
            }

            if (refreshCameraAlarmListInfo.Count > 0 && PictureTog.isOn == false && !SmallPictureWindow.activeSelf && AlarmPushManage.Instance.IsNewAlarm == true)
            {
                NewstAlarmTag.SetActive(true);
            }
            else
            {
                Log.Info("RefreshCameraAlarmInfo", "ELSE1");
            }

            if (PictureTog.isOn == true && SmallPictureWindow.activeSelf)
            {
                NewstAlarmTag.SetActive(false);
                if (refreshCameraAlarmListInfo.Count != 0 && AlarmPushManage.Instance.IsNewAlarm == true)
                {
                    PictureInfo.text = CurrentCameraDev.Info.Name;
                    Texture2D texture = new Texture2D(width, height);
                    byte[] Pic = PictureData(refreshCameraAlarmListInfo[refreshCameraAlarmListInfo.Count - 1].pic_data);
                    texture.LoadImage(Pic);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    Picture.sprite = sprite;
                    AlarmPushManage.Instance.IsNewAlarm = false;
                }
                else
                {
                    Log.Info("RefreshCameraAlarmInfo", string.Format("ELSE2,{0},{1}", refreshCameraAlarmListInfo.Count, AlarmPushManage.Instance.IsNewAlarm));
                }

                Log.Info("RefreshCameraAlarmInfo", string.Format("count1:{0}", refreshCameraAlarmListInfo.Count));
                //CurrentCameraAlarmInfo.Clear();
                CurrentCameraAlarmInfo=refreshCameraAlarmListInfo;
            }
            else
            {
                Log.Info("RefreshCameraAlarmInfo", "ELSE3");
            }

            RefreshCameraAlarmListInfo = refreshCameraAlarmListInfo;
        }
        catch (Exception ex)
        {
            Log.Error("RefreshCameraAlarmInfo", "Exception :"+ex);
        }
       
    }
    /// <summary>
    /// 展示最新告警图片
    /// </summary>
    public void ShowPictureInfo()
    {
        RefreshCameraAlarmInfo();
        NewstAlarmTag.SetActive(false);
    }
    private DateTime recordTag;
    public void ShowCameraAlarmList()
    {
        if (!CameraAlarmWindow.activeInHierarchy) return;//界面被外部关闭，不刷新
        Log.Info("ShowCameraAlarmList:" + isRefresh);
        if (isRefresh) return;
        isRefresh = true;
        DelectChildItem();        
        CameraHistoryAlarm = new List<CameraAlarmInfo>();
        int id = CurrentCameraDevID.ToInt();
        DevInfo devInfo = new DevInfo();
        devInfo.Id = id;
        recordTag = DateTime.Now;
        Dev_CameraInfo info = CommunicationObject.Instance.GetCameraInfoByDevInfo(devInfo);     
        ThreadManager.Run(()=> 
        {
            CameraHistoryAlarm = CommunicationObject.Instance.GetCameraAlarms(info.Ip, true);
        },()=> 
        {
            Debug.LogError("CommunicationObject.Instance.GetCameraAlarms,cost :" + (DateTime.Now - recordTag).TotalSeconds + " s");
            if (CameraHistoryAlarm != null)
            {

                CameraHistoryAlarm.Reverse();
                ShowCurrentCameraAlarm(CameraHistoryAlarm);
            }
        }, "ShowCameraAlarmList");       
    }
    /// <summary>
    /// 打开当前摄像头告警界面
    /// </summary>
    public void ShowCurrentCameraAlarm(List<CameraAlarmInfo> AlarmList)
    {
        isRefresh = false;
        if (AlarmList.Count == 0)
        {
            Debug.LogError("AlarmList.Count==0  return");
            return;
        }

        for (int i = 0; i < AlarmList.Count; i++)
        {
            if (IsSameCamera(CurrentCameraDevID, AlarmList[i]) && grid.transform.childCount <= 5)
            {
                GameObject Obj = InstantiateLine();
                CameraAlarmFollowUIItem Item = Obj.GetComponent<CameraAlarmFollowUIItem>();
                Item.GetCameraAlarmData(AlarmList[i]);
            }
        }
    }

    public bool IsSameCamera(string devId, CameraAlarmInfo info)
    {
        if (info == null) return false;
        if (devId.ToInt() == info.DevID)
        {

            return true;
        }
        else
        {

            return false;
        }
    }
    /// <summary>
    /// 通过rtspUrl获取设备Ip
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public  string GetCameraInfoIp(string url)
    {
        if (string.IsNullOrEmpty(url)) return "";
        string[] ips = url.Split('@');
        if (ips == null || ips.Length < 2) return "";
        string[] ipTemp = ips[1].Split(':');
        if (ipTemp == null || ipTemp.Length < 2) return "";
        string ipFinal = ipTemp[0];
        if (string.IsNullOrEmpty(ipFinal))
        {
            return "";
        }
        else
        {
            return ipFinal;
        }
    }
   // /// <summary>
   // /// 通过rtspUrl获取设备ID
   // /// </summary>
   // /// <param name="url"></param>
   // /// <returns></returns>
   //public  int? GetCameraInfoId(string url)
   // {
   //     //"rtsp://admin:admin@ 192.168.1.27:554/ch1/main/h264",
   //     if (string.IsNullOrEmpty(url)) return null;
   //     string[] ips = url.Split('@');
   //     if (ips == null || ips.Length < 2) return null;
   //     string[] ipTemp = ips[1].Split(':');
   //     if (ipTemp == null || ipTemp.Length < 2) return null;
   //     string ipFinal = ipTemp[0];
   //     if (string.IsNullOrEmpty(ipFinal)) return null;
   //     CommunicationObject service = CommunicationObject.Instance;
   //     if (service)
   //     {
   //         Dev_CameraInfo info = service.GetCameraInfoByIp(ipFinal);
   //         if (info != null)
   //         {
   //             return info.DevInfoId;
   //         }
   //         else
   //         {
   //             return null;
   //         }
   //     }
   //     return null;
   // }
    public bool IsMinPrompt = true;//在摄像头链接不成功，点击放大按钮，是不会有提示框的，切换按钮下有提示框
    public void ShowCameraVedioWindow(bool b)
    {
        Debug.LogError("ShowCameraVideoWindow:"+b);
        CloseReconnectInvoke();
        currentReconnectTime = 0;
        Close();
        if (VedioPrompt)
        {
            VedioPrompt.text = "视频连接中...";
        }            
        CameraVedioWindow.SetActive(b);
        if (b)
        {
            ShowCurrentCameraVedio(CurrentCameraDev);
        }
    }
    /// <summary>
    /// 关闭视频
    /// </summary>
    public void Close()
    {

        if (player == null) return;
        player.Stop();
        DestroyImmediate(player.gameObject);

    }
    public void ShowCameraAlarmWindow(bool b)
    {
        CameraAlarmWindow.SetActive(b);
        if (b)
        {
            DelectChildItem();
            Close();
            if (!IsInvoking("ShowCameraAlarmList"))
            {
                InvokeRepeating("ShowCameraAlarmList", 0, ReshTime);

            }
        }
        else
        {
            if (IsInvoking("ShowCameraAlarmList"))
            {
                CancelInvoke("ShowCameraAlarmList");
                Debug.LogError("min关闭刷新摄像机告警信息");
            }
        }
    }
    /// <summary>
    /// 摄像头跟随界面
    /// </summary>
    /// <param name="b"></param>
    public void ShowFollowUI(bool b)
    {
        FollowUI.SetActive(b);
        if (b == false)
        {
            if (IsInvoking("RefreshCameraAlarmInfo"))
            {
                CancelInvoke("RefreshCameraAlarmInfo");
                Debug.LogError("关闭刷新摄像机告警信息picture");
            }
            if (IsInvoking("ShowCameraAlarmList"))
            {
                CancelInvoke("ShowCameraAlarmList");
                Debug.LogError("min关闭刷新摄像机告警信息");
            }
            CloseReconnectInvoke();
            Close();
        }
        else
        {
            //todo:打开界面时，如果在图片界面，刷新图片；在告警列表界面，刷新告警列表
        }

    }
    /// <summary>
    /// 打开跟随界面展示视频
    /// </summary>
    /// <param name="cameraDev"></param>
    public void StaetOpenWindowShowInfo(GameObject followParent)
    {
        ShowFollowUI(true);
        FollowParentObj = followParent;
        if (CameraDev == null)
        {
            UGUIMessageBox.Show("Camera.Devinfo is null...");
            return;
        }
        CameraDevController cameraInfo = CameraDev as CameraDevController;
        if (cameraInfo.Info == null) return;
        CurrentCameraDev = cameraInfo;
        CurrentCameraDevID = CurrentCameraDev.Info.Id.ToString();
        TitleText.text = CurrentCameraDev.Info.Name;
        GetCurrentCameraAlarmList();
    }


    /// <summary>
    /// 判断当前摄像头有没有告警，如果有告警就显示告警图片，否则显示视频
    /// </summary>
    public void GetCurrentCameraAlarmList()
    {
        try
        {
            Log.Error("CameraAlarmFollowUI.GetCurrentCameraAlarmList", "CurrentCameraDevID:"+ CurrentCameraDevID);
            var currentCameraAlarmInfo = new List<CameraAlarmInfo>();
            if (AlarmPushManage.Instance.AllCameraAlarmPush != null)
            {
                for (int i = 0; i < AlarmPushManage.Instance.AllCameraAlarmPush.Count; i++)
                {
                    var alarm = AlarmPushManage.Instance.AllCameraAlarmPush[i];
                    int? DevID = CommunicationObject.Instance.GetCameraDevIdByURL(alarm.cid_url);
                    Log.Info("CameraAlarmFollowUI.GetCurrentCameraAlarmList",string.Format("url:{0},dev:{1}",alarm.cid_url,DevID));
                    if (CurrentCameraDevID == DevID+"")
                    {
                        currentCameraAlarmInfo.Add(alarm);
                    }
                }
            }
            else
            {
                Log.Error("CameraAlarmFollowUI.GetCurrentCameraAlarmList", "AlarmPushManage.Instance.AllCameraAlarmPush == null");
            }

            if (currentCameraAlarmInfo.Count == 0)
            {
                VideoTog.isOn = true;
                //ShowCameraVedioWindow(true);
                PictureInfo.text = "";
                Picture.sprite = PictureWindow.Instance.TransperantBack;
                NewstAlarmTag.SetActive(false);
            }
            else
            {
                currentCameraAlarmInfo.Reverse();
                PictureTog.isOn = true;
                if (!SmallPictureWindow.activeSelf)
                {
                    SmallPictureWindow.SetActive(true);
                }

                PictureInfo.text = CurrentCameraDev.Info.Name + "_" + CurrentCameraDev.Info.Id;
                Texture2D texture = new Texture2D(width, height);
                int count = currentCameraAlarmInfo.Count;
                if (count == 0)
                {
                    Log.Error("GetCurrentCameraAlarmList", "currentCameraAlarmInfoCount == 0");
                    return;
                }
                int last = count - 1;
                Log.Info("GetCurrentCameraAlarmList", "last:" + last);
                var info = currentCameraAlarmInfo[last];
                if (info == null)
                {
                    Log.Error("GetCurrentCameraAlarmList", "info == null");
                    return;
                }

                Log.Info("GetCurrentCameraAlarmList", "pic_data:" + info.pic_data.Length);

                byte[] Pic = PictureData(info.pic_data);
                texture.LoadImage(Pic);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Picture.sprite = sprite;

                CurrentCameraAlarmInfo = currentCameraAlarmInfo;
            }
            if (!IsInvoking("RefreshCameraAlarmInfo"))
            {
                InvokeRepeating("RefreshCameraAlarmInfo", 0, ReshTime);

            }
        }
        catch (Exception ex)
        {
            Log.Error("GetCurrentCameraAlarmList", "" + ex); ;
        }
    }
    public byte[] PictureData(string picture)
    {
        picture = picture.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
        byte[] bytes = Convert.FromBase64String(picture);
        return bytes;
    }
    public GameObject InstantiateLine()
    {
        GameObject AlarmObj = Instantiate(LineExample);
        AlarmObj.transform.parent = grid.transform;
        AlarmObj.transform.localScale = new Vector3(1, 1, 1);
        AlarmObj.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(400, -230f, 0);
        AlarmObj.SetActive(true);

        return AlarmObj;
    }
    public void DelectChildItem()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    /// <summary>
    /// 显示视屏（通过Rtsp地址）
    /// </summary>
    /// <param name="rtspUrl"></param>
    public void ShowVideo(string rtspUrl, UniversalMediaPlayer vedioPlayer)
    {
        if (connectBg) connectBg.SetActive(true);
        if (vedioPlayer)
        {

            if (vedioPlayer.IsPlaying) vedioPlayer.Stop();
            vedioPlayer.Path = rtspUrl;
            vedioPlayer.Volume = 100;
            vedioPlayer.PlayRate = 1;
            vedioPlayer.Play();
        }
        else
        {
            Debug.LogError("Error : RtspVideo.ShowVideo-> UniversalMediaPlayer is null!");
        }
    }
    /// <summary>
    /// 绑定播放错误事件
    /// </summary>
    /// <param name="action"></param>
    private void AddEncounteredEvent(UniversalMediaPlayer vedioPlayer, Action action = null)
    {
        if (vedioPlayer)
        {
            vedioPlayer.AddEncounteredErrorEvent(() =>
            {
                OnEncounteredError(vedioPlayer);
            });
            vedioPlayer.AddPlayingEvent(OnVideoPlay);
        }
        if (action != null) action();
    }

    private void OnVideoPlay()
    {
        Debug.Log("Video start play...");
        connectBg.SetActive(false);
    }

    public int currentReconnectTime=0;
    private int defaultReconnectTime = 5;

    /// <summary>
    /// 播放错误回调
    /// </summary>
    private void OnEncounteredError(UniversalMediaPlayer p)
    {
        int maxReconnectTime = TryGetMaxConnectTime();
        if (currentReconnectTime < maxReconnectTime)
        {
            currentReconnectTime++;
            if (CurrentCameraDev != null)
            {
                string path = p == null ? "" : p.Path;
                Debug.LogErrorFormat("ReconnectVideo:{0} connectTimes:{1}", path, currentReconnectTime);
                DestroyImmediate(p.transform.gameObject);
                if (!IsInvoking("ShowVideoByReconnect"))
                {
                    Invoke("ShowVideoByReconnect",0.1f);
                }
            }
            else
            {
                Log.Error("CameraAlarmFollowUI.OnEncounteredError.CurrentCameraDev is null!");
            }
        }
        else
        {
            //Debug.LogError("Error:" + player.LastError);
            DestroyImmediate(p.transform.gameObject);
            connectBg.SetActive(true);
            if(VedioPrompt)
                VedioPrompt.text = "视频连接失败!";
            UGUIMessageBox.Show("视频连接失败！");
        }
    }
    /// <summary>
    /// 关闭重连的Invoke
    /// </summary>
    private void CloseReconnectInvoke()
    {
        if (IsInvoking("ShowVideoByReconnect"))
        {
            CancelInvoke("ShowVideoByReconnect");
        }
    }
    /// <summary>
    /// 通过重连显示视频
    /// </summary>
    private void ShowVideoByReconnect()
    {
        ShowCurrentCameraVedio(CurrentCameraDev);
    }

    private int TryGetMaxConnectTime()
    {
        try
        {
            int maxReconnectTime = SystemSettingHelper.systemSetting.HoneyWellSetting.MaxConnectTime.ToInt();
            return maxReconnectTime;
        }catch(Exception e)
        {
            return defaultReconnectTime;
        }
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void SetInfo(DevNode devNode, Toggle Tog)
    {
        SelectTog = Tog;
        CameraDev = devNode;
    }
    Toggle SelectTog;
    public void CloseSelectFollowUI()
    {
        SelectTog.isOn = false;
    }
    public void Max_ButClick()
    {
        CameraAlarmManage.Instance.SetInfo(this,CurrentCameraDev);
        CameraAlarmManage.Instance.ShowMaxCameraALarmWindow(true);
        ShowFollowUI(false);
        if (VideoTog.isOn)
        {
            CameraAlarmManage.Instance.IsPrompt = false;
            CameraAlarmManage.Instance.ShowCameraVedio(CurrentCameraDev, this);
            VideoTog.isOn = false;
            //ShowCameraVedioWindow(false);
        }
        else if (AlarmTog.isOn)
        {
            CameraAlarmManage.Instance.ShowCurrentCameraDevAlarm(CurrentCameraDevID, this, CurrentCameraDev);
            AlarmTog.isOn = false;
            ShowCameraAlarmWindow(false);
        }
        else if (PictureTog.isOn)
        {
            CameraAlarmManage.Instance.ShowPictureInfo(CurrentCameraDev, this, RefreshCameraAlarmListInfo);
            PictureTog.isOn = false;
            ShowSmallPictureWindow(false);
        }
    }
    public void ShowSmallPictureWindow(bool b)
    {
        SmallPictureWindow.SetActive(b);
        if (b == true)
        {
            ShowPictureInfo();
        }
    }

    public void FixedTog_Click(bool b)
    {
        if (b)
        {
            this.gameObject.AddComponent<UGUIWindowDrag>();
            FollowParentPos = this.gameObject.GetComponent<RectTransform>().localPosition;
            this.gameObject.transform.parent = CameraAlarmFollowUILocation.Instance.CameraAlarmFollowUILocationWindow.transform;
            FixedTog.GetComponent<ControlMenuToolTip>().TipContent = "拖动并固定";
        }
        else
        {
            RecoverFollowParentPos();
        }
    }
    public void RecoverFollowParentPos()
    {
        if (FixedTog == null) return;
        this.gameObject.RemoveComponent<UGUIWindowDrag>();
        if (this.gameObject.transform.parent != FollowParentObj.transform)
        {
            this.gameObject.transform.parent = FollowParentObj.transform;
            this.gameObject.GetComponent<RectTransform>().localPosition = FollowParentPos;
            FixedTog.GetComponent<ControlMenuToolTip>().TipContent = "跟随摄像头";
        }



    }
    void Update()
    {

    }
}
