using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultHistoryPlayUINew : MonoBehaviour
{
    public class PersonPointDict: Dictionary<Personnel, PositionInfoList>
    {
        public void AddPerson(Personnel p, PositionInfoList ps)
        {
            try
            {
                if (ps == null)
                {
                    ps = new PositionInfoList();
                }
                if (this.ContainsKey(p))
                {
                    this[p] = ps;
                }
                else
                {
                    this.Add(p, ps);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("PersonPointDict.AddPerson:"+ ex.ToString());
            }
        }

        public int GetTotalCount()
        {
            int totalCount = 0;
            foreach (Personnel p in this.Keys)
            {
                PositionInfoList ps = this[p];
                if(ps!=null)
                    totalCount += ps.Count;
            }
            return totalCount;
        }
    }

    public bool istest;

    public static MultHistoryPlayUINew Instance;
    private bool isShowHistoryPathMode = true;//是否显示历史轨迹模式
    public HistoryMode mode = HistoryMode.Normal;//是否显示历史轨迹模式
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    /// <summary>
    /// 播放器窗口
    /// </summary>
    public MultHistoryProgressWindow progressWindow;
    ///// <summary>
    ///// 新的人员执行历史轨迹的方法，目前处于测试阶段
    ///// </summary>
    //public bool isNewWalkPath;
    //日期
    public Text dayTxt;
    //关闭按钮
    public Button exitBtn;
    //历史轨迹聚焦人员时,返回视角按钮
    public Button backViewBtn;
    //是否播放
    public bool isPlay;
    //是否停止（不是暂停）
    public bool isStop;
    //停止按钮
    //public Button StopBtn;

    public CalendarChange calendar;

    [HideInInspector]
    public float timeStart;    //时间起始播放值
    public DateTime datetimeStart;    //时间起始播放值
    public double timeLength;    //播放时间,单位秒
    public double timeSum;//时间和
    public Button LocationBut;//定位统计

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

    [HideInInspector]
    private PersonPointDict ppDict = new PersonPointDict();

    //public Button modeSwithBtn;//轨迹模式转换按钮
    //public Text modeName;//轨迹模式名称

    public Button loadbtn;//加载按钮

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Personnel_HistoryPlayBar = new Dictionary<Personnel, HistoryPlayBar>();

        exitBtn.onClick.AddListener(CloseBtn_OnClick);
        playBtn.onClick.AddListener(PlayBtn_OnClick);
        playBtn.onClick.AddListener(ChangeButtonSprite);
        //StopBtn.onClick.AddListener(Stop);

        rateDropdown.onValueChanged.AddListener(RateDropdown_ValueChanged);
        //rateBtn.onClick.AddListener(RateBtn_OnClick);
        calendar.onDayClick.AddListener(Calendar_onDayClick);

        //slider.OnValuesChange.AddListener(RangeSliderChanged);
        processSlider.onValueChanged.AddListener(ProcessSlider_ValueChanged);

        if (processHistoryPlaySlider == null)
        {
            processHistoryPlaySlider = processSlider.GetComponent<HistoryPlaySlider>();
        }
        processHistoryPlaySlider.onPointerDown = ProcessSliderHandle_onPointerDown;
        processHistoryPlaySlider.onPointerUp = ProcessSliderHandle_onPointerUp;

        editPersonBtn.onClick.AddListener(EditPersonBtn_OnClick);

        //modeSwithBtn.onClick.AddListener(Mode_Changed);

        //EventTriggerListener lis = EventTriggerListener.Get(ProcessSliderHandle.gameObject);
        //lis.onBeginDrag = ProcessSliderHandle_OnBeginDrag;
        //lis.onEndDrag = ProcessSliderHandle_OnEndDrag;

        normalModeToggle.onValueChanged.AddListener(NormalModeToggle_ValueChanged);
        drawingModeToggle.onValueChanged.AddListener(DrawingModeToggle_ValueChanged);
        personDropdown.onValueChanged.AddListener(PersonDropdown_ValueChanged);
        hourDropdown.onValueChanged.AddListener(HourDropdown_ValueChanged);
        minuteDropdown.onValueChanged.AddListener(MinuteDropdown_ValueChanged);
        durationDropdown.onValueChanged.AddListener(DurationDropdown_ValueChanged);

        loadbtn.onClick.AddListener(Loadbtn_OnClick);
        backViewBtn.onClick.AddListener(BackView_On_Click);

        personDropdown.ClearOptions();
        ClearPersons();
        InitDropdown();
        InitRateDropdown();
        if (LocationBut != null)
        {
            LocationBut.onClick.AddListener(ShowHistoricalPathStatistics);
        }
        else
        {
            Log.Error("MultHistoryPlayUINew.LocationBut == null");
        }
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
                    LocationHistoryManager.Instance.RecoverBeforeFocusAlign();
                    //ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    //Stop();
                    //RefleshProcessSlider("00:00:00",false);
                    RefleshProcessSlider(false);
                }
            }
            else
            {
                if (IsMouseDragSlider)
                {
                    //timeSum = timeLength * processSlider.value;
                    ////print(string.Format("HistoryPlayUI进度时间:{0},进度值:{1}", timeSum, processSlider.value));
                    //SetProcessCurrentTime((float)timeSum);
                    //SetHistoryPath();
                    MouseDragSlider(processSlider.value);
                }
            }
        }


    }

    /// <summary>
    /// 设置当前执行时间
    /// </summary>
    public void SetTimeSum(double timeSumT)
    {
        timeSum = timeSumT;
    }

    /// <summary>
    /// 鼠标拖动进度条
    /// </summary>
    public void MouseDragSlider(float v)
    {
        timeSum = timeLength * v;
        //print(string.Format("HistoryPlayUI进度时间:{0},进度值:{1}", timeSum, processSlider.value));
        SetProcessCurrentTime((float)timeSum);
        SetHistoryPath();
    }
    /// <summary>
    /// 展示历史统计界面（蔡露写的）
    /// </summary>
    public void ShowHistoricalPathStatistics()
    {
        HistoricalPathStatisticsManage.Instance.ShowHistoricalPathStatisticsWindow();
        HistoricalPathStatisticsManage.Instance.OpenHistoricalPathStatisticsInfo();
    }
    public void SetIsMouseDragSlider(bool b)
    {
        if (IsMouseDragSlider != b)
        {
            IsMouseDragSlider = b;

            if (mode == HistoryMode.Drawing)
            {
                if (!IsMouseDragSlider)
                {
                    if (isPlay)
                    {
                        SetHistoryLineDrawing(!IsMouseDragSlider, true);//拖动进度条后，需要新创建一条轨迹绘制线，防止前一个点和后一个点连在一起
                    }
                }
                else
                {
                    SetHistoryLineDrawing(!IsMouseDragSlider, true);
                }
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

    //[ContextMenu("On_Click1")]
    //public void On_Click1()
    //{
    //    ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    //}
    //[ContextMenu("On_Click2")]
    //public void On_Click2()
    //{
    //    //按钮点击的变色
    //    ExecuteEvents.Execute<ISubmitHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
    //}

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

        //FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
        PersonnelTreeManage.Instance.CloseWindow();
        SmallMapController.Instance.Hide();
        LocationManager.Instance.HideLocation();
        //LocationManager.Instance.HideAndClearLocation();
        StartOutManage.Instance.Hide();
        LocationManager.Instance.RecoverBeforeFocusAlign();
        //FactoryDepManager.Instance.GetColliders();
        FactoryDepManager.Instance.SetAllColliderIgnoreRaycastOP(true);
        SetWindowActive(true);
        BindTogglesCall();
        SetIsStop(false);
        if (currentPersonnels.Count == 0)//如果打开没有选中的人员就弹出人员选择界面
        {
            if(AutoShowSelectPersonWindow)
                ClickEditPersonButton();
        }
        processSlider.interactable = false;
    }

    public bool AutoShowSelectPersonWindow = true;

    private void ClickEditPersonButton()
    {
        ExecuteEvents.Execute<IPointerClickHandler>(editPersonBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
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
        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Hide()
    {
        if (window.activeInHierarchy)
        {
            LocationHistoryManager.Instance.RecoverBeforeFocusAlign();
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            FunctionSwitchBarManage.Instance.SetlightToggle(true);
            Stop();
            SetWindowActive(false);
            AlarmPushManage.Instance.CloseAlarmPushWindow(true );
            PersonnelTreeManage.Instance.ShowWindow();
            SmallMapController.Instance.Show();
            StartOutManage.Instance.Show();
            LocationManager.Instance.ShowLocation();
            //ControlMenuController.Instance.ShowLocation();
            //LocationHistoryManager.Instance.RecoverBeforeFocusAlign();
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(false);
        }
    }

    public void CloseBtn_OnClick()
    {
        PersonSubsystemManage.Instance.SetMultHistoryToggle(false);
        UGUIFollowManage.Instance.RemoveGroupUIbyName("LocationNameUI");
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
        //if (isPlay)
        //{
        //    //timeLength = (slider.ValueMax - slider.ValueMin) * 3600;
        //    timeLength = GetTimeLengthForSeconds();
        //    ShowHistoryData();

        //    FunctionSwitchBarManage.Instance.SetlightToggle(false);
        //}
        //FunctionSwitchBarManage.Instance.SetlightToggle(false);

        SetHistoryLineDrawing(isPlay);

    }

    /// <summary>
    /// 加载历史数据
    /// </summary>
    public void LoadData()
    {
        Log.Info("MultiHisotryPlayUINew.LoadData Start");
        GetStartTimeOP();
        GetEndTimeOP();
        BackView_On_Click();
        //isLoadDataSuccessed = true;
        Stop();
        //SetIsStop(false);
        //isPlay = !isPlay;
        if (isPlay)
        {
            //如果在播放就让它暂停
            ClickPlayButton();
        }
        timeLength = GetTimeLengthForSeconds();//获取时间长度 value=0=>1小时=>3600s

        ShowHistoryData();//显示历史轨迹，里面通过创建LocationHistoryPath_M具体显示轨迹

        FunctionSwitchBarManage.Instance.SetlightToggle(false);
        FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
        lightToggle.SetState(false);//关闭灯光

        SetHistoryLineDrawing(isPlay);//是否开启历史轨迹实时绘制
        SetProcessEndTime(GetTimeLengthForHours());
        isPlay = false;
        playBtn.interactable = true;

        ClickPlayButton();//=>PlayBtn_OnClick()=>Play()
        Log.Info("MultiHisotryPlayUINew.LoadData End");
    }

    private void ClickPlayButton()
    {
        ExecuteEvents.Execute<IPointerClickHandler>(playBtn.gameObject, new PointerEventData(EventSystem.current),
            ExecuteEvents.pointerClickHandler);
        //触发PlayBtn_OnClick和ChangeButtonSprite
    }


    /// <summary>
    /// 是否开启历史轨迹实时绘制
    /// </summary>
    public void SetHistoryLineDrawing(bool isDrawing, bool isAddLine = false)
    {
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
        ClearHistoryPlayBars();
        //RangeSliderChanged(slider.ValueMin, slider.ValueMax);
        RefleshProcessSlider("00:00:00");
        //ClearHistoryPath();
        isLoadDataSuccessed = false;
        timeSum = 0;
        //SetIsStop(true);
        processSlider.interactable = false;
        playBtn.interactable = false;
        FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
        FunctionSwitchBarManage.Instance.SetlightToggle(true);

        BackView_On_Click();//返回视角
    }

    public bool IsLineTest = false;

    [ContextMenu("SetIsLineTest")]
    public void SetIsLineTest()
    {
        IsLineTest = !IsLineTest;
    }

    Position firstPoint = null;//第一个点的位置 自动移动进度条到该点
    List<List<Position>> psList;
    List<LocationHistoryPath_M> paths;

    private void ClearData()
    {
        Personnel_HistoryPlayBar.Clear();
        personDropdown.ClearOptions();
        personDropdown.AddOptions(new List<string>() { "全部" });
        ClearHistoryPlayBars();
        if (psList == null)
        {
            psList = new List<List<Position>>();
        }
        else
        {
            psList.Clear();
        }
        if (ppDict == null)
        {
            ppDict = new PersonPointDict();
        }
        else
        {
            ppDict.Clear();
        }
        //List<Position> ps = new List<Position>();
        if (paths == null)
        {
            paths = new List<LocationHistoryPath_M>();
        }
        else
        {
            paths.Clear();
        }
        progressbarLoadValue = 0;
    }

    /// <summary>
    /// 计算所有点的navMesh的坐标
    /// </summary>
    private void CalculateNavMeshPoint()
    {
        var tmp = NavMeshHelper.IsDebug;//保存
        NavMeshHelper.IsDebug = false;//不创建调试用的点
        foreach (KeyValuePair<Personnel, PositionInfoList> keyValuePair in ppDict)
        {
            foreach (PositionInfo info in keyValuePair.Value)
            {
                var navMeshPos = NavMeshHelper.GetClosetPoint(info.Vec,null);
                info.NavPos = navMeshPos;
            }
        }

        NavMeshHelper.IsDebug = tmp;//还原
    }

    /// <summary>
    /// 显示历史轨迹
    /// </summary>
    /// <param name="code"></param>
    public void ShowHistoryData()
    {
        if (isLoadDataSuccessed) return;
        //LocationManager.Instance.ClearHistoryPaths();
        //string code = "0002";
        ClearData();//清理数据
        List<HistoryPersonUIItem> historyPersonUIItems = personsGrid.GetComponentsInChildren<HistoryPersonUIItem>().ToList();      
        ProgressbarLoad.Instance.Show(progressbarLoadValue);
        List<int> topoNodeIds = RoomFactory.Instance.GetCurrentDepNodeChildNodeIds(SceneEvents.DepNode);

        Loom.StartSingleThread(() =>
        {
            Log.Info("ShowHistoryData.StartSingleThread GetHistoryData Start");
            GetHistoryData(topoNodeIds);//获取历史轨迹数据
            Log.Info("ShowHistoryData.StartSingleThread GetHistoryData End");

            //Log.Info("ShowHistoryData.StartSingleThread CalculateNavMeshPoint Start");
            //CalculateNavMeshPoint();//计算所有点的navMesh的坐标
            //Log.Info("ShowHistoryData.StartSingleThread CalculateNavMeshPoint End");
            //问题1.无法在子线程中使用

            Loom.DispatchToMainThread(() =>
            {
                //Log.Info("ShowHistoryData.StartSingleThread CalculateNavMeshPoint Start");
                //CalculateNavMeshPoint();//计算所有点的navMesh的坐标
                //Log.Info("ShowHistoryData.StartSingleThread CalculateNavMeshPoint End");
                //问题2.计算时间太久 5k多点要8分钟。

                Log.Info("ShowHistoryData.DispatchToMainThread ShowHistoryData Start");
                if (ShowHistoryData(historyPersonUIItems))//显示历史轨迹
                {
                    Loom.DispatchToMainThread(() =>
                    {
                        Log.Info("ShowHistoryData.DispatchToMainThread SetLoadDataSuccess");
                        SetLoadDataSuccess();
                    });
                }
                Log.Info("ShowHistoryData.DispatchToMainThread ShowHistoryData End");
            });
        });
    }

    private void SetLoadDataSuccess()
    {
        isLoadDataSuccessed = true;
        //timeStart = Time.time;
        timeSum = 0;
        if (firstPoint != null)
        {
            DateTime t = LocationManager.GetTimestampToDateTime(firstPoint.Time);
            //timeSum = t.Hour * 3600 + t.Minute * 60 + t.Second - slider.ValueMin * 3600;
            timeSum = (t - GetStartTime()).TotalSeconds;
            Debug.Log(string.Format("firstPoint:{0},TotalSeconds:{1}s", t, timeSum));
        }
    }

    private bool ShowHistoryData(List<HistoryPersonUIItem> historyPersonUIItems)
    {
        Log.Info("ShowHistoryData Start");
        ProgressbarLoad.Instance.Show(1);
        ProgressbarLoad.Instance.Hide();
        int k = 0;

        int totalCount = ppDict.GetTotalCount();
        Log.Info("ShowHistoryData totalCount：" + totalCount);
        if (totalCount == 0)
        {
            UGUIMessageBox.Show("当前查询条件下无历史轨迹信息！");
            return false;
        }
        foreach (Personnel p in ppDict.Keys)
        {
            Log.Info("ShowHistoryData Person Start:"+ p.Name);
            
            HistoryPersonUIItem item = historyPersonUIItems.Find((i) => i.personnel.Id == p.Id);
            Color colorT = colors[k % colors.Count];
            if (item != null)
            {
                colorT = item.color;
            }

            PositionInfoList ps = ppDict[p];
            //PositionInfoList posInfoList = InitPositionInfoList(ps);
            ShowHistoryPersonData(p, ps, colorT);//显示一条历史轨迹
            k++;
            Log.Info("ShowHistoryData Person End");
        }
        progressWindow.CreateTipItems();
        Log.Info("ShowHistoryData End");
        return true;
    }

    private void ShowHistoryPersonData(Personnel p, PositionInfoList posInfoList, Color colorT)
    {
        //List<Position> ps = ppDict[p];
        Debug.LogError("点数：" + posInfoList.Count);
        if (posInfoList.Count < 2) return;
        //PositionInfoList posInfoList = InitPositionInfoList(ps);

        UGUIFollowManage.Instance.RemoveGroupUIbyName("LocationNameUI");

        PathInfo pathInfo = new PathInfo();
        pathInfo.personnelT = p;
        pathInfo.color = colorT;
        pathInfo.posList = posInfoList;
        pathInfo.timeLength = timeLength;

        //var histoyObj = LocationHistoryManager.Instance.ShowLocationHistoryPath_M(pathInfo);
        //LocationHistoryManager.Instance.AddHistoryPath_M(histoyObj);
        ////histoyObj.InitData(timeLength);
        //var historyManController = histoyObj.gameObject.AddComponent<HistoryManController>();
        //histoyObj.historyManController = historyManController;
        //historyManController.Init(colorT, histoyObj);
        //var personAnimationController = histoyObj.gameObject.GetComponent<PersonAnimationController>();
        //personAnimationController.DoMove();//人员行走动画

        LocationHistoryManager.Instance.CreateHistoryPath(pathInfo);//上面的代码都独立的，移动到LocationHistoryManager里面

        Debug.Log("StartSingleThread2");


        HistoryPlayBar bar = CreateHistoryPlayBar(pathInfo);
        Personnel_HistoryPlayBar.Add(p, bar);
        personDropdown.AddOptions(new List<string>() { p.Name });
    }

    private PositionInfoList InitPositionInfoList(List<Position> ps)
    {
        var posInfoList = new PositionInfoList();
        DateTime st = GetStartTime();
        for (int i = 0; i < ps.Count; i++)
        {
            var posInfo = new PositionInfo(ps[i], st);
            posInfoList.Add(posInfo);
        }

        return posInfoList;
    }

    private void GetHistoryData(List<int> topoNodeIds)
    {
        DateTime end = GetEndTime();
        DateTime start = GetStartTime();

        firstPoint = null;
        foreach (Personnel p in currentPersonnels)
        {
            try
            {
                List<Position> ps = GetHistoryData(p.Id, topoNodeIds, start, end, 1440f);//从数据库获取历史轨迹数据

                psList.Add(ps);

                PositionInfoList posInfoList = InitPositionInfoList(ps);

                ppDict.AddPerson(p, posInfoList);



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
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
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
        if (ppDict.ContainsKey(p))
        {
            return ppDict[p].PosList;
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
    //public RangeSlider slider;
    /// <summary>
    /// 获取起始时间
    /// </summary>
    [ContextMenu("GetStartTime")]
    public DateTime GetStartTime()
    {
        ////DateTime starttime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        //DateTime starttime = Convert.ToDateTime(dayTxt.text);
        //starttime = starttime.AddHours(GetStartHours());
        //starttime = starttime.AddMinutes(GetStartMinutes());
        ////slider.ValueMin
        return starttime;
    }

    DateTime starttime;

    private DateTime GetStartTimeOP()
    {
        //DateTime starttime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        //DateTime starttime = Convert.ToDateTime(dayTxt.text);
        starttime = Convert.ToDateTime(dayTxt.text);
        starttime = starttime.AddHours(GetStartHours());
        starttime = starttime.AddMinutes(GetStartMinutes());
        //slider.ValueMin
        return starttime;
    }

    /// <summary>
    /// 获取结束时间
    /// </summary>
    public DateTime GetEndTime()
    {
        ////DateTime endtime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        //DateTime endtime = Convert.ToDateTime(dayTxt.text);
        //int hours = GetEndHours();
        //int minutes = GetEndMinutes();
        //int timeLengthHours = GetTimeLengthForHours();
        //endtime = endtime.AddHours(hours + timeLengthHours);
        //endtime = endtime.AddMinutes(minutes);

        ////slider.ValueMax
        return endtime;
    }

    DateTime endtime;

    /// <summary>
    /// 获取结束时间
    /// </summary>
    private DateTime GetEndTimeOP()
    {
        //DateTime endtime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        //DateTime endtime = Convert.ToDateTime(dayTxt.text);
        endtime = Convert.ToDateTime(dayTxt.text);
        int hours = GetEndHours();
        int minutes = GetEndMinutes();
        int timeLengthHours = GetTimeLengthForHours();
        endtime = endtime.AddHours(hours + timeLengthHours);
        endtime = endtime.AddMinutes(minutes);

        return endtime;
    }

    /// <summary>
    /// 时间范围改变
    /// </summary>
    public void Calendar_onDayClick(DateTime dateTime)
    {
        //RangeSliderChanged(slider.ValueMin, slider.ValueMax);
        Stop();
    }

    /// <summary>
    /// 时间范围改变
    /// </summary>
    //public void RangeSliderChanged(int min, int max)
    //{
    //    int s = max - min;
    //    string timestr = s.ToString();
    //    if (s < 9)
    //    {
    //        timestr = "0" + s + ":00:00";
    //    }
    //    else
    //    {
    //        timestr = s + ":00:00";
    //    }
    //    RefleshProcessSlider(timestr);
    //    isLoadDataSuccessed = false;
    //}


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
        //if (isRefleshProcessSlider)
        //{
        //    isRefleshProcessSlider = false;
        //    return;
        //}
        //else
        //{
            if (currentPersonnels == null || currentPersonnels.Count == 0)
            {
                //UGUIMessageBox.Show("请先添加人员！",
                //    () =>
                //    {
                //        EditPersonBtn_OnClick();
                //    }, null);
            }
            else
            {
                Play();
            }

        //}
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
        //DateTime currentTime = GetStartTime().AddSeconds(timeT);
        //string currentTimestr1 = currentTime.AddHours(-GetStartHours()).ToString("HH:mm:ss");
        //string currentTimestr2 = currentTime.ToString("HH:mm:ss");
        //string currentTimestr = currentTimestr1 + "(" + currentTimestr2 + ")";

        DateTime timet = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string currentTimestr = timet.AddSeconds(timeT).ToString("HH:mm:ss"); ;
        if (currentTimestr.Contains("01:00:00"))
        {
            int i = 0;
        }
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
        processEndTime.text = timestr;
    }

    public void SetProcessEndTime(int t)
    {
        string timestr = t.ToString();
        if (t < 9)
        {
            timestr = "0" + t + ":00:00";
        }
        else
        {
            timestr = t + ":00:00";
        }
        SetProcessEndTime(timestr);
    }

    /// <summary>
    /// 刷新时间进度条
    /// </summary>
    public void RefleshProcessSlider(string timeEndstr, bool isClearHistoryPathT = true)
    {
        SetProcessEndTime(timeEndstr);
        //processSlider.value = 0;
        //SetProcessCurrentTime("00:00:00");
        //if (isPlay)
        //{
        //    //如果在播放就让它暂停
        //    ClickPlayButton();
        //}
        //if (isClearHistoryPathT)
        //{
        //    ClearHistoryPath();
        //}
        //SetIsStop(true);
        RefleshProcessSlider(isClearHistoryPathT);
    }

    private bool isRefleshProcessSlider = false;

    public void RefleshProcessSlider(bool isClearHistoryPathT = true)
    {
        processSlider.value = 0;
        SetProcessCurrentTime("00:00:00");
        if (isPlay)
        {
            //isRefleshProcessSlider = true;
            //如果在播放就让它暂停
            ClickPlayButton();
            //isRefleshProcessSlider = false;
        }
        if (isClearHistoryPathT)
        {
            ClearHistoryPath();
        }
        if (!isLoadDataSuccessed)
        {
            SetIsStop(true);
        }
    }
    /// <summary>
    /// 设置进度条值改变
    /// </summary>
    public void SetProcessSliderValue(float v)
    {
        //print("processSlider前:" + v);
        processSlider.value = v;
        //print("processSlider后:" + processSlider.value);
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
    //public Button rateBtn;
    //相关倍数按钮的信息列表
    //public List<HistoryPlayUI_SpeedButton> rateButtonEtys;
    [HideInInspector]
    public int rateIndex = 0;//当前倍数按钮索引

    public Dropdown rateDropdown;//速率下拉列表框

    /// <summary>
    /// 初始化速率下拉列表框
    /// </summary>
    public void InitRateDropdown()
    {
        rateDropdown.ClearOptions();
        List<string> strs = new List<string>();
        strs.Add("1倍数");
        strs.Add("2倍数");
        strs.Add("4倍数");
        strs.Add("8倍数");
        rateDropdown.AddOptions(strs);
    }

    /// <summary>
    /// 速率按钮触发事件
    /// </summary>
    public void RateDropdown_ValueChanged(int v)
    {
        rateIndex = v;
        CurrentSpeed = Mathf.Pow(2, rateIndex);//1 2 4 8 16 32
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
    public List<Personnel> currentPersonnels = new List<Personnel>();//当前显示历史轨迹的人员信息
    public List<HistoryPersonUIItem> items;

    /// <summary>
    /// 添加按钮触发事件
    /// </summary>
    public void EditPersonBtn_OnClick()
    {
        //if (currentPersonnels == null)
        //{
        //    currentPersonnels = new List<Personnel>();
        //}
        List<Personnel> currentPersonnelsT = new List<Personnel>(currentPersonnels);
        HistoryPersonsSearchUI.Instance.Show(currentPersonnelsT);
        if (isPlay)
        {
            //如果在播放就让它暂停
            ClickPlayButton();
        }
    }

    public void ShowPersons(List<Personnel> personnelsT)
    {
        Log.Info("MultHistoryPlayUINew.ShowPersons");
        //如果在播放就让它终止
        //ExecuteEvents.Execute<IPointerClickHandler>(MultHistoryPlayUI.Instance.StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
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
        if (currentPersonnels != null)
        {
            currentPersonnels.Clear();
        }
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

    ///// <summary>
    ///// 模式切换
    ///// </summary>
    //public void Mode_Changed()
    //{
    //    Stop();
    //    isShowHistoryPathMode = !isShowHistoryPathMode;
    //    if (isShowHistoryPathMode)
    //    {
    //        SwithMode(Mode.Normal);
    //        modeName.text = "轨道模式";
    //        SetHistoryLineDrawing(false);
    //    }
    //    else
    //    {
    //        SwithMode(Mode.Drawing);
    //        modeName.text = "涎线模式";
    //        SetHistoryLineDrawing(true, true);
    //    }
    //}

    /// <summary>
    /// 切换轨迹模式
    /// </summary>
    public void SwithMode(HistoryMode modeT)
    {
        mode = modeT;
    }

    #region 选择时间段

    public Dropdown hourDropdown;//时下拉列表框
    public Dropdown minuteDropdown;//分下拉列表框
    public Dropdown durationDropdown;//播放时长下拉列表框


    public void HourDropdown_ValueChanged(int i)
    {
        Stop();
    }

    public void MinuteDropdown_ValueChanged(int i)
    {
        Stop();
    }

    public void DurationDropdown_ValueChanged(int i)
    {
        Stop();
    }

    public void SetTime(int hour,int minute,int duration)
    {
        hourDropdown.value = hour;
        minuteDropdown.value = minute;
        durationDropdown.value = duration;
    }

    public void SetDate(int year, int month, int day)
    {
        dayTxt.text = string.Format("{0}/{1}/{2}",year,month,day);
    }

    public void SetMiniteDropDownList(int max,int pow)
    {
        minutePower = pow;
        List<string> minuteStrs = new List<string>();
        for (int i = 0; i < max; i++)//一个单位代表10分钟
        {
            minuteStrs.Add((i * pow).ToString());
        }
        minuteDropdown.ClearOptions();
        minuteDropdown.AddOptions(minuteStrs);
    }

    /// <summary>
    /// 初始化下拉列表框
    /// </summary>
    public void InitDropdown()
    {
        //初始时间从8:30开始
        List<string> hourStrs = new List<string>();
        for (int i = 0; i < 24; i++)
        {
            hourStrs.Add(i.ToString());
        }
        hourDropdown.ClearOptions();
        hourDropdown.AddOptions(hourStrs);
        //hourDropdown.captionText.text = hourStrs[8];
        try
        {
            hourDropdown.value = SystemSettingHelper.historyPathSetting.StartHour;
        }
        catch
        {
            hourDropdown.value = 8;
        }

        SetMiniteDropDownList(6,10);//6,10=>0,10,20,30,40,50. 60,1=>0-59
        //minuteDropdown.captionText.text = minuteStrs[30];

        try
        {
            minuteDropdown.value = SystemSettingHelper.historyPathSetting.StartMinute;
        }
        catch
        {
            minuteDropdown.value = 3;
        }

        List<string> durationStrs = new List<string>();
        for (int i = 1; i <= 24; i++)
        {
            durationStrs.Add(i.ToString() + "小时");
        }
        durationDropdown.ClearOptions();
        durationDropdown.AddOptions(durationStrs);

        try
        {
            durationDropdown.value = SystemSettingHelper.historyPathSetting.Duration - 1;
        }
        catch
        {
            durationDropdown.value = 8 - 1;
        }
        //ScrollRect r = new ScrollRect();r.scro
        //Scrollbar 

    }

    /// <summary>
    /// 获取时间长度
    /// </summary>
    public float GetTimeLengthForSeconds()
    {
        float timet = (durationDropdown.value + 1) * 3600f; //value = 0=>1小时 => 3600s
        return timet;
    }

    public int GetTimeLengthForHours()
    {
        int timet = (durationDropdown.value + 1);
        return timet;
    }

    /// <summary>
    /// 获取小时，开始
    /// </summary>
    public int GetStartHours()
    {
        int timet = hourDropdown.value;
        return timet;
    }

    private int minutePower = 10;

    /// <summary>
    /// 获取分钟，开始
    /// </summary>
    public int GetStartMinutes()
    {
        int timet = minuteDropdown.value * minutePower;
        return timet;
    }

    /// <summary>
    /// 获取小时，结束
    /// </summary>
    public int GetEndHours()
    {
        int timet = hourDropdown.value;
        return timet;

    }

    /// <summary>
    /// 获取分钟，结束
    /// </summary>
    public int GetEndMinutes()
    {
        int timet = minuteDropdown.value * minutePower;
        return timet;
    }


    #endregion


    #region 模式切换,建筑透明，打开灯光

    public Toggle normalModeToggle;//轨道模式Toggle
    public Toggle drawingModeToggle;//绘制模式Toggle

    public ToggleButton3 buildingToggle;//建筑透明
    public ToggleButton3 lightToggle;//打开灯光

    /// <summary>
    /// 建筑透明.打开灯光BindToggles
    /// </summary>
    public void BindTogglesCall()
    {
        buildingToggle.SetState(FunctionSwitchBarManage.Instance.TransparentToggle.ison);
        lightToggle.SetState(FunctionSwitchBarManage.Instance.lightToggle.ison);
        SetBuildingToggleCall(BuildingToggle_OnValueChanged);
        SetLightToggleCall(LightToggle_OnValueChanged);
    }

    public void SetBuildingToggleCall(Action<bool> call)
    {
        buildingToggle.OnValueChanged = call;
    }

    public void SetLightToggleCall(Action<bool> call)
    {
        lightToggle.OnValueChanged = call;
    }

    public void BuildingToggle_OnValueChanged(bool b)
    {
        FunctionSwitchBarManage.Instance.SetTransparentToggle(b);
    }

    public void LightToggle_OnValueChanged(bool b)
    {
        FunctionSwitchBarManage.Instance.SetlightToggle(b);
    }

    //public void ResetToggles()
    //{
    //    SetBuildingToggleCall(null);
    //    SetLightToggleCall(null);
    //}

    /// <summary>
    /// 轨道模式Toggle改变触发事件
    /// </summary>
    public void NormalModeToggle_ValueChanged(bool b)
    {
        if (b)
        {
            SwitchMode(HistoryMode.Normal);
        }
    }

    /// <summary>
    /// 绘制模式Toggle改变触发事件
    /// </summary>
    public void DrawingModeToggle_ValueChanged(bool b)
    {
        if (b)
        {
            SwitchMode(HistoryMode.Drawing);
        }
    }

    /// <summary>
    /// 模式切换
    /// </summary>
    public void SwitchMode(HistoryMode modet)
    {
        Log.Info("SwitchMode", mode + "->" + modet);
        //Stop();

        RefleshProcessSlider(false);
        if (firstPoint != null)
        {
            DateTime t = LocationManager.GetTimestampToDateTime(firstPoint.Time);
            timeSum = (t - GetStartTime()).TotalSeconds;
        }
        else
        {
            timeSum = 0;
        }

        LocationHistoryManager.Instance.PathList.Hide();

        if (modet == HistoryMode.Normal)
        {
            SwithMode(HistoryMode.Normal);
            //modeName.text = "轨迹模式";

            SetHistoryLineDrawing(false);

            SetHistoryPathLines(true);
            ClearDrawingLines();
        }
        else
        {
            SwithMode(HistoryMode.Drawing);
            //modeName.text = "涎线模式";

            SetHistoryLineDrawing(true, true);

            SetHistoryPathLines(false);
        }

    }

    ///// <summary>
    ///// 设置历史人员显示隐藏
    ///// </summary>
    //public void SetHistoryPersonRender(bool isEnable)
    //{
    //    foreach (LocationHistoryPath_M patht in LocationHistoryManager.Instance.historyPath_Ms)
    //    {
    //        patht.Hide(isEnable);
    //    }
    //}

    /// <summary>
    /// 设置历史轨迹显示隐藏
    /// </summary>
    public void SetHistoryPathLines(bool b)
    {
        //foreach (LocationHistoryPath_M patht in LocationHistoryManager.Instance.historyPath_Ms)
        //{
        //    Transform t = patht.transform.parent.Find("HistoryLines");
        //    int count = t.transform.childCount;
        //    for (int i = count - 1; i >= 0; i--)
        //    {
        //        t.transform.GetChild(i).gameObject.SetActive(b);
        //    }
        //    if (patht.PosCount > 0)
        //    {
        //        patht.SetFisrtPos();
        //        patht.SetCurrentPointIndex(0);
        //    }

        //    patht.ClearPreviousInfo();
        //}

        LocationHistoryManager.Instance.PathList.SetHistoryPathLines(b);
    }

    /// <summary>
    /// 清除实时绘制的轨迹
    /// </summary>
    public void ClearDrawingLines()
    {
        LocationHistoryManager.Instance.PathList.ClearDrawingLines();
    }

    #endregion

    /// <summary>
    /// 加载按钮点击触发
    /// </summary>
    public void Loadbtn_OnClick()
    {
        Log.Info("Loadbtn_OnClick");
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
            LoadData();
        }
    }

    #region 播放控制界面

    public VerticalLayoutGroup linesGrid;//线列表
    public HistoryPlayBar lineUIPrefab;//线预设
    public float barLimitTime = 3;//3s时间限制，两点超过限制时间，线条断开
    public Dropdown personDropdown;//人员下拉列表
    public Dictionary<Personnel, HistoryPlayBar> Personnel_HistoryPlayBar;//名称和HistoryPlayBar

    /// <summary>
    /// 创建历史线路ui条
    /// </summary>
    public HistoryPlayBar CreateHistoryPlayBar()
    {
        HistoryPlayBar bar = Instantiate(lineUIPrefab);
        bar.gameObject.SetActive(true);
        bar.transform.SetParent(linesGrid.transform);
        bar.transform.localScale = Vector3.one;
        return bar;
    }

    /// <summary>
    /// 创建历史线路ui条
    /// </summary>
    public HistoryPlayBar CreateHistoryPlayBar(List<double> plist, List<Position> positions, double timeLengthT, Color colorT)
    {
        HistoryPlayBar bar = CreateHistoryPlayBar();
        bar.UpdateData(plist, positions, timeLengthT, colorT, barLimitTime);
        return bar;
    }

    /// <summary>
    /// 创建历史线路ui条
    /// </summary>
    public HistoryPlayBar CreateHistoryPlayBar(PathInfo pathInfo)
    {
        HistoryPlayBar bar = CreateHistoryPlayBar();
        bar.UpdateData(pathInfo.posList.GetTimeStampList(), pathInfo.posList.GetPositionList(), pathInfo.timeLength, pathInfo.color, barLimitTime);
        return bar;
    }

    

    /// <summary>
    /// 清除历史线路条
    /// </summary>
    public void ClearHistoryPlayBars()
    {
        int count = linesGrid.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(linesGrid.transform.GetChild(i).gameObject);
        }
    }


    /// <summary>
    /// PersonDropdown_ValueChanged
    /// </summary>
    public void PersonDropdown_ValueChanged(int i)
    {

        if (personDropdown.captionText.text == "全部")
        {
            foreach (HistoryPlayBar bar in Personnel_HistoryPlayBar.Values)
            {
                bar.gameObject.SetActive(true);
            }

            //foreach (LocationHistoryPath_M patht in LocationHistoryManager.Instance.historyPath_Ms)
            //{
            //    patht.SetPathEnable(true, processSlider.value > 0);
            //}

            LocationHistoryManager.Instance.PathList.SetPathEnable(true, processSlider.value > 0);
        }
        else
        {
            Personnel pt = null;
            foreach (Personnel p in Personnel_HistoryPlayBar.Keys)
            {
                if (p.Name == personDropdown.captionText.text)
                {
                    pt = p;
                    Personnel_HistoryPlayBar[p].gameObject.SetActive(true);
                }
                else
                {
                    Personnel_HistoryPlayBar[p].gameObject.SetActive(false);
                }
            }

            if (pt != null)
            {
                //foreach (LocationHistoryPath_M patht in LocationHistoryManager.Instance.historyPath_Ms)
                //{
                //    if (pt.Id == patht.personnel.Id)
                //    {
                //        SetPathEnable(patht, true);
                //    }
                //    else
                //    {
                //        SetPathEnable(patht, false);
                //    }
                //}

                LocationHistoryManager.Instance.PathList.SetPathEnable(pt.Id, processSlider.value > 0);
            }
        }
    }

    ///// <summary>
    ///// 设置某条路径的是否启用
    ///// </summary>
    ///// <param name="patht"></param>
    //private void SetPathEnable(LocationHistoryPath_M patht, bool isEnabled)
    //{
    //    bool isShow = isEnabled && processSlider.value > 0;
    //    patht.SetPathEnable(isEnabled, processSlider.value > 0);
    //}

    #endregion

    /// <summary>
    /// 设置返回按钮的显示隐藏
    /// </summary>
    public void SetBackViewBtnActive(bool isActive)
    {
        backViewBtn.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 返回视角按钮触发
    /// </summary>
    public void BackView_On_Click()
    {
        LocationHistoryManager.Instance.ClearFollowUIState(LocationHistoryManager.Instance.CurrentFocusController);
        if (LocationHistoryManager.Instance.CurrentFocusController != null)
        {
            LocationHistoryManager.Instance.CurrentFocusController.historyNameUI.SetCameraFollowToggleButtonActive(false);
        }
        LocationHistoryManager.Instance.RecoverBeforeFocusAlign();
    }
}

public enum HistoryMode
{
    Normal,//显示全部轨迹
    Drawing //绘制走过的轨迹
}
