using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Base.Common.Extensions;
using UnityEngine;
using UnityEngine.UI;
using StardardShader;
using MonitorRange;

/// <summary>
/// 人员定位管理
/// </summary>
public class LocationManager : MonoBehaviour
{
    public class LocationObjectDictionary: Dictionary<string, LocationObject>
    {
        /// <summary>
        /// 获取LocationObject通过PersonnelId
        /// </summary>
        public LocationObject GetByTagId(int tagId)
        {
            var list = this.Values.ToList();
            var result = list.Find((item) => item.Tag.Id == tagId);
            return result;
        }

        /// <summary>
        /// 设置人员信息界面，历史按钮是否关闭
        /// </summary>
        public void SetPersonInfoHistoryUI(bool isActive)
        {
            foreach (LocationObject obj in this.Values)
            {
                if (obj != null && obj.personInfoUI != null)
                {
                    obj.personInfoUI.SetHistoryButton(isActive);
                }
            }
        }

        /// <summary>
        /// 清除人物
        /// </summary>
        public void ClearCharacter()
        {
            try
            {
                List<LocationObject> objs = new List<LocationObject>(this.Values);
                foreach (LocationObject obj in objs)
                {
                    if(obj==null)continue;
                    //DestroyImmediate(obj.gameObject);
                    Destroy(obj.gameObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            this.Clear();
        }

        /// <summary>
        /// 清除人物
        /// </summary>
        public void ClearCharacter(List<string> keyslist)
        {
            foreach (string key in keyslist)
            {
                DestroyImmediate(this[key].gameObject);
                this.Remove(key);
            }
        }

        /// <summary>
        /// 隐藏人物
        /// </summary>
        public void HideCharacter(List<string> keyslist)
        {
            foreach (string key in keyslist)
            {
                this[key].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 隐藏人物
        /// </summary>
        public void HideCharacter()
        {
            try
            {
                List<LocationObject> objs = new List<LocationObject>(this.Values);
                foreach (LocationObject obj in objs)
                {
                    if (obj == null) continue;
                    //obj.gameObject.SetActive(false);
                    obj.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// 设置跟随UI检测视线遮挡碰撞
        /// </summary>
        public void SetFollowuiIsCheckCollider(bool IsCheck, LocationObject currentLocationFocusObj)
        {
            foreach (LocationObject obj in this.Values)
            {
                if (obj.personInfoUI == null) continue;
                UGUIFollowTarget follow = obj.personInfoUI.GetComponent<UGUIFollowTarget>();
                if (IsCheck)
                {
                    if (obj == currentLocationFocusObj)
                    {
                        follow.SetIsRayCheckCollision(false);
                        if(!follow.gameObject.activeInHierarchy)
                        {
                            follow.gameObject.SetActive(true);                            
                        }
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
    }
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
    /// <summary>
    /// 使用NevMesh跟随
    /// </summary>
    public bool isNevMeshPersonFollow;
    /// <summary>
    /// 使用NevMesh跟随
    /// </summary>
    public GameObject nevMeshFollowPerson;

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
    /// 女性人物预设
    /// </summary>
    public GameObject characterWomanPrefab;
    /// <summary>
    /// 卡对应人物列表
    /// </summary>
    [System.NonSerialized] private LocationObjectDictionary code_character = new LocationObjectDictionary();

    public List<LocationObject> GetPersonObjects()
    {
        return code_character.Values.ToList();
    }

    //private bool isRefleshPositionComplete = true;//是否可以刷新人员位置及相关信息信息
    /// <summary>
    /// 位置变化速度
    /// </summary>
    public float damper = 2;
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

    ///// <summary>
    ///// 根节点内容
    ///// </summary>
    //public string RootNodeName = "四会热电厂";

    [System.NonSerialized]
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
        //code_character = new Dictionary<string, LocationObject>();
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
        SceneEvents.DepNodeChanged += OnDepChanged;
        SceneEvents.BuildingOpenCompleteAction += ShowPersonAfterExpand;
        SceneEvents.BuildingCloseCompleteAction += ShowPersonAfterExpand;
        SceneEvents.BuildingOpenStartAction += OnBuildingExpandStart;
        SceneEvents.BuildingStartCloseAction += OnBuildingExpandStart;
    }
    private void OnBuildingExpandStart()
    {
        HideAndClearLocation();
    }


    /// <summary>
    /// 当前节点和子节点集合（如果当前是楼层以下级别节点，就所在楼层及以下节点集合）
    /// </summary>
    [System.NonSerialized]
    public List<DepNode> currentChildDepNodeList;

    private void OnDepChanged(DepNode oldDep, DepNode currentDep)
    {
        DepNode currentDepT = currentDep;
        if (currentDepT.IsRoom())
        {
            //currentDepT = depnodeT.ParentNode;
            var room=GetRoomInFloor(currentDepT);
            if(room!=null)
                currentDepT = room;
        }
        var depNodes = currentDepT.GetComponentsInChildren<DepNode>(true);
        if(depNodes!=null)
            currentChildDepNodeList = depNodes.ToList();
        else
        {
            currentChildDepNodeList = new List<DepNode>();
        }
    }

    /// <summary>
    /// 获取房间所在的楼层
    /// </summary>
    public DepNode GetRoomInFloor(DepNode depNodeT)
    {
        if (depNodeT == null) return null;
        if (depNodeT.IsFloor())
        {
            return depNodeT;
        }
        else
        {
            return GetRoomInFloor(depNodeT.ParentNode);
        }
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

    /// <summary>
    /// 字符串转Vector3
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 StringToVector3(string value)
    {
        string[] pars = value.Split(',');
        return new Vector3(pars[0].ToFloat(), pars[1].ToFloat(), pars[2].ToFloat());
    }

    //todo:添加修改坐标系转换配置信息的界面和代码

    public Action firstShowLocationCall;
    /// <summary>
    /// 显示人员定位
    /// </summary>
    public void ShowLocation()
    {
        if (this.enabled==false) return;//手动不激活这个脚本
        Debug.LogError("LocationManager.ShowLocation");
        if (!IsShowLocation)
        {
            //firstShowLocationCall = LocationManager.Instance.RefleshLocationAlarms;
            firstShowLocationCall = LocationManager.Instance.InitLocationAlarms;
            IsShowLocation = true;
        }

        //code_character = new Dictionary<string, LocationObject>();
        //ThreadManager.Run(
        //() =>
        //{
        //    RefleshTags();
        //}, () =>
        //{
        //    ShowTagsPosition();
        //}, "ShowLocation");
        //ShowTagsPosition();
        InvokeRepeating("ShowTagsPosition", 0, CommunicationObject.Instance.RefreshSetting.TagPos);//这里是每隔20秒重复刷新显示
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
        code_character.ClearCharacter(); //清除人物
        //code_character.HideCharacter();
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
                        List<U3DPosition> u3dlist = new List<U3DPosition>();
                        List<LocationObject> objList = code_character.Values.ToList();

                        for (int i = 0; i < objList.Count; i++)
                        {
                            CreateU3DPosition(objList[i].tagPosInfo, objList[i].currentPos);
                            //t.StartSendData();
                        }
                        CommunicationObject.Instance.AddU3DPosition(u3dlist);
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

    [System.NonSerialized]
    List<Tag> _tags;
    //List<TagPosition> tagsPos; //2019_03_02_cww:因为位置信息已经存到Tag里面的Pos传过来了 没必要另外放一个列表
    /// <summary>
    /// 显示定位卡的位置信息
    /// </summary>
    public void ShowTagsPosition()
    {
        if (IsShowLocation == false) return;
        CreateTagsParent();
        if (!isBusy)
        {
            isBusy = true;
            CommunicationObject.Instance.GetTags((tagsT) =>
            {
                isBusy = false;
                if (tagsT == null)
                {
                    Debug.LogError("tagsT==null");
                }
                else
                {
                    if (tagsT != null)
                    {
                        _tags = new List<Tag>(tagsT);
                    }
                    RefleshTagsPosition(_tags);
                    if (firstShowLocationCall != null)
                    {
                        firstShowLocationCall();
                        firstShowLocationCall = null;
                    }
                }
            });
        }
        else
        {
            //Log.Alarm("ShowTagsPosition", "isBusy:" + isBusy);
        }
    }

    public class TagListInfo
    {
        public Dictionary<int, int> tagToArea = new Dictionary<int, int>();

        private string tagToAreasIds = "";

        private List<Tag> tags = new List<Tag>();

        public List<Tag> showTags = new List<Tag>();

        private string showTagIds = "";

        private List<Tag> hideTags = new List<Tag>();

        private int totalCount = 0;

        private int showCount = 0;

        private int hideCount = 0;

        public TagListInfo(List<Tag> tags)
        {
            this.tags = tags;
            totalCount = tags.Count;
            FilterTags(tags);//过滤,removeList是删除掉的人员,tags是显示的人员
            showCount = showTags.Count;
            hideCount = hideTags.Count;

            StringBuilder temp = new StringBuilder();
            foreach (Tag tag in showTags)
            {
                int areaId = -1;
                if (tag.Pos != null)
                {
                    areaId = tag.Pos.AreaId ?? 0;
                }
                tagToArea.Add(tag.Id, areaId);//定位卡所在区域，显示的
                temp.Append(string.Format("{0}->{1},",tag.Id ,areaId));
            }
            tagToAreasIds = temp.ToString();

            //Log.Info("TagListInfo", string.Format("{0}=>{1}", totalCount, showCount));
        }

        /// <summary>
        /// 过滤标签卡
        /// </summary>
        /// <param name="layerIndex"></param>
        public void FilterTags(List<Tag> tags)
        {
            List<int> currentDepIDList = GetCurrentDepsId();            
            foreach (Tag tag in tags)
            {
                if (tag == null) continue;             
                if (tag.Pos == null || (tag.Pos != null && tag.Pos.IsHide))
                {
                    //tags.Remove(tag);
                    hideTags.Add(tag);
                }
                else
                {
                    if(UnLocatedAreas.Instance)
                    {                       
                        Vector3 cadPos = new Vector3(tag.Pos.X,tag.Pos.Y,tag.Pos.Z);
                        bool isNoProductionArea = UnLocatedAreas.Instance.IsInNonProductionArea(cadPos);
                        if (isNoProductionArea)
                        {
                            //Debug.Log("Person enter nonProdectionArea...");
                            hideTags.Add(tag);
                        }
                        else
                        {
                            AddShowTags(tag, currentDepIDList);
                            //showTagIds += tag.Id + " ";
                            //showTags.Add(tag);
                        }
                    }
                    else
                    {
                        AddShowTags(tag, currentDepIDList);
                        //showTagIds += tag.Id + " ";
                        //showTags.Add(tag);
                    }                   
                }
                //TimeSpan time = DateTime.Now - LocationManager.GetTimestampToDateTime(tag.Pos.Time);
                //if (time.TotalHours >= SystemSettingHelper.locationSetting.hideTimeHours)//移除大于24小时，没有检测到信号的卡
                //{
                //    tags.Remove(tag);
                //}
            }
        }
        /// <summary>
        /// 获取当前聚焦人员的Tag
        /// </summary>
        /// <returns></returns>
        private Tag GetCurretFocusTag()
        {
            if (Instance.currentLocationFocusObj == null || Instance.currentLocationFocusObj.Tag == null) return null;
            else
            {
                return Instance.currentLocationFocusObj.Tag;
            }
        }

        /// <summary>
        /// 添加需要显示的Id
        /// </summary>
        /// <param name="tagTemp"></param>
        /// <param name="depIdList"></param>
        private void AddShowTags(Tag tagTemp,List<int>depIdList)
        {
            Tag currentFoucs = GetCurretFocusTag();
            bool isHistoryMode = PersonSubsystemManage.Instance==null?false:PersonSubsystemManage.Instance.IsHistorical;
            //1.属于当前区域，非历史模式，是当前定位的人
            if (!isHistoryMode&&IsTagBelognsDep(tagTemp,depIdList)||(currentFoucs!=null&&tagTemp.Id== currentFoucs.Id))
            {
                showTagIds += tagTemp.Id + " ";
                showTags.Add(tagTemp);
            }
            else
            {
                hideTags.Add(tagTemp);
            }
        }

        /// <summary>
        /// 获取当前区域ID
        /// </summary>
        /// <returns></returns>
        public List<int>GetCurrentDepsId()
        {
            List<int> depListTemp = new List<int>();
            if (FactoryDepManager.currentDep == null) return depListTemp;
            List<DepNode> depNodeListT = FactoryDepManager.currentDep.GetComponentsInChildren<DepNode>().ToList();
            foreach(var item in depNodeListT)
            {
                if (item == null || depListTemp.Contains(item.NodeID)) continue;
                depListTemp.Add(item.NodeID);
            }
            return depListTemp;
        }
        /// <summary>
        /// Tag是否属于当前区域
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="depList"></param>
        /// <returns></returns>
        private bool IsTagBelognsDep(Tag tag,List<int>depList)
        {
            if (tag.Pos == null||depList==null||depList.Count==0) return false;
            if(depList.Contains((int)tag.Pos.AreaId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool IsChanged(TagListInfo last)
        {
            bool isSameCount =
                last.totalCount == this.totalCount && last.hideCount == this.hideCount && last.showCount ==
                this.showCount; //数量相同
            if (isSameCount == false)
            {
                Log.Info("LocationManager.IsChanged",string.Format("人数:{0}->{1}", last.showCount, showCount));
                return true; //人数不同
            }

            bool isSameShow = this.showTagIds == last.showTagIds;
            if (isSameShow == false)
            {
                Log.Info("LocationManager.IsChanged",string.Format("显示人员:{0}->{1}", last.showTagIds, showTagIds));
                return true; //显示人员不同
            }

            bool isSameArea = this.tagToAreasIds == last.tagToAreasIds;
            if (isSameArea == false)
            {
                //列出详细的区域变化
                StringBuilder changed = new StringBuilder();
                foreach (var item in this.tagToArea)
                {
                    var tag = item.Key;
                    var area2 = 0;
                    var area = item.Value;
                    if (last.tagToArea.ContainsKey(tag))
                    {
                        area2 = last.tagToArea[tag];
                        last.tagToArea.Remove(tag);
                    }
                    if (area != area2)
                    {
                        changed.Append(string.Format("({0},{1}->{2});", tag, area2, area));
                    }
                }
                foreach (var item in last.tagToArea)
                {
                    var tag = item.Key;
                    var area = item.Value;
                    changed.Append(string.Format("({0},{1}->{2});", tag, area, "NULL"));
                }
                Log.Info("LocationManager.IsChanged", string.Format("所在区域:{0}", changed));

                //Log.Info("LocationManager.IsChanged",string.Format("所在区域:{0}->{1}", last.tagToAreasIds,tagToAreasIds));
                return true; //所在区域不同
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("show:{0},hide:{1},showTagIds:{2},tagToAreasIds:{3}", showCount,hideCount, showTagIds, tagToAreasIds);
        }
    }

    public TagListInfo LasTagListInfo;
    /// <summary>
    /// 刷新定位卡信息
    /// </summary>
    /// <param name="tags"></param>
    private void RefleshTagsPosition(List<Tag> tags)
    {
        if (IsShowLocation == false) return;
        if (tags == null) return;
        //Debug.LogErrorFormat("RefleshTagsPosition.Tags.Count:{0}",tags==null?"null":tags.Count.ToString());
        TagListInfo info = new TagListInfo(tags);
        if(info.showTags!=null&&ParkInformationManage.Instance)
        {
            ParkInformationManage.Instance.SetFactoryLocatedPerson(info.showTags.Count);
        }
        //PersonnelTreeManage.Instance.RemovePersons(tags);       
        List<string> keyslist = code_character.Keys.ToList();
        //Debug.LogErrorFormat("RefleshTagsPosition.code_character.Count:{0}\n ShowTagsCount:{1}", keyslist == null ? "null" : keyslist.Count.ToString(),info.showTags==null?"null":info.showTags.Count.ToString());
        foreach (Tag tag in info.showTags)
        {
            if (tag == null) continue;
            if (tag.IsActive == false) continue;
            if (tag.PersonId == null) continue;
            //TagPosition tagp = tagsPos.Find((item) => item.Tag == tag.Code);
            TagPosition tagp = tag.Pos;
            if (tagp == null) continue;
            if (code_character.ContainsKey(tag.Code))//更新
            {
                LocationObject locationObject = code_character[tag.Code];
                locationObject.InitPersonnel();
                locationObject.SetTagPostion(tagp);
                keyslist.Remove(tag.Code);//存在的人员
            }
            else
            {
                CreateLocationObject(tag);//新增
                //Transform tran = CreateCharacter(tag);
                //LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();//这里就会脚本中的
                //locationObject.Init(tag);
                //code_character.Add(tag.Code, locationObject);
                ////SetTagPostion(locationObject, tag, tagsPosT);
                //SetTagPostion(locationObject, tagp);
            }
        }
        code_character.HideCharacter(keyslist);//改成隐藏
        //Debug.LogErrorFormat("RefleshTagsPosition.HideCharacter.Count:{0}",keyslist == null ? "null" : keyslist.Count.ToString());
        IsCreatePersonComplete = true;

        if (LasTagListInfo != null)
        {
            if (info.IsChanged(LasTagListInfo))
            {
                Log.Info(string.Format("IsChanged:{0}->{1}", LasTagListInfo,info));
                PersonnelTreeManage.Instance.areaDivideTree.RefreshPersonnel();
                PersonnelTreeManage.Instance.departmentDivideTree.RefreshActivePerson();//刷新数据，当部门树不显示离线人员时
                //ParkInformationManage.Instance.StartRefreshData();
            }
        }
        LasTagListInfo = info;
    }
    /// <summary>
    /// 建筑展开/关闭完成
    /// </summary>
    /// <param name="controller"></param>
    private void ShowPersonAfterExpand(BuildingController controller)
    {
        ShowPersonAfterExpand();
    }

    public int WaitBeforeShowLocation = 1;

    /// <summary>
    /// 建筑加载完成,显示定位人员
    /// </summary>
    private void ShowPersonAfterExpand()
    {
        Log.Info("LocationManager.ShowPersonAfterExpand");
        
        //if (ActionBarManage.Instance && ActionBarManage.Instance.PersonnelToggle.isOn)
        //{
        //    if (IsInvoking("ShowLocation")) CancelInvoke("ShowLocation");
        //    Invoke("ShowLocation", WaitBeforeShowLocation);//等待分层NaveMesh加载
        //}
    }

    private void CreateLocationObject(Tag tagT)
    {
        TagPosition tagpT = tagT.Pos;
        Transform tran = CreateCharacter(tagT);
        LocationObject locationObject = tran.gameObject.AddComponent<LocationObject>();//这里就会脚本中的

        locationObject.SetTagPostion(tagpT);//要放到SetNavAgent前面
        if (PathFindingManager.Instance != null)
        {
            //SetNavAgent要放到init前面,init里面有设置跟谁UI的地方
            PathFindingManager.Instance.SetNavAgent(locationObject,()=>
            {
                locationObject.Init(tagT);
            });
            code_character.Add(tagT.Code, locationObject);
        }
        else//老的方式
        {
            locationObject.Init(tagT);
            code_character.Add(tagT.Code, locationObject);
        }
        
    }

    bool isBusy;

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

    ///// <summary>
    ///// 设置定位卡的位置信息
    ///// </summary>
    //private void SetTagPostion(LocationObject locationObject, Tag tag, List<TagPosition> tagsPos)
    //{
    //    if (tagsPos == null) return;
    //    TagPosition tagp = tagsPos.Find((item) => item.Tag == tag.Code);
    //    locationObject.SetTagPostion(tagp);
    //}

    /// <summary>
    /// 隐藏人物
    /// </summary>
    public void HideCharacter()
    {
        code_character.HideCharacter();
    }

    ///// <summary>
    ///// 根据时间戳，获取距离现在的时间(s)
    ///// </summary>
    ///// <param name="tagp"></param>
    ///// <returns></returns>
    //public static float GetTimeInterval(long timeT)
    //{
    //    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
    //    startTime = startTime.AddMilliseconds(timeT);
    //    float seconds = (float)(DateTime.Now - startTime).TotalSeconds;
    //    return seconds;
    //}

    /// <summary>
    /// 根据时间戳，获取DateTime类型时间(ms)
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
        Personnel personnel = PersonnelTreeManage.Instance.GetTagPerson(tag.Id);
        GameObject o = null;
        if (personnel != null && personnel.Sex == "女")//女性
        {
            o = CreateWomanCharacter();
        }
        else
        {
            o = CreateCharacter();
        }
        //GameObject o = CreateCharacter();
        //o.transform.position = new Vector3(999,999,999);
        o.SetActive(false);
        o.name = tag.Name + tag.Code;
        return o.transform;
    }

    /// <summary>
    /// 创建定位人物
    /// </summary>
    private GameObject CreateCharacter()
    {
        GameObject o = Instantiate(characterPrefab);
        o.transform.SetParent(tagsParent);
        return o;
    }

    /// <summary>
    /// 创建定位人物女性
    /// </summary>
    private GameObject CreateWomanCharacter()
    {
        GameObject o = Instantiate(characterWomanPrefab);
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

    /// <summary>
    /// Unity位置转CAD位置
    /// </summary>
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

    public static Vector3 GetRealVector(Position pointT)
    {
        Vector3 vector3 = new Vector3((float)pointT.X, (float)pointT.Y, (float)pointT.Z);
        return GetRealVector(vector3);//进行坐标转换
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
    /// 是否执行了聚焦操作（是否执行了FocusPersonAndShowInfo()方法）
    /// </summary>
    public bool isFocusPersonAndShowInfo;

    /// <summary>
    /// 聚焦人员,根据tagId
    /// </summary>
    public void FocusPersonAndShowInfo(int tagId)
    {

        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
        LocationObject locationObjectT = code_character.GetByTagId(tagId);
        //if (locationObjectT.IsRenderEnable == false && FactoryDepManager.currentDep == FactoryDepManager.Instance)
        //{
        //    UGUIMessageBox.Show("当前人员不在监控区域！");
        //    return;
        //}
        if (locationObjectT == null) return;
        if(PathFindingManager.Instance&&PathFindingManager.Instance.useFollowNavAgent) locationObjectT.SetRendererEnable(false,false);//启用NavMesh跟随，隐藏当前人员
        //if (!locationObjectT.isInLocationRange && locationObjectT.IsRenderEnable == false)
        //{
        //    UGUIMessageBox.Show("该人员不在监控范围内！");
        //    locationObjectT.personInfoUI.SetOpenOrClose(false);
        //    PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaDeselectNodeByData(locationObjectT.personInfoUI.personnel.TagId);
        //    PersonnelTreeManage.Instance.departmentDivideTree.Tree.DeselectNodeByData(tagId);
        //    return;
        //}
        Debug.LogError("locationObjectT.FocusPersonAndShowInfo:" + locationObjectT.name);
        isFocusPersonAndShowInfo = true;
        if (currentLocationFocusObj != null && currentLocationFocusObj != locationObjectT)
        {
            IsSwitchFocus = true;
            HideCurrentPersonInfoUI();
            CameraSceneManager.Instance.SetTheThirdPersonCameraFalse();
            currentLocationFocusObj.personInfoUI.ResetCameraFollowToggleButton();
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
            if(locationObjectT==null)
            {
                Debug.LogError("LocationManager.FocusPersonAndShowInfo.locationObjectT is null! TagId:"+tagId);
                locationObjectT = code_character.GetByTagId(tagId);
                SetCurrentLocationFocusObj(locationObjectT);
            }
            FocusPerson(locationObjectT.AlignTarget, () =>
             {
                //CameraThroughWallManage.Instance.SetIsCanThroughWall(false);
                CameraSceneManager.Instance.alignCamera.SetisCameraCollider(true);
             });

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
        LocationObject locationObjectT = code_character.GetByTagId(id);
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
        FocusPerson(locationObjectT.AlignTarget);

    }


    /// <summary>
    /// 聚焦人员
    /// </summary>
    private void FocusPerson(AlignTarget alignTargetT, Action onFocusComplete = null)
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
        CameraSceneManager.Instance.FocusTarget(alignTargetT, onFocusComplete);
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
    /// isSetSelectNode:是否设置选中树节点
    public void RecoverBeforeFocusAlign(Action onComplete = null, bool isSetSelectNode = true)
    {
        RecoverFocus(() =>
        {
            AlarmPushManage.Instance.CloseAlarmPushWindow(true);
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
        }, isSetSelectNode);

    }

    /// <summary>
    /// 恢复聚焦
    /// </summary>
    /// <param name="onComplete"></param>
    /// <param name="isSetSelectNode"></param>
    private void RecoverFocus(Action onComplete = null, bool isSetSelectNode = true)
    {
        if (isFocusPersonAndShowInfo)
        {
            if (IsFocus && !IsSwitchFocus)
            {
                AlarmPushManage.Instance.CloseAlarmPushWindow(true);
               // AlarmPushManage.Instance.IsShow.isOn = true ;
                RecoverFocusOP(onComplete, isSetSelectNode);
            }
        }
        else
        {
            RecoverFocusOP(onComplete, isSetSelectNode);
        }
    }

    /// <summary>
    /// 恢复聚焦
    /// </summary>
    private void RecoverFocusOP(Action onComplete = null, bool isSetSelectNode = true)
    {
        //CameraThroughWallManage.Instance.SetIsCanThroughWall(true);
        CameraSceneManager.Instance.alignCamera.SetisCameraCollider(false);
        CameraSceneManager.Instance.SetTheThirdPersonCameraFalse();
        SetIsIsFocus(false);

        if (currentLocationFocusObj != null)
        {
            currentLocationFocusObj.personInfoUI.ResetCameraFollowToggleButton();
        }

        StartOutManage.Instance.HideBackButton();
        IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        if (RoomFactory.Instance.IsFocusingDep)
        {
            //IsClickUGUIorNGUI.Instance.SetIsCheck(true);
            if (onComplete != null)
            {
                onComplete();
            }
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
            if (depnodeT.IsRoom())
            {
                depnodeT = depnodeT.ParentNode;
            }
            RoomFactory.Instance.FocusNode(depnodeT, () =>
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            }, isSetSelectNode);
        }
        if (currentLocationFocusObj == null) return;
        PersonnelTreeManage.Instance.DeselectPerson(currentLocationFocusObj.personInfoUI.personnel);
        //IsFocus = false;
        //SetIsIsFocus(false);
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
        if (Building == null)
        {
            Building = GameObject.FindObjectOfType<StardardMaterialController>();
        }
        if (Building != null)
        {
            Building.AnewGetMaterials();
            Building.SetMatsTransparent(TransparentColor);
        }
        else
        {
            Debug.LogError("LocationManager.TransparentPark Building == null");
        }
    }

    /// <summary>
    /// 恢复园区样式
    /// </summary>
    public void RecoverParkMaterial()
    {
        if (Building == null)
        {
            Building = GameObject.FindObjectOfType<StardardMaterialController>();
        }
        if (Building != null)
        {
            Building.RecoverMaterials();
        }
        else
        {
            Debug.LogError("LocationManager.RecoverParkMaterial Building == null");
        }
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
        code_character.SetPersonInfoHistoryUI(isActive);
    }

    /// <summary>
    /// 设置跟随UI检测视线遮挡碰撞
    /// </summary>
    public void SetFollowuiIsCheckCollider(bool IsCheck)
    {
        code_character.SetFollowuiIsCheckCollider(IsCheck, currentLocationFocusObj);
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
        ParkInformationManage.Instance.StartRefreshData();
    }

    /// <summary>
    /// 展示定位告警
    /// </summary>
    /// <param name="objs"></param>
    private void ShowLocationAlarms(List<LocationAlarm> objs)
    {
        if (PersonSubsystemManage.Instance && PersonSubsystemManage.Instance.IsHistorical)
        {
            return;
        }
        foreach (LocationAlarm locationAlarm in objs)
        {
            ////处理人员告警
            LocationObject locationObject = code_character.GetByTagId(locationAlarm.TagId);
            //if (locationObject == null || !locationObject.IsRenderEnable) continue;
            if (locationObject == null || !locationObject.IsBelongtoCurrentDep()) continue;

            if (locationAlarm.AlarmLevel == LocationAlarmLevel.正常)
            {
                locationObject.HideAlarm(locationAlarm);
            }
            else
            {
                locationObject.ShowAlarm(locationAlarm);
                Debug.LogError(locationAlarm.AlarmLevel.ToString() + "：" + locationAlarm.TagId + "  |  " + locationAlarm.Content);
            }

            MonitorRangeObject monitorRangeObject = MonitorRangeManager.Instance.GetMonitorRangeObjectByAreaId(locationAlarm.AreaId);
            //if (monitorRangeObject.name.Contains("主厂房0m层"))
            //{
            //    int i = 0;
            //}

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
    /// 告警信息集合
    /// </summary>
    [System.NonSerialized]
    public LocationAlarm[] locationAlarms;

    /// <summary>
    /// 初始化园区告警状态
    /// </summary>
    public void InitLocationAlarms()
    {
        AlarmSearchArg alarmSearchArg = new AlarmSearchArg();

        Loom.StartSingleThread(() =>
        {
            locationAlarms = CommunicationObject.Instance.GetLocationAlarms(alarmSearchArg);
            Loom.DispatchToMainThread(() =>
            {
                if (locationAlarms != null)
                {
                    List<LocationAlarm> personnelAlarmList = new List<LocationAlarm>(locationAlarms);
                    ShowLocationAlarms(personnelAlarmList);
                }
            });

        });

    }

    /// <summary>
    /// 刷新园区告警状态
    /// </summary>
    public void RefleshLocationAlarms()
    {
        if (locationAlarms != null)
        {
            List<LocationAlarm> personnelAlarmList = new List<LocationAlarm>(locationAlarms);
            ShowLocationAlarms(personnelAlarmList);
        }
    }

    #endregion

    public bool UseShowPos = false;
}

public class PositionInfo
{
    public Position Pos { get; set; }

    public Vector3 Vec { get; set; }

    /// <summary>
    /// NavMesh计算的坐标
    /// </summary>
    public Vector3 NavPos { get; set; }

    public DateTime Time { get; set; }

    public double TimeStamp { get; set; }

    public PositionInfo(Position pos,DateTime startTime)
    {
        Pos = pos;
        Vec = LocationManager.GetRealVector(pos);//进行坐标转换
        Time = LocationManager.GetTimestampToDateTime(pos.Time);
        TimeStamp = (Time - startTime).TotalSeconds;
    }

    public override string ToString()
    {
        return string.Format("Vec={0},Time={1},TimeStamp={2}", Vec,Time,TimeStamp);
    }

    public PositionInfo Pre { get; set; }

    public PositionInfo Next { get; set; }

    public void SetPre(PositionInfo pre)
    {
        this.Pre = pre;
        pre.Next = this;
    }
}

public class PositionInfoList:List<PositionInfo>
{
    public List<Position> PosList = new List<Position>();

    public PositionInfo LastItem = null;

    public new void Add(PositionInfo item)
    {
        if (LastItem != null)
        {
            item.SetPre(LastItem);//设置前后关联关系
        }
        PosList.Add(item.Pos);
        base.Add(item);//加到列表中
        LastItem = item;
    }

    /// <summary>
    /// 获取点列表，为了先于原来的代码兼容
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetVector3List()
    {
       var list = new List<Vector3>();
        foreach (PositionInfo info in this)
        {
            if (info == null) continue;
            list.Add(info.Vec);
        }
        return list;
    }


    /// <summary>
    /// 获取点列表，为了先于原来的代码兼容
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetNavMeshPosList()
    {
        var list = new List<Vector3>();
        foreach (PositionInfo info in this)
        {
            if (info == null) continue;
            list.Add(info.NavPos);
        }
        return list;
    }

    /// <summary>
    /// 获取点列表，为了先于原来的代码兼容
    /// </summary>
    /// <returns></returns>
    public List<double> GetTimeStampList()
    {
        var list = new List<double>();
        foreach (PositionInfo info in this)
        {
            if (info == null) continue;
            list.Add(info.TimeStamp);
        }
        return list;
    }

    /// <summary>
    /// 获取点列表，为了先于原来的代码兼容
    /// </summary>
    /// <returns></returns>
    public List<Position> GetPositionList()
    {
        var list = new List<Position>();
        foreach (PositionInfo info in this)
        {
            if (info == null) continue;
            list.Add(info.Pos);
        }
        return list;
    }

    public int FindIndexByTime(DateTime startTime, double f, float accuracy = 0.1f)
    {
        return this.FindIndex((item) =>
        {
            if (item == null) return false;
            double timeT = (item.Time - startTime).TotalSeconds;
            if (Math.Abs(f - timeT) < accuracy)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
    }

    /// <summary>
    /// 获取当前点和上一点的时间差(s)
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public double GetTimeSpane(int index)
    {
        if (index > 0 && index < this.Count)
        {
            double temp = (this[index].Time - this[index - 1].Time).TotalSeconds;
            return temp;
        }
        else
        {
            return -1;
        }
    }
}

public class PositionInfoGroup:List<PositionInfoList>
{
    public void AddList(List<PositionInfo> list)
    {
        Log.Info("PositionInfoGroup.AddList");
        if (list==null)return;
        PositionInfoList posList=new PositionInfoList();
        posList.AddRange(list);
        this.Add(posList);
    }
}
