using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using HighlightingSystem;
public class BuildingController : DepNode {

    #region Field and Property
    /// <summary>
    /// 楼层展开动画部分
    /// </summary>
    public BuildingFloorTweening FloorTween;
    ///// <summary>
    ///// 建筑下的楼层
    ///// </summary>
    //[HideInInspector]
    //public BuildingFloor[] Floors;
    /// <summary>
    /// Target of camera align.
    /// </summary>
    public AlignTarget alignTarget;
    /// <summary>
    /// 建筑的碰撞体
    /// </summary>
    [HideInInspector]
    public BoxCollider BuildingCollider;
    /// <summary>
    /// 保存大楼下所有机房信息
    /// </summary>
    public Dictionary<int, GameObject> RoomDic;
    /// <summary>
    /// 当前显示的物体
    /// </summary>
    [HideInInspector]
    public FloorController currentRoom;
    /// <summary>
    /// 楼层是否展开
    /// </summary>
    [HideInInspector]
    public bool IsFloorExpand;
    /// <summary>
    /// 楼层展开时，需要关闭的物体
    /// </summary>
    public List<GameObject> HideObjectOnTweening;
    /// <summary>
    /// 是否处于楼层展开合并动画状态
    /// </summary>
    public static bool isTweening;

    private GameObject _devContainer;
    /// <summary>
    /// 漫游人物，是否进入
    /// </summary>
    [HideInInspector]
    public bool IsFPSEnter;
    /// <summary>
    /// 机房存放设备处
    /// </summary>
    public GameObject DevContainer
    {
        get
        {
            if (_devContainer == null)
            {
                InitContainer();
            }
            return _devContainer;
        }
    }
    #endregion

    void Awake()
    {
        //Reset align target.
        RoomDic = new Dictionary<int, GameObject>();
        depType = DepType.Building;
        if(NodeObject==null)
        {
            NodeObject = this.gameObject;
        }
        BuildingCollider = transform.GetComponent<BoxCollider>();
        alignTarget = new AlignTarget(transform, new Vector2(30, 0), 1, new Range(10, 90), new Range(0.5f, 1.5f));
    }
    void Start()
    {
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick = OnClick;
        trigger.onDoubleClick = OnDoubleClick;
        AddBuildingTitle();
        //gameObject.AddComponent<HighlighterOccluder>();
    }

    //private bool isInitHighLighter;
    //private Highlighter HighLightPart;

    //private void InitHighLighter()
    //{
    //    if (!isInitHighLighter)
    //    {
    //        isInitHighLighter = true;
    //        HighLightPart = transform.GetComponent<Highlighter>();
    //    }
    //}
    //void OnMouseEnter()
    //{
    //    if (!isInitHighLighter)
    //    {
    //        InitHighLighter();
    //    }
    //    if (HighLightPart != null)
    //    {

    //    }
    //}

    //void OnMuoseExit()
    //{

