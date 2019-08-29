using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using UMP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

public class NVSPlayerClient : MonoBehaviour
{

    public InputField TbCid;

    public InputField TbIp;
    public InputField TbChannel;

    public InputField TbStart;

    public InputField TbEnd;

    public InputField TbURL;

    public UniversalMediaPlayer Player;

    public Toggle CbIsFinished;

    public Slider Slider1;

    public Text restul;

    private LocationServiceClient client;

    private DownloadInfo info;
    

    public void Play()
    {
        info = new DownloadInfo();
        info.CId = int.Parse(TbCid.text);
        info.StartTime = TbStart.text;
        info.EndTime = TbEnd.text;
        info.Channel = TbChannel.text;
        info.Ip = TbIp.text;

        Download(info);
    }

    public void Download(DownloadInfo info)
    {
        Debug.Log(string.Format("Download:{0},{1},{2},{3},{4}", info.CId, info.Ip, info.Channel, info.StartTime, info.EndTime));
        this.info = info;//将参数设置到属性中
        if (client == null)
        {
            client = CommunicationObject.Instance.GetServiceClient();
        }

        ThreadManager.Run(() =>
        {
            var r = client.StartGetNVSVideo(info);//开始下载视频
            return r;
        }, (r) =>
        {
            Debug.Log("Info==null?"+info==null+ transform.name);
            if (r != null)
            {
                WriteLog(r.Result + "|" + r.Message + "|" + r.Url);
                this.info = r;//服务端会把下载用的id返回过来
                if (r.Result)
                {
                    if (string.IsNullOrEmpty(r.Url))
                    {
                        InvokeRepeating("GetProgress", 0, 1);//开始获取下载进度
                    }
                    else
                    {
                        Play(r.Url);//已经查询并下载成功了的时间段
                    }
                }
                else
                {
                    if (r.Message == "It is downloading!")//有文件在下载
                    {
                        //todo:等待，休眠1s，并重新尝试下载。重试次数10次，还是这样则提示，并不重试。
                    }
                    else
                    {
                        //todo:提示Message内容
                    }
                }
            }
            else
            {
                WriteLog("结果为空");
            }
        }, "");
    }

    private string _log;

    private void WriteLog(string log)
    {
        string txt = string.Format("[{0}]{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), log);
        _log = txt + "\n" + _log;
        restul.text = _log;
        Debug.Log(log);
    }

    public void GetProgress()
    {
        if (client == null)
        {
            client = CommunicationObject.Instance.GetServiceClient();
        }
        
        var progress = client.GetNVSProgress(info);
        if (progress != null)
        {
            if(restul) restul.text = progress.ToString();
            //ProgressBar1.Value = progress.Progress;

            if (Slider1 != null)
            {
                Slider1.value = progress.Progress;
            }
            
            WriteLog(progress.ProgressText);
            if (progress.IsFinished)
            {
               CancelInvoke("GetProgress");
               WriteLog("下载完成:"+progress.Url);
               Play(progress.Url);
            }
        }
        else
        {
            CancelInvoke("GetProgress");
            WriteLog("进度为空");
        }
    }

    public event Action<float, string> ProgressChanged;

    public string Url;

    private void Play(string url)
    {
        Url = url;

        if (TbURL != null)
        {
            TbURL.text = url;
        }
        else
        {
            Debug.LogError("TbURL == null");
        }

        if (Player != null)
        {
            Player.Path = url;
        }
        else
        {
            Debug.LogError("Player == null");
        }

        if (PlayAction != null)
        {
            PlayAction(url);
        }
        else
        {
            Player.Play();
            PlayVideo();
        }
    }

    public Action<string> PlayAction;

	// Use this for initialization
    void Start()
    {
        Debug.Log("Start" );
        LoginInfo loginInfo = new LoginInfo();
        loginInfo.UserName = "Admin";
        loginInfo.Password = "Admin@123456";
        //Debug.Log("loginInfo:" + loginInfo);
        //var r = client.Login(loginInfo);
        //Debug.Log("r:"+ r);
        //WriteLog(r.Result + "|" + r.Authority+"|"+r.Session);
    }

    public VideoPlayer VideoPlayer;

    public void PlayVideo()
    {
        //if (VideoPlayer != null)
        //{

        //    if (string.IsNullOrEmpty(Url))
        //    {
        //        Url = Player.Path;
        //    }

        //    VideoPlayer.url = Url;
        //    VideoPlayer.Play();
        //}
    }

    // Update is called once per frame
	void Update () {
		
	}

    public void OpenExe()
    {
        string args = string.Format("{0} {1} {2} {3} {4}", TbIp.text, "3000", "Admin", "1111add", TbChannel.text);
        string path = Application.dataPath + "/../NVS/NVSPlayer.exe";
        Debug.Log("path:" + path);
        WriteLog("path:" + path);
        FileInfo file = new FileInfo(path);
        Debug.Log("file.Exists:" + file.Exists);
        WriteLog("file.Exists:" + file.Exists);
        if (file.Exists)
        {
            Process.Start(path, args);
        }
        else
        {

        }
    }


    //开启exe
    private void StartEXE(string folderName)
    {
        FileInfo[] files = new DirectoryInfo(Application.dataPath + "/../../" + folderName).GetFiles();
        UnityEngine.Debug.Log(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            UnityEngine.Debug.Log(files[i].FullName);

            if (files[i].FullName.Contains(".exe") || files[i].FullName.Contains(".EXE"))
            {
                Process.Start(files[i].FullName);
                break;
            }
        }

    }
}
