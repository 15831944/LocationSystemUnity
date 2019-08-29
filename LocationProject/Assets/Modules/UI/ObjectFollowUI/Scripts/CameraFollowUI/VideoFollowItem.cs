using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;
using UnityEngine.UI;

public class VideoFollowItem : MonoBehaviour {

    public GameObject window;
    public Text TitleText;
    public Button MaxBut;//最大化按钮
    public Toggle FixedTog;//固定按钮

    public GameObject connectBg;//连接时提示的背景
    public GameObject VedioPrefab;
    public GameObject VideoWindow;    

    public GameObject FollowParentObj;

    private DevNode CameraDev;
    private UniversalMediaPlayer player;
    private CameraDevController CurrentCameraDev;
    private Toggle SelectTog;//漂浮UI的Toggle
    private Vector3 FollowParentPos;
    private bool isVideoPlay;//视频是否播放
    private bool isDestoryingPlayer;
    // Use this for initialization
    void Start () {
        MaxBut.onClick.AddListener(Max_ButClick);
        FixedTog.onValueChanged.AddListener(FixedTog_Click);
    }
    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void SetInfo(DevNode devNode, Toggle Tog)
    {
        SelectTog = Tog;
        CameraDev = devNode;
        CurrentCameraDev = CameraDev as CameraDevController;
    }

