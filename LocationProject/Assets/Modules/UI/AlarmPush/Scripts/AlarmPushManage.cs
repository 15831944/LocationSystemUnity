using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AlarmPushManage : MonoBehaviour
{
    private static AlarmPushManage _instance;
    public static AlarmPushManage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AlarmPushManage>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private string Message;
    int LinNum;

    private Button AlarmPushBut;
    public Toggle IsShow;
    public GameObject AlarmPushWindow;
    [System.NonSerialized]
    List<LocationAlarm> personAlarm = new List<LocationAlarm>();

    [System.NonSerialized]
    List<AlarmPushInfo> CurrentAlarmPushInfoList = new List<AlarmPushInfo>();
    [System.NonSerialized]
    List<AlarmPushInfo> CompleteAlarmPushInfoList = new List<AlarmPushInfo>();
    [System.NonSerialized]
    List<AlarmPushInfo> CloseAlarmPushInfoList = new List<global::AlarmPushInfo>();
    [System.NonSerialized]
    List<AlarmPushInfo> ShowAlarmPushInfoList = new List<AlarmPushInfo>();

    //  AlarmPushInfo AlarmInformation;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GameObject grid;
    public GameObject TitleObj;//显示有几条未告警数据
    bool isPushInfo = false;//是否正在推送信息

    Color LowColor = new Color(46 / 255f, 157 / 255f, 255 / 255f, 255 / 255f);
    Color MiddleColor = new Color(255 / 255f, 151 / 255f, 51 / 255f, 255 / 255f);
    Color HighColor = new Color(255 / 255f, 80 / 255f, 110 / 255f, 255 / 255f);
    Color SecondColor = new Color(255 / 255f, 253 / 255f, 57 / 255f, 255 / 255f);
    GameObject AlarmObj;

    [System.NonSerialized]
    List<LocationAlarm> AllPerAlarmPushInfo;
    [System.NonSerialized]
    List<DeviceAlarm> AllDevAlarmPushInfo;
    public GameObject LineExample;//宝兴行的模板
    bool IsBaoXin = false;
    [System.NonSerialized]
    public List<CameraAlarmInfo> AllCameraAlarmPush;
    [System.NonSerialized]
    public List<CameraAlarmInfo> CameraAlarmPushList;
    [System.NonSerialized]
    public List<CameraAlarmInfo> CurrentCameraAlarmList;
    [System.NonSerialized]
    public List<CameraAlarmInfo> NewestCameraAlarmPush;
    public bool IsNewAlarm;//判断是不是有新的告警baoxin
    void Start()
    {
        Instance = this;
        CommunicationCallbackClient.Instance.alarmHub.OnDeviceAlarmRecieved += OnDeviceAlarmRecieved;
        CommunicationCallbackClient.Instance.alarmHub.OnLocationAlarmRecieved += OnLocationAlarmRecieved;
        CommunicationCallbackClient.Instance.cameraAlarmHub.OnCameraAlarmsRecieved += CameraAlarmHub_OnCameraAlarmsRecieved;

        SceneEvents.FullViewStateChange += OnFullViewChange;
        IsShow.onValueChanged.AddListener(ShowAndCloseAlarmpush);

    }



    private void CameraAlarmHub_OnCameraAlarmsRecieved(List<CameraAlarmInfo> CameraInfo)
    {
        try
        {
            if (CameraInfo == null || CameraInfo.Count == 0) return;
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            Log.Info("CameraAlarmHub_OnCameraAlarmsRecieved","count:"+ CameraInfo.Count);
            //Debug.LogError(json);
            IsBaoXin = true;
            IsNewAlarm = true;
            AllCameraAlarmPush = new List<CameraAlarmInfo>();

            if (NewestCameraAlarmPush == null)
            {
                NewestCameraAlarmPush = new List<CameraAlarmInfo>();
            }
            CameraAlarmPushList = new List<CameraAlarmInfo>();

            FullViewController mainPage = FullViewController.Instance;
            foreach (var cam in CameraInfo)
            {
                if (cam.status == 1)
                {
                    AllCameraAlarmPush.Add(cam);
                }
                else
                {
                    Log.Error("CameraAlarmHub_OnCameraAlarmsRecieved", "cam.status != 1 :" + cam.status);
                }
            }

            Log.Info("CameraAlarmHub_OnCameraAlarmsRecieved", "AllCameraAlarmPush:" + AllCameraAlarmPush.Count);
            if (mainPage && mainPage.IsFullView) return;

            foreach (var camAlarm in AllCameraAlarmPush)
            {
                NewestCameraAlarmPush.Add(camAlarm);
                AlarmPushInfo AlarmInformation = new AlarmPushInfo();
                AlarmInformation.SetAlarmInfo(camAlarm);
                CameraAlarmInfo CurrentCameraAlarm = new CameraAlarmInfo();
                AlarmPushInfo CompleteCameraAlarm = new AlarmPushInfo();

                CurrentCameraAlarm = NewestCameraAlarmPush.Find(i => i.cid == camAlarm.cid);
                CompleteCameraAlarm = CompleteAlarmPushInfoList.Find(i => i.CameraAlarmInfor.cid == camAlarm.cid);
                if (camAlarm.AlarmType == 2)
                {
                    CurrentCameraAlarm = NewestCameraAlarmPush.Find(i => i.cid == camAlarm.cid && i.FlameData == camAlarm.FlameData);
                    CompleteCameraAlarm = CompleteAlarmPushInfoList.Find(i => i.CameraAlarmInfor.cid == camAlarm.cid && i.CameraAlarmInfor.FlameData != null);
                }
                else if (camAlarm.AlarmType == 1)
                {
                    CurrentCameraAlarm = NewestCameraAlarmPush.Find(i => i.cid == camAlarm.cid);
                    CompleteCameraAlarm = CompleteAlarmPushInfoList.Find(i => i.CameraAlarmInfor.cid == camAlarm.cid && i.CameraAlarmInfor.HeadData != null);

                }
                else if (camAlarm.AlarmType == 3)
                {
                    CurrentCameraAlarm = NewestCameraAlarmPush.Find(i => i.cid == camAlarm.cid);
                    CompleteCameraAlarm = CompleteAlarmPushInfoList.Find(i => i.CameraAlarmInfor.cid == camAlarm.cid && i.CameraAlarmInfor.SmogData != null);
                }
                if (CurrentCameraAlarm != null)
                {
                    if (CompleteCameraAlarm != null)
                    {
                        isPushInfo = true;
                        int? DevID = GetCameraInfoId(camAlarm.cid_url);
                        if (grid.transform.childCount == 0)
                        {
                            isPushInfo = false;
                        }
                        for (int i = 0; i < grid.transform.childCount; i++)
                        {
                            if (grid.transform.GetChild(i).GetChild(3).GetComponent<Text>().text == DevID.ToString())
                            {

                                int k = i;
                                if (grid.transform.childCount <= 5 && !TitleObj.activeSelf)
                                {
                                    if (grid.transform.childCount == 1)//只有一条告警
                                    {
                                        DestroyImmediate(grid.transform.GetChild(k).gameObject);

                                    }
                                    else
                                    {
                                        RemoveAlarmTween(k);
                                    }
                                }
                                else
                                {
                                    RemoveChildTween(k);
                                }

                            }
                        }
                    }

                }
                CurrentAlarmPushInfoList.Add(AlarmInformation);

                CameraAlarmPushList.AddRange(AllCameraAlarmPush);

                Log.Info("CameraAlarmHub_OnCameraAlarmsRecieved", "CurrentAlarmPushInfoList:" + CurrentAlarmPushInfoList.Count);

                Log.Info("CameraAlarmHub_OnCameraAlarmsRecieved", "CameraAlarmPushList:" + CameraAlarmPushList.Count);

                CameraAlarmFollowUI.RefreshAll();
            }
        }
        catch (Exception ex)
        {

            Log.Error("CameraAlarmHub_OnCameraAlarmsRecieved", "Exception :" + ex);
        }
        
    }

    private void Update()
    {

        PushInformation();
    }

    /// <summary>
    /// 设备告警
    /// </summary>
    /// <param name="devList"></param>
    public void OnDeviceAlarmRecieved(List<DeviceAlarm> devList)
    {
        try
        {
            FullViewController mainPage = FullViewController.Instance;
            AllDevAlarmPushInfo = new List<DeviceAlarm>();
            if (mainPage && mainPage.IsFullView)
            {
                foreach (var dev in devList)
                {
                    if (dev == null) continue;
                    if (AllDevAlarmPushInfo.Count != 0)
                    {
                        DeviceAlarm devAlarm = AllDevAlarmPushInfo.Find(m => m != null && m.Level == Abutment_DevAlarmLevel.无 && m.DevId == dev.DevId);
                        if (devAlarm == null)
                        {
                            AllDevAlarmPushInfo.Remove(devAlarm);
                        }
                        else
                        {
                            AllDevAlarmPushInfo.Add(devAlarm);
                        }
                    }

                }
            }
            else
            {
                foreach (var alarm in devList)
                {
                    if (alarm == null) continue;
                    AllDevAlarmPushInfo.Add(alarm);
                }
                foreach (var dev in AllDevAlarmPushInfo)
                {
                    if (dev == null) continue;
                    if (dev.Level == Abutment_DevAlarmLevel.无)
                    {

                        AlarmPushInfo Alarm = CurrentAlarmPushInfoList.Find(m => m != null && m.devAlarmInfo.Level == Abutment_DevAlarmLevel.无 && m.devAlarmInfo.DevId == dev.DevId);
                        AlarmPushInfo CompleteAlarm = CompleteAlarmPushInfoList.Find(n => n != null && n.devAlarmInfo.DevId == dev.DevId && n.devAlarmInfo.Level != Abutment_DevAlarmLevel.无);

                        if (Alarm != null || CompleteAlarm != null)
                        {
                            AlarmPushInfo NormalAlarmInformation = new AlarmPushInfo();
                            NormalAlarmInformation.SetAlarmInfo(dev);
                            CurrentAlarmPushInfoList.Add(NormalAlarmInformation);
                        }
                    }
                    else
                    {

                        AlarmPushInfo CurrentAlarm = CurrentAlarmPushInfoList.Find(m => m != null && m.devAlarmInfo.DevId == dev.DevId && m.devAlarmInfo.Level == dev.Level);
                        if (CurrentAlarm == null)
                        {
                            AlarmPushInfo AlarmInformation = new AlarmPushInfo();
                            AlarmInformation.SetAlarmInfo(dev);
                            CurrentAlarmPushInfoList.Add(AlarmInformation);
                        }

                    }

                }
            }
        }
        catch (Exception e)
        {
            Log.Error("AlarmPushManage.Error:" + e.ToString());
        }
    }
    public void OnFullViewChange(bool isOn)
    {

        if (isOn)
        {
            ShowAlarmPushWindow(false);
            CloseIsShowTog();

        }
        else
        {
            IsShow.isOn = SystemSettingHelper.alarmSetting.PushAlarmShow;
            ShowAlarmPushWindow(SystemSettingHelper.alarmSetting.PushAlarmShow);
        }
    }
    /// <summary>
    /// 获取到人员告警信息放到指定的列表里
    /// </summary>
    /// <param name="LocationaList"></param>
    public void OnLocationAlarmRecieved(List<LocationAlarm> LocationaList)
    {
        FullViewController mainPage = FullViewController.Instance;
        AllPerAlarmPushInfo = new List<LocationAlarm>();
        if (mainPage && mainPage.IsFullView)
        {
            foreach (var alarm in LocationaList)
            {
                LocationAlarm PerAlarm = AllPerAlarmPushInfo.Find(a => a.Id == alarm.Id && alarm.AlarmLevel == LocationAlarmLevel.正常 && a.AlarmLevel != LocationAlarmLevel.正常);
                if (PerAlarm != null)
                {
                    AllPerAlarmPushInfo.Remove(PerAlarm);
                }
                else
                {
                    AllPerAlarmPushInfo.Add(alarm);
                }

            }
        }
        else
        {
            foreach (var alarm in LocationaList)
            {
                AllPerAlarmPushInfo.Add(alarm);
            }
            foreach (var per in AllPerAlarmPushInfo)
            {

                if (per.AlarmLevel == LocationAlarmLevel.正常)
                {

                    AlarmPushInfo Alarm = CurrentAlarmPushInfoList.Find(m => m.AlarmType == AlarmPushInfoType.locationAlarm && m.locationAlarmInfo.Id == per.Id && m.locationAlarmInfo.AlarmLevel != LocationAlarmLevel.正常);
                    AlarmPushInfo CompleteAlarm = CompleteAlarmPushInfoList.Find(n => n.AlarmType == AlarmPushInfoType.locationAlarm && n.locationAlarmInfo.Id == per.Id && n.locationAlarmInfo.AlarmLevel != LocationAlarmLevel.正常);

                    if (Alarm != null || CompleteAlarm != null)
                    {
                        AlarmPushInfo NormalAlarmInformation = new AlarmPushInfo();
                        NormalAlarmInformation.SetAlarmInfo(per);
                        CurrentAlarmPushInfoList.Add(NormalAlarmInformation);
                    }
                }
                else
                {
                    AlarmPushInfo CurrentAlarm = CurrentAlarmPushInfoList.Find(m => m.AlarmType == AlarmPushInfoType.locationAlarm && m.locationAlarmInfo.Id == per.Id && m.locationAlarmInfo.AlarmLevel == per.AlarmLevel);
                    if (CurrentAlarm == null)
                    {
                        AlarmPushInfo AlarmInformation = new AlarmPushInfo();
                        AlarmInformation.SetAlarmInfo(per);
                        CurrentAlarmPushInfoList.Add(AlarmInformation);
                    }

                }

            }
        }

    }

    /// <summary>
    /// 开始推送信息
    /// </summary>
    public void PushInformation()
    {
        if (CurrentAlarmPushInfoList.Count == 0 || isPushInfo) return;

        isPushInfo = true;
        AlarmPushInfo value = CurrentAlarmPushInfoList[0];
        ShowAlarmLevel(value);


    }
    /// <summary>
    /// 获取设备推送告警信息
    /// </summary>
    public void GetDevAlarmPushData(AlarmPushInfo devAlarm)
    {
        GameObject obj = InstantiateLine();
        if (devAlarm.devAlarmInfo.Level == Abutment_DevAlarmLevel.低)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = LowColor;
        }
        if (devAlarm.devAlarmInfo.Level == Abutment_DevAlarmLevel.中)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = MiddleColor;
        }
        if (devAlarm.devAlarmInfo.Level == Abutment_DevAlarmLevel.高)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = HighColor;
        }
        Message = devAlarm.devAlarmInfo.Title;
        obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Message;

        if (CloseAlarmPushInfoList.Count != 0)
        {
            AlarmPushInfo overAlarm = CloseAlarmPushInfoList.Find(m => m.devAlarmInfo.Id == devAlarm.devAlarmInfo.Id);
            if (overAlarm != null)
            {
                CloseAlarmPushInfoList.Remove(overAlarm);
            }
        }
        SetTween(devAlarm, obj);
        int devId = devAlarm.devAlarmInfo.DevId;
        obj.transform.GetChild(3).GetComponent<Text>().text = devId.ToString();
        if (devAlarm.devAlarmInfo.DevId != 0)
        {
            int depId = (int)devAlarm.devAlarmInfo.AreaId;
            Button but = obj.transform.GetComponent<Button>();
            but.onClick.RemoveAllListeners();
            but.onClick.AddListener(() =>
            {
                DevBut_Click(devAlarm.devAlarmInfo, depId, Message);
            });
        }
        else
        {
            Debug.LogError("devAlarm.devAlarmInfo.Dev == null，请检查原因！");
        }

    }
    /// <summary>
    /// 定位告警推送信息
    /// </summary>
    /// <param name="LocationaList"></param>
    public void GetLocationAlarmPushData(AlarmPushInfo LocationaList)
    {
        GameObject obj = InstantiateLine();
        if (LocationaList.locationAlarmInfo.AlarmLevel == LocationAlarmLevel.一级告警)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = LowColor;
        }
        if (LocationaList.locationAlarmInfo.AlarmLevel == LocationAlarmLevel.二级告警)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = SecondColor;
        }
        if (LocationaList.locationAlarmInfo.AlarmLevel == LocationAlarmLevel.三级告警)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = MiddleColor;
        }
        if (LocationaList.locationAlarmInfo.AlarmLevel == LocationAlarmLevel.四级告警)
        {
            obj.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = HighColor;
        }
        Message = LocationaList.locationAlarmInfo.Content;
        obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Message;
        if (CloseAlarmPushInfoList.Count != 0)
        {
            AlarmPushInfo overAlarm = CloseAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == LocationaList.locationAlarmInfo.Id);
            if (overAlarm != null)
            {
                CloseAlarmPushInfoList.Remove(overAlarm);
            }
        }
        SetTween(LocationaList, obj);
        string PersonId = LocationaList.locationAlarmInfo.Id.ToString();
        obj.transform.GetChild(3).GetComponent<Text>().text = PersonId;
        string PerTagId = LocationaList.locationAlarmInfo.TagId.ToString();
        Button But = obj.GetComponent<Button>();
        But.onClick.RemoveAllListeners();
        But.onClick.AddListener(() =>
      {
          PerAlarmBut_Click(PerTagId);
      });
    }
    /// <summary>
    /// 告警信息是人员还是设备
    /// </summary>
    /// <param name="alarm"></param>
    public void ShowAlarmLevel(AlarmPushInfo alarm)
    {

        if (alarm.AlarmType == AlarmPushInfoType.locationAlarm)
        {
            ShowLocationAlarmInformation(alarm);
        }
        else if (alarm.AlarmType == AlarmPushInfoType.devAlarm)
        {
            ShowDevAlarmInformation(alarm);
        }
        else if (alarm.AlarmType == AlarmPushInfoType.CameraAlarmInfo)
        {
            ShowCameraAlarmInformation(alarm);
        }
    }
    /// <summary>
    /// 展示人员告警信息
    /// </summary>
    /// <param name="alarm"></param>
    public void ShowLocationAlarmInformation(AlarmPushInfo alarm)
    {
        AlarmPushInfo Alarm = CurrentAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == alarm.locationAlarmInfo.Id);
        CurrentAlarmPushInfoList.Remove(Alarm);
        if (alarm.locationAlarmInfo.AlarmLevel != LocationAlarmLevel.正常)
        {

            GetLocationAlarmPushData(alarm);
            CompleteAlarmPushInfoList.Add(alarm);
        }
        else
        {
            try
            {

                string CurrentId = alarm.locationAlarmInfo.Id.ToString();
                AlarmPushInfo showAlarm = ShowAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == alarm.locationAlarmInfo.Id);
                Debug.LogError("showAlarm" + ShowAlarmPushInfoList.Count);
                if (showAlarm != null)
                {
                    ShowAlarmPushInfoList.Remove(showAlarm);
                    for (int i = 0; i < grid.transform.childCount; i++)
                    {
                        if (CurrentId == grid.transform.GetChild(i).GetChild(3).GetComponent<Text>().text)
                        {
                            int k = i;
                            CompleteAlarmPushInfoList.Remove(alarm);
                            grid.transform.GetChild(i).DOScaleX(0, 0.1f).OnComplete(() =>
                            {
                                if (grid.transform.childCount != 0)
                                {
                                    if (grid.transform.childCount <= 5 && !TitleObj.activeSelf)
                                    {
                                        if (grid.transform.childCount == 1)//只有一条告警
                                        {
                                            DestroyImmediate(grid.transform.GetChild(k).gameObject);
                                            isPushInfo = false;
                                        }
                                        else //大于1小于6条告警
                                        {

                                            RemoveAlarmTween(k);
                                        }
                                    }
                                    else if (grid.transform.childCount <= 5 && TitleObj.activeSelf)//告警条数标题栏存在
                                    {

                                        RemoveChildTween(k);

                                    }
                                }
                            });
                        }
                    }
                }
                else
                {
                    AlarmPushInfo overAlarm = CloseAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == alarm.locationAlarmInfo.Id);
                    AlarmPushInfo ShowAlarm = ShowAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == alarm.locationAlarmInfo.Id);
                    if (overAlarm != null && ShowAlarm == null)
                    {

                        CloseAlarmPushInfoList.Remove(overAlarm);
                        AlarmNumChange();
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0}：{1},!!!", e.ToString(), alarm.locationAlarmInfo.Id);
            }
        }
        if (grid.transform.childCount == 0)
        {
            isPushInfo = false;
        }
    }

    /// <summary>
    /// 展示设备告警信息
    /// </summary>
    /// <param name="alarm"></param>
    public void ShowDevAlarmInformation(AlarmPushInfo alarm)
    {
        AlarmPushInfo Alarm = CurrentAlarmPushInfoList.Find(m => m.devAlarmInfo.Id == alarm.devAlarmInfo.Id);
        CurrentAlarmPushInfoList.Remove(Alarm);
        if (alarm.devAlarmInfo.Level != Abutment_DevAlarmLevel.无)
        {

            GetDevAlarmPushData(alarm);
            CompleteAlarmPushInfoList.Add(alarm);
        }
        else
        {
            try
            {
                string CurrentId = alarm.devAlarmInfo.Id.ToString();
                AlarmPushInfo showAlarm = ShowAlarmPushInfoList.Find(m => m.devAlarmInfo.Id == alarm.devAlarmInfo.Id);
                if (showAlarm != null)
                {
                    ShowAlarmPushInfoList.Remove(showAlarm);
                    for (int i = 0; i < grid.transform.childCount; i++)
                    {
                        if (CurrentId == grid.transform.GetChild(i).GetChild(3).GetComponent<Text>().text)
                        {
                            int k = i;
                            CompleteAlarmPushInfoList.Remove(alarm);
                            grid.transform.GetChild(i).DOScaleX(0, 0.1f).OnComplete(() =>
                            {
                                if (grid.transform.childCount != 0)
                                {
                                    if (grid.transform.childCount <= 5 && !TitleObj.activeSelf)
                                    {
                                        if (grid.transform.childCount == 1)//只有一条告警
                                        {
                                            DestroyImmediate(grid.transform.GetChild(k).gameObject);
                                            isPushInfo = false;
                                        }
                                        else //大于1小于6条告警
                                        {

                                            RemoveAlarmTween(k);
                                        }
                                    }
                                    else if (grid.transform.childCount <= 5 && TitleObj.activeSelf)//告警条数标题栏存在
                                    {

                                        RemoveChildTween(k);

                                    }
                                }
                            });
                        }
                    }
                }
                else
                {
                    AlarmPushInfo overAlarm = CloseAlarmPushInfoList.Find(m => m.devAlarmInfo.Id == alarm.devAlarmInfo.Id);
                    AlarmPushInfo ShowAlarm = ShowAlarmPushInfoList.Find(m => m.devAlarmInfo.Id == alarm.devAlarmInfo.Id);
                    if (overAlarm != null && ShowAlarm == null)
                    {

                        CloseAlarmPushInfoList.Remove(overAlarm);
                        AlarmNumChange();
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0}：{1},!!!", e.ToString(), alarm.locationAlarmInfo.Id);
            }
        }

    }
    public void ShowCameraAlarmInformation(AlarmPushInfo cameraAlarm)
    {
        AlarmPushInfo Alarm = CurrentAlarmPushInfoList.Find(m => m.CameraAlarmInfor.cid == cameraAlarm.CameraAlarmInfor.cid);
        CompleteAlarmPushInfoList.Add(Alarm);
        CurrentAlarmPushInfoList.Remove(Alarm);
        GameObject obj = InstantiateLineBaoxin();
        CameraAlarmInfoItem item = obj.GetComponent<CameraAlarmInfoItem>();
        if (item == null)
        {
            Log.Error("ShowCameraAlarmInformation", "item==null");
        }
        else
        {
            item.GetCameraAlarmData(cameraAlarm);
            item.MoveTween();
        }
        CompleteAlarmPushInfoList.Add(cameraAlarm);
    }
    public void ShowTitleObj()
    {
        TitleObj.SetActive(true);

    }
    bool IsTitle = false;
    public void SetCameraAlarmNumber(AlarmPushInfo alarm)
    {
        if (grid.transform.childCount > 5)
        {
            ShowTitleObj();
            AlarmNum = AlarmNum + 1;
            TitleObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<color=#58A2B4FF>还有</color>" + AlarmNum.ToString() + "<color=#58A2B4FF>条未处理告警未显示</color>";
            if (IsTitle)
            {
                grid.transform.GetChild(0).DOScaleX(0, 0.3f).OnComplete(() =>
                {
                    DestroyImmediate(grid.transform.GetChild(0).gameObject);
                    ChildMoveTween();
                });
            }
            else
            {
                TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-118, 197, 0), 0.1f).OnComplete(() =>
                {
                    TitleObj.transform.GetChild(0).GetComponent<Image>().DOFillAmount(1, 0.1f).OnComplete(() =>
                    {
                        IsTitle = true;
                        grid.transform.GetChild(0).DOScaleX(0, 0.3f).OnComplete(() =>
                        {
                            DestroyImmediate(grid.transform.GetChild(0).gameObject);
                            ChildMoveTween();
                        });
                    });
                });

            }

        }
        else
        {
            ChildMoveTween();
        }

        ShowAlarmPushInfoList.Add(alarm);

    }

    public void ChildMoveTween()
    {
        for (int i = 0; i <= grid.transform.childCount; i++)
        {
            RectTransform ObjY;
            int mun = 50;
            if (i == 0)
            {
                ObjY = grid.transform.GetChild(0).GetComponent<RectTransform>();
            }
            else
            {
                ObjY = grid.transform.GetChild(i - 1).GetComponent<RectTransform>();
            }
            if (ObjY != null)
            {
                float posY = ObjY.anchoredPosition3D.y + mun;
                if (i == grid.transform.childCount - 1)
                {
                    AlarmTw = ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F).OnComplete(() =>
                    {
                        isPushInfo = false;
                        BaoXinDelete_ClickAlarm();
                    });
                }
                else
                {
                    AlarmTw = ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F);
                }

            }

        }
    }
    /// <summary>
    ///
    /// </summary>
    public void AlarmNumChange()
    {
        if (grid.transform.childCount <= 5)
        {
            AlarmNum = AlarmNum - 1;
            IsExitAlarmTitle = true;
            if (AlarmNum == 0)
            {
                ExitAlarmNumTween();
            }
            else if (AlarmNum > 0)
            {
                TitleObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<color=#58A2B4FF>还有</color>" + AlarmNum.ToString() + "<color=#58A2B4FF>条未处理告警未显示</color>";
                isPushInfo = false;
            }
        }
    }
    private int AlarmNum;//告警未显示数量
    List<string> AlarmId = new List<string>();//未显示告警的ID
    private Tweener AlarmTw;
    private Tweener AlarmTween;
    /// <summary>
    /// 设置推送告警的数量
    /// </summary>
    /// 
    public void SetAlarmPushInFo(AlarmPushInfo alarm)
    {
        if (grid.transform.childCount != 0)
        {
            if (grid.transform.childCount <= 5)
            {
                

                for (int i = 0; i < grid.transform.childCount; i++)
                {
                    int mun = 50;
                    if (grid.transform.childCount == 0)
                    {
                        RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();

                        if (ObjY != null)
                        {
                            float posY = ObjY.anchoredPosition3D.y + mun;
                            AlarmTw = ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F).OnComplete(() =>
                            {
                                isPushInfo = false;
                            });
                        }
                    }
                    else
                    {
                        if (i < grid.transform.childCount - 1)
                        {
                            RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();
                            if (ObjY != null)
                            {
                                float posY = ObjY.anchoredPosition3D.y + mun;
                                AlarmTw = ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F);

                            }
                        }
                        else
                        {
                            if (i == grid.transform.childCount - 1)
                            {
                                RectTransform Objy = grid.transform.GetChild(i).GetComponent<RectTransform>();

                                if (Objy != null)
                                {
                                    float posY = Objy.anchoredPosition3D.y + mun;
                                    AlarmTw = Objy.DOAnchorPos(new Vector2(0, posY), 0.1F).OnComplete(() =>
                                    {
                                        isPushInfo = false;
                                    });
                                }
                            }
                        }

                    }


                }
            }
            else if (grid.transform.childCount > 5)
            {
                TitleObj.SetActive(true);
                AlarmNum = AlarmNum + 1;

               // ShowAlarmPushInfoList.Add(alarm);
                string CloseId = grid.transform.GetChild(0).GetChild(3).GetComponent<Text>().text;
                int strId = int.Parse(CloseId);
                AlarmPushInfo showAlarmInfo = ShowAlarmPushInfoList.Find(m => m.AlarmType == AlarmPushInfoType.locationAlarm && m.locationAlarmInfo.Id == strId);
                if (showAlarmInfo != null)
                {
                    CloseAlarmPushInfoList.Add(showAlarmInfo);
                    ShowAlarmPushInfoList.Remove(showAlarmInfo);
                }
                else
                {
                    AlarmPushInfo showAlarm = ShowAlarmPushInfoList.Find(m => m.devAlarmInfo.Id == strId);
                    {
                        if (showAlarm != null)
                        {
                            CloseAlarmPushInfoList.Add(showAlarm);
                            ShowAlarmPushInfoList.Remove(showAlarm);
                        }
                    }
                }
                TitleObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<color=#58A2B4FF>还有</color>" + AlarmNum.ToString() + "<color=#58A2B4FF>条未处理告警未显示</color>";
                TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-70, 197, 0), 0.1f).OnComplete(() =>
                {
                    TitleObj.transform.GetChild(0).GetComponent<Image>().DOFillAmount(1, 0.1f).OnComplete(() =>
                    {
                        grid.transform.GetChild(0).DOScaleX(0, 0.3f).OnComplete(() =>
                        {
                            DestroyImmediate(grid.transform.GetChild(0).gameObject);

                            for (int i = 0; i < grid.transform.childCount; i++)
                            {
                                if (i < 4)
                                {
                                    RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();
                                    if (ObjY != null)
                                    {

                                        float posY = ObjY.anchoredPosition3D.y + 50;
                                        AlarmTween = ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F);
                                    }
                                }
                                else
                                {
                                    if (i == 4)
                                    {
                                        RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();
                                        if (ObjY != null)
                                        {

                                            float posY = ObjY.anchoredPosition3D.y + 50;
                                            AlarmTween = ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F).OnComplete(() =>
                                            {
                                                isPushInfo = false;
                                            });
                                        }

                                    }
                                }
                            }

                        });
                    });
                });

            }
           
        }
        else
        {
            isPushInfo = false;
        }
    }

    private Image TextBg;//字体背景
    private Text AlarmText;//告警信息
    private Transform OutLevel1;//旋转外框
    private Transform OutLevel2;//旋转外框
    private Transform Level;//等级旋转框
    private Transform mask1;//遮罩
    private Transform mask2;//zhezhao

    /// <summary>
    /// 动画
    /// </summary>
    public void SetTween(AlarmPushInfo alarm, GameObject Item)
    {
        ShowAlarmPushInfoList.Add(alarm);
        Debug.LogError("ShowAlarmPushInfoList" + ShowAlarmPushInfoList.Count);
        Vector3 V3 = new Vector3(1, 1, 1);
        Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);
        try
        {
            TextBg = Item.transform.GetChild(0).GetComponent<Image>();

            OutLevel1 = Item.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
            OutLevel2 = Item.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
            Level = Item.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();
            mask1 = Item.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>();
            mask2 = Item.transform.GetChild(2).GetChild(1).GetComponent<RectTransform>();
            Tween aa = DOTween.To(() => TextBg.fillAmount, x => TextBg.fillAmount = x, 1, 0.1f);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("{0}：{1},!!!", e.ToString(), 123);
        }
        if (Item.transform.GetComponent<Image>() == null)
        {
            Debug.LogErrorFormat("{0}：{1},!!!", 123);
        }

        // Item.transform.GetComponent<Image>().DOColor(new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255), 0.5f);
        Item.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, -104, 0), 0.1f)
        .OnComplete(() =>
        {

            Tween s = OutLevel1.DORotate(new Vector3(0, 0, 60), 0.1f).SetLoops(-1, LoopType.Yoyo);//外框旋转
            Tween t = OutLevel2.DORotate(new Vector3(0, 0, 60), 0.1f).SetLoops(-1, LoopType.Yoyo);//外框旋转
            Tween Alarm = Level.DORotate(new Vector3(0, 0, 60), 0.1f).SetLoops(-1, LoopType.Yoyo);//小图标旋转
            OutLevel1.DOScale(scale, 0.01f);
            OutLevel2.DOScale(scale, 0.01f);
            Level.DOScale(scale, 0.01f);

            mask1.DOLocalMoveX(-300, 0.01f).OnComplete(() =>
            {
                mask1.gameObject.SetActive(false);
                SetAlarmPushInFo(alarm);
                s.Kill();
                t.Kill();
                Alarm.Kill();
                OutLevel1.DOScale(V3, 0.01f);
                OutLevel2.DOScale(V3, 0.01f);
                OutLevel1.DORotate(new Vector3(0, 0, 0), 0.001f);
                OutLevel2.DORotate(new Vector3(0, 0, 0), 0.001f);
                Level.DORotate(new Vector3(0, 0, 0), 0.001f);
                Level.DOScale(V3, 0.1f).OnComplete(() =>
                {


                });
            }); //第一个光片



            mask2.DOLocalMoveX(-300, 0.1f).OnComplete(() =>
           {
               mask2.gameObject.SetActive(false);
              

           }); //第2个光片

        });

    }

    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLineBaoxin()
    {
        AlarmObj = Instantiate(LineExample);
        //AlarmObj = Instantiate(TemplateInformation);
        AlarmObj.transform.parent = grid.transform;
        AlarmObj.transform.localScale = new Vector3(1, 1, 1);
        AlarmObj.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(400, -230f, 0);
        AlarmObj.SetActive(true);

        return AlarmObj;
    }
    public GameObject InstantiateLine()
    {

        AlarmObj = Instantiate(TemplateInformation);
        AlarmObj.transform.parent = grid.transform;
        AlarmObj.transform.localScale = new Vector3(1, 1, 1);
        AlarmObj.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(400, -230f, 0);
        AlarmObj.SetActive(true);

        return AlarmObj;
    }
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public void DevBut_Click(DeviceAlarm alarmInfo, int DepID, string msg)
    {
        RoomFactory.Instance.FocusDev(alarmInfo.DevId.ToString(), DepID, result =>
         {
             if (!result)
             {
                 //string msgTitle = DevAlarmListManage.TryGetDeviceAlarmInfo(alarmInfo);
                 string msgTitle = string.Format("未找到对应设备，设备ID：{0} 区域ID：{1}", alarmInfo.DevId, DepID);
                 UGUIMessageBox.Show(msgTitle);
             }
         });
    }
    public void PerAlarmBut_Click(string tagNum)
    {
        int tagID = int.Parse(tagNum);
        LocationManager.Instance.FocusPersonAndShowInfo(tagID);
    }
    bool IsExitAlarmTitle = true;
    /// <summary>
    /// 提示告警条数消失的动画
    /// </summary>
    public void ExitAlarmNumTween()
    {
        TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(116, 197, 0), 0.1f).OnComplete(() =>
        {
            TitleObj.SetActive(false);
            isPushInfo = false;
        });
    }



    /// <summary>
    /// 消除告警，其余的向xia移动
    /// </summary>
    public void RemoveAlarmTween(int num)
    {

        DestroyImmediate(grid.transform.GetChild(num).gameObject);
        //if (num == 0)
        //{
        //    isPushInfo = false;
        //}
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();

            if (num - 1 > i)
            {

                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F);
                }

            }
            else if (num - 1 == i)
            {
                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F).OnComplete(() =>
                    {
                        isPushInfo = false;
                    });
                }
            }
        }
    }
    /// <summary>
    /// 消除告警，其余的向下移动
    /// </summary>
    public void RemoveChildTween(int num)
    {
        DestroyImmediate(grid.transform.GetChild(num).gameObject);

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();

            if (num - 1 > i)
            {

                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F);
                }

            }
            else if (num - 1 == i)
            {
                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.1F).OnComplete(() =>
                    {
                        AlarmNumberChange();
                    });
                }
            }
        }
    }

    public void AlarmNumberChange()
    {
        Text massage = TitleObj.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        if (grid.transform.childCount <= 5)
        {
            AlarmNum = AlarmNum - 1;
            IsExitAlarmTitle = true;
            if (AlarmNum == 0)
            {
                try
                {
                    TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(116, 197, 0), 0.1f).OnComplete(() =>
                    {
                        try
                        {
                            TitleObj.SetActive(false);
                            if (!IsBaoXin)
                            {
                                ShowAlarmLevel(CloseAlarmPushInfoList[0]);
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.LogErrorFormat("{0}：{1},!!!", e.ToString(), AlarmNum);
                        }
                    });
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("{0}：{1},!!!", e.ToString(), AlarmNum);
                }
            }
            else if (AlarmNum > 0)
            {
                try
                {
                    massage.text = "<color=#58A2B4FF>还有</color>" + AlarmNum.ToString() + "<color=#58A2B4FF>条未处理告警未显示</color>";
                    ShowAlarmLevel(CloseAlarmPushInfoList[0]);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("{0}：{1},!!!", e.ToString(), AlarmNum);
                }
            }
        }
    }
    public void ShowAlarmPushWindow(bool b)
    {
        if (b)
        {
            AlarmPushWindow.SetActive(true);
        }
        else
        {
            AlarmPushWindow.SetActive(false);
        }
    }
    public void ShowAndCloseAlarmpush(bool b)
    {
        if (IsShow.isOn == b)
        {
            AlarmPushWindow.SetActive(b);
        }
        SystemSettingHelper.alarmSetting.PushAlarmShow = b;
        SystemSettingHelper.SaveSystemSetting();
    }
    /// <summary>
    /// 显示显示告警推送的按钮
    /// </summary>
    public void ShowIsShow()
    {
        if (IsShow.isOn == true)
        {
            IsShow.gameObject.SetActive(true);
        }

    }
    /// <summary>
    /// guanbi告警推送的按钮
    /// </summary>
    public void CloseIsShowTog()
    {
        IsShow.gameObject.SetActive(false);
    }
    public void CloseAlarmPushWindow(bool b)
    {
        IsShow.gameObject.SetActive(b);
        if (IsShow.isOn == true&& IsShow.gameObject.activeSelf )
        {
            AlarmPushWindow.SetActive(true);
        }
        else
        {
            AlarmPushWindow.SetActive(false);
        }    
    }
    /// <summary>
    /// 删除点击过的告警
    /// </summary>
    public void BaoXinDelete_ClickAlarm()
    {
        if (!isPushInfo)
        {
            if (ClickAlarmList.Count == 0) return;
            isPushInfo = true;
            if (grid.transform.childCount == 0)
            {
                isPushInfo = false;
            }
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                string AlarmID = ClickAlarmList.Find(N => N == grid.transform.GetChild(i).GetChild(3).GetComponent<Text>().text);
                if (!string.IsNullOrEmpty(AlarmID))
                {
                    int k = i;
                    if (grid.transform.childCount <= 5 && !TitleObj.activeSelf)
                    {
                        if (grid.transform.childCount == 1)//只有一条告警
                        {
                            DestroyImmediate(grid.transform.GetChild(k).gameObject);

                        }
                        else
                        {
                            RemoveAlarmTween(k);
                        }
                    }
                    else
                    {
                        RemoveChildTween(k);
                    }
                    ClickAlarmList.Remove(AlarmID);
                }
            }
        }

    }
    public List<string> ClickAlarmList;
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
        string ipFinal = ipTemp[0];
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
}




