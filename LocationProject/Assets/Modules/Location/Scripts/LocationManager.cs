using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Base.Common.Extensions;
using UnityEngine;
using UnityEngine.UI;
using StardardShader;
using MonitorRange;

public class LocationManager : MonoBehaviour
{

    public static LocationManager Instance;
    /// <summary>
    /// 显示离开状态人员实时位置
    /// </summary>
    public bool isShowLeavePerson;
    /// <summary>
    /// 显示定位真实高度，不以楼层地板高度来设置
    /// </summary>
    public bool isShowRealLocationHeight;
    /// <summary>
    /// 是否开启人员打射线，来设置人员的高度，因为可能存在像主厂房二楼一样，地板平面高低不齐
    /// </summary>
    public bool isSetPersonHeightByRay;
    /// <summary>
    /// 是否设置人员父物体,用于解决在楼层展开时，人员正确根据楼层展开显示在楼层地板上
    /// </summary>
    public bool isSetPersonObjParent;

    public Vector3 LocationOffsetScale = new Vector3(1.3125f, 0.9622f, 1.3607f);//new Vector3(1.27f, 0.9622f, 1.27f)
    /// <summary>
    /// 位置偏移量,就是计算出来的三维坐标原点位置
    /// </summary>
    public Vector3 axisZero;
    /// <summary>
    /// 方向校准
    /// </summary>
    public Vector3 direction = Vector3.one;
    ///// <summary>
    ///// 定位卡信息
    ///// </summary>
    //private List<Tag> tags;
    ///// <summary>
    ///// 定位卡位置信息
    ///// </summary>
    //private List<TagPosition> tagsPos;
    /// <summary>
    /// 定位卡的空间父物体
    /// </summary>
    public Transform tagsParent;
    /// <summary>
    /// 人物预设
    /// </summary>
    public GameObject characterPrefab;
    /// <summary>
    /// 卡对应人物列表
    /// </summary>
    public Dictionary<string, LocationObject> code_character;
    //private bool isRefleshPositionComplete = true;//是否可以刷新人员位置及相关信息信息
    /// <summary>
    /// 位置变化速度
    /// </summary>
    public float damper = 5;
    /// <summary>
    /// 定位用户名称UI预设
    /// </summary>
    public RectTransform userNameUI;
    /// <summary>
    /// 定位人员信息UI
    /// </summary>
    public RectTransform PersonInfoUIPrefab;
    /// <summary>
    /// 更新实时位置信息协程
    /// </summary>
    private Coroutine coroutine;
    ///// <summary>
    ///// 历史轨迹集合
    ///// </summary>
    //public List<LocationHistoryPath> historyPaths;

    private bool isShowLocation;
    /// <summary>
    /// 是否显示定位模式下
    /// </summary>
    public bool IsShowLocation
    {
        get
        {
            return isShowLocation;
        }

        set
        {
            isShowLocation = value;
        }
    }
    private Thread addU3dPositionsThread;//添加3D历史位置数据线程

    /// <summary>
    /// 根节点内容
    /// </summary>
    public string RootNodeName = "四会热电厂";

    public List<Archor> archors;
    /// <summary>
    /// 是否创建人员结束
    /// </summary>
    private bool isCreatePersonComplete;
    /// <summary>
    /// 是否创建人员结束
    /// </summary>
    public bool IsCreatePersonComplete
    {
        get
        {
            return isCreatePersonComplete;
        }

        set
        {
            isCreatePersonComplete = value;
        }
    }

