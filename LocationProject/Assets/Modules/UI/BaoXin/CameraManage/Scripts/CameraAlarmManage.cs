using Base.Common.Extensions;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMP;
using UnityEngine;
using UnityEngine.UI;

public class CameraAlarmManage : MonoBehaviour
{
    public static CameraAlarmManage Instance;
    public Toggle VideoTog;
    public Toggle AlarmTog;
    public Toggle PictureTog;
    public GameObject CameraAlarmWindow;
    public GameObject MaxGrid;
    public GameObject MaxLineExample;
    public GameObject VedioWindow;
    public GameObject AlarmWindow;
    public GameObject AlarmScreen;
    public Text TitleText;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    const int pageSize = 10;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pegeTotalText;
    /// <summary>
    /// 输入页数
    /// </summary>
    public InputField pegeNumText;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;

    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;

    List<CameraAlarmInfo> ShowList;
    List<CameraAlarmInfo> ScreenList;
    public Sprite DoubleImage;
    public Sprite OddImage;
    public Text AlarmTimeText;
    public Button CloseBut;
    string CurrentDevID;
    bool isRefresh = false;
    CameraAlarmFollowUI SmallCameraAlarmFollow;
    public Button MaxBut;
    public CalendarChange CameraAlarmAlendarDay;
    public CameraDropdown CameraAlarmDropdown;
    public GameObject VedioPrefabs;
    public GameObject PlayParent;
    UniversalMediaPlayer player;
    public GameObject connectBg;//连接时提示的背景
    public CameraDevController currentCameraDev;
    public GameObject AlarmPictureWindow;
    public Image Picture;
    public Text VedioPrompt;
    int width = 400;
    int height = 225;
    List<CameraAlarmInfo> MaxCameraAlarmList;
    List<CameraAlarmInfo> RefreshCameraAlarmListInfo;
    public GameObject NewstAlarmTag;//有新告警图片提示
    AlarmSearchArg CameraAlarmSearchArg;
    List<CameraAlarmInfo> CameraHistoryAlarm;
    List<CameraAlarmInfo> CurrentCameraHistoryAlarm;
    public Sprite nullPicture;//没有告警，告警图片显示背景图片
    int ReshTime=20;
    void Start()
    {
        VideoTog.onValueChanged.AddListener(ShowVedioWindow);
        AlarmTog.onValueChanged.AddListener(ShowAlarmWindow);
        AddPageBut.onClick.AddListener(AddDepartmentPage);
        MinusPageBut.onClick.AddListener(MinusDepartmentPage);
        pegeNumText.onValueChanged.AddListener(InputCreamAlarmtPage);
        CloseBut.onClick.AddListener(() =>
       {
           CloseAllWindow();
       });
        MaxBut.onClick.AddListener(ShowSmallCameraAlarmFollowUI);
        SmallCameraAlarmFollow = new global::CameraAlarmFollowUI();
        CameraAlarmAlendarDay.onDayClick.AddListener(ScreenCameraAlarmTime);


        MaxCameraAlarmList = new List<CameraAlarmInfo>();
        PictureTog.onValueChanged.AddListener(ShowPictureWindow);
    }
    private void Awake()
    {
        Instance = this;
    }

    public void ShowCameraVideo()
    {
        ShowCameraVedio(currentCameraDev, SmallCameraAlarmFollow);
    }

