using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultHistoryPlayUI : MonoBehaviour
{
    //public enum Mode { Normal, Drawing }

    public static MultHistoryPlayUI Instance;
    private bool isShowHistoryPathMode = true;//是否显示历史轨迹模式
    public HistoryMode mode = HistoryMode.Normal;//是否显示历史轨迹模式
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    ///// <summary>
    ///// 新的人员执行历史轨迹的方法，目前处于测试阶段
    ///// </summary>
    //public bool isNewWalkPath;
    //日期
    public Text dayTxt;
    //关闭按钮
    public Button closeBtn;
    //是否播放
    public bool isPlay;
    //是否停止（不是暂停）
    public bool isStop;
    //停止按钮
    public Button StopBtn;

    public CalendarChange calendar;

    [HideInInspector]
    public float timeStart;    //时间起始播放值
    public DateTime datetimeStart;    //时间起始播放值
    public double timeLength;    //播放时间,单位秒
    public double timeSum;//时间和

    [System.NonSerialized]
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

    private float progressbarLoadValue = 0;//数据加载进度条

    [System.NonSerialized]
    Dictionary<Personnel, List<Position>> personnel_Points;

    public Button modeSwithBtn;//轨迹模式转换按钮
    public Text modeName;//轨迹模式名称

    // Use this for initialization
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseBtn_OnClick);
        playBtn.onClick.AddListener(PlayBtn_OnClick);
        playBtn.onClick.AddListener(ChangeButtonSprite);
        StopBtn.onClick.AddListener(Stop);
        rateBtn.onClick.AddListener(RateBtn_OnClick);
        calendar.onDayClick.AddListener(Calendar_onDayClick);

        slider.OnValuesChange.AddListener(RangeSliderChanged);
        processSlider.onValueChanged.AddListener(ProcessSlider_ValueChanged);

        if (processHistoryPlaySlider == null)
        {
            processHistoryPlaySlider = processSlider.GetComponent<HistoryPlaySlider>();
        }
        processHistoryPlaySlider.onPointerDown = ProcessSliderHandle_onPointerDown;
        processHistoryPlaySlider.onPointerUp = ProcessSliderHandle_onPointerUp;

        editPersonBtn.onClick.AddListener(EditPersonBtn_OnClick);

        if(modeSwithBtn!=null)
            modeSwithBtn.onClick.AddListener(Mode_Changed);

        //EventTriggerListener lis = EventTriggerListener.Get(ProcessSliderHandle.gameObject);
        //lis.onBeginDrag = ProcessSliderHandle_OnBeginDrag;
        //lis.onEndDrag = ProcessSliderHandle_OnEndDrag;
    }

    float m;

    // Update is called once per frame
    void Update()
    {
        ////UGUI类似按钮的控件才能被选中
        //if (EventSystem.current.currentSelectedGameObject != null && Input.GetMouseButton(0))
        //{
        //    //print("currentSelectedGameObject:" + EventSystem.current.currentSelectedGameObject.name);
        //    if (!isStop)
        //    {
        //        Slider selectSlider = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Slider>();
        //        if (selectSlider == processSlider)
        //        {
        //            SetIsMouseDragSlider(true);
        //        }
        //        else
        //        {
        //            SetIsMouseDragSlider(false);
        //        }
        //    }
        //}
        //else
        //{
        //    SetIsMouseDragSlider(false);
        //}

        m += Time.deltaTime;
        //if (isPlay && isLoadDataSuccessed)
        if (!isStop && isLoadDataSuccessed)
        {
            //if (!IsMouseDragSlider)
            if (isPlay && !IsMouseDragSlider)
            {
                //float timeT = Time.time - timeStart;
                timeSum += Time.deltaTime * CurrentSpeed;
                float valueT = (float)(timeSum / timeLength);
                SetProcessSliderValue(valueT);
                SetProcessCurrentTime((float)timeSum);
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
                SetProcessCurrentTime((float)timeSum);
                SetHistoryPath();
            }
        }


    }


    private void SetIsMouseDragSlider(bool b)
    {
        if (IsMouseDragSlider != b)
        {
            IsMouseDragSlider = b;

            if (!IsMouseDragSlider)
            {
                SetHistoryLineDrawing(!IsMouseDragSlider, true);//拖动进度条后，需要新创建一条轨迹绘制线，防止前一个点和后一个点连在一起
            }
            else
            {
                SetHistoryLineDrawing(!IsMouseDragSlider);
            }

        }
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    private void SetHistoryPath()
    {
        LocationHistoryManager.Instance.SetHistoryPath_M(processSlider.value);
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
    public void ShowT()
    {
        FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
        PersonnelTreeManage.Instance.CloseWindow();
        SmallMapController.Instance.Hide();
        LocationManager.Instance.HideLocation();
        StartOutManage.Instance.Hide();
        LocationManager.Instance.RecoverBeforeFocusAlign();
        FactoryDepManager.Instance.SetAllColliderIgnoreRaycastOP(true);
        SetWindowActive(true);
    }

    /// <summary>
    /// 展示
    /// </summary>
    /// <param name="personnelT"></param>
    public void Show(Personnel personnelT)
    {
        SetIsStop(true);
        Init(personnelT);
        //ShowBaseInfo(personnelT.Name, personnelT.Pst.Name);
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
            FunctionSwitchBarManage.Instance.SetlightToggle(true);
            Stop();
            SetWindowActive(false);
            PersonnelTreeManage.Instance.ShowWindow();
            SmallMapController.Instance.Show();
            StartOutManage.Instance.Show();
            LocationManager.Instance.ShowLocation();
            //ControlMenuController.Instance.ShowLocation();
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(false);
        }
    }

    public void CloseBtn_OnClick()
    {
        PersonSubsystemManage.Instance.SetMultHistoryToggle(false);
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


        SetHistoryLineDrawing(isPlay);

    }

    /// <summary>
    /// 是否开启历史轨迹实时绘制
    /// </summary>
    public void SetHistoryLineDrawing(bool isDrawing, bool isAddLine = false)
    {
        //foreach (LocationHistoryPath_M h in LocationHistoryManager.Instance.historyPath_Ms)
        //{
        //    if (isDrawing)
        //    {
        //        if (mode == Mode.Drawing)
        //        {
        //            h.historyPathDrawing.Drawing();
        //            if (isAddLine)
        //            {
        //                h.historyPathDrawing.AddLine();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        h.historyPathDrawing.PauseDraw();
        //    }
        //}

        LocationHistoryManager.Instance.PathList.SetHistoryLineDrawing(mode, isDrawing, isAddLine);
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
        SetIsStop(true);
    }

    public bool IsLineTest = false;

    [ContextMenu("SetIsLineTest")]
    public void SetIsLineTest()
    {
        IsLineTest = !IsLineTest;
    }

    Position firstPoint = null;//第一个点的位置 自动移动进度条到该点

    /// <summary>
    /// 显示某个标签的历史数据
    /// </summary>
    /// <param name="code"></param>
    public void ShowHistoryData()
    {
        if (isLoadDataSuccessed) return;
        //LocationManager.Instance.ClearHistoryPaths();
        //string code = "0002";

        List<HistoryPersonUIItem> historyPersonUIItems = personsGrid.GetComponentsInChildren<HistoryPersonUIItem>().ToList();

        DateTime end = GetEndTime();
        DateTime start = GetStartTime();
        List<List<Position>> psList = new List<List<Position>>();
        personnel_Points = new Dictionary<Personnel, List<Position>>();
        //List<Position> ps = new List<Position>();
        List<LocationHistoryPath_M> paths = new List<LocationHistoryPath_M>();

        progressbarLoadValue = 0;

        List<int> topoNodeIds = RoomFactory.Instance.GetCurrentDepNodeChildNodeIds(SceneEvents.DepNode);

        Loom.StartSingleThread(() =>
        {
            firstPoint = null;
            foreach (Personnel p in currentPersonnels)
            {
                List<Position> ps = GetHistoryData(p.Id, topoNodeIds, start, end, 1440f);
                psList.Add(ps);
                if (personnel_Points.ContainsKey(p))
                {
                    personnel_Points[p] = ps;
                }
                else
                {
                    personnel_Points.Add(p, ps);
                }
                if (ps != null && ps.Count > 0)
                {
                    Position fps = ps[0];
                    if (firstPoint == null)
                    {
                        firstPoint = fps;
                    }
                    else
                    {
                        if (fps.Time < firstPoint.Time)
                        {
                            firstPoint = fps;
                        }
                    }
                }

            }
            Debug.Log("StartSingleThread1");
            Loom.DispatchToMainThread(() =>
            {
                ProgressbarLoad.Instance.Show(1);
                ProgressbarLoad.Instance.Hide();
                int k = 0;
                foreach (Personnel p in personnel_Points.Keys)
                {
                    List<Position> ps = personnel_Points[p];
                    Debug.LogError("点数：" + ps.Count);
                    if (ps.Count < 2) continue;
                    var posInfoList = new PositionInfoList();
                    for (int i = 0; i < ps.Count; i++)
                    {
                        var posInfo = new PositionInfo(ps[i], start);
                        posInfoList.Add(posInfo);
                    }

                    Color colorT = colors[k % colors.Count];
                    HistoryPersonUIItem item = historyPersonUIItems.Find((i) => i.personnel.Id == p.Id);
                    if (item != null)
                    {
                        colorT = item.color;
                    }

                    PathInfo pathInfo = new PathInfo();
                    pathInfo.personnelT = p;
                    pathInfo.color = colorT;
                    pathInfo.posList = posInfoList;
                    pathInfo.timeLength = timeLength;

                    LocationHistoryPath_M histoyObj = LocationHistoryManager.Instance.ShowLocationHistoryPath_M(pathInfo);
                    //histoyObj.InitData(timeLength, timelist);
                    HistoryManController historyManController = histoyObj.gameObject.AddComponent<HistoryManController>();
                    histoyObj.historyManController = historyManController;
                    historyManController.Init(colorT, histoyObj);
                    PersonAnimationController personAnimationController = histoyObj.gameObject.GetComponent<PersonAnimationController>();
                    personAnimationController.DoMove();
                    Debug.Log("StartSingleThread2");
                    k++;
                }
            });

            //Debug.Log("StartSingleThread3");
            Loom.DispatchToMainThread(() =>
            {
                isLoadDataSuccessed = true;
                //timeStart = Time.time;
                timeSum = 0;
                Debug.Log("StartSingleThread3");
                if (firstPoint != null)
                {
                    DateTime t = LocationManager.GetTimestampToDateTime(firstPoint.Time);
                    Debug.Log(firstPoint.Time);
                    timeSum = t.Hour * 3600 + t.Minute * 60 + t.Second - slider.ValueMin * 3600;
                    Debug.Log(timeSum);
                }
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
        //float valueT = 0;
        float sum = 0;
        while (sum < counts)
        {
            DateTime startTemp;
            DateTime endTemp;
            if (sum + 1 <= counts)
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = startT.AddMinutes(intervalMinute * (sum + 1));
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByTime(code, startTemp, endTemp);
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
            float valueT = 1f / counts;
            progressbarLoadValue += valueT / currentPersonnels.Count;
            //print("valueT:" + valueT);
            Loom.DispatchToMainThread(() =>
            {
                ProgressbarLoad.Instance.Show(progressbarLoadValue);
            });
        }
        //Loom.DispatchToMainThread(() =>
        //{
        //    ProgressbarLoad.Instance.Hide();
        //});
        return ps;
    }

    /// <summary>
    /// 获取人员历史轨迹数据根据人员信息
    /// </summary>
    public List<Position> GetPositionsByPersonnel(Personnel p)
    {
        if (personnel_Points.ContainsKey(p))
        {
            return personnel_Points[p];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 清空历史轨迹
    /// </summary>
    public void ClearHistoryPath()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths_M();
    }


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

    private HistoryPlaySlider processHistoryPlaySlider;
    ////播放进度条的滑块
    //public Image ProcessSliderHandle;
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
        if (currentPersonnels == null || currentPersonnels.Count == 0)
        {
            UGUIMessageBox.Show("请先添加人员！",
                () =>
                {
                    EditPersonBtn_OnClick();
                }, null);
        }
        else
        {
            Play();
        }
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
        print("processSlider前:" + v);
        processSlider.value = v;
        print("processSlider后:" + processSlider.value);
    }

    public void ProcessSlider_ValueChanged(float v)
    {
        if (IsMouseDragSlider)
        {
            //print("ProcessSlider_ValueChanged");
        }
    }


    public void ProcessSliderHandle_onPointerDown()
    {
        Debug.LogError("ProcessSliderHandle_onPointerDown!");


        SetIsMouseDragSlider(true);
    }

    public void ProcessSliderHandle_onPointerUp()
    {
        Debug.LogError("ProcessSliderHandle_onPointerUp!");
        SetIsMouseDragSlider(false);
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

        LocationHistoryManager.Instance.SetRateChanged(true);
    }

    #endregion

    #region 播放历史轨迹界面的人员列表

    public Button editPersonBtn;//编辑人员按钮
    public HistoryPersonUIItem PersonItemPrefab;//播放历史轨迹界面的人员列表
    public VerticalLayoutGroup personsGrid;
    public List<Color> colors = new List<Color>() { Color.red, Color.green, Color.blue, Color.yellow };
    public int limitPersonNum = 4;//限制显示历史轨迹人员数量
    [System.NonSerialized]
    public List<Personnel> currentPersonnels;//当前显示历史轨迹的人员信息
    public List<HistoryPersonUIItem> items;

    /// <summary>
    /// 添加按钮触发事件
    /// </summary>
    public void EditPersonBtn_OnClick()
    {
        if (currentPersonnels == null)
        {
            currentPersonnels = new List<Personnel>();
        }
        List<Personnel> currentPersonnelsT = new List<Personnel>(currentPersonnels);
        HistoryPersonsSearchUI.Instance.Show(currentPersonnelsT);
        if (isPlay)
        {
            //如果在播放就让它暂停
            ExecuteEvents.Execute<IPointerClickHandler>(playBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }

    public void ShowPersons(List<Personnel> personnelsT)
    {
        //如果在播放就让它终止
        ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        CreatePersons(personnelsT);
        items = new List<HistoryPersonUIItem>(personsGrid.GetComponentsInChildren<HistoryPersonUIItem>());
    }

    public void CreatePersons(List<Personnel> personnelsT)
    {
        ClearPersons();
        currentPersonnels = personnelsT;
        for (int i = 0; i < personnelsT.Count; i++)
        {
            int j = i / colors.Count;
            CreatePersonItem(personnelsT[i], colors[i]);
        }
    }

    /// <summary>
    /// 创建人员列表
    /// </summary>
    public HistoryPersonUIItem CreatePersonItem(Personnel personnelT, Color colorT)
    {
        HistoryPersonUIItem item = Instantiate(PersonItemPrefab);
        item.Init(personnelT, colorT);
        item.transform.SetParent(personsGrid.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
        return item;
    }

    /// <summary>
    /// 清除显示历史轨迹的人员
    /// </summary>
    public void ClearPersons()
    {
        currentPersonnels.Clear();
        int n = personsGrid.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            DestroyImmediate(personsGrid.transform.GetChild(i).gameObject);
        }

    }

    /// <summary>
    /// 移除显示轨迹中的某个人员
    /// </summary>
    /// <param name="personnelT"></param>
    public void RemovePerson(Personnel personnelT)
    {
        currentPersonnels.Remove(personnelT);
        //List<HistoryPersonUIItem> pList = new List<HistoryPersonUIItem>(personsGrid.GetComponentsInChildren<HistoryPersonUIItem>());
        //HistoryPersonUIItem item = pList.Find((i) => i.personnel == personnelT);
        HistoryPersonUIItem item = items.Find((i) => i.personnel == personnelT);
        DestroyImmediate(item.gameObject);
        items.Remove(item);
    }

    /// <summary>
    /// 设置人员历史所在区域
    /// </summary>
    public void SetItemArea(Personnel p, string areaStr)
    {
        HistoryPersonUIItem item = items.Find((i) => i.personnel == p);
        if (item != null)
        {
            item.RefleshTxtPlace(areaStr);
        }
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

    /// <summary>
    /// 模式切换
    /// </summary>
    public void Mode_Changed()
    {
        Stop();
        isShowHistoryPathMode = !isShowHistoryPathMode;
        if (isShowHistoryPathMode)
        {
            SwithMode(HistoryMode.Normal);
            modeName.text = "轨道模式";
            SetHistoryLineDrawing(false);
        }
        else
        {
            SwithMode(HistoryMode.Drawing);
            modeName.text = "涎线模式";
            SetHistoryLineDrawing(true, true);
        }
    }

    /// <summary>
    /// 切换轨迹模式
    /// </summary>
    public void SwithMode(HistoryMode modeT)
    {
        mode = modeT;
    }
}
