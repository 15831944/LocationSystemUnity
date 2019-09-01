using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using MonitorRange;
using System;
using System.Collections.Generic;
using System.Threading;
using UIWidgets;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 注意：对于人员，我们在PhysicsManager设置人员所在layer不能与地板所在layer和自身碰撞,
/// </summary>
public class LocationObject : MonoBehaviour
{
    /// <summary>
    /// 用于测试，可以查看定位数据
    /// </summary>
    public Vector3 dataPos;
    /// <summary>
    /// NavMeshAgent
    /// </summary>
    public NavMeshAgent agent;
    /// <summary>
    /// 定位卡信息
    /// </summary>
    public Tag Tag;
    /// <summary>
    /// 人员信息
    /// </summary>
    public Personnel personnel;
    /// <summary>
    /// 定位卡位置信息
    /// </summary>
    public TagPosition tagPosInfo;
    /// <summary>
    /// 目标位置
    /// </summary>
    public Vector3 targetPos;
    /// <summary>
    /// 保存3D位置信息协程
    /// </summary>
    private Coroutine coroutine;

    Thread thread;
    /// <summary>
    /// 当前位置
    /// </summary>
    public Vector3 currentPos;

    private AlignTarget alignTarget;
    /// <summary>
    /// 摄像头聚交用
    /// </summary>
    public AlignTarget AlignTarget
    {
        get {
            alignTarget= GetAlignTarget();
            return alignTarget;
        }
    }

    //[HideInInspector]
    public PersonInfoUI personInfoUI;

    ///// <summary>
    ///// 碰撞器
    ///// </summary>
    //private Collider myCollider;
    /// <summary>
    /// 渲染器
    /// </summary>
    private Renderer[] renders;

    /// <summary>
    /// 正常区域
    /// </summary>
    public List<MonitorRangeObject> normalAreas;

    /// <summary>
    /// 人员进入的定位区域集合
    /// </summary>
    public List<MonitorRangeObject> locationAreas;

    /// <summary>
    /// 人员进入的告警区域集合
    /// </summary>
    public List<MonitorRangeObject> alarmAreas;
    /// <summary>
    /// 告警信息列表
    /// </summary>
    public List<LocationAlarm> alarmList;
    /// <summary>
    /// 当前人员所在的建筑节点,是根据基站计算位置来的
    /// </summary>
    public DepNode currentDepNode;
    /// <summary>
    /// 当前人员所在的建筑节点,是根据基站计算位置来的,根据传上来数据
    /// </summary>
    public DepNode dataCurrentDepNode;
    /// <summary>
    /// 开始碰撞检测
    /// </summary>
    public bool isStartOnTrigger;

    /// <summary>
    /// 开始碰撞OnTriggerStay检测一次
    /// </summary>
    public bool isOnTriggerStayOnce;
    /// <summary>
    /// 人员是否显示了
    /// </summary>
    private bool isRenderEnable;
    public bool IsRenderEnable
    {
        get
        {
            return isRenderEnable;
        }
    }
    /// <summary>
    /// TagCode
    /// </summary>
    public string tagcode;
    /// <summary>
    /// 人员动画控制器
    /// </summary>
    public PersonAnimationController personAnimationController;

    /// <summary>
    /// 位置点是否在所在区域范围内部
    /// </summary>
    public bool isInCurrentRange = true;
    /// <summary>
    /// 区域状态，0:在定位区域，1:不在定位区域
    /// </summary>
    public string areaState;
    /// <summary>
    /// 是否处于告警中
    /// </summary>
    public bool isAlarming;
    /// <summary>
    /// CharacterController：控制人物移动，
    /// </summary>
    PersonMove personmove;

    [HideInInspector]
    public Transform titleTag;
    private void Awake()
    {
        normalAreas = new List<MonitorRangeObject>();
        locationAreas = new List<MonitorRangeObject>();
        alarmAreas = new List<MonitorRangeObject>();
        alarmList = new List<LocationAlarm>();

        GetAlignTarget();

    }

    /// <summary>
    /// 获取AlignTarget
    /// </summary>
    private AlignTarget GetAlignTarget()
    {
        Quaternion quaDir = Quaternion.LookRotation(-transform.forward, Vector3.up);
        Transform target = GetAlignTargetObject();
        alignTarget = new AlignTarget(target, new Vector2(60, quaDir.eulerAngles.y), 5, new Range(5, 90), new Range(1, 40));
        return alignTarget;
    }

    private Transform GetAlignTargetObject()
    {
        if (navAgentFollow != null)
        {
			titleTag = UGUIFollowTarget.CreateTitleTag(navAgentFollow.gameObject, new Vector3(0, 0.1f, 0)).transform;
            return titleTag.transform;
        }

        if (titleTag == null)
        {
            return transform;
        }
        else
        {
            return titleTag;
        }
    }

    void OnEnable()
    {
        //人员预设物体状态必须为未激活的 不然在AddComponent<LocationObject>()后 在Init()前 OnEnable就会被调用 此时Tag等数据还未设置
        transform.position = targetPos;
        if (navAgentFollow != null)
        {
            navAgentFollow.gameObject.SetActive(true);
        }
        if (personInfoUI == null)
        {
            FollowUINormalOn();
        }
        else
        {
            SetFollowPersonInfoUIActive(isRenderEnable);
        }
        HighlightOn(Color.green);
    }

    // Use this for initialization
    void Start()
    {
        titleTag = transform.Find("TitleTag");
        DoubleClickEventTrigger_u3d lis = DoubleClickEventTrigger_u3d.Get(gameObject);
        lis.onDoubleClick = On_DoubleClick;
        if (personAnimationController == null)
        {
            personAnimationController = GetComponent<PersonAnimationController>();
        }
        ////transform.localPosition = targetPos;
        agent = gameObject.GetComponent<NavMeshAgent>();
        personmove = gameObject.GetComponent<PersonMove>();
        ////agent.radius = 0.2f;
        //agent.height = 1.7f;
        //agent.speed = 3.5f;
        SetRendererEnable(true);
        //FollowUINormalOn();

    }