    /// <summary>
    /// 是否创建人员结束
    /// </summary>
    private bool isInitLocationAlarms;
    /// <summary>
    /// 是否创建人员结束
    /// </summary>
    public bool IsInitLocationAlarms
    {
        get
        {
            return isInitLocationAlarms;
        }

        set
        {
            isInitLocationAlarms = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //tagsPos = new List<TagPosition>();
        //historyPaths = new List<LocationHistoryPath>();
        code_character = new Dictionary<string, LocationObject>();
        //Debug.LogError("RefleshTags!");
        //ShowLocation();
        if (exitFocusbtn)
        {
            exitFocusbtn.GetComponentInChildren<Button>().onClick.AddListener(() => { RecoverBeforeFocusAlign(); });
        }
        //ShowLocation();

        if (transparentToggle != null)
        {
            transparentToggle.onValueChanged.AddListener(TransparentToggle_ValueChanged);
        }

        Loom.StartSingleThread(() =>
        {
            archors = CommunicationObject.Instance.GetArchors();
            //Loom.DispatchToMainThread(() =>
            //{
            //    //Debug.LogError("点数：" + positions.Count);

            //});
        });

        CommunicationCallbackClient.Instance.alarmHub.OnLocationAlarmRecieved += AlarmHub_OnLocationAlarmRecieved; ;

    }


    [Tooltip("是否加载数据库配置信息")]
    public bool isChuLingDemo;//是否加载数据库，定位配置信息

    public static TransferOfAxesConfig AxesConfig;

    /// <summary>
    /// 获取数据库中保存的坐标系转换配置信息
    /// </summary>
    public static void LoadTransferOfAxesConfig()
    {
        try
        {
            if (Instance.isChuLingDemo) return;
            AxesConfig = CommunicationObject.Instance.GetTransferOfAxesConfig();
            Instance.axisZero = StringToVector3(AxesConfig.Zero.Value);
            Instance.direction = StringToVector3(AxesConfig.Direction.Value);
            Instance.LocationOffsetScale = StringToVector3(AxesConfig.Scale.Value);
        }
        catch (Exception ex)
        {
            Log.Error("LoadTransferOfAxesConfig", ex);
        }
    }
    public static Vector3 StringToVector3(string value)
    {
        string[] pars = value.Split(',');
        return new Vector3(pars[0].ToFloat(), pars[1].ToFloat(), pars[2].ToFloat());
    }

    //todo:添加修改坐标系转换配置信息的界面和代码


    /// <summary>
    /// 显示人员定位
    /// </summary>
    public void ShowLocation()
    {
        Debug.LogError("ShowLocation");
        IsShowLocation = true;
        //code_character = new Dictionary<string, LocationObject>();
        //ThreadManager.Run(
        //() =>
        //{
        //    RefleshTags();
        //}, () =>
        //{
        //    ShowTagsPosition();
        //}, "ShowLocation");

        InvokeRepeating("ShowTagsPosition", 0, CommunicationObject.Instance.TagPosRefreshInterval);//这里是每隔20秒重复刷新显示

        //StartAddU3dPositions();
    }

    /// <summary>
    /// 关闭并清除定位人员
    /// </summary>
    public void HideAndClearLocation()
    {
        IsShowLocation = false;
        //OnDisable();
        OnDestroy();
        ClearCharacter();

    }

    /// <summary>
    /// 关闭定位
    /// </summary>
    public void HideLocation()
    {
        IsShowLocation = false;
        //OnDisable();
        OnDestroy();
        HideCharacter();

    }

    private void OnEnable()
    {
        if (addU3dPositionsThread != null)
        {
            addU3dPositionsThread.Resume();
        }
    }

    public void OnDisable()
    {
        CancelInvoke("ShowTagsPosition");
        //if (coroutine != null)
        //{
        //    StopCoroutine(coroutine);
        //}
        if (addU3dPositionsThread != null)
        {
            addU3dPositionsThread.Suspend();
        }
    }

    private void OnDestroy()
    {
        CancelInvoke("ShowTagsPosition");
        try
        {
            if (addU3dPositionsThread != null)
            {
                addU3dPositionsThread.Abort();
            }
        }
        catch (Exception ex)
        {
            Log.Error("LocationManager.OnDestroy", ex.ToString());
        }
        //Debug.LogError("OnDestroy");
    }

    /// <summary>
    /// 开始发送3D保存位置历史数据
    /// </summary>
    public void StartAddU3dPositions()
    {
        addU3dPositionsThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    if (code_character != null)
                    {
                        List<U3DPosition> u3dlist = new List<U3DPosition>();
                        List<LocationObject> objList = code_character.Values.ToList();

                        for (int i = 0; i < objList.Count; i++)
                        {
                            CreateU3DPosition(objList[i].tagPosInfo, objList[i].currentPos);
                            //t.StartSendData();
                        }
                        CommunicationObject.Instance.AddU3DPosition(u3dlist);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("StartAddU3dPositions", ex.ToString());
                }
                Thread.Sleep(500);
            }
        });
        addU3dPositionsThread.Start();
    }

    ///// <summary>
    ///// 刷新标签
    ///// </summary>
    //private List<Tag> RefleshTags()
    //{
    //    Tag[] tags0 = CommunicationObject.Instance.GetTags();
    //    if (tags0 != null)
    //    {
    //        _tags = new List<Tag>(tags0);
    //    }

    //    //List<Tag> tagsT = new List<Tag>();
    //    //foreach (Tag t in tags)
    //    //{
    //    //    for (int i = 0; i < 5; i++)
    //    //    {
    //    //        Tag tt = new Tag();
    //    //        tt.Id = t.Id + i;
    //    //        tt.Code = t.Code + i;
    //    //        tagsT.Add(tt);
    //    //    }
    //    //}
    //    //tags.AddRange(tagsT);

    //    return _tags;
    //}

    public void On_Test()
    {
        List<TagPosition> tagsPos = new List<TagPosition>();
        for (int i = 1; i <= 20; i++)
        {
            tagsPos = CommunicationObject.Instance.GetRealPositons();
            Debug.LogError("On_Test:" + i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isRefleshPositionComplete)
        //{
        //    RefleshTagsPosition(tagsPos);
        //}
        if (IsCreatePersonComplete && MonitorRangeManager.Instance.IsCreateAreaComplete && IsInitLocationAlarms == false)
        {
            IsInitLocationAlarms = true;
            InitLocationAlarms();
        }
    }

    List<Tag> _tags;
    //List<TagPosition> tagsPos; //2019_03_02_cww:因为位置信息已经存到Tag里面的Pos传过来了 没必要另外放一个列表
    /// <summary>
    /// 显示定位卡的位置信息
    /// </summary>
    public void ShowTagsPosition()
    {
        if (IsShowLocation == false) return;
        CreateTagsParent();

        ////tagsPos.Clear();
        //if (!CommunicationObject.Instance.isAsync)
        //{
        //    #region 原本
        //    if (!isBusy)
        //    {
        //        isBusy = true;
        //        ThreadManager.Run(
        //                () =>
        //                {
        //                    Tag[] tags0 = CommunicationObject.Instance.GetTags();
        //                    if (tags0 != null)
        //                    {
        //                        _tags = new List<Tag>(tags0);
        //                    }
        //                    return _tags;

        //                }, (tags) =>
        //                {
        //                    RefleshTagsPosition(tags);
        //                    isBusy = false;
        //                }, "GetTags");
        //    }
        //    else
        //    {
        //        Log.Alarm("ShowTagsPosition", "isBusy:" + isBusy);
        //    }
        //    #endregion
        //}
        //else
        //{
        //    #region WCF异步方法
        //    if (!isBusy)
        //    {
        //        isBusy = true;
        //        CommunicationObject.Instance.GetTagsAsync((tagsT) =>
        //        {
        //            if (tagsT != null)
        //            {
        //                _tags = new List<Tag>(tagsT);
        //            }
        //            RefleshTagsPosition(_tags);
        //            isBusy = false;
        //        });
        //    }
        //    else
        //    {
        //        Log.Alarm("ShowTagsPosition", "isBusy:" + isBusy);
        //    }
        //    #endregion
        //}

        if (!isBusy)
        {
            isBusy = true;
            CommunicationObject.Instance.GetTagsAsync((tagsT) =>
            {
                if (tagsT != null)
                {
                    _tags = new List<Tag>(tagsT);
                }
                RefleshTagsPosition(_tags);
                isBusy = false;
            });
        }
        else
        {
            Log.Alarm("ShowTagsPosition", "isBusy:" + isBusy);
        }
    }

    //private void RefleshTagsPosition(List<TagPosition> tagsPosT)
    //{
    //    if (tagsPosT.Count == 0) return;
    //    if (IsShowLocation == false) return;
    //    if (tags == null) return;
    //    TagPosition tagp = tagsPosT.Find((item) => item.Tag == "0009");
    //    //Debug.LogErrorFormat("名称:{0},Code:{1}", tagp.Tag, tagp.X);
    //    //Debug.LogError("RefleshTagsPosition");
    //    List<string> keyslist = code_character.Keys.ToList();
    //    foreach (Tag tag in tags)
    //    {
    //        if (code_character.ContainsKey(tag.Code))
    //        {
    //            LocationObject locationObject = code_character[tag.Code];
    //            locationObject.InitPersonnel();
    //            SetTagPostion(locationObject, tag, tagsPosT);
    //            //code_character[tag.Code].gameObject.SetActive(true);
    //            keyslist.Remove(tag.Code);
    //        }
    //        else
    //        {
    //            Transform tran = CreateCharacter(tag);
    //            LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();//这里就会脚本中的
    //            locationObject.Init(tag);
    //            code_character.Add(tag.Code, locationObject);
    //            SetTagPostion(locationObject, tag, tagsPosT);
    //        }
    //    }

    //    foreach (string key in keyslist)
    //    {
    //        if (key == "55555") continue;
    //        DestroyImmediate(code_character[key].gameObject);
    //        code_character.Remove(key);
    //    }
    //}

    private void RefleshTagsPosition(List<Tag> tags)
    {
        if (IsShowLocation == false) return;
        if (tags == null) return;
        List<string> keyslist = code_character.Keys.ToList();
        foreach (Tag tag in tags)
        {
            if (tag.IsActive == false) continue;
            if (tag.PersonId == null) continue;
            //TagPosition tagp = tagsPos.Find((item) => item.Tag == tag.Code);
            TagPosition tagp = tag.Pos;
            if (tagp == null) continue;
            if (code_character.ContainsKey(tag.Code))
            {
                LocationObject locationObject = code_character[tag.Code];
                locationObject.InitPersonnel();
                //SetTagPostion(locationObject, tag, tagsPosT);
                SetTagPostion(locationObject, tagp);
                //code_character[tag.Code].gameObject.SetActive(true);
                keyslist.Remove(tag.Code);
            }
            else
            {
                Transform tran = CreateCharacter(tag);
                LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();//这里就会脚本中的
                locationObject.Init(tag);
                code_character.Add(tag.Code, locationObject);
                //SetTagPostion(locationObject, tag, tagsPosT);
                SetTagPostion(locationObject, tagp);
            }
        }
        foreach (string key in keyslist)
        {
            if (key == "55555") continue;
            DestroyImmediate(code_character[key].gameObject);
            code_character.Remove(key);
        }
        IsCreatePersonComplete = true;
    }

    //private void RefleshTagsPosition(List<TagPosition> tagsPosT)
    //{
    //    if (tagsPosT.Count == 0) return;
    //    if (IsShowLocation == false) return;
    //    if (_tags == null) return;
    //    //TagPosition tagp = tagsPosT.Find((item) => item.Tag == "0009");
    //    //Debug.LogErrorFormat("名称:{0},Code:{1}", tagp.Tag, tagp.X);
    //    //Debug.LogError("RefleshTagsPosition");
    //    List<string> keyslist = code_character.Keys.ToList();
    //    foreach (Tag tag in _tags)
    //    {
    //        if (tag.IsActive == false) continue;
    //        if (tag.Person == null) continue;
    //        TagPosition tagp = tagsPos.Find((item) => item.Tag == tag.Code);
    //        if (tagp == null) continue;
    //        if (code_character.ContainsKey(tag.Code))
    //        {
    //            LocationObject locationObject = code_character[tag.Code];
    //            locationObject.InitPersonnel();
    //            //SetTagPostion(locationObject, tag, tagsPosT);
    //            SetTagPostion(locationObject, tagp);
    //            //code_character[tag.Code].gameObject.SetActive(true);
    //            keyslist.Remove(tag.Code);
    //        }
    //        else
    //        {
    //            Transform tran = CreateCharacter(tag);
    //            LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();//这里就会脚本中的
    //            locationObject.Init(tag);
    //            code_character.Add(tag.Code, locationObject);
    //            //SetTagPostion(locationObject, tag, tagsPosT);
    //            SetTagPostion(locationObject, tagp);
    //        }
    //    }
    //    foreach (string key in keyslist)
    //    {
    //        if (key == "55555") continue;
    //        DestroyImmediate(code_character[key].gameObject);
    //        code_character.Remove(key);
    //    }
    //    IsCreatePersonComplete = true;
    //}

    bool isBusy;

    /// <summary>
    /// AddTagTest
    /// </summary>
    [ContextMenu("AddTagTest")]
    public void AddTagTest()
    {
        //Tag t = new Tag();
        //t.Code = "55555";
        //Transform tran = CreateCharacter(t);
        //LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();//这里就会脚本中的
        //locationObject.Init(t);
        //code_character.Add(t.Code, locationObject);
        //List<TagPosition> tagsPosT = new List<TagPosition>();
        //TagPosition TAGP = new TagPosition();
        //TAGP.AreaId = 2;
        //TAGP.Tag = "55555";
        ////locationObject.transform.position = new Vector3(0, 2, 0);
        //Vector3 p = GetDisRealVector(new Vector3(0, 2, 0));
        //TAGP.X = p.x;
        //TAGP.Y = 2;
        //TAGP.Z = p.z;
        //tagsPosT.Add(TAGP);
        //SetTagPostion(locationObject, t, tagsPosT);
    }

    //public IEnumerator ShowTagsPosition_Coroutine()
    //{
    //    yield return null;
    //    while (true)
    //    {
    //        if (!isBusy)
    //        {
    //            isBusy = true;
    //            Loom.StartSingleThread(() =>
    //            {
    //                List<TagPosition> tagsPos = new List<TagPosition>();
    //                tagsPos = CommunicationObject.Instance.GetTagsPosition();
    //                isBusy = false;
    //                Loom.DispatchToMainThread(() =>
    //                {
    //                    if (tags == null) return;
    //                    foreach (Tag tag in tags)
    //                    {
    //                        if (code_character.ContainsKey(tag.Code))
    //                        {
    //                            LocationObject locationObject = code_character[tag.Code].gameObject.GetComponent<LocationObject>();
    //                            SetTagPostion(locationObject, tag, tagsPos);
    //                            //code_character[tag.Code].gameObject.SetActive(true);
    //                        }
    //                        else
    //                        {
    //                            Transform tran = CreateCharacter(tag);
    //                            LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();
    //                            locationObject.Init(tag);
    //                            code_character.Add(tag.Code, tran);
    //                            SetTagPostion(locationObject, tag, tagsPos);
    //                        }
    //                    }
    //                }, true);
    //            }, System.Threading.ThreadPriority.Normal, true);
    //        }
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}

    /// <summary>
    /// 设置定位卡的位置信息
    /// </summary>
    private void SetTagPostion(LocationObject locationObject, Tag tag, List<TagPosition> tagsPos)
    {
        if (tagsPos == null) return;
        if ("0003694" == tag.Code)
        {
            int i = 0;
        }

        TagPosition tagp = tagsPos.Find((item) => item.Tag == tag.Code);
        //print(string.Format("{0}:({1},{2},{3})", tag.Name, tagp.X, tagp.Y, tagp.Z));


        if (tagp != null)
        {
            locationObject.SetPositionInfo(tagp);
            locationObject.gameObject.SetActive(true);
        }
        else
        {
            locationObject.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置定位卡的位置信息
    /// </summary>
    private void SetTagPostion(LocationObject locationObject, TagPosition tagp)
    {

        if (tagp != null)
        {
            locationObject.SetPositionInfo(tagp);
            locationObject.gameObject.SetActive(true);
        }
        else
        {
            locationObject.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 清除人物
    /// </summary>
    public void ClearCharacter()
    {
        try
        {
            List<LocationObject> objs = new List<LocationObject>(code_character.Values);
            foreach (LocationObject obj in objs)
            {
                //DestroyImmediate(obj.gameObject);
                Destroy(obj.gameObject);
            }
        }
        catch (Exception ex)
        {

        }

        code_character.Clear();
    }

    /// <summary>
    /// 隐藏人物
    /// </summary>
    public void HideCharacter()
    {
        try
        {
            List<LocationObject> objs = new List<LocationObject>(code_character.Values);
            foreach (LocationObject obj in objs)
            {
                obj.gameObject.SetActive(false);
            }
        }
        catch (Exception ex)
        {

        }

    }

    /// <summary>
    /// 根据时间戳，获取距离现在的时间
    /// </summary>
    /// <param name="tagp"></param>
    /// <returns></returns>
    public static float GetTimeInterval(long timeT)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        startTime = startTime.AddMilliseconds(timeT);
        float seconds = (float)(DateTime.Now - startTime).TotalSeconds;
        return seconds;
    }

    /// <summary>
    /// 根据时间戳，获取DateTime类型时间
    /// </summary>
    /// <param name="tagp"></param>
    /// <returns></returns>
    public static DateTime GetTimestampToDateTime(long timeT)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        startTime = startTime.AddMilliseconds(timeT);
        //float seconds = (float)(DateTime.Now - startTime).TotalSeconds;
        return startTime;
    }

    /// <summary>
    /// 创建定位卡的空间父物体
    /// </summary>
    private Transform CreateTagsParent()
    {
        tagsParent = transform.Find("Location Parent");
        if (tagsParent == null)
        {
            GameObject o = new GameObject("Location Parent");
            tagsParent = o.transform;
            tagsParent.SetParent(transform);
            tagsParent.transform.localPosition = Vector3.zero;
        }
        return tagsParent;
    }

    /// <summary>
    /// 创建定位人物
    /// </summary>
    private Transform CreateCharacter(Tag tag)
    {
        GameObject o = CreateCharacter();
        o.name = tag.Name + tag.Code;
        return o.transform;
    }

    private GameObject CreateCharacter()
    {
        GameObject o = Instantiate(characterPrefab);
        o.transform.SetParent(tagsParent);
        return o;
    }

    /// <summary>
    /// 获取位置偏移量
    /// </summary>
    public Vector3 GetPosOffset()
    {
        return axisZero;
    }


    #region CAD和Unity位置转换
    /// <summary>
    /// CAD位置转Unity位置
    /// </summary>
    /// <param name="cadPos"></param>
    /// <param name="isLocalPos"></param>
    /// <returns></returns>
    public static Vector3 CadToUnityPos(Vector3 cadPos, bool isLocalPos)
    {
        Vector3 pos;
        if (!isLocalPos)
        {
            pos = GetRealVector(cadPos);
        }
        else
        {
            pos = CadToUnityLocalPos(cadPos);
        }
        return pos;
    }

    public static Vector3 UnityToCadPos(Vector3 unityPos, bool isLocalPos)
    {
        Vector3 pos;
        if (!isLocalPos)
        {
            pos = GetCadVector(unityPos);
        }
        else
        {
            pos = UnityLocalPosToCad(unityPos);
        }
        return pos;
    }
    /// <summary>
    /// 获取CAD在3D中的LocalPos
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private static Vector3 CadToUnityLocalPos(Vector3 p)
    {
        Vector3 pos;
        Vector3 offsetScale = Instance.LocationOffsetScale;
        if (offsetScale.y == 0)
        {
            pos = new Vector3(p.x / offsetScale.x, p.y / offsetScale.x, p.z / offsetScale.z);
        }
        else
        {
            pos = new Vector3(p.x / offsetScale.x, p.y / offsetScale.y, p.z / offsetScale.z);
        }
        return pos;
    }
    /// <summary>
    /// UnityLocalPos转CADPos
    /// </summary>
    /// <param name="localPos"></param>
    /// <returns></returns>
    private static Vector3 UnityLocalPosToCad(Vector3 localPos)
    {
        Vector3 tempPos;
        Vector3 offsetScale = Instance.LocationOffsetScale;
        if (offsetScale.y == 0)
        {
            tempPos = new Vector3(localPos.x * offsetScale.x, localPos.y * offsetScale.x, localPos.z * offsetScale.z);
        }
        else
        {
            tempPos = new Vector3(localPos.x * offsetScale.x, localPos.y * offsetScale.y, localPos.z * offsetScale.z);
        }
        return tempPos;
    }
    #endregion
    /// <summary>
    /// Postion转换成CAD位置
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetCadVector(Vector3 position)
    {
        position -= Instance.axisZero;
        Vector3 tempPos;
        if (Instance.LocationOffsetScale.y == 0)
        {
            tempPos = new Vector3(position.x * Instance.LocationOffsetScale.x, position.y * Instance.LocationOffsetScale.x, position.z * Instance.LocationOffsetScale.z);
        }
        else
        {
            tempPos = new Vector3(position.x * Instance.LocationOffsetScale.x, position.y * Instance.LocationOffsetScale.y, position.z * Instance.LocationOffsetScale.z);
        }
        tempPos = new Vector3(tempPos.x / Instance.direction.x, tempPos.y, tempPos.z / Instance.direction.z);
        return tempPos;
    }
    /// <summary>
    /// 根据实际比例来，获取3D场景的位置
    /// </summary>
    /// <param name="p"></param>
    public static Vector3 GetRealVector(Vector3 p)
    {
        Vector3 pos = GetRealSizeVector(p);
        pos = pos + Instance.axisZero;
        return pos;
    }

    /// <summary>
    /// 根据实际比例来，获取3D场景的位置
    /// </summary>
    /// <param name="p"></param>
    public static Vector3 GetRealVector(TransformM tranM)
    {
        Vector3 pos = new Vector3((float)tranM.X, (float)tranM.Y, (float)tranM.Z);
        return GetRealVector(pos);
    }

    /// <summary>
    /// 根据实际比例来，来计算在3D场景的尺寸
    /// </summary>
    /// <param name="p"></param>
    public static Vector3 GetRealSizeVector(Vector3 p)
    {
        //这里由于现实场景跟三维模型的角度不同
        //p = new Vector3(-p.x, p.y, -p.z);
        p = new Vector3(Instance.direction.x * p.x, p.y, Instance.direction.z * p.z);

        Vector3 pos;
        if (Instance.LocationOffsetScale.y == 0)
        {
            pos = new Vector3(p.x / Instance.LocationOffsetScale.x, p.y / Instance.LocationOffsetScale.x, p.z / Instance.LocationOffsetScale.z);
        }
        else
        {
            pos = new Vector3(p.x / Instance.LocationOffsetScale.x, p.y / Instance.LocationOffsetScale.y, p.z / Instance.LocationOffsetScale.z);
        }

        return pos;
    }

    /// <summary>
    /// 根据实际比例来，计算现实世界的位置
    /// </summary>
    /// <param name="p"></param>
    public static Vector3 GetDisRealVector(Vector3 p)
    {
        p = p - Instance.axisZero;
        Vector3 pos = GetDisRealSizeVector(p);

        return pos;
    }

    /// <summary>
    /// 根据实际比例来，来计算在现实世界的尺寸
    /// </summary>
    /// <param name="p"></param>
    public static Vector3 GetDisRealSizeVector(Vector3 p)
    {
        Vector3 pos;
        if (Instance.LocationOffsetScale.y == 0)
        {
            pos = new Vector3(p.x * Instance.LocationOffsetScale.x, p.y * Instance.LocationOffsetScale.x, p.z * Instance.LocationOffsetScale.z);
        }
        else
        {
            pos = new Vector3(p.x * Instance.LocationOffsetScale.x, p.y * Instance.LocationOffsetScale.y, p.z * Instance.LocationOffsetScale.z);
        }

        //这里由于现实场景跟三维模型的角度不同
        pos = new Vector3(Instance.direction.x * pos.x, pos.y, Instance.direction.z * pos.z);
        return pos;
    }


    /// <summary>
    /// 获取定位卡的状态，是否是待机状态（就是省电状态）
    /// 卡就两种状态一种是正常监控状态，一种是省电状态
    /// </summary>
    public bool IsStandby(TagPosition tagposition)
    {
        string[] strs = tagposition.Flag.Split(':');
        if (strs.Length == 5)
        {
            if (strs[4] == "1")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 创建U3DPosition对象
    /// </summary>
    /// <param name="tp"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private U3DPosition CreateU3DPosition(TagPosition tp, Vector3 p)
    {
        U3DPosition u3dPos = new U3DPosition();
        u3dPos.Tag = tp.Tag;
        u3dPos.Time = tp.Time;
        u3dPos.Number = tp.Number;
        u3dPos.Power = tp.Power;
        u3dPos.Flag = tp.Flag;
        //Vector3 temp= LocationManager.GetDisRealVector(targetPos);
        u3dPos.X = p.x;
        u3dPos.Y = p.y;
        u3dPos.Z = p.z;
        return u3dPos;
    }

    #region 人员定位历史轨迹相关管理
    ////人员定位历史轨父物体
    //private Transform historyPathParent;



    ///// <summary>
    ///// 设置历史轨迹执行的值
    ///// </summary>
    //public void SetHistoryPath(float v)
    //{
    //    foreach (LocationHistoryPath hispath in historyPaths)
    //    {
    //        hispath.Set(v);
    //    }
    //}

    ///// <summary>
    ///// 清除历史轨迹路线
    ///// </summary>
    //public void ClearHistoryPaths()
    //{
    //    foreach (LocationHistoryPath path in historyPaths)
    //    {
    //        DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
    //        //DestroyImmediate(path.gameObject);
    //    }

    //    historyPaths.Clear();
    //}

    ///// <summary>
    ///// 创建历史轨迹父物体
    ///// </summary>
    //public Transform GetHistoryAllPathParent()
    //{
    //    if (historyPathParent == null)
    //    {
    //        //historyPathParent = GameObject.Find("HistoryPathParent").transform;
    //        historyPathParent = new GameObject("HistoryPathParent").transform;
    //        return historyPathParent;
    //    }
    //    else
    //    {
    //        return historyPathParent;
    //    }
    //}

    #endregion
    //public bool IsCreateHistoryPath(string code)
    //{
    //    bool b = historyPaths.Find((item) => item.code == code);
    //    return b;
    //}


    #region 人员镜头管理

    /// <summary>
    /// Current focus state.
    /// </summary>
    public bool IsFocus { protected set; get; }

    /// <summary>
    /// 转换聚焦,一个人员切到另一个人员
    /// </summary>
    [HideInInspector]
    public bool IsSwitchFocus;
    /// <summary>
    /// 摄像机聚焦前状态
    /// </summary>
    private AlignTarget beforeFocusAlign;
    /// <summary>
    /// 返回按钮
    /// </summary>
    public GameObject exitFocusbtn;

    /// <summary>
    /// 当前聚焦定位人员
    /// </summary>
    [HideInInspector]
    public LocationObject currentLocationFocusObj;

    /// <summary>
    /// 设置当前聚焦定位人员
    /// </summary>
    public void SetCurrentLocationFocusObj(LocationObject locationObjectT)
    {
        //UGUIFollowTarget follow = locationObjectT.personInfoUI.GetComponent<UGUIFollowTarget>();
        if (currentLocationFocusObj != null)
        {
            currentLocationFocusObj.HighlightOff();
        }
        if (locationObjectT != null)
        {
            locationObjectT.HighlightOnByFocus();

        }
        currentLocationFocusObj = locationObjectT;
    }

    /// <summary>
    /// 获取LocationObject通过PersonnelId
    /// </summary>
    public LocationObject GetLocationObjByTagId(int tagId)
    {
        List<LocationObject> listT = code_character.Values.ToList();
        LocationObject locationObjectT = listT.Find((item) => item.Tag.Id == tagId);
        return locationObjectT;
    }

    /// <summary>
    /// 是否执行了聚焦操作（是否执行了FocusPersonAndShowInfo()方法）
    /// </summary>
    public bool isFocusPersonAndShowInfo;

    /// <summary>
    /// 聚焦人员,根据tagId
    /// </summary>
    public void FocusPersonAndShowInfo(int tagId)
    {

        AlarmPushManage.Instance.ShowAlarmPushWindow(false );
        List<LocationObject> listT = code_character.Values.ToList();
        LocationObject locationObjectT = listT.Find((item) => item.Tag.Id == tagId);
        //if (locationObjectT.IsRenderEnable == false && FactoryDepManager.currentDep == FactoryDepManager.Instance)
        //{
        //    UGUIMessageBox.Show("当前人员不在监控区域！");
        //    return;
        //}
        if (locationObjectT == null) return;

        //if (!locationObjectT.isInLocationRange && locationObjectT.IsRenderEnable == false)
        //{
        //    UGUIMessageBox.Show("该人员不在监控范围内！");
        //    locationObjectT.personInfoUI.SetOpenOrClose(false);
        //    PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaDeselectNodeByData(locationObjectT.personInfoUI.personnel.TagId);
        //    PersonnelTreeManage.Instance.departmentDivideTree.Tree.DeselectNodeByData(tagId);
        //    return;
        //}
        Debug.LogError("locationObjectT:" + locationObjectT.name);
        isFocusPersonAndShowInfo = true;
        if (currentLocationFocusObj != null && currentLocationFocusObj != locationObjectT)
        {
            IsSwitchFocus = true;
            HideCurrentPersonInfoUI();
        }
        else
        {
            IsSwitchFocus = false;
            if (currentLocationFocusObj == locationObjectT) return;
        }

        isFocusPersonAndShowInfo = false;
        //Debug.LogError("FocusPersonAndShowInfo:" + tagId);
        SetCurrentLocationFocusObj(locationObjectT);
        if (!IsFocus)
        {
            beforeFocusAlign = CameraSceneManager.Instance.GetCurrentAlignTarget();
        }
        RoomFactory.Instance.FocusNodeForFocusPerson(locationObjectT.currentDepNode, () =>
        {

            FocusPerson(locationObjectT.alignTarget);

            if (locationObjectT.personInfoUI != null)
            {
                locationObjectT.personInfoUI.SetOpenOrClose(true);
            }
        }, false);


        //if (currentLocationFocusObj != null && currentLocationFocusObj != locationObjectT)
        //{
        //    IsSwitchFocus = true;
        //    HideCurrentPersonInfoUI();
        //}
        //else
        //{
        //    IsSwitchFocus = false;
        //}
        //SetCurrentLocationFocusObj(locationObjectT);
        ////CameraSceneManager.Instance.FocusTarget(locationObjectT.alignTarget);


        //FocusPerson(locationObjectT.alignTarget);
        ////IsBelongtoCurrentDep(locationObjectT);

        //if (locationObjectT.personInfoUI != null)
        //{
        //    //locationObjectT.personInfoUI.SetContentGridActive(true);
        //    //locationObjectT.personInfoUI.SetContentToggle(true);
        //    locationObjectT.personInfoUI.SetOpenOrClose(true);
        //}


    }

    /// <summary>
    /// 判断当前人员是否在当前区域下，并执行相关操作
    /// </summary>
    /// <param name="locationObjectT"></param>
    /// <returns></returns>
    public static bool IsBelongtoCurrentDep(LocationObject locationObjectT)
    {
        List<DepNode> depNodeListT = FactoryDepManager.currentDep.GetComponentsInChildren<DepNode>().ToList();
        if (!depNodeListT.Contains(locationObjectT.currentDepNode))
        {
            //RoomFactory.Instance.ChangeDepNodeNoTween();
            ////RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);

            //LocationManager.Instance.RecoverBeforeFocusAlignToOrigin();
            return false;
        }
        return true;
    }

    /// <summary>
    /// 聚焦人员,根据Id（数据库生成的）不是Code
    /// </summary>
    public void FocusPerson(int id)
    {

        List<LocationObject> listT = code_character.Values.ToList();
        LocationObject locationObjectT = listT.Find((item) => item.Tag.Id == id);
        if (locationObjectT == null) return;
        if (currentLocationFocusObj != null && currentLocationFocusObj != locationObjectT)
        {
            IsSwitchFocus = true;
            HideCurrentPersonInfoUI();
        }
        else
        {
            IsSwitchFocus = false;
        }
        SetCurrentLocationFocusObj(locationObjectT);
        //CameraSceneManager.Instance.FocusTarget(locationObjectT.alignTarget);
        if (!IsFocus)
        {
            beforeFocusAlign = CameraSceneManager.Instance.GetCurrentAlignTarget();
        }
        FocusPerson(locationObjectT.alignTarget);

    }


    /// <summary>
    /// 聚焦人员
    /// </summary>
    private void FocusPerson(AlignTarget alignTargetT)
    {
        if (IsFocus == false)
        {
            //beforeFocusAlign = CameraSceneManager.Instance.GetCurrentAlignTarget();
            //IsFocus = true;
            SetIsIsFocus(true);
            SetExitFocusbtn(true);
            //FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(true);
        }
        SetFollowuiIsCheckCollider(IsFocus);
        IsClickUGUIorNGUI.Instance.SetIsCheck(false);//不关闭UI检测，会导致人员移动时，鼠标移动在UI上，场景出现异常
        CameraSceneManager.Instance.FocusTarget(alignTargetT);
    }

    /// <summary>
    /// 恢复上个AlignTarget到初始状态，
    /// </summary>
    public void RecoverBeforeFocusAlignToOrigin()
    {
        beforeFocusAlign = CameraSceneManager.Instance.GetDefaultAlign();
    }

    /// <summary>
    /// 恢复在聚焦之前的摄像机状态
    /// </summary>
    public void RecoverBeforeFocusAlign(Action onComplete = null)
    {
        RecoverFocus(() =>
        {
           
            //if (PersonSubsystemManage.Instance.IsHistorical == false)
            //{
            //    //DepNode dep = FactoryDepManager.Instance;
            //    //ParkInformationManage.Instance.TitleText.text = dep.NodeName.ToString();
            //    //ParkInformationManage.Instance.RefreshParkInfo(dep.NodeID);
            //    ParkInformationManage.Instance.ShowParkInfoUI(true);
            //}


            if (onComplete != null)
            {
                onComplete();
            }
        });

    }

    private void RecoverFocus(Action onComplete = null)
    {
        if (isFocusPersonAndShowInfo)
        {
            if (IsFocus && !IsSwitchFocus)
            {
                AlarmPushManage.Instance.ShowAlarmPushWindow(true);
                RecoverFocusOP();
            }
        }
        else
        {
            RecoverFocusOP();
        }
    }

    private void RecoverFocusOP()
    {

        StartOutManage.Instance.HideBackButton();
        IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        if (RoomFactory.Instance.IsFocusingDep)
        {
            //IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        }
        else
        {
            //CameraSceneManager.Instance.FocusTarget(beforeFocusAlign, () =>
            //{
            //    IsClickUGUIorNGUI.Instance.SetIsCheck(true);
            //    if (onComplete != null)
            //    {
            //        onComplete();
            //    }
            //});
            if (currentLocationFocusObj == null) return;
            DepNode depnodeT = currentLocationFocusObj.currentDepNode;
            if (depnodeT.TopoNode.Type == AreaTypes.机房 || depnodeT.TopoNode.Type == AreaTypes.范围)
            {
                depnodeT = depnodeT.ParentNode;
            }
            RoomFactory.Instance.FocusNode(depnodeT);
        }
        if (currentLocationFocusObj == null) return;
        PersonnelTreeManage.Instance.departmentDivideTree.Tree.DeselectNodeByData(currentLocationFocusObj.personInfoUI.personnel.TagId);
        //PersonNode nodeT = PersonnelTreeManage.Instance.PersonnelToPersonNode(currentLocationFocusObj.personInfoUI.personnel.Id);
        PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaDeselectNodeByData(currentLocationFocusObj.personInfoUI.personnel.Id);
        //IsFocus = false;
        SetIsIsFocus(false);
        SetExitFocusbtn(false);

        currentLocationFocusObj.personInfoUI.SetOpenOrClose(false);

        SetCurrentLocationFocusObj(null);

        if (PersonSubsystemManage.Instance.SearchToggle.isOn == true)//人员搜索定位，返回时的操作
        {
            //PersonnelSearchTweener.Instance.ShowMinWindow(false);
            DataPaging.Instance.ClosepersonnelSearchWindow();
        }

        SetFollowuiIsCheckCollider(false);
        //FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(false);
    }

    /// <summary>
    /// 进入单人历史轨迹，恢复在聚焦之前的摄像机状态
    /// </summary>
    public void EnterHistory_One()
    {
        CameraSceneManager.Instance.FocusTarget(beforeFocusAlign);
    }


    /// <summary>
    /// 退出单人历史轨迹，恢复在聚焦之前的摄像机状态
    /// </summary>
    public void ExitHistory_One()
    {
        if (currentLocationFocusObj == null) return;
        FocusPerson(currentLocationFocusObj.Tag.Id);
    }

    /// <summary>
    /// 设置退出聚焦按钮
    /// </summary>
    public void SetExitFocusbtn(bool b)
    {
        //exitFocusbtn.SetActive(b);
    }

    /// <summary>
    /// 关闭人员信息界面UI
    /// </summary>
    public void HideCurrentPersonInfoUI()
    {
        //locationObjectT.personInfoUI.SetContentGridActive(false);
        if (currentLocationFocusObj != null)
        {
            if (currentLocationFocusObj.personInfoUI != null)
            {
                //currentLocationFocusObj.personInfoUI.SetContentToggle(false);
                currentLocationFocusObj.personInfoUI.SetOpenOrClose(false);
            }
        }
    }

    /// <summary>
    /// 设置是否聚焦
    /// </summary>
    /// <param name="b"></param>
    public void SetIsIsFocus(bool b)
    {
        IsFocus = b;

        //SetUIGraphicRaycaster(!b);
    }

    ///// <summary>
    ///// 设置UI的是否可以响应（为了解决鼠标移动到走动的人员的UI上时，界面闪动）
    ///// </summary>
    ///// <param name="b"></param>
    //public void SetUIGraphicRaycaster(bool b)
    //{
    //    //GraphicRaycaster[] rs = GameObject.FindObjectsOfType<GraphicRaycaster>();
    //    //foreach (GraphicRaycaster r in rs)
    //    //{
    //    //    r.enabled = b;
    //    //}
    //}



    #endregion

    #region 定位透明相关

    /// <summary>
    /// 透明的建筑
    /// </summary>
    public StardardMaterialController Building;
    /// <summary>
    /// 透明Toggle
    /// </summary>
    public Toggle transparentToggle;
    /// <summary>
    /// 透明的颜色
    /// </summary>
    public Color TransparentColor; //= new Color32(107, 161, 193, 90);

    /// <summary>
    /// 透明园区
    /// </summary>
    public void TransparentPark()
    {
        //Color colorT = new Color(0.1f, 0.1f, 0.1f, 0.2F);
        Building.SetMatsTransparent(TransparentColor);
    }

    /// <summary>
    /// 恢复园区样式
    /// </summary>
    public void RecoverParkMaterial()
    {
        Building.RecoverMaterials();
    }

    /// <summary>
    /// 透明Toggle改变，事件触发
    /// </summary>
    /// <param name="b"></param>
    public void TransparentToggle_ValueChanged(bool b)
    {
        if (b)
        {
            TransparentPark();
        }
        else
        {
            RecoverParkMaterial();
        }
    }

    #endregion

    /// <summary>
    /// 设置人员信息界面，历史按钮是否关闭
    /// </summary>
    public void SetPersonInfoHistoryUI(bool isActive)
    {
        foreach (LocationObject obj in code_character.Values)
        {
            if (obj != null && obj.personInfoUI != null)
            {
                obj.personInfoUI.SetHistoryButton(isActive);
            }
        }
    }

    /// <summary>
    /// 设置跟随UI检测视线遮挡碰撞
    /// </summary>
    public void SetFollowuiIsCheckCollider(bool IsCheck)
    {
        foreach (LocationObject obj in code_character.Values)
        {
            if (obj.personInfoUI == null) continue;
            UGUIFollowTarget follow = obj.personInfoUI.GetComponent<UGUIFollowTarget>();
            if (IsCheck)
            {
                if (obj == currentLocationFocusObj)
                {
                    follow.SetIsRayCheckCollision(false);
                    //Debug.LogError("SetFollowuiIsCheckCollider:"+obj.name);
                    continue;//开启检测时，当前聚焦人物不检测
                }
            }

            if (obj.personInfoUI != null)
            {
                follow.SetIsRayCheckCollision(IsCheck);
            }
        }
    }

    //public TagPosition GetPositionByTag(Tag tagT)
    //{
    //    if (tagsPos != null)
    //    {
    //        TagPosition tagp = tagsPos.Find((i) => i.Tag == tagT.Code);
    //        return null;
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 获取区域Id根据标签Tag
    ///// </summary>
    ///// <param name="tagT"></param>
    ///// <returns></returns>
    //public int? GetAreaByTag(Tag tagT)
    //{
    //    TagPosition tagposT = GetPositionByTag(tagT);
    //    if (tagposT != null)
    //    {
    //        return tagposT.AreaId;
    //    }
    //    return null;
    //}

    /// <summary>
    /// 获取基站根据基站编号
    /// </summary>
    public Archor GetArchorByCode(string code)
    {
        Archor a = archors.Find((i) => i.Code == code);

        return a;
    }

    #region 人员告警接收、实现
    private void AlarmHub_OnLocationAlarmRecieved(List<LocationAlarm> objs)
    {
        //throw new NotImplementedException();
        Debug.Log("AlarmHub_OnLocationAlarmRecieved:" + objs.Count);

        ShowLocationAlarms(objs);
    }

    private void ShowLocationAlarms(List<LocationAlarm> objs)
    {
        foreach (LocationAlarm locationAlarm in objs)
        {
            ////处理人员告警
            LocationObject locationObject = GetLocationObjByTagId(locationAlarm.TagId);
            if (locationObject == null) continue;

            if (locationAlarm.AlarmLevel == LocationAlarmLevel.正常)
            {
                locationObject.HideAlarm(locationAlarm);
            }
            else
            {
                locationObject.ShowAlarm(locationAlarm);
            }

            MonitorRangeObject monitorRangeObject = MonitorRangeManager.Instance.GetMonitorRangeObjectByAreaId(locationAlarm.AreaId);
            if (monitorRangeObject == null) continue;
            if (locationAlarm.AlarmLevel == LocationAlarmLevel.正常)
            {
                monitorRangeObject.HideAlarm(locationObject);
            }
            else
            {
                monitorRangeObject.ShowAlarm(locationObject);
            }

        }
    }

    /// <summary>
    /// 初始化园区告警状态
    /// </summary>
    private void InitLocationAlarms()
    {
        AlarmSearchArg alarmSearchArg = new AlarmSearchArg();
        LocationAlarm[] personnelAlarms;
        //personnelAlarms = CommunicationObject.Instance.GetLocationAlarms(alarmSearchArg);
        //if (personnelAlarms != null)
        //{
        //    List<LocationAlarm> personnelAlarmList = new List<LocationAlarm>();
        //    ShowLocationAlarms(personnelAlarmList);
        //}
        Loom.StartSingleThread(() =>
        {
            personnelAlarms = CommunicationObject.Instance.GetLocationAlarms(alarmSearchArg);
            Loom.DispatchToMainThread(() =>
            {
                if (personnelAlarms != null)
                {
                    List<LocationAlarm> personnelAlarmList = new List<LocationAlarm>(personnelAlarms);
                    ShowLocationAlarms(personnelAlarmList);
                }
            });

        });
        //if (personnelAlarm != null)
        //{
        //    PerAlarmList = new List<LocationAlarm>(personnelAlarm);
        //    foreach (var devAlarm in PerAlarmList)
        //    {
        //        if (devAlarm.AlarmLevel != LocationAlarmLevel.正常)
        //        {
        //            AlarmItem.Add(devAlarm);
        //        }

        //    }
        //}

    }

    #endregion



}
