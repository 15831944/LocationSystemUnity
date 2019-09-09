using System;
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using Base.Common.Extensions;
using UMP;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PlaybackWindow : MonoBehaviour
{

    public GameObject Window;

    public NVSPlayerClient Client;

    public DatePickerChange Date;

    public Dropdown StartH;

    public Dropdown StartM;

    public int[] TimeSpaneItems = new int[]{1,3,5,10,15,20,25,30};

    public Dropdown TimeSpane;

    void Start () {

        Client.Player = Player;
        Client.ProgressChanged += Client_ProgressChanged;
        Client.PlayAction = url => {
            Play();
        };
        InitUI();
    }

    private void InitUI()
    {
        Date.DateTime = DateTime.Now;

        //初始时间从8:30开始
        List<string> hourStrs = new List<string>();
        for (int i = 0; i < 24; i++)
        {
            hourStrs.Add(i.ToString());
        }

        StartH.ClearOptions();
        StartH.AddOptions(hourStrs);


        List<string> minuteStrs = new List<string>();
        for (int i = 0; i < 60; i++) //一个单位代表10分钟
        {
            minuteStrs.Add((i * 1).ToString());
        }

        StartM.ClearOptions();
        StartM.AddOptions(minuteStrs);


        List<string> ts = new List<string>();
        for (int i = 0; i < TimeSpaneItems.Length; i++)
        {
            ts.Add(TimeSpaneItems[i] + "");
        }

        TimeSpane.ClearOptions();
        TimeSpane.AddOptions(ts);
    }

    public Text ProgressText;

    private void Client_ProgressChanged(float arg1, string arg2)
    {
        //ProgressbarLoad.Instance.Show(arg1);
        ProgressText.text = string.Format("加载视频...{0}%", arg1);
    }

    public void Show()
    {
        ProgressText.text = "";

        if (Window != null)
        {
            Window.SetActive(true);
        }
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        InitUI();

        //if (HideWindows != null)
        //{
        //    HideWindows.Hide();
        //}

        CameraAlarmManage.Instance.CloseAllWindow();
    }

    public void Hide()
    {
        if (Window != null)
        {
            Window.SetActive(false);
        }

        Stop();

        //if (HideWindows != null)
        //{
        //    HideWindows.Show();
        //}

        CameraAlarmManage.Instance.ShowCameraVideo();
    }

    public GameObject[] HideWindows;

    public void OpenExe()
    {
        var camera = CameraAlarmManage.Instance.currentCameraDev; //里面的属性要改成public
        if (camera == null)
        {
            Debug.LogError("OpenExe camera == null");
            return;
        }
        var info = camera.GetCameraInfo(camera.Info);
        GetIpAndChannel(info);
        string args = string.Format("{0} {1} {2} {3} {4}", ip, "3000", "Admin", "1111add", channel);
        Debug.Log("args:" + args);
        string path = Application.dataPath + "/../NVS/NVSPlayer.exe";
        Debug.Log("path:" + path);
        //WriteLog("path:" + path);
        FileInfo file = new FileInfo(path);
        Debug.Log("file.Exists:" + file.Exists);
        //WriteLog("file.Exists:" + file.Exists);
        if (file.Exists)
        {
            Process.Start(path, args);
        }
        else
        {

        }
    }

    public UniversalMediaPlayer Player;

    public GameObject ContentMask;

    public void Play()
    {
        Log.Info("PlaybackWindow.Play1",
            string.Format("IsPlaying:{0},AbleToPlay:{1},IsReady:{2},Length:{3},AutoPlay:{4}",
            Player.IsPlaying,
            Player.AbleToPlay,
            Player.IsReady, Player.Length, Player.AutoPlay));

        if (Player.IsPlaying)
        {
            Player.Stop();

            Log.Info("PlaybackWindow.Play", "Stop");
        }

        //Player.Play();
        //ContentMask.SetActive(false);



        Player.AutoPlay = true;
        Player.Play();
        ContentMask.SetActive(false);

        StartPlay = true;

        Log.Info("PlaybackWindow.Play2",
            string.Format("IsPlaying:{0},AbleToPlay:{1},IsReady:{2},Length:{3},AutoPlay:{4}",
            Player.IsPlaying,
            Player.AbleToPlay,
            Player.IsReady, Player.Length, Player.AutoPlay));
    }

    public bool StartPlay = false;

    public void Stop()
    {
        StartPlay = false;
        Player.Stop();
        ContentMask.SetActive(true);
        LengthText.text = "";
    }

    public void Pause()
    {
        Player.Pause();
        ContentMask.SetActive(true);
        LengthText.text = "";
    }

    /// <summary>
    /// 播放视频长度
    /// </summary>
    public Text LengthText;

    /// <summary>
    /// 当前播放时间
    /// </summary>
    public Text PositionText;

    /// <summary>
    /// 进度条
    /// </summary>
    public Slider ProgressSlider;

    // Update is called once per frame
    void Update()
    {
        if (Player.IsPlaying)
        {
            TimeSpan time1 = TimeSpan.FromMilliseconds(Player.Length * Player.Position);
            PositionText.text = GetTimeText(time1);
            TimeSpan time2 = TimeSpan.FromMilliseconds(Player.Length);
            LengthText.text = GetTimeText(time2);

            if(ShowUpdateLog)
                Debug.Log(string.Format("position:{0}({1},length:{2}ms({3})", Player.Position, Player.Length, PositionText.text, LengthText.text));
            //要先手动移除Editor里面添加的事件
            ProgressSlider.onValueChanged.RemoveAllListeners();//不知道有没有必要
            ProgressSlider.value = Player.Position;
            ProgressSlider.onValueChanged.AddListener(OnSliderChanged);//不知道有没有必要
            Position = Player.Position;
        }
        else
        {
            if (StartPlay)
            {
                if(Player.AbleToPlay && Player.IsReady)
                {
                    //Player.Play();
                    //ContentMask.SetActive(false);
                    StartPlay = false;
                }
                else
                {
                   // Log.Info("PlaybackWindow.Play2",
                   //string.Format("IsPlaying:{0},AbleToPlay:{1},IsReady:{2},Length:{3},AutoPlay:{4}",
                   //Player.IsPlaying,
                   //Player.AbleToPlay,
                   //Player.IsReady, Player.Length, Player.AutoPlay));
                }
            }
        }

        
    }

    public bool ShowUpdateLog = false;

    public float Position;

    public void OnSliderChanged(float value)
    {
        Debug.Log(string.Format("OnSliderChanged:{0},Position:{1}", value, Position));
        SetPostion(value);
    }

    private void SetPostion(float v)
    {
        Player.Position=v;
    }

    private string GetTimeText(TimeSpan t)
    {
        if (t.TotalHours >= 1)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
        }
    }

    public void Download()
    {
        var camera = CameraAlarmManage.Instance.currentCameraDev; //里面的属性要改成public

        if (camera == null)
        {
            Debug.LogError("Download camera == null");
            return;
        }

        var info = camera.GetCameraInfo(camera.Info);

        GetIpAndChannel(info);
        GetTime();

        ////DV_192.168.108.107_1_1_20190720150000-20190720150500
        //ip = "192.168.108.107";
        //channel = "1";
        //start = new DateTime(2019, 7, 20,15,0, 0);
        //end = start.AddMinutes(5);

        DownloadInfo dInfo = new DownloadInfo();
        dInfo.CId = info.Id;
        dInfo.Ip = ip;
        dInfo.Channel = channel;

        

        dInfo.StartTime = start.ToString("yyyy-MM-dd HH:mm:ss");
        dInfo.EndTime = end.ToString("yyyy-MM-dd HH:mm:ss");
        //dInfo.Channel = TbChannel.text;
        //dInfo.Ip = TbIp.text;
        Stop();
        Client.Download(dInfo);
    }

    private string ip;
    private string channel;
    private void GetIpAndChannel(Dev_CameraInfo info)
    {
        string rtsp = info.RtspUrl;
        Debug.Log("rtsp:" + rtsp);
        string[] parts = rtsp.Split('@');
        string[] parts2 = parts[1].Split('/');
        ip = parts2[0];
        channel = parts2[1];
    }

    private DateTime start;
    private DateTime end;

    private void GetTime()
    {
        DateTime day = Date.DateTime;
        string h = StartH.captionText.text;
        string m = StartM.captionText.text;
        string t = TimeSpane.captionText.text;
        start = new DateTime(day.Year, day.Month, day.Day, h.ToInt(), m.ToInt(), 0);
        end = start.AddMinutes(t.ToInt());
    }

}