    /// <summary>
    /// 打开摄像头监控视频
    /// </summary>
    /// <param name="cameraDev"></param>
    public void ShowCameraVedio(CameraDevController cameraDev, CameraAlarmFollowUI SmallUI)
    {
        if(VedioPrompt!=null)VedioPrompt.text = "视频连接中...";
        SmallCameraAlarmFollow = SmallUI;
        currentCameraDev = cameraDev;
        CurrentDevID = currentCameraDev.Info.Id.ToString();
        if (MaxCameraAlarmList.Count != 0)
        {
            MaxCameraAlarmList.Clear();
        }
        if (AlarmPushManage.Instance.NewestCameraAlarmPush != null)
        {
            MaxCameraAlarmList.AddRange(AlarmPushManage.Instance.NewestCameraAlarmPush);
        }


        if (!VedioWindow.activeSelf)
        {
            VedioWindow.SetActive(true);
        }
        VideoTog.isOn = true;

        TitleText.text = cameraDev.Info.Name;
        if (CameraVideoManage.Instance)
        {
            Dev_CameraInfo camInfo = cameraDev.GetCameraInfo(cameraDev.Info);
            //   if (camInfo != null) CameraVideoManage.Instance.rtspVideo.ShowVideo(camInfo.RtspUrl, cameraDev.Info);
            if (IsPrompt)
            {
                if (string.IsNullOrEmpty(camInfo.RtspUrl))
                {
                    Debug.LogError("Error : RtspVideo.ShowVideo-> rtspURL is null!");
                    if(VedioPrompt)
                        VedioPrompt.text = "Rtsp地址未输入!";
                    UGUIMessageBox.Show("Rtsp地址未输入!");
                    if(SmallCameraAlarmFollow.VedioPrompt)
                        SmallCameraAlarmFollow.VedioPrompt.text = "Rtsp地址未输入!";

                    return;
                }

                if (!IsEffectiveURL(camInfo.RtspUrl))
                {
                    UGUIMessageBox.Show("Rtsp地址无效!");
                    VedioPrompt.text = "Rtsp地址无效!";

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
    /// <summary>
    /// 打开当前摄像头告警界面
    /// </summary>
    /// <param name="cameraDev"></param>
    public void ShowCurrentCameraDevAlarm(string DevID, CameraAlarmFollowUI SmallUI, CameraDevController cameraDev)
    {
        SmallCameraAlarmFollow = SmallUI;
        DelectChildItem();
        if (MaxCameraAlarmList.Count != 0)
        {
            MaxCameraAlarmList.Clear();
        }
        if (AlarmPushManage.Instance.NewestCameraAlarmPush != null)
        {
            MaxCameraAlarmList.AddRange(AlarmPushManage.Instance.NewestCameraAlarmPush);
        }
        currentCameraDev = cameraDev;
        AlarmTog.isOn = true;
        CurrentDevID = DevID;
       
        if (!AlarmWindow.activeSelf)
        {
            AlarmWindow.SetActive(true);
            AlarmScreen.SetActive(true);
        }
        if (!IsInvoking("RefreshCameraAlarmList"))
        {
            InvokeRepeating("RefreshCameraAlarmList", 0, ReshTime);
        }


    }
    private DateTime recordTag;
    public void RefreshCameraAlarmList()
    {
        if (isRefresh) return;
        isRefresh = true;
        Debug.LogError("刷新Max摄像机告警信息");
        DelectChildItem();
        
        CameraHistoryAlarm = new List<CameraAlarmInfo>();
        int id = CurrentDevID.ToInt();
        DevInfo devInfo = new DevInfo();
        devInfo.Id = id;
        Dev_CameraInfo info = CommunicationObject.Instance.GetCameraInfoByDevInfo(devInfo);
        recordTag = DateTime.Now;
        CameraHistoryAlarm = CommunicationObject.Instance.GetCameraAlarms(info.Ip, true);
        Debug.LogError("CommunicationObject.Instance.GetTags,cost :" + (DateTime.Now - recordTag).TotalSeconds + " s");
        CurrentCameraHistoryAlarm = new List<CameraAlarmInfo>();
        if (CameraHistoryAlarm != null)
        {
            for (int i = 0; i < CameraHistoryAlarm.Count; i++)
            {
                if (SmallCameraAlarmFollow.IsSameCamera(CurrentDevID, CameraHistoryAlarm[i]))
                {
                    CurrentCameraHistoryAlarm.Add(CameraHistoryAlarm[i]);
                }
            }
        }
        CurrentCameraHistoryAlarm.Reverse();
        if (CurrentCameraHistoryAlarm.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            isRefresh = false;
            return;
        }

        if (ScreenList == null)
        {
            ScreenList = new List<CameraAlarmInfo>();
        }
        else
        {
            ScreenList.Clear();
        }
        ScreenList.AddRange(CurrentCameraHistoryAlarm);
        int level = CameraAlarmDropdown.CameraTypeDropdown.value;
        string AlarmTime = AlarmTimeText.GetComponent<Text>().text;
        DateTime dateTime = Convert.ToDateTime(AlarmTime);
        ScreenCameraAlarmInfo(level, dateTime);
        //  GetCurrentCameraDevAlarm();

    }
   
    public void SetCameraAlarmInfo(List<CameraAlarmInfo> alarm)
    {
        for (int i = 0; i < alarm.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            CameraAlarmFollowUIItem Item = Obj.GetComponent<CameraAlarmFollowUIItem>();
            Item.GetCameraAlarmData(alarm[i]);
            if (i % 2 == 0)
            {
                Item.GetComponent<Image>().sprite = DoubleImage;
            }
            else
            {
                Item.GetComponent<Image>().sprite = OddImage;
            }
        }

    }

    public void ScreenCameraAlarmType(int level)
    {
        string AlarmTime = AlarmTimeText.GetComponent<Text>().text;
        DateTime dateTime = Convert.ToDateTime(AlarmTime);
        ScreenCameraAlarmInfo(level, dateTime);
    }
    public void ScreenCameraAlarmTime(DateTime dateTime)
    {
        int level = CameraAlarmDropdown.CameraTypeDropdown.value;
        ScreenCameraAlarmInfo(level, dateTime);
    }
    public DateTime GetDataTime(long time_stamp)
    {
        DateTime dtStart = new DateTime(1970, 1, 1);
        long lTime = ((long)time_stamp * 10000000);
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime AlarmTime = dtStart.Add(toNow);
        return AlarmTime;
    }
    public void ScreenCameraAlarmInfo(int level, DateTime dateTime)
    {
        pegeNumText.text = "1";
        ScreenList = new List<CameraAlarmInfo>();
        DateTime TimeAlarm = Convert.ToDateTime(dateTime);
        DateTime NewAlaemTime = TimeAlarm.AddHours(24);

        for (int i = 0; i < CurrentCameraHistoryAlarm.Count; i++)
        {
            bool IsTime = DateTime.Compare(NewAlaemTime, GetDataTime(CurrentCameraHistoryAlarm[i].time_stamp)) >= 0;
            string AlarmType="";
            if (CurrentCameraHistoryAlarm[i].FlameData!= null)
            {
                AlarmType = "火警";
            }
            else if (CurrentCameraHistoryAlarm[i].HeadData!= null)
            {
                AlarmType = "未戴安全帽";
            }
            if (IsTime && ScreenCameraAlarmType(AlarmType, level))
            {
                ScreenList.Add(CurrentCameraHistoryAlarm[i]);
            }
        }
        if (ScreenList.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            DelectChildItem();
        }
        else
        {
            TotaiLine(ScreenList);
            GetPageData(ScreenList);
        }

    }

    public void AddDepartmentPage()
    {
        StartPageNum += 1;
        if (StartPageNum <= ScreenList.Count / pageSize)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPageData(ScreenList);
        }
        else
        {
            StartPageNum -= 1;
        }
    }
    public void MinusDepartmentPage()
    {
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pegeNumText.text = "1";
            }
            else
            {
                pegeNumText.text = PageNum.ToString();

            }
        }
    }
    /// <summary>
    /// 得到第几页数据
    /// </summary>
    /// <param name="depList"></param>
    /// <param name="perInfo"></param>
    public void GetPageData(List<CameraAlarmInfo> alarmList)
    {
        DelectChildItem();
        ShowList = new List<CameraAlarmInfo>();
        if (StartPageNum * pageSize < alarmList.Count)
        {
            var QueryData = alarmList.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                ShowList.Add(per);
            }
            SetCameraAlarmInfo(ShowList);
        }
        isRefresh = false;
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputCreamAlarmtPage(string value)
    {

        if (ScreenList != null && ScreenList.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            return;
        }
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumText.text);
        }
        if (ScreenList == null)
        {
            ScreenList = new List<CameraAlarmInfo>();
        }
        int maxPage = (int)Math.Ceiling((double)(ScreenList.Count) / (double)pageSize);
        if (currentPage > maxPage)
        {
            currentPage = maxPage;
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(ScreenList);
    }
    public void DelectChildItem()
    {
        for (int j = MaxGrid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(MaxGrid.transform.GetChild(j).gameObject);
        }
    }
    public GameObject InstantiateLine()
    {
        GameObject AlarmObj = Instantiate(MaxLineExample);
        AlarmObj.transform.parent = MaxGrid.transform;
        AlarmObj.transform.localScale = new Vector3(1, 1, 1);
        AlarmObj.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(400, -230f, 0);
        AlarmObj.SetActive(true);

        return AlarmObj;
    }
    public bool ScreenCameraAlarmType(string type, int level)
    {
        string Type;
        if (level == 0) return true;
        else if (type == GetCameraAlarmType(level))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public string GetCameraAlarmType(int level)
    {
        if (level == 1) return "火警";
        else
        {
            return "未戴安全帽";
        }
    }
    public void CloseAllWindow()
    {
        ShowMaxCameraALarmWindow(false);
        VedioWindow.SetActive(false);
        ShowAlarmWindow(false);
        SmallCameraAlarmFollow.RecoverFollowParentPos();
        SmallCameraAlarmFollow.CloseSelectFollowUI();
        SmallCameraAlarmFollow.FixedTog.isOn = false;
        SmallCameraAlarmFollow.FollowParentObj = null;
        SmallCameraAlarmFollow = null;
    }
    public void SetInfo(CameraAlarmFollowUI followUI, CameraDevController dev)
    {
        SmallCameraAlarmFollow = followUI;
        currentCameraDev = dev;
    }
    public void ShowMaxCameraALarmWindow(bool b)
    {
        pegeNumText.text = "1";
        pegeTotalText.text = "1";
        CameraAlarmWindow.SetActive(b);
        if (b == false)
        {
            Close();
            if (IsInvoking("RefreshCameraAlarmInfo"))
            {
                CancelInvoke("RefreshCameraAlarmInfo");
                Debug.LogError("关闭刷新摄像机告警信息");
            }
        }
        else
        {
            RefreshCameraAlarmInfo();
            //if (!IsInvoking("RefreshCameraAlarmInfo"))
            //{
            //    InvokeRepeating("RefreshCameraAlarmInfo", 0, ReshTime);

            //}
        }
    }
    public bool IsPrompt = false;//在摄像头链接不成功，点击放大按钮，是不会有提示框的，切换按钮下有提示框
    public void ShowVedioWindow(bool b)
    {
        AlarmScreen.SetActive(false);
        if (b == false)
        {
            VedioPrompt.text = "视频连接中...";
            Close();
        }
        else
        {
            if (!VedioWindow.activeSelf)
            {
                IsPrompt = true;
                ShowCameraVedio(currentCameraDev, SmallCameraAlarmFollow);
            }
        }
        VedioWindow.SetActive(b);
    }

    public void ShowAlarmWindow(bool b)
    {
        isRefresh = false;
        DelectChildItem();
        if (!AlarmWindow.activeSelf && b == true)
        {
            AlarmWindow.SetActive(true);
            AlarmScreen.SetActive(true);
        }
        else
        {
            AlarmWindow.SetActive(b);
            AlarmScreen.SetActive(b);
        }

        if (b)
        {
            if (!IsInvoking("RefreshCameraAlarmList"))
            {
                InvokeRepeating("RefreshCameraAlarmList", 0, ReshTime);
                Debug.LogError("1111111111开始刷新告警数据");
            }
        }
        else
        {
            if (IsInvoking("RefreshCameraAlarmList"))
            {
                CancelInvoke("RefreshCameraAlarmList");
                Debug.LogError("关闭刷新摄像机告警信息");
            }

        }
        Close();
    }
    public void TotaiLine(List<CameraAlarmInfo> data)
    {
        if (data.Count != 0)
        {
            if (data.Count % pageSize == 0)
            {
                pegeTotalText.text = (data.Count / pageSize).ToString();
            }
            else
            {
                pegeTotalText.text = Convert.ToString(Math.Ceiling((double)data.Count / (double)pageSize));
            }
        }
        else
        {
            pegeTotalText.text = "1";
        }
    }
    public void ShowSmallCameraAlarmFollowUI()
    {

        SmallCameraAlarmFollow.ShowFollowUI(true);
        if (VideoTog.isOn == true)
        {
            SmallCameraAlarmFollow.IsMinPrompt = false;
            SmallCameraAlarmFollow.VideoTog.isOn = true;
            VideoTog.isOn = false;
            SmallCameraAlarmFollow.ShowCameraVedioWindow(true);
        }
        else if (AlarmTog.isOn == true)
        {
            SmallCameraAlarmFollow.AlarmTog.isOn = true;
            AlarmTog.isOn = false;
            SmallCameraAlarmFollow.ShowCameraAlarmWindow(true);
        }
        else if (PictureTog.isOn == true)
        {
            PictureTog.isOn = false;
            SmallCameraAlarmFollow.PictureTog.isOn = true;
            SmallCameraAlarmFollow.ShowSmallPictureWindow(true);
        }
        ShowMaxCameraALarmWindow(false);
        VedioWindow.SetActive(false);
        ShowAlarmWindow(false);
        SmallCameraAlarmFollow = null;

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
    /// <summary>
    /// 播放错误回调
    /// </summary>
    private void OnEncounteredError(UniversalMediaPlayer p)
    {
        //Debug.LogError("Error:" + player.LastError);
        DestroyImmediate(p.transform.gameObject);
        connectBg.SetActive(true);
        VedioPrompt.text = "视频连接失败!";
        UGUIMessageBox.Show("视频连接失败！");
    }
    List<CameraAlarmInfo> CurrentPictureAlarmList;
    public void ShowPictureInfo(CameraDevController cameraDev, CameraAlarmFollowUI SmallUI, List<CameraAlarmInfo> infoList)
    {
        if (MaxCameraAlarmList.Count != 0)
        {
            MaxCameraAlarmList.Clear();
        }
        CurrentPictureAlarmList = new List<CameraAlarmInfo>();
        CurrentPictureAlarmList.AddRange(infoList);
        CurrentDevID = cameraDev.Info.Id.ToString();
        MaxCameraAlarmList.AddRange(infoList);
        SmallCameraAlarmFollow = SmallUI;
        currentCameraDev = cameraDev;
        PictureTog.isOn = true;
        if (infoList.Count != 0)
        {
            Texture2D texture = new Texture2D(width, height);
            byte[] Pic = PictureData(infoList[infoList.Count - 1].pic_data);
            texture.LoadImage(Pic);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Picture.sprite = sprite;
        }
        //if (!IsInvoking("RefreshCameraAlarmInfo"))
        //{
        //    InvokeRepeating("RefreshCameraAlarmInfo", 0, 2);

        //}
        RefreshCameraAlarmInfo();
    }
   
    public void RefreshCameraAlarmInfo()
    {
        if (SmallCameraAlarmFollow == null|| currentCameraDev==null) return;
        Debug.LogError("max刷新摄像机告警信息Picture");
        RefreshCameraAlarmListInfo = new List<CameraAlarmInfo>();
        //  RefreshCameraAlarmListInfo.AddRange(AlarmPushManage.Instance.CameraAlarmPushList);
        if (AlarmPushManage.Instance.NewestCameraAlarmPush != null)
        {
            for (int i = 0; i < AlarmPushManage.Instance.AllCameraAlarmPush.Count; i++)
            {
                int? DevID = CommunicationObject.Instance.GetCameraDevIdByURL(AlarmPushManage.Instance.AllCameraAlarmPush[i].cid_url);
                if (DevID == null) return;
                if (currentCameraDev.Info.Id.ToString() == DevID.ToString ())
                {
                    RefreshCameraAlarmListInfo.Add(AlarmPushManage.Instance.NewestCameraAlarmPush[i]);
                }
            }
        }
        //if (RefreshCameraAlarmListInfo.Count==0&& AlarmPictureWindow.activeSelf && PictureTog.isOn == true )
        //{
        //    if (CurrentPictureAlarmList == null) return;
        //        if (CurrentPictureAlarmList.Count==0) return;
        //    Texture2D texture = new Texture2D(width, height);
        //    byte[] Pic = PictureData(CurrentPictureAlarmList[CurrentPictureAlarmList.Count - 1].pic_data);
        //    texture.LoadImage(Pic);
        //    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //    Picture.sprite = sprite;
        //}
        if (RefreshCameraAlarmListInfo.Count > 0 && PictureTog.isOn == false && !AlarmPictureWindow.activeSelf && AlarmPushManage.Instance.IsNewAlarm == true)
        {
            NewstAlarmTag.SetActive(true);
        }
        if (PictureTog.isOn == true && AlarmPictureWindow.activeSelf&& RefreshCameraAlarmListInfo.Count > 0 && AlarmPushManage.Instance.IsNewAlarm == true)
        {
            NewstAlarmTag.SetActive(false);
            if (RefreshCameraAlarmListInfo.Count != 0)
            {
                Texture2D texture = new Texture2D(width, height);
                byte[] Pic = PictureData(RefreshCameraAlarmListInfo[RefreshCameraAlarmListInfo.Count - 1].pic_data);
                texture.LoadImage(Pic);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Picture.sprite = sprite;
                AlarmPushManage.Instance.IsNewAlarm = false;
            }
            MaxCameraAlarmList.Clear();
            MaxCameraAlarmList.AddRange(RefreshCameraAlarmListInfo);
        }

    }
    public byte[] PictureData(string picture)
    {
        picture = picture.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
        byte[] bytes = Convert.FromBase64String(picture);
        return bytes;
    }
    public void ShowPictureWindow(bool b)
    {
        AlarmPictureWindow.SetActive(b);
        if (b)
        {
            NewstAlarmTag.SetActive(false);
            RefreshCameraAlarmInfo();
            //if (!IsInvoking("RefreshCameraAlarmInfo"))
            //{
            //    InvokeRepeating("RefreshCameraAlarmInfo", 0, ReshTime);
            //}
        }
        else
        {
            Picture.sprite = nullPicture;
            NewstAlarmTag.SetActive(false);
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
    // Update is called once per frame
    void Update()
    {

    }
}