    void OnDisable()
    {
        ClearAreas();
        FollowUIOff();
        try
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }
        catch (Exception ex)
        {
            Log.Error("LocationObject.OnDisable", ex.ToString());
        }
        if(navAgentFollow!=null)
        {
            navAgentFollow.gameObject.SetActive(false);
        }
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            SetPosSphereActive(false);
            FlashingOffArchors();
        }
        HighlightOff();
    }
    /// <summary>
    /// 人员被删除时，恢复聚焦
    /// </summary>
    private void RecoverFocusOnDestory()
    {
        //if (LocationManager.Instance.currentLocationFocusObj == this)
        //{
        //    LocationManager.Instance.currentLocationFocusObj = null;
        //    if (PersonSubsystemManage.Instance.IsHistorical == false)
        //    {
        //        ParkInformationManage.Instance.ShowParkInfoUI(true);
        //    }
        //    LocationManager.Instance.RecoverBeforeFocusAlign();
        //}
    }

    void OnDestroy()
    {
        Debug.Log("LocationObject.OnDestroy:" + this);
        RecoverFocusOnDestory();
        Transform titleTag = transform.Find("TitleTag");
        if (titleTag != null)
        {
            UGUIFollowManage.Instance.RemoveUIbyTarget("LocationNameUI", titleTag.gameObject);
        }
        GameObject.Destroy(posSphere);

        if (navAgentFollow != null)
        {
            GameObject.Destroy(navAgentFollow.gameObject);
        }
    }

    public void On_DoubleClick()
    {
        if (LocationManager.Instance.currentLocationFocusObj != this)
        {
            LocationManager.Instance.FocusPersonAndShowInfo((int)personnel.TagId);
        }
        else
        {
            LocationManager.Instance.RecoverBeforeFocusAlign();
        }
    }

    /// <summary>
    /// 清除人员所在区域范围的相关信息
    /// </summary>
    private void ClearAreas()
    {
        if (normalAreas != null)
        {
            normalAreas.Clear();
        }

        if (alarmAreas != null)
        {
            foreach (MonitorRangeObject obj in alarmAreas)
            {
                if (obj == null) continue;
                obj.OnTriggerExitEx(this);
            }
            alarmAreas.Clear();
        }

        if (locationAreas != null)
        {
            foreach (MonitorRangeObject obj in locationAreas)
            {
                if (obj == null) continue;
                obj.OnTriggerExitEx(this);
            }
            locationAreas.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ViewState.人员定位 != ActionBarManage.Instance.CurrentState) return;
        //GetAlignTarget();
        SetPosition();
        currentPos = transform.position;

        //if (Tag.Code == "0997")
        //{
        //    LODGroup p = GetComponent<LODGroup>();
        //    LOD[] ps = p.GetLODs();
        //}
    }



    void LateUpdate()
    {
        if (ViewState.人员定位 != ActionBarManage.Instance.CurrentState) return;
        if (!LocationManager.Instance.IsFocus)//摄像机不属于人员聚焦状态
        {
            if (locationAreas.Count == 0)
            {
                if (isRenderEnable == false) return;
                SetRendererEnableFalse();
            }
        }
    }



    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Tag t)
    {
        Tag = t;
        if (Tag == null)
        {
            Log.Error("LocationObject.Init", "Tag == null");
            return;
        }
        InitPersonnel();
        tagcode = Tag.Code;
    }

    /// <summary>
    /// 初始化人员信息
    /// </summary>
    public void InitPersonnel()
    {
        if (Tag == null)
        {
            Log.Error("LocationObject.InitPersonnel", "Tag == null");
            return;
        }

        var tag = Tag;
        personnel = PersonnelTreeManage.Instance.GetTagPerson(tag.Id);
        if (personInfoUI == null)//如果跟随UI没创建，需要创建
        {
            FollowUINormalOn();
            SetFollowPersonInfoUIActive(isRenderEnable);
        }
        if (personnel == null)
        {
            Log.Error("LocationObject.Init", "personnel == null");
            gameObject.name = tag.Name + tag.Code;
        }
        else
        {
            try
            {
                gameObject.name = tag.Name + tag.Code + personnel.Name;
            }
            catch (Exception ex)
            {
                Debug.LogError("LocationObject.InitPersonnel p=" + personnel.Name + ex);
            }
        }
    }

   //float halfoffestttt=0;
   // GameObject floorCubeTttt;
    //float ttttt;

    /// <summary>
    /// 设置定位卡的位置信息
    /// </summary>
    public void SetTagPostion(TagPosition tagp)
    {
        if (tagp != null)
        {
            this.SetPositionInfo(tagp);
            //locationObject.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            //locationObject.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private TagPosInfo posInfo;
    /// <summary>
    /// 设置位置信息
    /// </summary>
    public void SetPositionInfo(TagPosition tagPos)
    {
        posInfo = new TagPosInfo(tagPos);
        posInfo.CurrentPos = transform.position;
        tagPosInfo = tagPos;
        SetState(tagPosInfo);//根据信息修改显示的信息（包括待机时间）
        areaState = tagPosInfo.AreaState.ToString();        
        //if (personInfoUI != null && personInfoUI.state == PersonInfoUIState.Leave) return; //人员处于离开状态，就不移动了
        //tagPosInfo = tagPos;
        SetisInRange(true);
        //Vector3 targetPosVT;
        //Vector3 offset = LocationManager.Instance.GetPosOffset();
        Vector3 posOrigin = new Vector3((float)tagPosInfo.X, (float)tagPosInfo.Y, (float)tagPosInfo.Z);
        Vector3 targetPosNew = LocationManager.GetRealVector(posOrigin);
        //posInfo.TargetPos = targetPosNew;
        DepNode depnode = null;
        if (tagPos.AreaId!=null) depnode = RoomFactory.Instance.GetDepNodeById((int)tagPos.AreaId);
        posInfo.TargetPos = ChangePosWhenExpandBuilding(targetPosNew, depnode);       
        Vector3 targetPosTemp = targetPosNew;
        dataPos = targetPosNew;

        DepNode currentDepNodeTemp = currentDepNode;
        //bool isFloorChanged = false;//是否是切换楼层，从一个楼层切换到另一个楼层
        //float halfHeight = gameObject.GetSize().y / 2;//当胶囊体时
        float halfoffest = 0;//当人物模型时
        GameObject floorCubeTt;       
        if (tagPos.AreaId != null)
        {
            //depnode = RoomFactory.Instance.GetDepNodeById((int)tagPos.AreaId);
            //Transform floorCubeT = GetFloorCube(depnode);
            if (depnode == null)
            {
                //Debug.LogError("LocationObject.SetPositionInfo depnode == null this:"+this+",areaId:"+ tagPos.AreaId);
                //depnode = RoomFactory.Instance.GetDepNodeById((int)tagPos.AreaId,true);
            }
            else
            {
                if (depnode.IsUnload)
                {
                    Debug.LogError("LocationObject.SetPositionInfo depnode.IsUnLoad this:" + this + ",areaId:" + tagPos.AreaId);
                }
            }

            if (currentDepNode != depnode || LocationManager.Instance.currentLocationFocusObj != this)
            {
                isOnTriggerStayOnce = false;
                bool b = IsBelongtoCurrentDep();
                if (b)
                {
                    SetRendererEnable(true);
                }
                else
                {
                    SetRendererEnable(false);//切换区域时判断人员的显示隐藏
                }
            }
            currentDepNode = depnode;
            dataCurrentDepNode = currentDepNode;
            DepNode depnodeT = MonitorRangeManager.Instance.GetDoZhuchangfang(depnode, targetPosTemp.y);
            if (depnodeT != null)
            {
                currentDepNode = depnodeT;
            }

            FloorCubeInfo floorCubeT = GetFloorCube(currentDepNode);
            if (floorCubeT != null)
            {
                floorCubeTt = floorCubeT.gameObject;
            }

            if (currentDepNode == null)
            {
                currentDepNode = FactoryDepManager.Instance;//如果人员的区域节点为空，就默认把他设为园区节点
            }

            if (LocationManager.Instance.isSetPersonObjParent)
            {
                SetParent(currentDepNode);
            }

            //isFloorChanged = IsDifferentFloor(currentDepNodeTemp, currentDepNode);

            //bool isboolT = MonitorRangeManager.Instance.IsBelongDepNodeByName("主厂房", currentDepNode) && currentDepNode.NodeName != "主厂房";//主厂房子区域

            //聚焦人员切换楼层控制
            //if (!isboolT)
            //{
            ChangeDep();
            //}

            //Debug.LogFormat("名称:{0},类型:{1}", depnode.name, depnode.NodeObject);
            if (depnode != null && floorCubeT != null)//二层267
            {
                if (floorCubeT != null)
                {
                    halfoffest = targetPosNew.y - floorCubeT.pos.y;
                    //halfoffestttt = halfoffest;
                    //floorCubeTttt = floorCubeT.gameObject;
                    //ttttt = halfoffest + floorCubeT.transform.position.y;
                    Vector3 targetPosT = new Vector3(targetPosNew.x, halfoffest + floorCubeT.transform.position.y, targetPosNew.z);
                    if (currentDepNode.monitorRangeObject)
                    {
                        bool isInRangeT = currentDepNode.monitorRangeObject.IsInRange(targetPosT.x, targetPosT.z);
                        //if (isInRangeT)
                        //{
                        //    isInRangeT = currentDepNode.monitorRangeObject.IsOnLocationArea;
                        //}
                        SetisInRange(isInRangeT);

                    }
                    targetPosNew = targetPosT;
                }
                else
                {
                    Debug.LogError("建筑物没有加楼层地板！！！");
                }
            }
            else
            {
                targetPosNew = new Vector3(targetPosNew.x, LocationManager.Instance.axisZero.y + halfoffest, targetPosNew.z);
            }
        }
        else
        {
            if (LocationManager.Instance.isSetPersonObjParent)
            {
                SetParent(null);
            }
            //if (currentDepNode != null)
            //{
            //    currentDepNode = null;
            //    //isOnTriggerStayOnce = false;//大数据测试修改
            //}
            //else
            //{
            currentDepNode = FactoryDepManager.Instance;//如果人员的区域节点为空，就默认把他设为园区节点
            //}

            targetPosNew = new Vector3(targetPosNew.x, LocationManager.Instance.axisZero.y + halfoffest, targetPosNew.z);
        }
        isStartOnTrigger = true;
        //targetPos = new Vector3(-targetPos.z, targetPos.y, targetPos.x);
        //targetPos = targetPos + offset;
        //targetPos = targetPos;
        //print(string.Format("name:{0}||位置:x({1}),y({2}),z({3})", name, targetPos.x, targetPos.y, targetPos.z));

        if (LocationManager.Instance.isShowRealLocationHeight)
        {
            targetPosNew = new Vector3(targetPosNew.x, targetPosTemp.y + halfoffest, targetPosNew.z);
        }
        if (LocationManager.Instance.currentLocationFocusObj == this)
        {
            ShowArchors();
        }
        else
        {
            FlashingOffArchors();
        }


        if (isInCurrentRange == false)//如果位置点不在当前所在区域范围内部
        {
            if (currentDepNode.monitorRangeObject && currentDepNode.monitorRangeObject.IsOnLocationArea)
            {
                if (!LocationManager.Instance.isShowLeavePerson)
                {
                    Vector2 v = currentDepNode.monitorRangeObject.PointForPointToPolygon(new Vector2(targetPosNew.x, targetPosNew.z));
                    targetPosNew = new Vector3(v.x, targetPosNew.y, v.y);
                }
                else
                {
                    ////区域状态，0:在定位区域，1:不在定位区域
                    //if (tagPosInfo.AreaState == 1)
                    //{
                    //    Vector2 v = currentDepNode.monitorRangeObject.PointForPointToPolygon(new Vector2(targetPosVT.x, targetPosVT.z));
                    //    targetPosVT = new Vector3(v.x, targetPosVT.y, v.y);
                    //}
                }
            }

        }                   
        targetPos = targetPosNew;
        posInfo.ShowPos = targetPosNew;

        if (gameObject.activeInHierarchy)
        {
            if (LocationManager.Instance.isSetPersonHeightByRay)
            {
                SetPersonHeightByRay();
            }
            //if (isFloorChanged)
            //{
            //    transform.position = targetPos;
            //}
        }
        else//人员隐藏时直接修改位置
        {
            transform.position = targetPos;
        }

        if (ViewState.人员定位 != ActionBarManage.Instance.CurrentState)
        {
            transform.position = targetPos;
        }
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            ShowPositionSphereTest(targetPos);
        }
    }
    /// <summary>
    /// 获取楼层展开时的高度
    /// </summary>
    /// <param name="lastPos"></param>
    /// <param name="dep"></param>
    /// <returns></returns>
    private Vector3 ChangePosWhenExpandBuilding(Vector3 lastPos,DepNode dep)
    {
        if(FactoryDepManager.currentDep==null||!(FactoryDepManager.currentDep is BuildingController))
        {
            return lastPos;
        }
        else
        {
            BuildingController building = FactoryDepManager.currentDep as BuildingController;
            if(building&&building.IsFloorExpand&&!BuildingController.isTweening)//楼层展开动画结束后
            {
                if (building.FloorTween == null||dep==null) return lastPos;
                DepNode floor = GetFloorController(dep);
                if (floor.ParentNode != building) return lastPos;//不属于当前展开建筑，不处理           
                float yTemp = lastPos.y+building.FloorTween.GetFloorExpandDistance(floor.gameObject);
                Vector3 vec = new Vector3(lastPos.x,yTemp,lastPos.z);
                return vec;
            }
            else
            {
                return lastPos;
            }
        }
    }
    private DepNode GetFloorController(DepNode dep)
    {
        if(dep is RoomController||dep is RangeController)
        {
            DepNode floor = dep.ParentNode == null ? dep : dep.ParentNode;
            return floor;
        }
        else
        {
            return dep;
        }
    }

    /// <summary>
    /// 设置父物体
    /// </summary>
    public void SetParent(DepNode depNodeT)
    {
        //if (transform.parent.name == "J1_F1" && Tag.Code == "0997")
        //{
        //    Debug.LogError("location_SetParent_J1_F1");
        //}
        //Debug.Log("LocationObject.SetParent:"+this);

        var newParent = LocationManager.Instance.tagsParent;
        if (depNodeT != null && depNodeT.NodeObject != null)
        {
            newParent = depNodeT.NodeObject.transform;
        }
        transform.SetParent(newParent);

        if (navAgentFollow)
        {
            navAgentFollow.transform.SetParent(newParent);
        }
    }

    /// <summary>
    /// 切换区域：在聚焦该人员时，人员移动到不同区域，场景需要切换区域
    /// </summary>
    public void ChangeDep()
    {
        if (LocationManager.Instance.IsFocus && LocationManager.Instance.currentLocationFocusObj == this && FactoryDepManager.currentDep != currentDepNode)
        {
            DepNode depnodeT = currentDepNode;
            if (depnodeT.IsRoom())
            {
                //depnodeT = depnodeT.ParentNode;
                depnodeT = GetRoomInFloor(depnodeT);
            }
            DepNode currentDepT = FactoryDepManager.currentDep;
            if (currentDepT.IsRoom())
            {
                //currentDepT = depnodeT.ParentNode;
                currentDepT = GetRoomInFloor(currentDepT);
            }
            if (depnodeT == currentDepT) return;

            RoomFactory.Instance.FocusNodeForFocusPerson(depnodeT, () =>
            {
                //LocationManager.Instance.FocusPersonAndShowInfo(Tag.Id);
                //FocusPerson(locationObjectT.alignTarget);

                //if (locationObjectT.personInfoUI != null)
                //{
                //    locationObjectT.personInfoUI.SetOpenOrClose(true);
                //}
            }, false);
        }
    }

    /// <summary>
    /// 是否在不同的楼层
    /// </summary>
    public bool IsDifferentFloor(DepNode lastDepNodeT, DepNode currentDepNodeT)
    {
        DepNode depnodeT1 = GetRoomInFloor(lastDepNodeT);
        DepNode depnodeT2 = GetRoomInFloor(currentDepNodeT);
        if (depnodeT1 != null && depnodeT2 != null)
        {
            if (depnodeT1 != depnodeT2) return true;
        }

        return false;
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

    /// <summary>
    /// 获取该区域节点计算位置的平面
    /// </summary>
    /// <param name="depnode"></param>
    public FloorCubeInfo GetFloorCube(DepNode depnode)
    {
        //Transform floorcube = null;
        if (depnode == null || depnode.TopoNode == null) return null;
        if (depnode.IsRoom())
        {
            return FilterFloorCube(depnode);
        }
        else
        {
            return depnode.floorCube;
        }
    }

    /// <summary>
    /// 过滤该区域节点计算位置的平面
    /// </summary>
    /// <param name="depnode"></param>
    public FloorCubeInfo FilterFloorCube(DepNode depnode)
    {
        if (depnode == null) return null;
        if (depnode.IsFloor())
        {
            return depnode.floorCube;
        }
        else
        {
            return FilterFloorCube(depnode.ParentNode);
        }
    }

    public bool debug = false;//某一个人加断点用

    public float distanceToTarget = 0;

    private float minDistanceToStop = 0.2f;//经测试0.2f差不多

    private float minDistanceToMove = 0.3f;

    private bool IsPersonLeave()
    {
        return personInfoUI != null && personInfoUI.state == PersonInfoUIState.Leave;
    }

    private void SetPosition()
    {
        //if (SystemSettingHelper.systemSetting.IsDebug)
        //{
        //    ShowPositionSphereTest(targetPos);
        //}
        //if (LocationManager.Instance.isSetPersonHeightByRay)
        //{
        //    SetPersonHeightByRay();
        //}

        if (!LocationManager.Instance.isShowLeavePerson)
        {
            if (IsPersonLeave()) return; //人员处于离开状态，就不移动了
        }

        //if (isInLocationRange == false) return;//如果位置点不在当前所在区域范围内部，就不设置点
        if (isInCurrentRange == false)//如果位置点不在当前所在区域范围内部
        {
            if (currentDepNode.monitorRangeObject && currentDepNode.monitorRangeObject.IsOnLocationArea)
            {
                //if (!LocationManager.Instance.isShowLeavePerson)
                //{
                //    Vector2 v = currentDepNode.monitorRangeObject.PointForPointToPolygon(new Vector2(targetPos.x, targetPos.z));
                //    targetPos = new Vector3(v.x, targetPos.y, v.y);
                //}
            }
            else
            {
                return;
            }
        }
        else
        {
            if (currentDepNode == null) return;
            if (!LocationManager.Instance.isShowLeavePerson)
            {
                //如果位置点在当前所在区域范围内部,但是当前区域不是定位区域返回
                if (currentDepNode.monitorRangeObject == null || currentDepNode.monitorRangeObject.IsOnLocationArea == false) return;
            }
        }
        if (debug)
        {
            Debug.Log("LocationObject debug :" + this);
        }

        //新的位置和当前位置距离很近，不用移动，避免一个人在原地踏步旋转。因为我们是用动画移动的，不会完全达到目标点的。
        distanceToTarget = Vector3.Distance(transform.position, targetPos);

        bool isStopAnimation = distanceToTarget < minDistanceToStop;

        bool isStartAnimation = distanceToTarget >minDistanceToMove;

        

        if (isStopAnimation)
        {
            //StopAnimation();
            SwitchUIByState(tagPosInfo);
            StartWait(true);//人停止移动后才显示待机UI
            return;
        }

        if (isStartAnimation)//不这样的话，在临界区域会闪烁
        {
            StartAnimation();
            SwitchNormal();
            StartWait(false);//人停止移动后才显示待机UI
        }

        if (personInfoUI != null)
        {
            if (personInfoUI.gameObject.activeInHierarchy == false)
            {
                personInfoUI.gameObject.SetActive(true);//不知道为什么改成NavAgent的跟踪对象时就没有了
            }
        }
        else
        {
            Debug.LogError("personInfoUI == null");
        }


        //SetPosition(targetPos);//原来的修改位置的代码
        SetPosition(posInfo);
    }

    private void SwitchUIByState(TagPosition tagpT)
    {
        if (tagpT.MoveState == 0)//卡正常运动状态
        {
            SwitchNormal();
            StartAnimation();
        }
        else if (tagpT.MoveState == 1 || tagpT.MoveState == 2)//待机状态
        {
            //Log.Info("SwitchStandby");
            SwitchStandby();
            StopAnimation();
        }
        else if (tagpT.MoveState == 3)//长时间不动状态
        {
            //Log.Info("SwitchStandbyLong");
            SwitchStandbyLong();
            StopAnimation();
        }
        else
        {

        }
    }

    private void StartWait(bool isStandBy)
    {
        if (personInfoUI)
        {
            personInfoUI.SetStandBy(isStandBy);
        }
    }

    private void StartAnimation()
    {
        personAnimationController.DoMove();


        if (navAgent)
        {
            navAgent.MovePerson();
        }

        if (navAgentFollow) //跟谁人员，不用管原来的代码，设置跟随人员的位置就行
        {
            navAgentFollow.MovePerson();
        }
    }

    private void StopAnimation()
    {
        personAnimationController.DoStop();//动画停下

        if (navAgent)
        {
            navAgent.StopPerson();
        }

        if (navAgentFollow) //跟谁人员，不用管原来的代码，设置跟随人员的位置就行
        {
            navAgentFollow.StopPerson();
        }
    }


    private void SetPosition(TagPosInfo hisPosInfo)
    {
        Vector3 showPos = hisPosInfo.ShowPos;
        if (navAgent && useNavAgent) //使用NavAgent修改位置
        {
            navAgent.SetDestination(hisPosInfo, 1); //新的设置位置的有效代码
        }
        else
        {
            SetPosition(targetPos); //原来的修改位置的代码
        }

        if (navAgentFollow) //跟谁人员，不用管原来的代码，设置跟随人员的位置就行
        {
            navAgentFollow.SetDestination(hisPosInfo, 1);
        }
    }

    private void SetPosition(Vector3 pos)
    {
        //获取方向
        SetRotation(pos);
        //agent.enabled = false;
        //Debug.LogError("BuildingController.isTweening:" + BuildingController.isTweening);
        if (BuildingController.isTweening)
        {
            if (currentDepNode != null && currentDepNode.floorCube != null)
            {
                //Debug.LogError("isTweening");
                //transform.position = targetPos;
            }
        }
        else
        {
            if (ThroughWallsManage.Instance && ThroughWallsManage.Instance.isCharacterControllerThroughWallsTest)
            {
                Vector3 posT = Vector3.Lerp(transform.position, pos, LocationManager.Instance.damper * Time.deltaTime);
                //posT = new Vector3(transform.position.x, targetPos.y, transform.position.z);
                //bool b = LocationManager.IsBelongtoCurrentDep(this);
                float disT = Vector3.Distance(pos, transform.position);
                if (disT < 1)//如果当前位置与目标位置超过三维里的一个单位(不考虑y轴方向)，就可以进行穿墙移动,小于一个单位会被阻挡
                {
                    bool b = CheckDep();
                    if (b)
                    {
                        //PersonMove personmove = gameObject.GetComponent<PersonMove>();

                        float yt = Mathf.Abs(pos.y - transform.position.y);
                        if (yt <= 0.1f)//在用角色控制器移动人员时，应该尽量让y轴方向位置，没什么阻挡，不然从1层切换到2层时，可能出现卡在2层下面
                        {
                            if (personmove != null)
                            {
                                personmove.SetPosition(posT);
                                //personmove.SetPosition(targetPos);
                            }
                        }
                        else
                        {
                            //transform.position = posT;
                            SetTransformPos(posT);
                        }
                    }
                    else//如果三维人员当前位置区域与定位信息里的区域不一致，就可以进行穿墙移动
                    {
                        //transform.position = posT;
                        SetTransformPos(posT);
                    }
                }
                else
                {
                    //transform.position = posT;
                    SetTransformPos(posT);
                }
            }
            else if (ThroughWallsManage.Instance && ThroughWallsManage.Instance.isNavMeshThroughWallsTest)
            {
                if (agent)
                {
                    agent.SetDestination(pos);
                }
            }
            else
            {
                //transform.position = Vector3.Lerp(transform.position, targetPos, LocationManager.Instance.damper * Time.deltaTime);
                SetTransformPos(Vector3.Lerp(transform.position, pos, LocationManager.Instance.damper * Time.deltaTime));
            }
        }
    }

    private void SetRotation(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
            //缓慢转动到目标点
            transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }
    }

    public void SetPositionInActiveFalse()
    {
        if (!LocationManager.Instance.isShowLeavePerson)
        {
            if (IsPersonLeave()) return; //人员处于离开状态，就不移动了
        }

        if (isInCurrentRange == false)//如果位置点不在当前所在区域范围内部
        {
            if (currentDepNode.monitorRangeObject && currentDepNode.monitorRangeObject.IsOnLocationArea)
            {
                //if (!LocationManager.Instance.isShowLeavePerson)
                //{
                //    Vector2 v = currentDepNode.monitorRangeObject.PointForPointToPolygon(new Vector2(targetPos.x, targetPos.z));
                //    targetPos = new Vector3(v.x, targetPos.y, v.y);
                //}
            }
            else
            {
                return;
            }
        }
        else
        {
            if (currentDepNode == null) return;
            if (!LocationManager.Instance.isShowLeavePerson)
            {
                //如果位置点在当前所在区域范围内部,但是当前区域不是定位区域返回
                if (currentDepNode.monitorRangeObject == null || currentDepNode.monitorRangeObject.IsOnLocationArea == false) return;
            }
        }

        float dis = Vector3.Distance(transform.position, targetPos);

        //获取方向
        Vector3 dir = targetPos - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
            //缓慢转动到目标点
            transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }


        if (BuildingController.isTweening)
        {
            if (currentDepNode != null && currentDepNode.floorCube != null)
            {
                //Debug.LogError("isTweening");
                transform.position = targetPos;
            }
        }
        else
        {
            if (ThroughWallsManage.Instance && ThroughWallsManage.Instance.isCharacterControllerThroughWallsTest)
            {
                Vector3 posT = Vector3.Lerp(transform.position, targetPos, LocationManager.Instance.damper * Time.deltaTime);
                posT = new Vector3(transform.position.x, targetPos.y, transform.position.z);
                //bool b = LocationManager.IsBelongtoCurrentDep(this);
                float disT = Vector3.Distance(targetPos, transform.position);
                if (disT < 1)//如果当前位置与目标位置超过三维里的一个单位(不考虑y轴方向)，就可以进行穿墙移动,小于一个单位会被阻挡
                {
                    bool b = CheckDep();
                    if (b)
                    {
                        //PersonMove personmove = gameObject.GetComponent<PersonMove>();

                        float yt = Mathf.Abs(targetPos.y - transform.position.y);
                        if (yt <= 0.1f)//在用角色控制器移动人员时，应该尽量让y轴方向位置，没什么阻挡，不然从1层切换到2层时，可能出现卡在2层下面
                        {
                            if (personmove != null)
                            {
                                personmove.SetPosition(posT);
                            }
                        }
                        else
                        {
                            //transform.position = posT;
                            SetTransformPos(posT);
                        }
                    }
                    else//如果三维人员当前位置区域与定位信息里的区域不一致，就可以进行穿墙移动
                    {
                        //transform.position = posT;
                        SetTransformPos(posT);
                    }
                }
                else
                {
                    //transform.position = posT;
                    SetTransformPos(posT);
                }
            }
            else if (ThroughWallsManage.Instance && ThroughWallsManage.Instance.isNavMeshThroughWallsTest)
            {
                if (agent)
                {
                    agent.SetDestination(targetPos);
                }
            }
            else
            {
                //transform.position = Vector3.Lerp(transform.position, targetPos, LocationManager.Instance.damper * Time.deltaTime);
                SetTransformPos(Vector3.Lerp(transform.position, targetPos, LocationManager.Instance.damper * Time.deltaTime));
            }
        }
    }

    /// <summary>
    /// 设置位置关闭CharecterController，不然会造成碰撞
    /// </summary>
    private void SetTransformPos(Vector3 pos)
    {
        //personAnimationController.SetAnimator(false);
        //personmove.SetCharacterController(false);
        transform.position = pos;
        //personmove.SetCharacterController(true);
        //personAnimationController.SetAnimator(true);
    }

    public Collider rayCollider;

    /// <summary>
    /// 设置人员高度，通过打射线。来判断最近的楼层地板来设置位置
    /// </summary>
    public void SetPersonHeightByRay()
    {
        Vector3 targetPosTT = targetPos;
        float yHeight = targetPos.y;

        float downFloorHeight = targetPos.y;
        bool isRaycasDownFloor = false;
        RaycastHit downhit;
        if (Physics.Raycast(targetPos, Vector3.down, out downhit, 200f, LayerMask.GetMask("Floor")))
        {
            downFloorHeight = downhit.point.y;
            isRaycasDownFloor = true;
        }

        if (!isRaycasDownFloor)//如果第一次向下射线检测没有打到地板，就位置上移0.3个单位打射线
        {
            Vector3 targetPosT = new Vector3(targetPos.x, targetPos.y + 0.3f, targetPos.z);
            if (Physics.Raycast(targetPosT, Vector3.down, out downhit, 200f, LayerMask.GetMask("Floor")))
            {
                downFloorHeight = downhit.point.y;
                isRaycasDownFloor = true;
            }
        }

        float upFloorHeight = targetPos.y;
        bool isRaycasUpFloor = false;
        RaycastHit uphit;
        if (Physics.Raycast(targetPos, Vector3.up, out uphit, 200f, LayerMask.GetMask("Floor")))
        {
            upFloorHeight = uphit.point.y;
            isRaycasUpFloor = true;
        }

        if (isRaycasUpFloor)//如果第一次向下射线检测打到打到地板，
        {
            if (!isRaycasDownFloor || (upFloorHeight - yHeight) < (yHeight - downFloorHeight))
            {
                float offsetT = 0.5f;
                Vector3 targetPosT = new Vector3(targetPos.x, upFloorHeight + offsetT, targetPos.z);
                if (Physics.Raycast(targetPosT, Vector3.down, out uphit, 200f, LayerMask.GetMask("Floor")))//改变打射线方向，位置上移0.3个单位向下打射线
                {
                    if (Mathf.Abs(uphit.point.y - upFloorHeight) < offsetT)
                    {
                        upFloorHeight = uphit.point.y;
                        isRaycasUpFloor = true;
                    }
                }
            }
        }

        //Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.green, 1f);
        //float yTemp = (upFloorHeight - yHeight) <= (yHeight - downFloorHeight) ? upFloorHeight : downFloorHeight;
        float yTemp = targetPos.y;
        if (isRaycasDownFloor || isRaycasUpFloor)
        {
            if (isRaycasDownFloor && isRaycasUpFloor)
            {
                yTemp = (upFloorHeight - yHeight) <= (yHeight - downFloorHeight) ? upFloorHeight : downFloorHeight;

                rayCollider = (upFloorHeight - yHeight) <= (yHeight - downFloorHeight) ? uphit.collider : downhit.collider;
            }
            else
            {
                if (isRaycasDownFloor)
                {
                    yTemp = downFloorHeight;
                    rayCollider = downhit.collider;
                }
                else if (isRaycasUpFloor)
                {
                    yTemp = upFloorHeight;
                    rayCollider = uphit.collider;
                }
                else
                {
                    rayCollider = null;
                }
            }
        }

        //float yLerp = Mathf.Lerp(transform.position.y, yTemp, LocationManager.Instance.damper * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, yLerp, transform.position.z);
        //if (yTemp > 0.8f) return;//如果上下调整距离需要超过0.8个单位，就不上下移动了
        yTemp = yTemp + 0.005f;//为了防止人物脚部与地板碰撞发生抖动，可以适当向上偏移一点点
        targetPos = new Vector3(targetPos.x, yTemp, targetPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStartOnTrigger) return;
        //MonitorRangeObject mapAreaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        //if (mapAreaObject)
        //{
        //    FollowUIAlarmOn(mapAreaObject.info.Name);
        //}
        //Debug.LogError("OnTriggerEnter");
        //Debug.LogFormat("code:{0},区域名称:{1},OnTriggerEnter", Tag.Code, other.name);

        MonitorRangeObject mapAreaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        if (mapAreaObject)
        {
            TransformM tranM = mapAreaObject.info.Transfrom;
            if (tranM != null)
            {
                bool isSameFloor = mapAreaObject.CheckFloorDepNode(currentDepNode);
                isSameFloor = true;//以后这个判断就不要了，在做完编辑区域之后
                if (tranM.IsOnLocationArea)
                {
                    if (!locationAreas.Contains(mapAreaObject) && isSameFloor)
                    {
                        if (locationAreas.Count == 0)//如果人员未处于显示状态
                        {
                            SetRendererEnable(true);
                        }
                        locationAreas.Add(mapAreaObject);

                    }
                }

                //三维里通过碰撞检测，来触发人员告警，这里注释是改成服务端发送过来触发告警（下面的ShowAlarm方法）
                //if (tranM.IsOnAlarmArea)
                //{
                //    if (!alarmAreas.Contains(mapAreaObject) && isSameFloor)
                //    {
                //        if (alarmAreas.Count == 0)//如果人员未处于告警状态
                //        {
                //            FollowUIAlarmOn(mapAreaObject.info.Name);
                //        }
                //        alarmAreas.Add(mapAreaObject);
                //    }
                //}

                if (!normalAreas.Contains(mapAreaObject))
                {
                    normalAreas.Add(mapAreaObject);
                    SetArea();
                }
            }
        }


    }

    ////20190118大量卡数据测试时，这里会导致卡顿
    //private void OnTriggerStay(Collider other)
    //{
    //    if (!isStartOnTrigger) return;

    //    if (!isOnTriggerStayOnce)
    //    {
    //        OnTriggerEnter(other);
    //        isOnTriggerStayOnce = true;
    //    }

    //    //OnTriggerEnter(other);//大数据测试修改
    //    //Debug.LogFormat("code:{0},区域名称:{1},OnTriggerStay", Tag.Code, other.name);
    //    //Debug.Log("区域名称OnTriggerStay");
    //}

    private void OnTriggerExit(Collider other)
    {
        if (!isStartOnTrigger) return;
        //MonitorRangeObject mapAreaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        //if (mapAreaObject)
        //{

        //    FollowUINormalOn();

        //}

        MonitorRangeObject areaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        OnTriggerExitEx(areaObject);
    }

    public void OnTriggerExitEx(MonitorRangeObject areaObject)
    {
        if (areaObject)
        {
            //Debug.LogFormat("LocationObject.OnTriggerExit code:{0},区域名称:{1}", Tag.Code, areaObject.name);
            if (locationAreas.Contains(areaObject))
            {
                locationAreas.Remove(areaObject);
                if (locationAreas.Count == 0)
                {
                    if (!LocationManager.Instance.isShowLeavePerson)
                    {
                        //SetRendererEnable(false);
                        SetRendererEnableFalse();
                    }
                }
            }

            //三维里通过碰撞检测来检测人员是否在告警区域内部，来关闭人员告警，这里注释是改成服务端发送过来关闭告警（下面的HideAlarm方法）
            //if (alarmAreas.Contains(areaObject))
            //{
            //    alarmAreas.Remove(areaObject);
            //    if (alarmAreas.Count == 0)
            //    {
            //        FollowUINormalOn();
            //    }
            //}

            if (normalAreas.Contains(areaObject))
            {
                normalAreas.Remove(areaObject);
                SetArea();
            }

            //TransformM tranM = mapAreaObject.info.Transfrom;
            //if (tranM != null)
            //{
            //    if (tranM.IsOnLocationArea)
            //    {
            //        SetRendererEnable(false);
            //        FollowUINormalOn();
            //    }
            //    else
            //    {

            //    }
            //}

        }
    }

    /// <summary>
    /// 检测当前定位信息所在的区域，是否与三维显示的区域位置一致
    /// </summary>
    public bool CheckDep()
    {
        //MonitorRangeObject monitorRangeObject = normalAreas.Find((i) => i.depNode.NodeID == currentDepNode.NodeID);
        MonitorRangeObject monitorRangeObject = normalAreas.Find((i) =>
        {
            if (i.depNode != null && i.depNode.NodeID == currentDepNode.NodeID)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        if (monitorRangeObject == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 创建跟随UI
    /// </summary>
    /// <returns></returns>
    public PersonInfoUI CreateFollowUI()
    {
        if (personnel == null) return null;
        if (personInfoUI != null) return personInfoUI;
        if (gameObject == null)
        {
            Debug.Log("LocationObject.CreateFollowUI gameObject == null");//模型加载卸载出问题导致在里面的人物模型也被卸载掉了
            return null;
        }

        GameObject targetTagObj = GetUITarget();
        //LayerMask mask = 0 << LayerMask.NameToLayer("Person");
        LayerMask mask = LayerMask.NameToLayer("Person");
        Log.Error("CreateFollowUI", "mask:"+mask.value);
        var uiPrefab = LocationManager.Instance.PersonInfoUIPrefab.gameObject;
        //GameObject obj = UGUIFollowManage.Instance.CreateItemEX(uiPrefab, targetTagObj, "LocationNameUI", mask.value);
        GameObject obj = UGUIFollowManage.Instance.CreateItemEX(uiPrefab, targetTagObj, "LocationNameUI",true);
        personInfoUI = obj.GetComponent<PersonInfoUI>();
        if (Tag == null)
        {
            Log.Error("Location.CreateFollowUI", "Tag == null");
        }
        else if (personnel == null)
        {
            //Log.Error("Location.CreateFollowUI", "personnel == null：" + Tag.Name);
        }
        personInfoUI.Init(personnel, this);
        return personInfoUI;
    }

    private GameObject GetUITarget()
    {
        GameObject targetTagObj =null;
        if (navAgentFollow != null)
        {
            targetTagObj = UGUIFollowTarget.CreateTitleTag(navAgentFollow.gameObject, Vector3.zero);
        }
        else
        {
            targetTagObj = UGUIFollowTarget.CreateTitleTag(this.gameObject, Vector3.zero);
        }
        
        return targetTagObj;
    }

    /// <summary>
    /// 人员跟随UI，进入告警状态
    /// </summary>
    public void FollowUIAlarmOn(string areaname)
    {
        Debug.LogFormat("Tag:{0},区域:{1},告警！", Tag.Code, areaname);
        CreateFollowUI();
        if (personInfoUI != null)
        {
            personInfoUI.ShowAlarm();
            personInfoUI.SetTxtAlarmAreaName(areaname);
        }
    }


    /// <summary>
    /// 人员跟随UI开启
    /// </summary>
    public void FollowUINormalOn()
    {
        CreateFollowUI();
        if (personInfoUI != null)
        {
            SetFollowPersonInfoUIActive(true);
            personInfoUI.ShowNormal();
        }
    }

    /// <summary>
    /// 人员跟随UI开启
    /// </summary>
    public void FollowUIOn()
    {
        //Log.Info("LocationObject.FollowUIOn", "name:" + Tag.Name);
        Transform titleTag = transform.Find("TitleTag");
        if (titleTag != null)
        {
            UGUIFollowManage.Instance.SetUIbyTarget("LocationNameUI", titleTag.gameObject, true);
        }
        SetFollowPersonInfoUIActive(true);
    }

    /// <summary>
    /// 人员跟随UI关闭
    /// </summary>
    public void FollowUIOff()
    {
        //Log.Info("LocationObject.FollowUIOff", "name:" + Tag.Name);
        Transform titleTag = transform.Find("TitleTag");
        if (titleTag != null)
        {
            UGUIFollowManage.Instance.SetUIbyTarget("LocationNameUI", titleTag.gameObject, false);
        }
        SetFollowPersonInfoUIActive(false);
    }

    ///// <summary>
    ///// 获取碰撞器
    ///// </summary>
    ///// <returns></returns>
    //public Collider GetCollider()
    //{
    //    if (myCollider == null)
    //    {
    //        myCollider = gameObject.GetComponent<Collider>();
    //    }
    //    return myCollider;
    //}

    ///// <summary>
    ///// 设置碰撞器是否启用
    ///// </summary>
    //public void SetColliderEnable(bool isEnable)
    //{
    //    GetCollider();
    //    myCollider.enabled = isEnable;
    //}

    /// <summary>
    /// 获取渲染器
    /// </summary>
    /// <returns></returns>
    public Renderer[] GetRenderer()
    {
        if (renders == null || renders.Length == 0)
        {
            renders = gameObject.GetComponentsInChildren<Renderer>();
        }
        return renders;
    }

    /// <summary>
    /// 设置渲染器是否启用
    /// </summary>
    public void SetRendererEnableFalse()
    {
        bool b = IsBelongtoCurrentDep();
        if (!b)
        {
            SetRendererEnable(false);
        }
    }

    /// <summary>
    /// 设置渲染器是否启用
    /// </summary>
    public void SetRendererEnable(bool isEnable,bool changeFollowUI=true)
    {       
        if (isEnable == false)
        {
            //Debug.Log("LocationObject.SetRendererEnable:" + isEnable);
        }

        if (ViewState.人员定位 != ActionBarManage.Instance.CurrentState && !isEnable) return;
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            SetPosSphereActive(isEnable);
        }
        if (navAgentFollow == null)
        {
            //NavMesh人物为空的情况，显示普通的人
            GetRenderer();
            isRenderEnable = isEnable;
            if (renders != null)
            {
                renders.ForEach(i => i.enabled = isEnable);
            }
        }      
        if(changeFollowUI)
        {
            if (isEnable)
            {
                FollowUIOn();
            }
            else
            {
                if (LocationManager.Instance.currentLocationFocusObj == this)
                {
                    //LocationManager.Instance.HideCurrentPersonInfoUI();
                    bool b = IsBelongtoCurrentDep();
                    if (!b)
                    {
                        LocationManager.Instance.HideCurrentPersonInfoUI();
                    }
                }
                FollowUIOff();
            }
        }       
    }



    /// <summary>
    /// 判断当前人员是否在当前区域下，并执行相关操作
    /// </summary>
    /// <param name="locationObjectT"></param>
    /// <returns></returns>
    public bool IsBelongtoCurrentDep()
    {
        DepNode currentDepT = FactoryDepManager.currentDep;
        if (currentDepT == null)
        {
            Debug.LogError("LocationObject.IsBelongtoCurrentDep currentDepT == null"); return false;
        }
        if (currentDepT.TopoNode == null)
        {
            //Debug.LogError("LocationObject.IsBelongtoCurrentDep currentDepT.TopoNode == null");
            return false;
        }
        if (currentDepT.IsRoom())
        {
            //currentDepT = depnodeT.ParentNode;
            currentDepT = GetRoomInFloor(currentDepT);
        }
        if (currentDepT == null)
        {
            currentDepT = FactoryDepManager.currentDep;
        }

        //List<DepNode> depNodeListT = currentDepT.GetComponentsInChildren<DepNode>(true).ToList();
        List<DepNode> depNodeListT = LocationManager.Instance.currentChildDepNodeList;
        //if (!depNodeListT.Contains(currentDepNode))
        //{
        //    //RoomFactory.Instance.ChangeDepNodeNoTween();
        //    ////RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);

        //    //LocationManager.Instance.RecoverBeforeFocusAlignToOrigin();
        //    return false;
        //}
        if (currentDepNode == null)
        {
            return false;
        }
        if (!depNodeListT.Find((item) => currentDepNode.NodeID == item.NodeID))
        {
            //RoomFactory.Instance.ChangeDepNodeNoTween();
            ////RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);

            //LocationManager.Instance.RecoverBeforeFocusAlignToOrigin();

            return false;
        }
        return true;
    }

    [ContextMenu("SetTTHide")]
    public void SetTTHide()
    {
        GetRenderer();
        isRenderEnable = false;
        if (renders != null)
        {
            renders.ForEach(i => i.enabled = false);
        }
    }

    [ContextMenu("SetTTShow")]
    public void SetTTShow()
    {
        GetRenderer();
        isRenderEnable = true;
        if (renders != null)
        {
            renders.ForEach(i => i.enabled = true);
        }
    }

    private U3DPosition CreateU3DPosition(TagPosition tp, Transform t)
    {
        U3DPosition u3dPos = new U3DPosition();
        u3dPos.Tag = tp.Tag;
        u3dPos.Time = tp.Time;
        u3dPos.Number = tp.Number;
        u3dPos.Power = tp.Power;
        u3dPos.Flag = tp.Flag;
        //Vector3 temp= LocationManager.GetDisRealVector(targetPos);
        u3dPos.X = t.position.x;
        u3dPos.Y = t.position.y;
        u3dPos.Z = t.position.z;
        return u3dPos;
    }

    private U3DPosition CreateU3DPosition(U3DPosition p)
    {
        U3DPosition u3dPos = new U3DPosition();
        u3dPos.Tag = p.Tag;
        u3dPos.Time = p.Time;
        u3dPos.Number = p.Number;
        u3dPos.Power = p.Power;
        u3dPos.Flag = p.Flag;
        //Vector3 temp= LocationManager.GetDisRealVector(targetPos);
        u3dPos.X = p.X;
        u3dPos.Y = p.Y;
        u3dPos.Z = p.Z;
        return u3dPos;
    }

    /// <summary>
    /// 保存3D位置信息
    /// </summary>
    public void SaveU3DHistoryPosition()
    {

        U3DPosition u3dPos = CreateU3DPosition(tagPosInfo, transform);

        if (!IsBusy)
        {
            Debug.Log(name + ":(" + Time.time + ")");
            IsBusy = true;
            u3dPos.Flag = u3dPos.Flag + "(" + name + ")";
            //u3dPos2.Flag = u3dPos2.Flag + "(" + name + ")";

            thread = new Thread(() =>
            {
                try
                {
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    U3DPosition u3dPos2 = CreateU3DPosition(u3dPos);
                    //    u3dPos2.Tag += i;
                    //    Thread thread2 = new Thread(() =>
                    //    {
                    //        CommunicationObject.Instance.AddU3DPosition(u3dPos2);
                    //    });
                    //    thread2.Start();
                    //}

                    CommunicationObject.Instance.AddU3DPosition(u3dPos);

                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                IsBusy = false;

            });
            thread.Start();
        }
        else
        {
            Debug.Log(string.Format("Tag:{0},IsBusy:{1}", name, IsBusy));
        }

    }

    bool isBusy;

    public bool IsBusy
    {
        get
        {
            return isBusy;
        }

        set
        {
            isBusy = value;
        }
    }



    /// <summary>
    /// 设置跟随UI是否显示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetFollowPersonInfoUIActive(bool isActive)
    {
        if (personInfoUI != null)
        {
            personInfoUI.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// 设置当前所在区域
    /// </summary>
    public void SetArea()
    {
        PhysicalTopology pty = GetAreaPhysicalTopologyByType(AreaTypes.机房);

        pty = pty == null ? GetAreaPhysicalTopologyByType(AreaTypes.楼层) : pty;
        pty = pty == null ? GetAreaPhysicalTopologyByType(AreaTypes.大楼) : pty;
        pty = pty == null ? GetAreaPhysicalTopologyByType(AreaTypes.区域) : pty;

        if (personInfoUI != null)
        {
            if (pty != null)
            {
                personInfoUI.SetTxtAreaName(pty.Name);
            }
            else
            {
                SetNearBuildingName();
                //personInfoUI.SetTxtAreaName("厂区内");
            }
        }
    }
    /// <summary>
    /// 人员靠近的建筑
    /// </summary>
    private void SetNearBuildingName()
    {
        try
        {
            if (personInfoUI == null) return;
            if (FactoryDepManager.Instance == null)
            {
                personInfoUI.SetTxtAreaName("厂区内");
            }
            else
            {
                List<BuildingController> buildings = FactoryDepManager.Instance.GetAllBuildngController();
                if (buildings == null || buildings.Count == 0)
                {
                    personInfoUI.SetTxtAreaName("厂区内");
                    return;
                }
                BuildingController colT = null;
                float dis = float.MaxValue;
                foreach (var building in buildings)
                {
                    float tempDis = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(building.transform.position.x, building.transform.position.z));
                    bool isMin = tempDis < dis ? true : false;
                    if (isMin)
                    {
                        dis = tempDis;
                        colT = building;
                    }
                }
                if (colT != null)
                {
                    if (string.IsNullOrEmpty(colT.NodeName))
                    {
                        personInfoUI.SetTxtAreaName("厂区内");
                    }
                    else
                    {
                        personInfoUI.SetTxtAreaName(string.Format("{0}附近", colT.NodeName));
                    }
                }
                else
                {
                    personInfoUI.SetTxtAreaName("厂区内");
                }
            }
        }catch(Exception e)
        {
            Debug.LogErrorFormat("Error->Info:{0}  Source:{1}",e.ToString(),e.Source);
            if (personInfoUI != null) personInfoUI.SetTxtAreaName("厂区内");
        }
        
    }

    /// <summary>
    /// 获取区域节点信息通过类型
    /// </summary>
    public PhysicalTopology GetAreaPhysicalTopologyByType(AreaTypes typeT)
    {
        foreach (MonitorRangeObject robj in normalAreas)
        {
            if (robj.info.Type == typeT)
            {
                return robj.info;
            }
        }
        return null;
    }

    /// <summary>
    /// 开启高亮
    /// </summary>
    public void HighlightOnByFocus()
    {
        //Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //h.ConstantOn(Color.green);
        if (personInfoUI.state != PersonInfoUIState.StandbyLong)
        {
            HighlightOn(Color.green);
        }

    }
    /// <summary>
    /// 开启高亮
    /// </summary>
    public void HighlightOn(Color color)
    {
        if (isAlarming) return;
        Highlighter h = GetHighLighter();
        if (h)
        {
            h.ConstantOn(color);
            //Debug.LogError("Highlight On:"+h.gameObject.name);
        }
    }

    /// <summary>
    /// 关闭高亮
    /// </summary>
    [ContextMenu("HighlightOff")]
    public void HighlightOff()
    {
        Highlighter h = GetHighLighter();
        if (h)
        {
            h.ConstantOff();
            //Debug.LogError("HighlightOff:" + h.gameObject.name);
        }
    }

    /// <summary>
    /// 开启高亮闪烁
    /// </summary>
    public void FlashingOn(Color color)
    {
        Highlighter h = GetHighLighter();
        if(h) h.FlashingOn(new Color(color.r, color.g, color.b, 0), new Color(color.r, color.g, color.b, 1));
    }

    /// <summary>
    /// 关闭高亮闪烁
    /// </summary>
    [ContextMenu("FlashingOff")]
    public void FlashingOff()
    {
        Highlighter h = GetHighLighter();
        if(h) h.FlashingOff();
    }

    /// <summary>
    /// 黄色高亮:长时间不动
    /// </summary>
    [ContextMenu("HighlightOnStandbyLong")]
    public void HighlightOnStandbyLong()
    {
        HighlightOn(Color.yellow);
    }

    /// <summary>
    /// 关闭因长时间不动黄色高亮
    /// </summary>
    public void HighlightOffStandbyLong()
    {
        if (LocationManager.Instance.currentLocationFocusObj == this)
        {
            HighlightOnByFocus();
        }
        else
        {
            //HighlightOff(); //目前都高亮
        }
    }
    /// <summary>
    /// 获取高亮插件
    /// </summary>
    /// <returns></returns>
    private Highlighter GetHighLighter()
    {
        try
        {
            Highlighter h = null;
            if (navAgentFollow != null)
            {
                h = navAgentFollow.gameObject.AddMissingComponent<Highlighter>();
            }
            else
            {
                h = gameObject.AddMissingComponent<Highlighter>();
            }
            return h;
        }catch(Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 展示告警
    /// </summary>
    public void ShowAlarm(LocationAlarm locationAlarm)
    {
        //if (!alarmAreas.Contains(mapAreaObject))
        //{
        //    if (alarmAreas.Count == 0)//如果人员未处于告警状态
        //    {
        //        if (isAlarming) return;
        //        FollowUIAlarmOn(mapAreaObject.info.Name);
        //        isAlarming = true;
        //        Debug.LogErrorFormat("区域：{0},告警了！", Tag.Code);
        //    }
        //    alarmAreas.Add(mapAreaObject);
        //}

        LocationAlarm locationAlarmT = alarmList.Find((i) => i.Id == locationAlarm.Id);

        if (locationAlarmT == null)
        {
            MonitorRangeObject monitorRangeObject = MonitorRangeManager.Instance.GetMonitorRangeObjectByAreaId(locationAlarm.AreaId);
            if (alarmList.Count == 0)//如果人员未处于告警状态
            {
                if (isAlarming) return;
                isAlarming = true;

                string nameT = "";
                if (monitorRangeObject != null)
                {
                    nameT = monitorRangeObject.info.Name;
                }
                FollowUIAlarmOn(nameT);
                FlashingOn(Color.red);
                Debug.LogErrorFormat("人员：{0},告警了！", Tag.Code);
            }
            alarmList.Add(locationAlarm);
            if (!alarmAreas.Contains(monitorRangeObject))
            {
                alarmAreas.Add(monitorRangeObject);
            }
        }

    }

    /// <summary>
    /// 关闭告警
    /// </summary>
    public void HideAlarm(LocationAlarm locationAlarm)
    {
        //if (alarmAreas.Contains(areaObject))
        //{
        //    alarmAreas.Remove(areaObject);
        //    if (alarmAreas.Count == 0)
        //    {
        //        if (isAlarming == false) return;
        //        FollowUINormalOn();
        //        isAlarming = false;
        //        Debug.LogErrorFormat("区域：{0},消警了！", Tag.Code);
        //    }
        //}

        LocationAlarm locationAlarmT = alarmList.Find((i) => i.Id == locationAlarm.Id);

        if (locationAlarmT != null)
        {
            alarmList.Remove(locationAlarmT);
            MonitorRangeObject monitorRangeObjectT = alarmAreas.Find((I) => I.depNode.NodeID == locationAlarm.AreaId);
            alarmAreas.Remove(monitorRangeObjectT);
            if (alarmList.Count == 0)
            {
                if (isAlarming == false) return;
                FollowUINormalOn();
                isAlarming = false;
                FlashingOff();
                Debug.LogErrorFormat("人员：{0},消警了！", Tag.Code);
            }
        }
    }

    /// <summary>
    /// 关闭告警
    /// </summary>
    public void HideAlarm(int areaid)
    {

        LocationAlarm locationAlarmT = alarmList.Find((i) => i.AreaId == areaid);

        if (locationAlarmT != null)
        {
            alarmList.Remove(locationAlarmT);
            MonitorRangeObject monitorRangeObjectT = alarmAreas.Find((I) => I.depNode.NodeID == areaid);
            alarmAreas.Remove(monitorRangeObjectT);
            if (alarmList.Count == 0)
            {
                if (isAlarming == false) return;
                FollowUINormalOn();
                isAlarming = false;
                FlashingOff();
                Debug.LogErrorFormat("人员：{0},消警了！", Tag.Code);
            }
        }
    }

    #region 设置人物相关状态

    /// <summary>
    /// 设置人员状态
    /// </summary>
    public void SetState(TagPosition tagpT)
    {
        //if()
        if (personInfoUI == null) return;
        if (personAnimationController == null)
        {
            personAnimationController = GetComponent<PersonAnimationController>();
        }

        if (tagpT.PowerState == 0)
        {
            SetLowBatteryActive(false);
        }
        else
        {
            SetLowBatteryActive(true);
        }
        //if (tagpT.AreaState == 1 && tagpT.MoveState == 0)//不在定位区域属于离开状态
        //{
        //    SwitchLeave();
        //    //personAnimationController.DoStop();
        //    personAnimationController.DoMove();
        //}
        //else//在定位区域
        //{

        //}

        //Log.Info("SetState");

        if (tagpT.MoveState == 0)//卡正常运动状态
        {
            //Log.Info("SwitchNormal");
            SwitchNormal();
            StartAnimation();
        }
        else if (tagpT.MoveState == 1 || tagpT.MoveState == 2)//待机状态
        {
            //Log.Info("SwitchStandby");
            //SwitchStandby();
            //StopAnimation();
        }
        else if (tagpT.MoveState == 3)//长时间不动状态
        {
            //Log.Info("SwitchStandbyLong");
            SwitchStandbyLong();
            StopAnimation();
        }
        else
        {

        }
    }

    /// <summary>
    /// 转换为正常状态
    /// </summary>
    [ContextMenu("SwitchNormal")]
    public void SwitchNormal()
    {
        if(personInfoUI==null)
        {
            return;
        }
        personInfoUI.personnelNodeManage.SwitchNormal();
        RecoverTransparentLeave();
        HighlightOffStandbyLong();
        //infoUi.l
        personInfoUI.HideStandByTime();
    }

    /// <summary>
    /// 待机状态，包含静止状态（待机之后小于300秒）
    /// </summary>
    [ContextMenu("SwitchStandby")]
    public void SwitchStandby()
    {
        if (personInfoUI == null)
        {
            return;
        }
        //SwitchStateSprite(PersonInfoUIState.Standby);
        personInfoUI.personnelNodeManage.SwitchStandby();
        RecoverTransparentLeave();
        HighlightOffStandbyLong();
        personInfoUI.ShowStandByTime();
    }

    /// <summary>
    /// 待机长时间不动
    /// </summary>
    [ContextMenu("SwitchStandby")]
    public void SwitchStandbyLong()
    {
        if (personInfoUI == null)
        {
            return;
        }
        //SwitchStateSprite(PersonInfoUIState.Standby);
        personInfoUI.personnelNodeManage.SwitchStandbyLong();
        RecoverTransparentLeave();
        HighlightOnStandbyLong();
        personInfoUI.ShowStandByTime();
    }

    /// <summary>
    /// 设置为离开状态
    /// </summary>
    [ContextMenu("SwitchLeave")]
    public void SwitchLeave()
    {
        if (personInfoUI == null)
        {
            return;
        }
        personInfoUI.personnelNodeManage.SwitchLeave();
        //TransparentLeave();
        HighlightOffStandbyLong();
        personInfoUI.HideStandByTime();
    }

    /// <summary>
    /// 设置为弱电状态
    /// </summary>
    [ContextMenu("SetLowBattery")]
    public void SetLowBattery()
    {
        if (personInfoUI == null)
        {
            return;
        }
        personInfoUI.personnelNodeManage.SetLowBattery();
    }

    /// <summary>
    /// 设置为弱电状态
    /// </summary>
    public void SetLowBatteryActive(bool isActive)
    {
        PersonnelNodeManage manageT = personInfoUI.personnelNodeManage;
        if (isActive) manageT.ShowAlarm(false);
        else manageT.ShowNormal(false);
        manageT.SetLowBatteryActive(isActive);
    }

    /// <summary>
    /// 离开时透明
    /// </summary>
    [ContextMenu("TransparentLeave")]
    public void TransparentLeave()
    {

        GameObjectMaterial.SetAllTransparent(gameObject, 0.5f);

        //SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (SkinnedMeshRenderer render in skinnedMeshRenderers)
        //{
        //    Material[] mats = render.materials;
        //    foreach (Material m in mats)
        //    {
        //        m.color = new Color(m.color.r, m.color.g, m.color.b, 0.3f); 
        //    }
        //}
    }

    public void SetisInRange(bool b)
    {
        isInCurrentRange = b;
        //if (Tag.Code == "0995" || Tag.Code == "097F")
        //{
        //    //if (isInRange)
        //    //{
        //    //    Debug.LogError(Tag.Code + ":在范围内");
        //    //}
        //    //else
        //    //{
        //    //    Debug.LogError(Tag.Code + ":不在范围内");
        //    //}
        //}
    }

    /// <summary>
    /// 恢复离开时透明状态到正常状态
    /// </summary>
    [ContextMenu("RecoverTransparentLeave")]
    public void RecoverTransparentLeave()
    {


        GameObjectMaterial.Recover(transform);
        //SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (SkinnedMeshRenderer render in skinnedMeshRenderers)
        //{
        //    Material[] mats = render.materials;
        //    foreach (Material m in mats)
        //    {
        //        m.color = new Color(m.color.r, m.color.g, m.color.b, 1f);
        //    }
        //}
    }

    #endregion

    #region 让参与计算的基站显示出来（测试）,及显示人员真实位置的测试小球
    //让参与计算的基站显示出来（测试）
    List<DevNode> archorObjs = new List<DevNode>();

    //public bool isShowArchor = false;//闪烁参与计算的基站

    /// <summary>
    /// 显示参与计算的基站
    /// </summary>
    public void ShowArchors()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            FlashingOnArchors();
        }
        //else
        //{
        //    FashingOffArchors();
        //}
    }

    /// <summary>
    /// 闪烁所有基站
    /// </summary>
    public void FlashingOnArchors()
    {
        FlashingOffArchors();

        archorObjs.Clear();

        if (tagPosInfo.Archors != null)
        {
            foreach (string astr in tagPosInfo.Archors)
            {
                Archor a = LocationManager.Instance.GetArchorByCode(astr);
                if (a == null) continue;
                int idT = a.DevInfoId;
                RoomFactory.Instance.GetDevByid(idT, (nodeT)
                    =>
                {
                    if (nodeT == null) return;
                    archorObjs.Add(nodeT);
                    nodeT.FlashingOn();
                });
            }
        }
    }

    /// <summary>
    /// 停止闪烁所有基站
    /// </summary>
    public void FlashingOffArchors()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            foreach (DevNode o in archorObjs)
            {
                if (o == null) continue;
                o.FlashingOff();
            }
        }
    }

    public GameObject posSphere;
    public Highlighter posSphereHighlighter;
    public static Transform PositionSphereParent;
    /// <summary>
    /// 创建高亮测试小球显示人员的真实位置
    /// </summary>
    public void ShowPositionSphereTest(Vector3 p)
    {

        if (PositionSphereParent == null)
        {
            GameObject o = new GameObject();
            o.name = "PositionSphereParent";
            PositionSphereParent = o.transform;
        }
        if (posSphere == null)
        {
            posSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            posSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            posSphere.name = Tag.Code;
            posSphere.transform.SetParent(PositionSphereParent);
        }

        if (posSphereHighlighter == null)
        {
            posSphereHighlighter = posSphere.AddMissingComponent<Highlighter>();
        }
        posSphereHighlighter.ConstantOn(Color.blue);
        posSphere.transform.position = p;
        //if (Tag.Code == "0988")
        //{
        //    Debug.LogError("Code0988:" + p);
        //}
    }

    /// <summary>
    /// 设置位置球的显示和
    /// </summary>
    public void SetPosSphereActive(bool b)
    {
        if (posSphere == null) return;
        posSphere.SetActive(b);
    }

    #endregion

    /// <summary>
    /// 设置是否激活状态
    /// </summary>
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public bool useNavAgent = true;

    /// <summary>
    /// 当前的
    /// </summary>
    public NavAgentControllerBase navAgent;

    /// <summary>
    /// 跟随的
    /// </summary>
    public NavAgentFollowPerson navAgentFollow;
}