    //}
    /// <summary>
    /// 初始化设备存放处
    /// </summary>
    private void InitContainer()
    {
        if (_devContainer!=null) return;
        _devContainer = new GameObject("RoomDevContainer");
        _devContainer.transform.parent = transform;
        if (monitorRangeObject != null)
        {
            _devContainer.transform.localScale = GetContainerScale(transform.lossyScale);
            Vector3 floorSize = monitorRangeObject.gameObject.GetSize();
            _devContainer.transform.position = monitorRangeObject.transform.position + new Vector3(floorSize.x / 2, -floorSize.y / 2, floorSize.z / 2);
            _devContainer.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
    /// <summary>
    /// 获取设备存放处的缩放值
    /// </summary>
    /// <param name="ParentLossyScale"></param>
    /// <returns></returns>
    private Vector3 GetContainerScale(Vector3 ParentLossyScale)
    {
        float x = ParentLossyScale.x;
        float y = ParentLossyScale.y;
        float z = ParentLossyScale.z;
        if (x != 0) x = 1 / x;
        if (y != 0) y = 1 / y;
        if (z != 0) z = 1 / z;
        return new Vector3(x, y, z);
    }
    private void OnClick()
    {
        //if (ChildNodes == null || ChildNodes.Count == 0) return;
        //if (!IsFocus || isTweening || IsHoverUI()) return;
        //if (!IsFloorExpand)
        //{
            
        //}
        //else
        //{
        //    CloseFloor();
        //}
    }
    private void OnDoubleClick()
    {
        if (LocationManager.Instance.IsFocus) return;
        if (IsHoverUI()|| DevSubsystemManage.IsRoamState) return;
        if (!IsFocus)
        {
            //FocusOn();
            RoomFactory.Instance.FocusNode(this);
        }
        else
        {
            if(!isTweening)
            {
                OpenFloor();
            }
        }
    }
    /// <summary>
    /// 打开区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void OpenDep(Action onComplete = null, bool isFocusT = true)
    {
        ShowFactory();
        if (NodeObject == null)
        {
            if (onComplete != null) onComplete();
            Debug.Log("DepObject is null...");
            return;
        }
        else
        {
            ShowBuildingDev(true);
            FactoryDepManager.Instance.HideOtherBuilding(this);
            if (isFocusT)
            {
                FocusOn(onComplete);
            }
            else
            {
                if (onComplete != null)
                {
                    onComplete();
                }
            }
            DepNode lastDep = FactoryDepManager.currentDep;
            lastDep.IsFocus = false;
            FactoryDepManager.currentDep = this;
            SceneEvents.OnDepNodeChanged(lastDep, FactoryDepManager.currentDep);
        }
    }
    /// <summary>
    /// 关闭区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void HideDep(Action onComplete = null)
    {
        IsFocus = false;
        ShowBuildingDev(false);
        if (IsFloorExpand) CloseFloor(true);
        FactoryDepManager.Instance.ShowOtherBuilding();
    }
    /// <summary>
    /// 聚焦建筑
    /// </summary>
    public override void FocusOn(Action onFocusComplete=null)
    {
        IsFocus = true;
        if (NodeObject == null)
        {
            if (onFocusComplete != null) onFocusComplete();
            Debug.Log("DepObject is null...");
            return;
        }
        else
        {            
            CameraSceneManager camera = CameraSceneManager.Instance;
            if (camera)
            {
                AlignTarget alignTargetTemp = GetTargetInfo(NodeObject);
                camera.FocusTargetWithTranslate(alignTargetTemp, AreaSize, onFocusComplete,()=> 
                {
                    if (RoomFactory.Instance) RoomFactory.Instance.SetDepFoucusingState(false);
                });
            }
            else
            {
                if (onFocusComplete != null) onFocusComplete();
                Log.Alarm("CameraSceneManager.Instance==null");
            }
        }
    }
    /// <summary>
    /// 取消聚焦，返回整厂视图
    /// </summary>
    public override void FocusOff(Action onComplete)
    {
        IsFocus = false;
        CameraSceneManager.Instance.ReturnToDefaultAlign(onComplete);
    }
    /// <summary>
    /// 是否显示楼层设备
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBuildingDev(bool isShow)
    {
        foreach(DepNode dep in ChildNodes)
        {
            FloorController floor = dep as FloorController;
            if (floor)
            {
                if (isShow) floor.ShowFloorDev();
                else floor.HideFloorDev();
            }
        }
    }
    /// <summary>
    /// 是否点击在UI上
    /// </summary>
    /// <returns></returns>
    private bool IsHoverUI()
    {
        IsClickUGUIorNGUI UICheck = IsClickUGUIorNGUI.Instance;
        if (UICheck && UICheck.isOverUI)
        {
            Debug.Log("Is Click UI!");
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 展开楼层
    /// </summary>
    /// <param name="IsImmediately">是否立即展开</param>
    public void OpenFloor(bool IsImmediately = false)
    {
        IsFloorExpand = true;
        if (FloorTween)
        {
            ShowTweenObject(false);
            isTweening = true;
            SceneEvents.OnBuildingOpenStartAction();
            FloorTween.OpenBuilding(IsImmediately, ()=> 
            {
                isTweening = false;
                SetFloorCollider(true);
                Debug.Log("Open Building Complete!");
                SceneEvents.OnBuildingOpenCompleteAction();
            });
        }
    }
    
    /// <summary>
    /// 楼层收起
    /// </summary>
    /// <param name="IsImmediately">是否立刻收起</param>
    public void CloseFloor(bool IsImmediately=false)
    {            
        if (FloorTween)
        {
            isTweening = true;
            SceneEvents.OnBuildingStartCloseAction();
            FloorTween.CloseBuilding(IsImmediately, () =>
            {
                isTweening = false;
                IsFloorExpand = false;
                SetFloorCollider(false);
                ShowTweenObject(true);
                Debug.Log("Close Building Complete!");
                SceneEvents.OnBuildingCloseCompleteAction();
            });
        }
    }
    /// <summary>
    /// 楼层展开，不相关物体的隐藏和显示
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowTweenObject(bool isShow)
    {
        if (HideObjectOnTweening != null && HideObjectOnTweening.Count != 0)
        {
            HideObjectOnTweening.SetActive(isShow);
        }
    }
    /// <summary>
    /// 设置楼层的Collier
    /// </summary>
    /// <param name="isShowCollider"></param>
    private void SetFloorCollider(bool isShowCollider)
    {       
        //foreach(var floor in ChildNodes)
        //{
        //    BoxCollider collider = floor.GetComponent<BoxCollider>();
        //    if (collider) collider.enabled = isShowCollider;
        //}
        if (BuildingCollider) BuildingCollider.enabled = !isShowCollider;
        if(FloorTween!=null)
        {
            foreach (var floor in FloorTween.FloorList)
            {
                Collider collider = floor.GetComponent<Collider>();
                if (collider) collider.enabled = isShowCollider;
            }
        }   
    }
    /// <summary>
    /// 机房返回大楼
    /// </summary>
    public void BackToBuilding()
    {
        if (currentRoom)
        {
            currentRoom.HideDep();
        }
        RoomFactory.Instance.FocusNode(this);
        currentRoom = null;
    }

    /// <summary>
    /// 载入机房
    /// </summary>
    /// <param name="depNode"></param>
    /// <param name="isFocusRoom"></param>
    /// <param name="onComplete"></param>
    public void LoadRoom(DepNode depNode, bool isFocusRoom = false, Action<FloorController> onComplete = null)
    {
        if (depNode==null)
        {
            Debug.Log("RoomID is null...");
            return;
        }
        //SceneBackButton.Instance.Show(BackToBuilding);
        HideFacotory();
        RoomCreate(depNode, isFocusRoom, onComplete);
    }

    ///// <summary>
    ///// 载入机房
    ///// </summary>
    ///// <param name="depNode"></param>
    ///// <param name="isFocusRoom"></param>
    ///// <param name="onComplete"></param>
    //public IEnumerator LoadRoomStartCoroutine(DepNode depNode, bool isFocusRoom = false, Action<FloorController> onComplete = null)
    //{
    //    if (depNode == null)
    //    {
    //        Debug.Log("RoomID is null...");
    //        yield break;
    //    }
    //    //SceneBackButton.Instance.Show(BackToBuilding);
    //    HideFacotory();
    //    RoomCreate(depNode, isFocusRoom, onComplete);
    //}

    /// <summary>
    /// 隐藏当前建筑
    /// </summary>
    private void HideFacotory()
    {        
        FactoryDepManager depManager = FactoryDepManager.Instance;
        depManager.HideFacotry();
    }

    /// <summary>
    /// 显示当前建筑
    /// </summary>
    public void ShowFactory()
    {
        FactoryDepManager depManager = FactoryDepManager.Instance;
        depManager.ShowFactory();
    }

    /// <summary>
    /// 加载机房
    /// </summary>
    /// <param name="depNode"></param>
    /// <param name="isFocusRoom"></param>
    /// <param name="OnComplete"></param>
    private void RoomCreate(DepNode depNode, bool isFocusRoom, Action<FloorController> OnComplete = null)
    {
        int roomId = depNode.NodeID;
        Log.Info(string.Format("RoomCreate2 DepNode:{0},{1}", depNode, depNode.TopoNode!=null));
        GameObject roomObject = GetRoomObject(roomId);
        FloorController controller = roomObject.GetComponent<FloorController>();
        controller.RecordPosInBuilding(depNode);  //记录在大楼中的位置信息
        controller.SetColliderState(false);
        DisplayFloor(roomObject);//单独展示楼层  
        controller.ShowFloorDev();    
        if (controller.TopoNode == null) controller.TopoNode = depNode.TopoNode;  //设置TopoNode
        currentRoom = controller;        
        if (!isFocusRoom)
        {
            SceneEvents.OnDepNodeChanged(currentRoom);
            if (!LocationManager.Instance.IsFocus)
            {
                //摄像头对焦完成后，开始加载设备
                FocusCamera(roomObject, () =>
                {
                    controller.CreateFloorDev(() =>
                    {
                        if (OnComplete != null) OnComplete(currentRoom);
                    });
                });
            }
            else
            {
                controller.CreateFloorDev(() =>
                {
                    if (OnComplete != null) OnComplete(currentRoom);
                });
            }
        }
        else
        {
            if (OnComplete != null) OnComplete(currentRoom);
        }            
    }

    private void FocusCamera(GameObject room,Action onCameraAlignEnd=null)
    {
        AlignTarget alignTargetTemp = GetFloorTargetInfo(room);
        CameraSceneManager cameraT = CameraSceneManager.Instance;
        cameraT.FocusTargetWithTranslate(alignTargetTemp, AreaSize, onCameraAlignEnd,()=> 
        {
            if (RoomFactory.Instance) RoomFactory.Instance.SetDepFoucusingState(false);
        });
    }

    /// <summary>
    /// 单独展示楼层
    /// </summary>
    /// <param name="roomObject"></param>
    private void DisplayFloor(GameObject roomObject)
    {
        roomObject.transform.parent = FactoryDepManager.Instance.FactoryRoomContainer.transform;
        roomObject.transform.localScale = roomObject.transform.lossyScale;
        //float posY = roomObject.GetSize().y/2;
        Vector3 lastPos = roomObject.transform.position;
        roomObject.transform.position = lastPos;       
    }
    /// <summary>
    /// 获取楼层物体
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    private GameObject GetRoomObject(int roomId)
    {
        return (from floor in ChildNodes where roomId == floor.NodeID select floor.gameObject).FirstOrDefault();
    }

    private void AddBuildingTitle()
    {
        //GameObject obj = new GameObject();
        //obj.transform.parent = transform;
        //obj.transform.localScale = Vector3.one;
        //obj.transform.localEulerAngles = Vector3.zero;
        //float height = gameObject.GetSize().y;
        //obj.transform.localPosition = new Vector3(0,height,0);
        //BuidlingInfoTarget target = obj.AddComponent<BuidlingInfoTarget>();
        //target.InitInfo(NodeName,"200","20","20");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.GetComponent<CharacterController>()!=null)
        {
            if (BuildingTopColliderManage.IsInBuildingRoof) return;//处于楼顶，不算楼内
            DevSubsystemManage fpsManager = DevSubsystemManage.Instance;
            if (fpsManager) fpsManager.SetTriggerBuilding(this, true);
            RoamManage.Instance.EntranceIndoor(true);
            ShowBuildingDev(true);
            IsFPSEnter = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<CharacterController>() != null)
        {
            IsFPSEnter = false;
            DevSubsystemManage fpsManager = DevSubsystemManage.Instance;
            if (fpsManager) fpsManager.SetTriggerBuilding(this, false);
            RoamManage.Instance.EntranceIndoor(false);
            //ShowBuildingDev(false);
            if (fpsManager && !fpsManager.IsFPSInBuilding()) RoamManage.Instance.SetLight(false); //人不在建筑中，才关闭灯光     
        }
    }
   
    #region 摄像头移动模块
    public Vector2 angleFocus = new Vector2(40, 270);
    public float camDistance = 10;
    [HideInInspector]
    public Range angleRange = new Range(0, 90);
    public Range disRange = new Range(2, 30);
    //拖动区域大小
    public Vector2 AreaSize = new Vector2(2, 2);
    /// <summary>
    /// 获取相机聚焦物体的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public AlignTarget GetTargetInfo(GameObject obj)
    {
        AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                               camDistance, angleRange, disRange);
        return alignTargetTemp;
    }
    /// <summary>
    /// 获取楼层对焦信息
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private AlignTarget GetFloorTargetInfo(GameObject room)
    {
        FloorController floor = room.GetComponent<FloorController>();
        if(floor!=null)
        {
            return floor.GetTargetInfo(room);
        }
        else
        {
            AlignTarget alignTargetTemp = new AlignTarget(room.transform, new Vector2(50, 0),
                                          30, angleRange, new Range(2, 40));
            return alignTargetTemp;
        }
    }
    #endregion
}
