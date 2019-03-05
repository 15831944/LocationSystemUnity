using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HistoryPlayUI : MonoBehaviour
{
    public static HistoryPlayUI Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;

    public Image photo;//头像

    //日期
    public Text dayTxt;
    //关闭按钮
    public Button closeBtn;
    //是否播放
    public bool isPlay;
    //停止按钮
    public Button StopBtn;
    //是否停止（不是暂停）
    public bool isStop;

    public CalendarChange calendar;

    [HideInInspector]
    public float timeStart;    //时间起始播放值
    public DateTime datetimeStart;    //时间起始播放值
    public float timeLength;    //播放时间,单位秒
    public float timeSum;//时间和
    /// <summary>
    /// 人员信息数据
    /// </summary>
    private Personnel personnel;
    /// <summary>
    /// 如果加载数据成功
    /// </summary>
    public bool isLoadDataSuccessed;
    [HideInInspector]
    public float CurrentSpeed = 1;//当前播放速度倍数
    public bool IsMouseDragSlider = false;//鼠标拖动进度条

    List<Position> positions;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(Hide);
        playBtn.onClick.AddListener(PlayBtn_OnClick);
        playBtn.onClick.AddListener(ChangeButtonSprite);
        StopBtn.onClick.AddListener(Stop);
        rateBtn.onClick.AddListener(RateBtn_OnClick);
        calendar.onDayClick.AddListener(Calendar_onDayClick);

        slider.OnValuesChange.AddListener(RangeSliderChanged);
        processSlider.onValueChanged.AddListener(ProcessSlider_ValueChanged);
    }

    float m;

    // Update is called once per frame
    void Update()
    {
        //UGUI类似按钮的控件才能被选中
        if (EventSystem.current.currentSelectedGameObject != null && Input.GetMouseButton(0))
        {
            //print("currentSelectedGameObject:" + EventSystem.current.currentSelectedGameObject.name);
            if (!isStop)
            {
                Slider selectSlider = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Slider>();
                if (selectSlider == processSlider)
                {
                    IsMouseDragSlider = true;
                }
                else
                {
                    IsMouseDragSlider = false;
                }
            }
        }
        else
        {
            IsMouseDragSlider = false;
        }

        m += Time.deltaTime;
        if (isPlay && isLoadDataSuccessed)
        {
            if (!IsMouseDragSlider)
            {
                //float timeT = Time.time - timeStart;
                timeSum += Time.deltaTime * CurrentSpeed;
                float valueT = timeSum / timeLength;
                SetProcessSliderValue(valueT);
                SetProcessCurrentTime(timeSum);
                //print("Time.time:" + Time.time);
                //print("Time.fixedTime:" + Time.fixedTime);
                //print("计算:" + m);
                //print("Time.deltaTime:" + Time.deltaTime);
                //print("Time.fixedDeltaTime:" + Time.fixedDeltaTime);

                if (processSlider.value >= 1)//播放结束
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
            }
            else
            {
                timeSum = timeLength * processSlider.value;
                //print(string.Format("HistoryPlayUI进度时间:{0},进度值:{1}", timeSum, processSlider.value));
                SetProcessCurrentTime(timeSum);
                SetHistoryPath();

            }
        }


    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    private void SetHistoryPath()
    {
        LocationHistoryManager.Instance.SetHistoryPath(processSlider.value);
    }

    [ContextMenu("On_Click1")]
    public void On_Click1()
    {
        ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }
    [ContextMenu("On_Click2")]
    public void On_Click2()
    {
        //按钮点击的变色
        ExecuteEvents.Execute<ISubmitHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="personnelT"></param>
    public void Init(Personnel personnelT)
    {
        personnel = personnelT;
    }

    /// <summary>
    /// 展示
    /// </summary>
    /// <param name="personnelT"></param>
    public void Show(Personnel personnelT)
    {
        FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
        PersonSubsystemManage.Instance.SetAllToggleActive(false);
        SetIsStop(true);
        //ActionBarManage.Instance.Hide();
        StartOutManage.Instance.Hide();
        Init(personnelT);
        ShowBaseInfo(personnelT.Name, personnelT.Pst);
        LocationUIManage.Instance.SetPhoto(photo, personnelT.Sex);
        SetWindowActive(true);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Hide()
    {
        if (window.activeInHierarchy)
        {
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            PersonSubsystemManage.Instance.SetAllToggleActive(true);
            FunctionSwitchBarManage.Instance.SetlightToggle(true);
            ExecuteEvents.Execute<ISubmitHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
            //ActionBarManage.Instance.Show();
            StartOutManage.Instance.Show();
            SetWindowActive(false);
            //ActionBarManage.Instance.ShowLocation();
            LocationManager.Instance.ShowLocation();
            LocationUIManage.Instance.Show();
            PersonnelTreeManage.Instance.ShowWindow();
            SmallMapController.Instance.Show();

            LocationManager.Instance.ExitHistory_One();
            LocationHistoryManager.Instance.ClearHistoryPaths();

        }
    }

    /// <summary>
    /// 设置窗口Active
    /// </summary>
    public void SetWindowActive(bool b)
    {
        window.SetActive(b);
    }

    /// <summary>
    /// 播放历史
    /// </summary>
    public void Play()
    {
        SetIsStop(false);
        isPlay = !isPlay;
        if (isPlay)
        {
            timeLength = (slider.ValueMax - slider.ValueMin) * 3600;
            //bool b = LocationManager.Instance.IsCreateHistoryPath(personnel.Tag.Code);
            //if (!b)
            //{
            ShowHistoryData();
            //}
            FunctionSwitchBarManage.Instance.SetlightToggle(false);
        }
    }

    ///// <summary>
    ///// 暂停历史
    ///// </summary>
    //public void Pause()
    //{

    //}

    /// <summary>
    /// 停止历史(终止)
    /// </summary>
    public void Stop()
    {
        Debug.LogError("Stop!");
        RangeSliderChanged(slider.ValueMin, slider.ValueMax);
        //RefleshProcessSlider("00:00:00");
        //ClearHistoryPath();
        isLoadDataSuccessed = false;
        timeSum = 0;
        //SetIsStop(true);
    }



    /// <summary>
    /// 显示某个标签的历史数据
    /// </summary>
    /// <param name="code"></param>
    public void ShowHistoryData()
    {
        if (isLoadDataSuccessed) return;
        //LocationManager.Instance.ClearHistoryPaths();
        //string code = "0002";
        List<Vector3> list = new List<Vector3>();
        List<DateTime> timelist = new List<DateTime>();
        DateTime end = GetEndTime();
        DateTime start = GetStartTime();
        //List<Position> positions = new List<Position>();
        positions = new List<Position>();

        List<int> topoNodeIds = RoomFactory.Instance.GetCurrentDepNodeChildNodeIds(SceneEvents.DepNode);


        Loom.StartSingleThread(() =>
        {
            //ps = CommunicationObject.Instance.GetHistoryPositonsByTime(code, start, end);
            positions = GetHistoryData(personnel.Id, topoNodeIds, start, end);

            Loom.DispatchToMainThread(() =>
            {
                Debug.LogError("点数：" + positions.Count);
                if (positions.Count < 2) return;
                //int temp = 1;
                //temp = ps.Count / LocationHistoryPath.segmentsMax;
                //if (ps.Count % LocationHistoryPath.segmentsMax > 0)
                //{
                //    temp += 1;//temp个点取一个有效点
                //}
                //foreach (Position p in ps)
                for (int i = 0; i < positions.Count; i++)
                {
                    //if (i % temp == 0)//temp个点取一个有效点
                    //{
                    Position p = positions[i];
                    Vector3 tempVector3 = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
                    tempVector3 = LocationManager.GetRealVector(tempVector3);
                    //Vector3 offset = LocationManager.Instance.transform.position;
                    //temp = new Vector3(temp.x + offset.x, temp.y + offset.y, temp.z + offset.z);
                    list.Add(tempVector3);
                    DateTime t = LocationManager.GetTimestampToDateTime(p.Time);
                    //DateTime t = p.DateTime;
                    timelist.Add(t);
                    //}
                }

                LocationHistoryPath histoyObj = LocationHistoryManager.Instance.ShowLocationHistoryPath(personnel, list, list.Count, Color.green, "HistoryPath0002");
                HistoryManController historyManController= histoyObj.gameObject.AddComponent<HistoryManController>();
                histoyObj.historyManController = historyManController;
                historyManController.Init(Color.green, histoyObj);
                PersonAnimationController personAnimationController = histoyObj.gameObject.GetComponent<PersonAnimationController>();
                personAnimationController.DoMove();
                isLoadDataSuccessed = true;
                timeStart = Time.time;
                timeSum = 0;
                histoyObj.InitData(timeLength, timelist);
            });
        });
    }

    /// <summary>
    /// 获取历史轨迹数据
    /// </summary>
    /// <param name="ps"></param>
    /// <param name="startT"></param>
    /// <param name="end"></param>
    /// <param name="intervalMinute">每次获取数据的时间长度，不超过改数值</param>
    public List<Position> GetHistoryData(int personnelID, List<int> topoNodeIdsT, DateTime startT, DateTime endT, float intervalMinute = 10f)
    {
        List<Position> ps = new List<Position>();
        double minutes = (endT - startT).TotalMinutes;
        float counts = (float)minutes / intervalMinute;
        float valueT = 0;
        float sum = 0;
        while (sum < counts)
        {
            DateTime startTemp;
            DateTime endTemp;
            if (sum + 1 <= counts)
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = startT.AddMinutes(intervalMinute * (sum + 1));
                List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPersonnelID(personnelID, startTemp, endTemp);
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT, startTemp, endTemp);
                ps.AddRange(listT);
            }
            else
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = endT;
                List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPersonnelID(personnelID, startTemp, endTemp);
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT, startTemp, endTemp);
                ps.AddRange(listT);
            }

            sum += 1;
            valueT = sum / counts;
            print("valueT:" + valueT);
            Loom.DispatchToMainThread(() =>
            {
                ProgressbarLoad.Instance.Show(valueT);
            });
        }
        Loom.DispatchToMainThread(() =>
        {
            ProgressbarLoad.Instance.Hide();
        });
        return ps;
    }

    /// <summary>
    /// 获取历史数据
    /// </summary>
    public List<Position> GetPositions()
    {
        return positions;
    }


    /// <summary>
    /// 清空历史轨迹
    /// </summary>
    public void ClearHistoryPath()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths();
    }

    #region 人员基本信息
    public Text nameTxt;//姓名
    public Text jobTxt;//工作岗位
    public Text areaNameTxt;//区域名称

    /// <summary>
    /// 显示基本信息
    /// </summary>
    public void ShowBaseInfo(string nameT, string jobNameT)
    {
        nameTxt.text = nameT;
        jobTxt.text = jobNameT;
    }

    /// <summary>
    /// 设置人员历史所在区域
    /// </summary>
    public void SetItemArea(string areaStr)
    {
        areaNameTxt.text = areaStr;
        //item.RefleshTxtPlace(areaStr);

    }

    #endregion

    #region 选择时间Slider
    //RangeSlider
    public RangeSlider slider;
    /// <summary>
    /// 获取起始时间
    /// </summary>
    [ContextMenu("GetStartTime")]
    public DateTime GetStartTime()
    {
        //DateTime starttime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        DateTime starttime = Convert.ToDateTime(dayTxt.text);
        starttime = starttime.AddHours(slider.ValueMin);

        //slider.ValueMin
        return starttime;
    }

    /// <summary>
    /// 获取结束时间
    /// </summary>
    public DateTime GetEndTime()
    {
        //DateTime endtime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        DateTime endtime = Convert.ToDateTime(dayTxt.text);
        endtime = endtime.AddHours(slider.ValueMax);
        //slider.ValueMax
        return endtime;
    }

    /// <summary>
    /// 时间范围改变
    /// </summary>
    public void Calendar_onDayClick(DateTime dateTime)
    {
        RangeSliderChanged(slider.ValueMin, slider.ValueMax);
    }

    /// <summary>
    /// 时间范围改变
    /// </summary>
    public void RangeSliderChanged(int min, int max)
    {
        int s = max - min;
        string timestr = s.ToString();
        if (s < 9)
        {
            timestr = "0" + s + ":00:00";
        }
        else
        {
            timestr = s + ":00:00";
        }
        RefleshProcessSlider(timestr);
        isLoadDataSuccessed = false;
    }


    #endregion

    #region 播放按钮
    //播放按钮
    public Button playBtn;
    //播放进度条
    public Slider processSlider;
    //进度当前时间
    public Text processCurrentTime;
    //进度结束时间
    public Text processEndTime;
    //播放按钮相关图片信息
    public HistoryPlayUI_ButtonEntity playButtonEty;
    //暂停按钮相关图片信息
    public HistoryPlayUI_ButtonEntity pauseButtonEty;

    /// <summary>
    /// 播放按钮触发事件
    /// </summary>
    public void PlayBtn_OnClick()
    {
        Play();

    }

    /// <summary>
    /// 改变按钮图片
    /// </summary>
    public void ChangeButtonSprite()
    {
        if (isPlay)
        {
            playBtn.image.sprite = pauseButtonEty.sprite;
            SetButtonHighlightSprite(playBtn, pauseButtonEty.highlightedSprite);
        }
        else
        {
            playBtn.image.sprite = playButtonEty.sprite;
            SetButtonHighlightSprite(playBtn, playButtonEty.highlightedSprite);
        }
    }

    private void SetProcessCurrentTime(float timeT)
    {
        DateTime currentTime = GetStartTime().AddSeconds(timeT);
        string currentTimestr1 = currentTime.AddHours(-slider.ValueMin).ToString("HH:mm:ss");
        string currentTimestr2 = currentTime.ToString("HH:mm:ss");
        string currentTimestr = currentTimestr1 + "(" + currentTimestr2 + ")";
        SetProcessCurrentTime(currentTimestr);
    }

    /// <summary>
    /// 设置进度条结束时间
    /// </summary>
    public void SetProcessCurrentTime(string timestr)
    {
        processCurrentTime.text = timestr;
    }

    /// <summary>
    /// 设置进度条当前执行时间
    /// </summary>
    public void SetProcessEndTime(string timestr)
    {
        processEndTime.text = "/" + timestr;
    }

    /// <summary>
    /// 刷新时间进度条
    /// </summary>
    public void RefleshProcessSlider(string timeEndstr)
    {
        SetProcessEndTime(timeEndstr);
        processSlider.value = 0;
        SetProcessCurrentTime("00:00:00");
        if (isPlay)
        {
            //如果在播放就让它暂停
            ExecuteEvents.Execute<IPointerClickHandler>(playBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        ClearHistoryPath();
        SetIsStop(true);
    }
    /// <summary>
    /// 设置进度条值改变
    /// </summary>
    public void SetProcessSliderValue(float v)
    {
        processSlider.value = v;
    }

    public void ProcessSlider_ValueChanged(float v)
    {
        if (IsMouseDragSlider)
        {
            //print("ProcessSlider_ValueChanged");
        }
    }

    #endregion

    /// <summary>
    /// 设置按钮的高亮图片
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="pauseHighligterSprite"></param>
    private void SetButtonHighlightSprite(Button btn, Sprite pauseHighligterSprite)
    {
        SpriteState ss = new SpriteState();
        ss.disabledSprite = btn.spriteState.disabledSprite;
        ss.highlightedSprite = pauseHighligterSprite;
        ss.pressedSprite = btn.spriteState.pressedSprite;
        btn.spriteState = ss;
    }

    #region 倍数按钮
    //速率按钮
    public Button rateBtn;
    //相关倍数按钮的信息列表
    public List<HistoryPlayUI_SpeedButton> rateButtonEtys;
    [HideInInspector]
    public int rateIndex = 0;//当前倍数按钮索引

    /// <summary>
    /// 速率按钮触发事件
    /// </summary>
    public void RateBtn_OnClick()
    {
        rateIndex = rateIndex + 1;
        if (rateIndex >= rateButtonEtys.Count)
        {
            rateIndex = 0;
        }
        rateBtn.image.sprite = rateButtonEtys[rateIndex].sprite;
        CurrentSpeed = rateButtonEtys[rateIndex].speed;
        SetButtonHighlightSprite(rateBtn, rateButtonEtys[rateIndex].highlightedSprite);
    }

    #endregion

    /// <summary>
    /// 设置isStop的值
    /// </summary>
    public void SetIsStop(bool isBool)
    {
        isStop = isBool;
        processSlider.interactable = !isBool;
    }
}
/// <summary>
/// 按钮的相关信息
/// </summary>
[Serializable]
public class HistoryPlayUI_ButtonEntity
{
    public Sprite sprite;
    public Sprite highlightedSprite;
}

/// <summary>
/// 按钮的相关信息
/// </summary>
[Serializable]
public class HistoryPlayUI_SpeedButton
{
    public int speed = 1;//速度
    public Sprite sprite;
    public Sprite highlightedSprite;
}
