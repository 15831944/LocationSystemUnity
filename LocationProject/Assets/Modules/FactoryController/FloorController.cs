using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
using System;
using Mogoson.CameraExtension;

public class FloorController : DepNode
{
    ///// <summary>
    ///// 机房物体
    ///// </summary>
    //[HideInInspector]
    //public GameObject RoomObject;
    private GameObject _roomDevContainer;
    /// <summary>
    /// 机房存放设备处
    /// </summary>
    public GameObject RoomDevContainer
    {
        get
        {
            if (_roomDevContainer == null)
            {
                InitContainer();
            }
            return _roomDevContainer;
        }
    }
    /// <summary>
    /// 静态设备存放处
    /// </summary>
    public GameObject StaticDevContainer;
    /// <summary>
    /// 楼层的父物体（大楼）
    /// </summary>
    private Transform FloorParent;
    /// <summary>
    /// 楼层的位置
    /// </summary>
    private Vector3 FloorPos;
    /// <summary>
    /// 楼层的缩放值
    /// </summary>
    private Vector3 FloorLossyScale;
    /// <summary>
    /// 设备存放处是否初始化
    /// </summary>
    private bool IsDevContainerInit;

    // Use this for initialization
    void Awake()
    {
        depType = DepType.Room;
    }

    protected override void Start()
    {
        CreateFloorCube();
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick = OnClick;
        trigger.onDoubleClick = OnDoubleClick;
    }
    private void OnClick()
    {
        //if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) return;
        //CloseBuilding();
    }
    private void OnDoubleClick()
    {
        if (LocationManager.Instance.IsFocus) return;
        if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI || DevSubsystemManage.IsRoamState) return;
        if (ParentNode is BuildingController)
        {
            BuildingController controller = ParentNode as BuildingController;
            if (!controller.IsFloorExpand || BuildingController.isTweening) return;
            ShowFloor();
        }
    }
    /// <summary>
    /// 双击展示楼层
    /// </summary>
    private void ShowFloor()
    {
        if (RoomFactory.Instance)
            RoomFactory.Instance.FocusNode(this);
    }
    /// <summary>
    /// 单击关闭楼层
    /// </summary>
    private void CloseBuilding()
    {
        if (ParentNode && ParentNode as BuildingController)
        {
            BuildingController controller = ParentNode as BuildingController;
            if (BuildingController.isTweening) return;
            controller.CloseFloor(false);
        }
    }
    /// <summary>
    /// 打开区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void OpenDep(Action onComplete = null, bool isFocusT = true)
    {
        HideFacotry();
        BuildingController parentNode = ParentNode as BuildingController;
        if (parentNode != null) parentNode.LoadRoom(this, false, floor =>
        {
            if (onComplete != null) onComplete();
        });
    }
    /// <summary>
    /// 关闭区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void HideDep(Action onComplete = null)
    {
        IsFocus = false;
        RecoverPosInBuilding();
        HideFloorDev();
        //SetColliderState(true);
    }
    /// <summary>
    /// 聚焦区域
    /// </summary>
    public override void FocusOn(Action onFocusComplete = null)
    {
        IsFocus = true;
        AlignTarget alignTargetTemp = GetTargetInfo(gameObject);
        CameraSceneManager cameraT = CameraSceneManager.Instance;
        cameraT.FocusTargetWithTranslate(alignTargetTemp, AreaSize, onFocusComplete, () =>
         {
             if (RoomFactory.Instance) RoomFactory.Instance.SetDepFoucusingState(false);
         });
    }
    /// <summary>
    /// 取消区域聚焦
    /// </summary>
    /// <param name="onFocusComplete"></param>
    public override void FocusOff(Action onFocusComplete = null)
    {
        IsFocus = false;
        CameraSceneManager.Instance.ReturnToDefaultAlign(onFocusComplete);
    }
    /// <summary>
    /// 隐藏厂区建筑物
    /// </summary>
    private void HideFacotry()
    {
        FactoryDepManager manager = FactoryDepManager.Instance;
        if (manager) manager.HideFacotry();
    }
    /// <summary>
    /// 设置监控区域
    /// </summary>
    /// <param name="oT"></param>
    public override void SetMonitorRangeObject(MonitorRangeObject oT)
    {
        monitorRangeObject = oT;
        InitContainer();
    }
    /// <summary>
    /// 初始化设备存放处
    /// </summary>
    private void InitContainer()
    {
        if (IsDevContainerInit) return;
        IsDevContainerInit = true;
        if (_roomDevContainer == null)
        {
            _roomDevContainer = new GameObject("RoomDevContainer");
            _roomDevContainer.transform.parent = transform;
        }
        if (monitorRangeObject != null)
        {
            _roomDevContainer.transform.localScale = GetContainerScale(transform.lossyScale);
            Vector3 floorSize = monitorRangeObject.gameObject.GetSize();
            _roomDevContainer.transform.position = monitorRangeObject.transform.position + new Vector3(floorSize.x / 2, -floorSize.y / 2, floorSize.z / 2);
            _roomDevContainer.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            _roomDevContainer.transform.localScale = GetContainerScale(transform.lossyScale);
            //_roomDevContainer.transform.localScale = transform.lossyScale;
            _roomDevContainer.transform.eulerAngles = new Vector3(0, 180, 0);

            PhysicalTopology topoNode = TopoNode;
            PhysicalTopology buildingNode = ParentNode.TopoNode;
            if (topoNode == null || buildingNode == null)
            {
                Debug.Log("TopoNode is null...");
                return;
            }
            TransformM tm = buildingNode.Transfrom;
            //Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
            //Log.Info("建筑物的右下角坐标:" + pos2D);
            //Vector3 buildPos = LocationManager.GetRealVector(pos2D);
            //_roomDevContainer.transform.position = buildPos;

            if (topoNode.Transfrom != null)
            {
                Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
                Log.Info("建筑物的右下角坐标:" + pos2D);
                Vector3 buildPos = LocationManager.GetRealVector(pos2D);
                _roomDevContainer.transform.position = buildPos;
            }
            else
            {
                Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), 0, (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
                Vector3 buildPos = LocationManager.GetRealVector(pos2D);
                buildPos.y = transform.position.y - gameObject.GetSize().y / 2;
                _roomDevContainer.transform.position = buildPos;
            }
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
    /// <summary>
    /// 记录楼层在大楼中的位置
    /// </summary>
    public void RecordPosInBuilding(DepNode dep)
    {
        if (FactoryDepManager.currentDep == dep) return;
        FloorParent = transform.parent;
        FloorPos = transform.position;
        //FloorLossyScale = transform.lossyScale;
        FloorLossyScale = transform.localScale;
    }
    /// <summary>
    /// 恢复楼层在大楼中的位置
    /// </summary>
    private void RecoverPosInBuilding()
    {
        transform.parent = FloorParent;
        transform.position = FloorPos;
        transform.localScale = FloorLossyScale;
    }
    /// <summary>
    /// 隐藏楼层设备
    /// </summary>
    public void HideFloorDev()
    {
        if (StaticDevContainer != null) StaticDevContainer.SetActive(false);
        if (_roomDevContainer != null) _roomDevContainer.SetActive(false);
        foreach (var room in ChildNodes)
        {
            RoomController controller = room as RoomController;
            if (controller) controller.HideRoomDev();
        }
    }
    /// <summary>
    /// 显示楼层设备
    /// </summary>
    public void ShowFloorDev()
    {
        if (StaticDevContainer != null) StaticDevContainer.SetActive(true);
        if (_roomDevContainer != null) _roomDevContainer.SetActive(true);
        foreach (var room in ChildNodes)
        {
            RoomController controller = room as RoomController;
            if (controller) controller.ShowRoomDev();
        }
    }
    /// <summary>
    /// 设置楼层的Collider
    /// </summary>
    /// <param name="isShowCollider"></param>
    public void SetColliderState(bool isShowCollider)
    {
        Collider colliderT = transform.GetComponent<Collider>();
        if (colliderT) colliderT.enabled = isShowCollider;
    }
    /// <summary>
    /// 创建楼层设备
    /// </summary>
    public void CreateFloorDev(Action onDevCreateComplete = null)
    {
        SetColliderState(false);
        Debug.Log("创建机房设备...");
        InitFloorDev(onDevCreateComplete);
    }
    /// <summary>
    /// 创建楼层设备
    /// </summary>
    private void InitFloorDev(Action onDevCreateComplete = null)
    {
        if (IsDevCreate)
        {
            if (onDevCreateComplete != null) onDevCreateComplete();
            return;
        }
        IsDevCreate = true;
        InitContainer();
        RoomFactory.Instance.CreateDepDev(this, onDevCreateComplete);
        //CreateRoomDev();
    }
    /// <summary>
    /// 获取设备的区域ID
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetDevNodeID(Vector3 pos)
    {
        foreach (var room in ChildNodes)
        {
            RoomController controller = room as RoomController;
            if (controller && controller.IsInDepField(pos))
            {
                Debug.Log("Dev is in room:" + controller.NodeName);
                return controller.NodeID;
            }
        }
        return this.NodeID;
    }

    public int GetDevBoundId(Vector3 devLocalPos)
    {
        PhysicalTopology[] childAreas = TopoNode.Children;
        if (childAreas == null) return NodeID;
        Vector3 cadPos = LocationManager.UnityToCadPos(devLocalPos, true);
        foreach (var child in childAreas)
        {
            Bound initBound = child.InitBound;
            if (initBound == null || child.Name.Contains("测试范围")) continue;
            bool isXField = cadPos.x > initBound.MinX && cadPos.x < initBound.MaxX ? true : false;
            bool isZField = cadPos.z > initBound.MinY && cadPos.z < initBound.MaxY ? true : false;
            if (isXField && isZField)
            {
                //initBound.Id不是机房对应的ID
                Debug.Log(string.Format("设备所属区域:{0} 区域ID:{1}", child.Id, child.Name));
                if (ChildNodes.Find(i => i.NodeID == child.Id) != null)
                {
                    return child.Id;
                }
                else
                {
                    break;
                }

            }
        }
        return NodeID;
    }

    /// <summary>
    /// 创建楼层底部平面
    /// </summary>
    [ContextMenu("CreateFloorCube")]
    public void CreateFloorCube()
    {
        if (floorCube == null)
        {
            floorCube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            Vector3 sizeT = gameObject.GetSize();
            floorCube.localScale = new Vector3(sizeT.x, 0.01f, sizeT.z);
            floorCube.position = new Vector3(transform.position.x, transform.position.y - sizeT.y / 2 + 0.09f, transform.position.z);//0.09f为高度偏移系数
            floorCube.transform.SetParent(transform);
            floorCube.GetComponent<Renderer>().enabled = false;
            //floorCube.gameObject.layer = LayerMask.NameToLayer("Floor");
            floorCube.name = "FloorCube";
            Collider collider = floorCube.GetComponent<Collider>();
            if (collider) collider.isTrigger = true;
            //Debug.LogFormat("floorCube位置:{0}", floorCube.transform.position);
        }

    }

    void OnMouseEnter()
    {
        if (ParentNode is BuildingController)
        {
            BuildingController controller = ParentNode as BuildingController;
            if (!controller.IsFloorExpand || BuildingController.isTweening) return;
            HighlightOn();
            DepNameUI.Instance.Show(NodeName);
        }
    }
    void OnMouseExit()
    {
        if (ParentNode is BuildingController)
        {
            BuildingController controller = ParentNode as BuildingController;
            if (!controller.IsFloorExpand) return;
            HighLightOff();
            DepNameUI.Instance.Close();
        }
    }
    #region 摄像头移动模块
    public Vector2 angleFocus = new Vector2(50, 0);
    public float camDistance = 30;
    [HideInInspector]
    public Range angleRange = new Range(0, 90);
    public Range disRange = new Range(2, 30);
    //拖动区域大小
    public Vector2 AreaSize = new Vector2(2, 40);
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
    #endregion
}
