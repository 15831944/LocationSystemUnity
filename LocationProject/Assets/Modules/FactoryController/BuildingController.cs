using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using HighlightingSystem;
using UnityEngine.SceneManagement;
using Jacovone.AssetBundleMagic;

public class BuildingController : DepNode {

    public bool IsSimple = false;

    public override void RefreshChildrenNodes()
    {
        ChildNodes = new List<DepNode>();
        var floors = transform.FindComponentsInChildren<FloorController>();
        foreach (var item in floors)
        {
            ChildNodes.Add(item);
            item.ParentNode = this;
        }
        foreach (var item in ChildNodes)
        {
            item.RefreshChildrenNodes();
        }
    }

    public int VertexCount = 0;

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

    /// <summary>
    /// 简化模型所在的脚本
    /// </summary>
    public BuildingBox buildingBox;

    /// <summary>
    /// 是否展开楼层
    /// </summary>
    public bool isExpandFloor = true;

    /// <summary>
    /// 是否隐藏墙壁
    /// </summary>
    public bool isHideWall = false;

    /// <summary>
    /// 顶部楼层
    /// </summary>
    public List<GameObject> TopFloor;


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
        InitBoxCollider();
        BuildingCollider = transform.GetComponent<BoxCollider>();
        if (BuildingCollider)
        {
            BuildingCollider.isTrigger = true;//为了漫游能够进去
        }
        alignTarget = new AlignTarget(transform, new Vector2(30, 0), 1, new Range(10, 90), new Range(0.5f, 1.5f));
    }

    [ContextMenu("CreateBox")]
    public void CreateBox()
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider)
        {
            GameObject.DestroyImmediate(collider);
        }
        BoxCollider boxCollider=gameObject.AddCollider(false);
        boxCollider.isTrigger = true;
    }


    void Start()
    {
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick += OnClick;
        trigger.onDoubleClick += OnDoubleClick;
        AddBuildingTitle();
        //gameObject.AddComponent<HighlighterOccluder>();

        //InitBoxCollider();
        InitParentNode();

        if (FloorTween == null)
        {
            FloorTween = gameObject.GetComponent<BuildingFloorTweening>();
        }

        if (NodeName.Contains("自然通风冷却塔"))
        {
            Transform da1 = transform.Find("冷却塔-1");
            Transform da2 = transform.Find("冷却塔-2");

            Transform tempTran = null;
            if (da1 != null)
            {
                for (int i = 0; i < da1.childCount; i++)
                {
                    tempTran = da1.GetChild(i);
                    if (tempTran.name.Contains("WateSplashes1"))
                    {
                        tempTran.gameObject.SetActive(false);
                    }
                }
            }

            if (da2 != null)
            {
                for (int i = 0; i < da2.childCount; i++)
                {
                    tempTran = da2.GetChild(i);
                    if (tempTran.name.Contains("WateSplashes1"))
                    {
                        tempTran.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void InitParentNode()
    {
        if (ParentNode == null)
        {
            if (transform.parent == null) return;//单独场景测试，没有父节点
            ParentNode = this.transform.parent.GetComponent<DepNode>();
            if (ParentNode != null)
            {
                ParentNode.ChildNodes.Add(this);
            }
        }
    }

    private void InitBoxCollider()
    {
        Collider collider = gameObject.GetComponent<Collider>();
        if (collider == null)//没有设置Collider
        {

            if (ChildNodes == null)
            {
                RefreshChildrenNodes();
            }
            if (ChildNodes.Count == 0)
            {
                CanBeEnter = false;//没有子节点,不能进入建筑内部
            }

            if (CanBeEnter == false)
            {
                AddAllMeshCollider(transform);
            }
            else
            {
                BoxCollider boxCollider = gameObject.AddCollider(false);
                boxCollider.isTrigger = true;
            }
        }
    }

    public static void AddAllMeshCollider(Transform parent)
    {
        List<MeshFilter> meshFilters = parent.gameObject.FindComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            Collider collider = meshFilter.gameObject.GetComponent<Collider>();
            if (collider == null)
            {
                MeshCollider meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        //ColliderHelper.CreateBoxCollider(parent);
    }

    /// <summary>
    /// 能够被进入
    /// </summary>
    public bool CanBeEnter = true;

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
    public void OnDoubleClick()
    {
        Debug.Log("BuildingController.OnDoubleClick: "+this);
        if (IsHoverUI() || DevSubsystemManage.IsRoamState)
        {
            Debug.Log("IsHoverUI() || DevSubsystemManage.IsRoamState");
            return;
        }

        //BuildingBox box = gameObject.GetComponent<BuildingBox>();
        //if (box)
        //{
        //    box.LoadBuilding();
        //    return;
        //}

        if (LocationManager.Instance.IsFocus) {
            Debug.Log("BuildingController.OnDoubleClick: LocationManager.Instance.IsFocus");
            return;
        }

        if (!IsFocus)
        {
            //FocusOn();
            RoomFactory.Instance.FocusNode(this);
        }
        else
        {
            if (!isTweening)
            {
                OpenFloor();
            }
            else
            {
                Debug.LogWarning("isTweening");
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
            //DepNode lastDep = FactoryDepManager.currentDep;
            //lastDep.IsFocus = false;
            //FactoryDepManager.currentDep = this;
            //SceneEvents.OnDepNodeChanged(lastDep, FactoryDepManager.currentDep);

            FactoryDepManager.currentDep.IsFocus = false;
            SceneEvents.OnDepNodeChanged(this);
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

    public override void Unload()
    {
        //UnloadDevices();//先卸载设备再卸载建筑，因为建筑卸载后设备也会被卸载
        if (buildingBox)
        {
            buildingBox.SetUnload(false);
        }
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
    /// 展开楼层(Editor)状态下
    /// </summary>
    [ContextMenu("ExpandInEditor")]

    public void ExpandInEditor()
    {
        Debug.Log("ExpandInEditor");
        if (FloorTween)
        {
            if (FloorTween.IsAllNull())
            {
                FloorTween.FloorList.Clear();
                foreach (DepNode node in ChildNodes)
                {
                    FloorTween.FloorList.Add(node.gameObject);
                }
            }
            FloorTween.ExpandInEditor();
        }

        var loader = NavMeshSceneLoader.Instance;
        if (loader)
        {
            loader.LoadSceneWhenExpand();
        }
    }

    [ContextMenu("CollapseInEditor")]
    public void CollapseInEditor()
    {
        Debug.Log("CollapseInEditor");
        if (FloorTween)
        {
            FloorTween.CollapseInEditor();
        }
        var loader = NavMeshSceneLoader.Instance;
        if (loader)
        {
            loader.LoadSceneWhenCollapse();
        }
    }

    /// <summary>
    /// 展开楼层
    /// </summary>
    [ContextMenu("ExpandFloors")]

    public void ExpandFloors()
    {
        OpenFloor(true);

        var loader = NavMeshSceneLoader.Instance;
        if (loader)
        {
            loader.LoadSceneWhenExpand();
        }
    }

    [ContextMenu("CollapseFloors")]
    public void CollapseFloors()
    {
        CloseFloor(true);

        var loader = NavMeshSceneLoader.Instance;
        if (loader)
        {
            loader.LoadSceneWhenCollapse();
        }
    }


    /// <summary>
    /// 展开楼层
    /// </summary>
    /// <param name="IsImmediately">是否立即展开</param>
    public void OpenFloor(bool IsImmediately = false)
    {
        IsFloorExpand = true;//要放到外面
        if (isExpandFloor)
        {
            if (FloorTween)
            {
                ShowTweenObject(false);
                isTweening = true;
                SceneEvents.OnBuildingOpenStartAction();
                FloorTween.OpenBuilding(IsImmediately, () =>
                {
                    if (isHideWall)
                    {
                        HideWalls();
                    }
                    isTweening = false;
                    SetFloorCollider(true);
                    Debug.Log("Open Building Complete!");
                    SceneEvents.OnBuildingOpenCompleteAction(this);
                    OnAfterOpenFloor();
                    //HideBuildTypePerson();

                    var loader = NavMeshSceneLoader.Instance;
                    if (loader)
                    {
                        loader.LoadSceneWhenExpand();
                    }
                });
            }
        }
        else
        {
            if (isHideWall)
            {
                HideWalls();
            }
            SetBuildingCollider(false);
            SceneEvents.OnBuildingOpenCompleteAction(this);
            OnAfterOpenFloor();
        }
        //LoadDevices();
    }

    public event Action<BuildingController> AfterOpenFloor;

    protected void OnAfterOpenFloor()
    {
        Debug.Log("OnAfterOpenFloor");
        if (AfterOpenFloor != null)
        {
            AfterOpenFloor(this);
        }
    }
    private int deviceAssetStartIndex = 0;//静态设备初始加载下标
    List<DeviceAssetInfo> deviceAssets = new List<DeviceAssetInfo>();
    /// <summary>
    /// 加载建筑内设备
    /// </summary>
    /// <param name="action"></param>
    public void LoadDevices(Action action = null)
    {
        GameObject buildingObject = gameObject;
        deviceAssets.Clear();
        DeviceAssetInfo buildingDevices = buildingObject.GetComponent<DeviceAssetInfo>();
        if (buildingDevices)
        {
            deviceAssets.Add(buildingDevices);
        }
        var floorDevices = buildingObject.FindComponentsInChildren<DeviceAssetInfo>();
        deviceAssets.AddRange(floorDevices);

        if(deviceAssets==null||deviceAssets.Count==0)
        {
            if (action != null) action();
            return;
        }
        for (int i = deviceAssets.Count - 1; i >= 0; i--)//先加载一楼
        {
            int currentIndex = i;
            deviceAssets[i].LoadAsset(depNode =>
            {
                if (currentIndex == 0)
                {
                    if (action != null)
                    {
                        Debug.LogError("LoadComplete:" + currentIndex);
                        action();
                    }
                }
            });
        }
    }
    ///// <summary>
    ///// 当前区域是大楼区域，展开大楼后，关闭在大楼区域的人员，不关闭楼层，房间等子区内的人员
    ///// </summary>
    //public void HideBuildTypePerson()
    //{
    //    if (FactoryDepManager.currentDep==this)
    //    {
    //        List<LocationObject> objlist = LocationManager.Instance.code_character.Values.ToList();
    //        foreach (LocationObject o in objlist)
    //        {
    //            if (o.currentDepNode == this)
    //            {
    //                o.SetRendererEnable(false);
    //            }
    //        }
    //    }
    //}

    //[ContextMenu("LoadDevices")]
    //private void LoadDevices()
    //{
    //    deviceAssets.Clear();
    //    DeviceAssetInfo buildingDevices = gameObject.GetComponent<DeviceAssetInfo>();
    //    if (buildingDevices)
    //    {
    //        deviceAssets.Add(buildingDevices);
    //    }
    //    var floorDevices = gameObject.FindComponentsInChildren<DeviceAssetInfo>();
    //    deviceAssets.AddRange(floorDevices);

    //    Debug.Log("deviceAssets:" + deviceAssets);

    //    for (int i = deviceAssets.Count-1; i >=0; i--)//先加载一楼
    //    {
    //        deviceAssets[i].LoadAsset(null);
    //    }
    //}

    //[ContextMenu("UnloadDevices")]
    //private void UnloadDevices()
    //{
    //    if(deviceAssets!=null)
    //        foreach (var deviceAsset in deviceAssets)
    //        {
    //            deviceAsset.SetUnload(true);
    //        }
    //}

    List<WallController> walls;
    [ContextMenu("HideWalls")]
    public void HideWalls()
    {
        Debug.Log("HideWalls:"+this);
        walls = gameObject.FindComponentsInChildren<WallController>();
        foreach (var wall in walls)
        {
            wall.StartHide();
        }

        HideTop();
    }

    public void HideTop()
    {
        if (TopFloor!=null)
        {
            TopFloor.SetActive(false);
            Debug.Log("TopFloor.SetActive(false)");
        }
        else
        {
            Debug.Log("TopFloor == null ");
        }
    }

    [ContextMenu("ShowWalls")]
    public void ShowWalls()
    {
        if (walls != null)
            foreach (var wall in walls)
            {
                wall.StopHide();
            }

        ShowTop();
    }

    public void ShowTop()
    {
        if (TopFloor!=null)
        {
            TopFloor.SetActive(true);
        }
    }
    
    /// <summary>
    /// 楼层收起
    /// </summary>
    /// <param name="IsImmediately">是否立刻收起</param>
    public void CloseFloor(bool IsImmediately=false)
    {
        if (isExpandFloor)
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
                    if (isHideWall)
                    {
                        ShowWalls();
                    }

                    var loader = NavMeshSceneLoader.Instance;
                    if (loader)
                    {
                        loader.LoadSceneWhenCollapse();
                    }
                });
            }
        }
        else
        {
            SetBuildingCollider(true);
            if (isHideWall)
            {
                ShowWalls();
            }
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
        SetBuildingCollider(!isShowCollider);
        if (FloorTween!=null)
        {
            foreach (var floor in FloorTween.FloorList)
            {
                Collider collider = floor.GetComponent<Collider>();
                if (collider) collider.enabled = isShowCollider;
            }
        }   
    }

    private void SetBuildingCollider(bool isShowCollider)
    {
        if (BuildingCollider == null) BuildingCollider = transform.GetComponent<BoxCollider>();
        if (BuildingCollider) BuildingCollider.enabled = isShowCollider;
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

        DisplayFloor(controller);//单独展示楼层  

        controller.ShowFloorDev();    
        if (controller.TopoNode == null) controller.SetTopoNode(depNode.TopoNode);  //设置TopoNode
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
        SetFloorTransform(roomObject);
        //if (ShowUnderFloors)
        //{
        //    foreach (var item in ChildNodes)
        //    {
        //        if (item.gameObject == roomObject)//展示到该楼层为止
        //        {
        //            break;
        //        }
        //        SetFloorTransform(item.gameObject);//该楼层以及下方楼层都显示，主要用于贯通性的建筑
        //    }
        //}
    }


    /// <summary>
    /// 单独展示楼层
    /// </summary>
    /// <param name="roomObject"></param>
    private void SetFloorTransform(GameObject roomObject)
    {
        roomObject.transform.parent = FactoryDepManager.Instance.FactoryRoomContainer.transform;
        roomObject.transform.localScale = roomObject.transform.lossyScale;
        //float posY = roomObject.GetSize().y/2;
        Vector3 lastPos = roomObject.transform.position;
        roomObject.transform.position = lastPos;
    }

    /// <summary>
    /// 是否显示当前对焦楼层下面的楼层，false的情况和原来一样的。
    /// </summary>
    public bool ShowUnderFloors = false;

    public void RecoverFloorsTransform()
    {
        if (ShowUnderFloors)
        {
            foreach (var item in ChildNodes)
            {
                FloorController floor = item as FloorController;
                if (floor)
                {
                    floor.RecoverPosInBuilding();
                }
            }
        }
    }

    private void DisplayFloor(FloorController controller)
    {
        controller.SetTransform(FactoryDepManager.Instance.FactoryRoomContainer.transform);
        if (ShowUnderFloors)//显示当前楼层下面的楼层 贯通结构的情况下
        {
            foreach (var item in ChildNodes)
            {
                FloorController floor = item.gameObject.GetComponent<FloorController>();
                floor.RecordPosInBuilding(null);
            }

            foreach (var item in ChildNodes)
            {
                if (item.gameObject == controller.gameObject)//展示到该楼层为止
                {
                    break;
                }
                FloorController floor = item.gameObject.GetComponent<FloorController>();
                floor.SetTransform(FactoryDepManager.Instance.FactoryRoomContainer.transform);//该楼层以及下方楼层都显示，主要用于贯通性的建筑
            }
        }
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
        //定位人员的Layer是Person，漫游人员没有设置是Default
        if(other.transform.GetComponent<CharacterController>()!=null && other.gameObject.layer != LayerMask.NameToLayer("Person"))
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
        if (other.transform.GetComponent<CharacterController>() != null && other.gameObject.layer != LayerMask.NameToLayer("Person"))
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
        Transform center = obj.transform;
        if (NodeName=="主厂房")
        {
            Transform t = transform.Find("主厂房中心");//这个中心是为了调整摄像头对焦，以至于可以看到一层
            if (t == null)
            {
                t = new GameObject("主厂房中心").transform;
                t.transform.position = obj.transform.position;
                //Vector3 sizeT = obj.GetSize();
                t.transform.position = new Vector3(t.transform.position.x, t.transform.position.y-7, t.transform.position.z);
                t.SetParent(obj.transform);
            }
            center = t;
        }

        if (center == null)
        {
            center = obj.transform;
        }
        AlignTarget alignTargetTemp = new AlignTarget(center, angleFocus,
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

    public void OnDestroy()
    {
        //Debug.Log("BuildingController.OnDestroy:"+this.name+ "|buildingBox:"+(buildingBox!=null));
        if (buildingBox)
        {
            //buildingBox.UnloadAsset();//以防万一在未经过正式路径卸载了模型，没有把原来的模型替换掉
            //AssetBundleMagic.UnloadBundleEx(buildingBox.AssetName);//UnloadBundle要放到这里，出现了GameObject还在，但是模型材质都没有了的情况
            Debug.Log("BuildingController.OnDestroy AssetBundleMagic.DoUnload");
            AssetBundleMagic.DoUnload();
        }

        IsDestroy = true;
    }

    public bool IsDestroy = false;
}