    public void Show()
    {
        isVideoPlay = false;
        window.SetActive(true);
        CameraDev.HighlightOn();
        ShowCurrentCameraVedio();
    }
    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        currentReconnectTime = 0;        
        if(FixedTog!=null)FixedTog.isOn = false;
        if (player != null)
        {
            if(player.IsPlaying) player.Stop();
            Destroy(player.gameObject);//等待一秒再删除老播放器
        }      
        window.SetActive(false);
    }


    public void FixedTog_Click(bool b)
    {
        if (b)
        {
            gameObject.AddComponent<UGUIWindowDrag>();
            FollowParentPos = gameObject.GetComponent<RectTransform>().localPosition;
            transform.parent = CameraAlarmFollowUILocation.Instance.CameraAlarmFollowUILocationWindow.transform;
            FixedTog.GetComponent<ControlMenuToolTip>().TipContent = "拖动并固定";
        }
        else
        {
            RecoverFollowParentPos();
        }
    }
    /// <summary>
    /// 打开大界面，查看监控视频
    /// </summary>
    public void Max_ButClick()
    {
        //OldMaxMethod();
        NewMaxMethod();
    }
    /// <summary>
    /// 把小窗口的RawImage移到大窗口
    /// </summary>
    private void NewMaxMethod()
    {
        if (!isVideoPlay)
        {
            UGUIMessageBox.Show("视频正在连接中，请稍后...");
            return;
        }
        if (CameraVideoRtsp.Instance&&CurrentCameraDev!=null)
        {
            string titleTemp= "监控视频";
            if (CurrentCameraDev.Info != null) titleTemp = CurrentCameraDev.Info.Name;
            if (DevSubsystemManage.IsRoamState)
            {
                Transform parent = RoamFollowMange.Instance == null ? transform : RoamFollowMange.Instance.gameObject.transform;
                CameraVideoRtsp.Instance.SetNewParent(parent);
            }
            CameraVideoRtsp.Instance.Show(titleTemp,player.gameObject);
        }
        else
        {
            Debug.LogError("NewRtspMaxMethod.CameraVideoRtsp.instance is null!");
        }
    }
    /// <summary>
    /// 创建一个新播放器
    /// </summary>
    private void OldMaxMethod()
    {
        if (SelectTog && SelectTog.isOn) SelectTog.isOn = false;
        if (CameraDev != null && CameraDev is CameraDevController)
        {
            if (CameraVideoManage.Instance)
            {
                if (DevSubsystemManage.IsRoamState)
                {
                    Transform parent = RoamFollowMange.Instance == null ? transform : RoamFollowMange.Instance.gameObject.transform;
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
    /// 打开当前设备的监控视频
    /// </summary>
    /// <param name="cameraDev"></param>
    private void ShowCurrentCameraVedio()
    {
        try
        {
            if (CurrentCameraDev == null) return;
            if (CurrentCameraDev.Info != null) TitleText.text = CurrentCameraDev.Info.Name;
            else
            {
                TitleText.text = "监控视频";
                Debug.LogError("Error.VideofollowItem.ShowCurrentCameraVideo: CurrentCameraDev.Info is null:" + CurrentCameraDev.transform.name);
                return;
            }
            Dev_CameraInfo camInfo = CurrentCameraDev.GetCameraInfo(CurrentCameraDev.Info);
            if (string.IsNullOrEmpty(camInfo.RtspUrl))
            {
                Debug.LogError("Error : RtspVideo.ShowVideo-> rtspURL is null!");
                UGUIMessageBox.Show("Rtsp地址未输入!");
                return;
            }
            if (!IsEffectiveURL(camInfo.RtspUrl))
            {
                UGUIMessageBox.Show("Rtsp地址无效!");
                return;
            }

            GameObject Obj = Instantiate(VedioPrefab, VideoWindow.transform) as GameObject;
            Obj.transform.SetAsFirstSibling();
            Obj.SetActive(true);
            VedioPrefab.SetActive(false);
            player = Obj.GetComponent<UniversalMediaPlayer>();
            AddEncounteredEvent(player, () =>
            {
                ShowVideo(camInfo.RtspUrl, player);
            });
        }catch(Exception e)
        {
            Debug.LogErrorFormat("VideofollowItem.ShowCurrentCameraVideo.ErrorInfo:{0}",e.ToString());
        }
        
    }
    public void RecoverFollowParentPos()
    {
        if (FixedTog == null) return;
        gameObject.RemoveComponent<UGUIWindowDrag>();
        if (transform.parent != FollowParentObj.transform)
        {
            transform.parent = FollowParentObj.transform;
            transform.GetComponent<RectTransform>().localPosition = FollowParentPos;
            FixedTog.GetComponent<ControlMenuToolTip>().TipContent = "跟随摄像头";
        }
    }
    #region 视频播放处理
    /// <summary>
    /// 显示视屏（通过Rtsp地址）
    /// </summary>
    /// <param name="rtspUrl"></param>
    private void ShowVideo(string rtspUrl, UniversalMediaPlayer vedioPlayer)
    {
        if (connectBg) connectBg.SetActive(true);
        if (vedioPlayer)
        {
            if (vedioPlayer.IsPlaying) vedioPlayer.Stop();
            vedioPlayer.Path = rtspUrl;
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
    /// <summary>
    /// 视频播放成功
    /// </summary>
    private void OnVideoPlay()
    {
        Debug.Log("Video start play...");
        isVideoPlay = true;
        connectBg.SetActive(false);
    }

    private int maxReconnectTime = 3;//最大重连次数
    private int currentReconnectTime;//当前重连次数
    /// <summary>
    /// 播放错误回调
    /// </summary>
    private void OnEncounteredError(UniversalMediaPlayer p)
    {
        Destroy(p.gameObject);//等待一秒再删除老播放器
        connectBg.SetActive(true);
        UGUIMessageBox.Show("视频连接失败！");
        //if (currentReconnectTime < maxReconnectTime)
        //{
        //    currentReconnectTime++;
        //    DestroyImmediate(p.transform.gameObject);//删除老的播放器
        //    ShowCurrentCameraVedio();
        //}
        //else
        //{
        //    //Debug.LogError("Error:" + player.LastError);
        //    DestroyImmediate(p.transform.gameObject);
        //    connectBg.SetActive(true);
        //    UGUIMessageBox.Show("视频连接失败！");
        //}
    }
    /// <summary>
    /// 是否有效地址
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private bool IsEffectiveURL(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion
}
