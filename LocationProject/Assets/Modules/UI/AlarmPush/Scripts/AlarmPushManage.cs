using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AlarmPushManage : MonoBehaviour
{
    public static AlarmPushManage Instance;
    private string Message;
    int LinNum;

    private Button AlarmPushBut;

    public GameObject AlarmPushWindow;
    List<LocationAlarm> personAlarm;

    List<AlarmPushInfo> CurrentAlarmPushInfoList;
    List<AlarmPushInfo> CompleteAlarmPushInfoList;
    List<AlarmPushInfo> CloseAlarmPushInfoList;
    List<AlarmPushInfo> ShowAlarmPushInfoList;
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
    //   List<GameObject> overObj = new List<GameObject>();//大于6条告警
    //   public GameObject overGrid;
    List<LocationAlarm> AllPerAlarmPushInfo;
    void Start()
    {
        Instance = this;
        CommunicationCallbackClient.Instance.alarmHub.OnDeviceAlarmRecieved += OnDeviceAlarmRecieved;
        CommunicationCallbackClient.Instance.alarmHub.OnLocationAlarmRecieved += OnLocationAlarmRecieved;
        CurrentAlarmPushInfoList = new List<AlarmPushInfo>();
        CompleteAlarmPushInfoList = new List<AlarmPushInfo>();
        ShowAlarmPushInfoList = new List<AlarmPushInfo>();
        //    AlarmInformation = new AlarmPushInfo();
        personAlarm = new List<LocationAlarm>();

        CloseAlarmPushInfoList = new List<global::AlarmPushInfo>();
        SceneEvents.FullViewStateChange += OnFullViewChange;
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
        FullViewController mainPage = FullViewController.Instance;
        if (mainPage && mainPage.IsFullView)
        {

        }
        foreach (var dev in devList)
        {
            if (dev.Level == Abutment_DevAlarmLevel.无)
            {
                AlarmPushInfo devAlarm = CurrentAlarmPushInfoList.Find(m => m.AlarmType == AlarmPushInfoType.devAlarm && m.devAlarmInfo.DevId == dev.DevId);
                if (devAlarm == null) return;
                AlarmPushInfo CompletDevealarm = CompleteAlarmPushInfoList.Find(n => n.AlarmType == AlarmPushInfoType.devAlarm && n.devAlarmInfo.DevId == devAlarm.devAlarmInfo.DevId);
                if (CompletDevealarm == null) return;
            }

        }
    }
    public void OnFullViewChange(bool isOn)
    {
        if (isOn )
        {
            ShowAlarmPushWindow(false );
        }
        else
        {
            
            ShowAlarmPushWindow(true );
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
                AllPerAlarmPushInfo.Add(alarm);
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
        GameObject obj = Instantiate();
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

        // SetTween(obj);
        string devId = devAlarm.devAlarmInfo.Dev.DevID.ToString();
        obj.transform.GetChild(3).GetComponent<Text>().text = devId;
        int depId = (int)devAlarm.devAlarmInfo.Dev.ParentId;
        obj.transform.GetComponent<Button>().onClick.AddListener(() =>
 {
     DevBut_Click(devId, depId);
 });

    }
    /// <summary>
    /// 定位告警推送信息
    /// </summary>
    /// <param name="LocationaList"></param>
    public void GetLocationAlarmPushData(AlarmPushInfo LocationaList)
    {
        GameObject obj = Instantiate();
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
        else
        {
            ShowDevAlarmInformation(alarm);
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
            Debug.LogErrorFormat("：{0},gao警了！", alarm.locationAlarmInfo.Id);
            //    SetAlarmPushInFo(alarm);
            GetLocationAlarmPushData(alarm);
            CompleteAlarmPushInfoList.Add(alarm);
        }
        else
        {
            try
            {
                Debug.LogErrorFormat("：{0},xiao警了！", alarm.locationAlarmInfo.Id);
                string CurrentId = alarm.locationAlarmInfo.Id.ToString();
                AlarmPushInfo showAlarm = ShowAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == alarm.locationAlarmInfo.Id);
                if (showAlarm != null)
                {
                    ShowAlarmPushInfoList.Remove(showAlarm);
                    for (int i = 0; i < grid.transform.childCount; i++)
                    {
                        if (CurrentId == grid.transform.GetChild(i).GetChild(3).GetComponent<Text>().text)
                        {
                            int k = i;
                            CompleteAlarmPushInfoList.Remove(alarm);
                            grid.transform.GetChild(i).DOScaleX(0, 0.5f).OnComplete(() =>
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
    }

    /// <summary>
    /// 展示设备告警信息
    /// </summary>
    /// <param name="alarm"></param>
    public void ShowDevAlarmInformation(AlarmPushInfo alarm)
    {

        if (alarm.devAlarmInfo.Level != Abutment_DevAlarmLevel.无)
        {
            Debug.LogErrorFormat("区域：", alarm.locationAlarmInfo.Id);
            // SetAlarmPushInFo(alarm);
            GetDevAlarmPushData(alarm);
            CompleteAlarmPushInfoList.Add(alarm);
        }
        else
        {
            Debug.LogErrorFormat("区域：", alarm.locationAlarmInfo.Id);
            string CurrentId = alarm.devAlarmInfo.Dev.DevID;
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                int k = i;
                if (CurrentId == grid.transform.GetChild(i).GetChild(3).GetComponent<Text>().text)
                {
                    //  grid.transform.GetChild(i).GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 0.48f).OnComplete(() =>
                    //  {
                    // });
                    grid.transform.GetChild(i).DOScaleX(0, 0.5f).OnComplete(() =>
                    {
                        DestroyImmediate(grid.transform.GetChild(k).gameObject);
                        if (grid.transform.childCount != 0)
                        {
                            if (TitleObj.activeInHierarchy)
                            {
                                //   RemoveChildTween(k);

                            }
                            else
                            {
                                RemoveAlarmTween(k);
                            }

                        }
                        else
                        {
                            //isPushInfo = false;
                        }

                    });

                }
            }
            //for (int b = 0; b < overGrid.transform.childCount; b++)
            //{
            //    if (CurrentId == overGrid.transform.GetChild(b).GetChild(3).GetComponent<Text>().text)
            //    {
            //        AlarmNum = AlarmNum - 1;
            //        if (AlarmNum == 0)
            //        {
            //            ExitAlarmNumTween();



            //        }
            //        else if (AlarmNum > 0)
            //        {
            //            TitleObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<color=#58A2B4FF>还有</color>" + AlarmNum.ToString() + "<color=#58A2B4FF>条未处理告警未显示</color>";
            //            isPushInfo = false;
            //        }
            //    }
            //}
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
                ShowAlarmPushInfoList.Add(alarm);

                for (int i = 0; i < grid.transform.childCount; i++)
                {
                    int mun = 50;
                    RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();

                    if (ObjY != null)
                    {
                        float posY = ObjY.anchoredPosition3D.y + mun;
                        AlarmTw = ObjY.DOAnchorPos(new Vector2(0, posY), 0.5F);
                    }
                    AlarmTw.OnComplete(() =>
                    {

                        AlarmTw.Kill();
                        isPushInfo = false;
                    });
                }
            }
            else if (grid.transform.childCount > 5)
            {

                TitleObj.SetActive(true);
                AlarmNum = AlarmNum + 1;
                ShowAlarmPushInfoList.Add(alarm);
                TitleObj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "<color=#58A2B4FF>还有</color>" + AlarmNum.ToString() + "<color=#58A2B4FF>条未处理告警未显示</color>";
                string CloseId = grid.transform.GetChild(0).GetChild(3).GetComponent<Text>().text;
                int strId = int.Parse(CloseId);
                AlarmPushInfo showAlarm = ShowAlarmPushInfoList.Find(m => m.locationAlarmInfo.Id == strId);
                if (showAlarm != null)
                {
                    CloseAlarmPushInfoList.Add(showAlarm);
                    ShowAlarmPushInfoList.Remove(showAlarm);
                }

                TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-70, 197, 0), 0.5f).OnComplete(() =>
                {
                    TitleObj.transform.GetChild(0).GetComponent<Image>().DOFillAmount(1, 0.5f).OnComplete(() =>
                    {
                        grid.transform.GetChild(0).DOScaleX(0, 0.3f).OnComplete(() =>
                        {
                            DestroyImmediate(grid.transform.GetChild(0).gameObject);

                            for (int i = 0; i < grid.transform.childCount; i++)
                            {
                                RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();
                                if (ObjY != null)
                                {

                                    float posY = ObjY.anchoredPosition3D.y + 50;
                                    AlarmTween = ObjY.DOAnchorPos(new Vector2(0, posY), 0.5F);
                                }
                            }
                            AlarmTween.OnComplete(() =>
                            {

                                AlarmTween.Kill();
                                isPushInfo = false;
                            });
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
            Tween aa = DOTween.To(() => TextBg.fillAmount, x => TextBg.fillAmount = x, 1, 0.5f);
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
        Item.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, -104, 0), 0.5f)
        .OnComplete(() =>
        {
            Tween s = OutLevel1.DORotate(new Vector3(0, 0, 60), 0.1f).SetLoops(-1, LoopType.Yoyo);//外框旋转
            Tween t = OutLevel2.DORotate(new Vector3(0, 0, 60), 0.1f).SetLoops(-1, LoopType.Yoyo);//外框旋转
            Tween Alarm = Level.DORotate(new Vector3(0, 0, 60), 0.1f).SetLoops(-1, LoopType.Yoyo);//小图标旋转
            OutLevel1.DOScale(scale, 0.1f);
            OutLevel2.DOScale(scale, 0.1f);
            Level.DOScale(scale, 0.1f);

            mask1.DOLocalMoveX(-300, 0.1f).OnComplete(() =>
            {
                s.Kill();
                t.Kill();
                Alarm.Kill();
                OutLevel1.DOScale(V3, 0.1f);
                OutLevel2.DOScale(V3, 0.1f);
                OutLevel1.DORotate(new Vector3(0, 0, 0), 0.001f);
                OutLevel2.DORotate(new Vector3(0, 0, 0), 0.001f);
                Level.DORotate(new Vector3(0, 0, 0), 0.001f);
                Level.DOScale(V3, 0.1f).OnComplete(() =>
                {
                    mask1.gameObject.SetActive(false);
                    SetAlarmPushInFo(alarm);
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
    public GameObject Instantiate()
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
    public void DevBut_Click(string devId, int DepID)
    {

        RoomFactory.Instance.FocusDev(devId, DepID);
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
        TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(116, 197, 0), 0.5f).OnComplete(() =>
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
        if (num == 0)
        {
            isPushInfo = false;
        }
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();

            if (num - 1 > i)
            {
             
                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.5F);
                }

            }
            else if (num - 1 == i)
            {
                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.5F).OnComplete(() =>
                    {
                        isPushInfo = false;
                    });
                }
            }
        }   
    }


    private Tweener tw;
    /// <summary>
    /// 消除告警，其余的向下移动
    /// </summary>
    public void RemoveChildTween(int num)
    {
        DestroyImmediate(grid.transform.GetChild(num).gameObject);
        if (num == 0)
        {
            isPushInfo = false;
        }
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            RectTransform ObjY = grid.transform.GetChild(i).GetComponent<RectTransform>();

            if (num - 1 > i)
            {

                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.5F);
                }

            }
            else if (num - 1 == i)
            {
                if (ObjY != null)
                {
                    float posY = ObjY.anchoredPosition3D.y - 50;
                    ObjY.DOAnchorPos(new Vector2(0, posY), 0.5F).OnComplete(() =>
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
                    TitleObj.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(116, 197, 0), 0.5f).OnComplete(() =>
                    {
                        try
                        {
                            TitleObj.SetActive(false);
                            GetLocationAlarmPushData(CloseAlarmPushInfoList[0]);
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
                    GetLocationAlarmPushData(CloseAlarmPushInfoList[0]);
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
        if (b )
        {
            AlarmPushWindow.SetActive(true );
        }
        else
        {
            AlarmPushWindow.SetActive(false );
        }
    }
}




