using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RtspVideo : MonoBehaviour {

    public GameObject window;
    public GameObject connectBg;//连接时提示的背景
    public UniversalMediaPlayer player;
    public Button closeButton;//窗口关闭按钮

    private DevInfo tempDevInfo;//没有Rtsp地址，使用devinfo调用外部霍尼韦尔视频
	// Use this for initialization
	void Start () {
        InitButtonMethod();
        AddEncounteredErrorEvent();
    }

	/// <summary>
    /// 显示视屏（通过Rtsp地址）
    /// </summary>
    /// <param name="rtspUrl"></param>
    public void ShowVideo(string rtspUrl,DevInfo devInfo)
    {
        if (window) window.SetActive(true);
        if(connectBg)connectBg.SetActive(true);
        tempDevInfo = devInfo;
        if (string.IsNullOrEmpty(rtspUrl))
        {
            Debug.LogError("Error : RtspVideo.ShowVideo-> rtspURL is null!");
            UGUIMessageBox.Show("Rtsp地址未输入,是否启用霍尼韦尔视频？","确定" ,"取消", ShowHoneyWellVideo, Close, Close);
            return;
        }
        if(player)
        {
            if (player.IsPlaying) player.Stop();
            player.Path = rtspUrl;
            player.Play();
        }
        else
        {
            Debug.LogError("Error : RtspVideo.ShowVideo-> UniversalMediaPlayer is null!");
        }
    }

    /// <summary>
    /// 使用霍尼韦尔，显示视频
    /// </summary>
    private void ShowHoneyWellVideo()
    {
        Close();
        if (CameraVideoManage.Instance.honeyWellVideo&&tempDevInfo!=null)
        {
            CameraVideoManage.Instance.honeyWellVideo.ShowVideo(tempDevInfo.Abutment_DevID);
        }
    }

    /// <summary>
    /// 关闭视频
    /// </summary>
    public void Close()
    {
        if (window) window.SetActive(false);
        if(player)
        {
            player.Stop();
        }
    }
    /// <summary>
    /// 给按钮绑定方法
    /// </summary>
    private void InitButtonMethod()
    {
        if(closeButton)
        {
            closeButton.onClick.AddListener(Close);
        }
    }
    /// <summary>
    /// 绑定播放错误事件
    /// </summary>
    /// <param name="action"></param>
    private void AddEncounteredErrorEvent()
    {
        if (player)
        {
            player.AddEncounteredErrorEvent(OnEncounteredError);
            player.AddPlayingEvent(OnVideoPlay);
        }
    }
    private void OnVideoPlay()
    {
        Debug.Log("Video start play...");
        connectBg.SetActive(false);
    }
    /// <summary>
    /// 播放错误回调
    /// </summary>
    private void OnEncounteredError()
    {
        //Debug.LogError("Error:"+ player.LastError);
        UGUIMessageBox.Show("视频连接失败！","取消","确定",Close,Close,Close);
    }
}
