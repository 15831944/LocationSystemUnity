using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Location.WCFServiceReferences.LocationServices;
using System;
/// <summary>
/// 区域类型
/// </summary>
public enum DepType
{
    /// <summary>
    /// 工厂
    /// </summary>
    Factory,
    /// <summary>
    /// 区域
    /// </summary>
    Dep,
    /// <summary>
    /// 建筑
    /// </summary>
    Building,
    /// <summary>
    /// 楼层
    /// </summary>
    Floor,
    /// <summary>
    /// 机房
    /// </summary>
    Room,
    /// <summary>
    /// 机房
    /// </summary>
    Range
}
public class FactoryDepManager : DepNode {
    public static FactoryDepManager Instance;

    public static DepNode _currentDep;
    /// <summary>
    /// 当前所属区域
    /// </summary>
    public static DepNode currentDep
    {
        get
        {
            if (_currentDep == null)//切换模型时当前建筑模型可能会丢失
            {
                if (RoomFactory.Instance)
                {
                    _currentDep = RoomFactory.Instance.GetDepNodeById(currentNodeId);
                }
            }
            return _currentDep;
        }
        set
        {
            _currentDep = value;
            if (_currentDep == null)
            {
                Debug.LogError("FactoryDepManager _currentDep == null");
            }
            else
            {
                currentNode = value.TopoNode;
                currentNodeId = value.NodeID;
            }
        }
    }

    public static PhysicalTopology currentNode;

    public static int currentNodeId;

    /// <summary>
    /// CAD图纸
    /// </summary>
    public GameObject planeCAD;
    /// <summary>
    /// CAD图纸
    /// </summary>
    public GameObject terrain;
    /// <summary>
    /// 厂区设备存放处
    /// </summary>
    public GameObject FactoryDevContainer;
    /// <summary>
    /// 厂区建筑存放处
    /// </summary>
    public GameObject FactoryBuilidngContainer;
    /// <summary>
    /// 厂区机房存放处
    /// </summary>
    public GameObject FactoryRoomContainer;
    /// <summary>
    /// 工厂整区模型
    /// </summary>
    public GameObject Facotory;
    /// <summary>
    /// 不控制建筑（后续加入拓扑树中）
    /// </summary>
    public GameObject OtherBuilding;
    /// <summary>
    /// 隐藏区域
    /// </summary>
    private List<DepNode> depHideList;
    /// <summary>
    /// 所有建筑的Collider
    /// </summary>
    private List<Collider> colliders;
    /// <summary>
    /// 更改Collider时，提前缓存Collider的layer
    /// </summary>
    private Dictionary<GameObject, int> layerDic=new Dictionary<GameObject, int>();

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        depHideList = new List<DepNode>();
        //Debug.LogError("FactoryDepManager_Awake");
        depType = DepType.Factory;
        DepController[] DepControllers = GameObject.FindObjectsOfType<DepController>();
        foreach (DepController dep in DepControllers)
        {
            if(!ChildNodes.Contains(dep))ChildNodes.Add(dep);
        }
        //colliders = GetComponentsInChildren<Collider>().ToList();
    }

    public void ReplaceNode(DepNode node)
    {
        var oldNode = depHideList.FirstOrDefault(i => i.NodeID == node.NodeID);
        if (depHideList != null)
        {
            depHideList.Remove(oldNode);
            depHideList.Add(node);
        }
        else
        {
            Debug.LogError("FactoryDepManager.ReplaceNode:"+node.NodeName);
        }
    }

    void Start()
    {
        GetColliders();
    }

    /// <summary>
    /// 获取建筑下所有子物体
    /// </summary>
    public void GetColliders()
    {
        colliders = GetComponentsInChildren<Collider>(true).ToList();
    }
    
    /// <summary>
    /// 获取所有建筑
    /// </summary>
    /// <returns></returns>
    public List<BuildingController>GetAllBuildngController()
    {
        if (ChildNodes == null) return null;
        List<BuildingController> buildings = new List<BuildingController>();
        foreach(var dep in ChildNodes)
        {
            if(dep!=null)
            {
                if (dep is BuildingController) buildings.Add(dep as BuildingController);
                else if(dep is DepController)
                {
                    DepController depTemp = dep as DepController;
                    if (depTemp.ChildNodes == null) continue;
                    foreach(var building in dep.ChildNodes)
                    {
                        if (!building.HaveTopNode) continue;
                        if(building is BuildingController) buildings.Add(building as BuildingController);
                    }
                }
            }
        }
        return buildings;
    }

    /// <summary>
    /// 关闭其他建筑
    /// </summary>
    /// <param name="currentBuilding"></param>
    public void HideOtherBuilding(DepNode currentBuilding)
    {
        try
        {
            ShowOtherBuilding();
            depHideList.Clear();
            int parentId;
            if (currentBuilding.ParentNode as DepController)
            {
                parentId = currentBuilding.ParentNode.NodeID;
            }
            else
            {
                parentId = currentBuilding.NodeID;
            }
            foreach (DepNode dep in ChildNodes)
            {
                if (dep == null) continue;
                if (dep.NodeID != parentId)
                {
                    dep.gameObject.SetActive(false);
                    depHideList.Add(dep);
                }
                else
                {
                    foreach (DepNode building in dep.ChildNodes)
                    {
                        if (building == null) continue;
                        if (building.NodeID != currentBuilding.NodeID)
                        {
                            building.gameObject.SetActive(false);
                            depHideList.Add(building);
                        }
                    }
                }
            }
            //if (OtherBuilding != null)
            //{
            //    Debug.Log("OtherBuilding.SetActive false...");
            //    OtherBuilding.SetActive(false);
            //}
            //else
            //{
            //    Debug.Log("OtherBuilding is null...");
            //}
            FactoryDevContainer.SetActive(false);
        }
        catch(Exception e)
        {
            Debug.LogError("FactoryDepManager.HideOtherBuilding Error:"+e.ToString());
        }
        
    }
    /// <summary>
    /// 显示其他隐藏建筑
    /// </summary>
    public void ShowOtherBuilding()
    {
        ShowFactory();
        foreach (DepNode dep in depHideList)
        {
            if (dep == null)
            {
                continue;
            }
            if (dep.gameObject == null)
            {
                Debug.LogError("FactoryDepManager.ShowOtherBuilding:dep.gameObject == null");
                continue;
            }
            if (!dep.gameObject.activeInHierarchy)
            {
                dep.gameObject.SetActive(true);
                dep.gameObject.transform.parent.gameObject.SetActive(true);//2019_05_21_cww:J6J11显示要把父物体也显示出来，其他物体则不受影响
            }
        }
        if (OtherBuilding != null) OtherBuilding.SetActive(true);
        FactoryDevContainer.SetActive(true);
    }
    /// <summary>
    /// 打开区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void OpenDep(Action onComplete=null, bool isFocusT = true)
    {
        ShowFactory();
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
        SceneBackButton.Instance.Hide();
        //ShowLocation();
        SetCurrentDepNode(this);
    }

    public static void SetCurrentDepNode(DepNode node)
    {
        //DepNode last = currentDep;
        //currentDep = node;
        //SceneEvents.OnDepNodeChanged(last, currentDep);

        SceneEvents.OnDepNodeChanged(node);
    }
    /// <summary>
    /// 聚焦整厂
    /// </summary>
    public override void FocusOn(Action onFocusComplete=null)
    {
        IsFocus = true;
        CameraSceneManager.Instance.ReturnToDefaultAlign(onFocusComplete,()=> 
        {
            if (RoomFactory.Instance) RoomFactory.Instance.SetDepFoucusingState(false);
        });
    }
    /// <summary>
    /// 取消聚焦
    /// </summary>
    /// <param name="onComplete"></param>
    public override void FocusOff(Action onComplete=null)
    {
        IsFocus = false;
        CameraSceneManager.Instance.ReturnToDefaultAlign(onComplete);
    }
    /// <summary>
    /// 显示人员定位
    /// </summary>
    private void ShowLocation()
    {
        ActionBarManage menu = ActionBarManage.Instance;
        if (menu && menu.PersonnelToggle.isOn)
        {
            menu.ShowLocation();
        }
    }
    /// <summary>
    /// 显示工厂模型
    /// </summary>
    public void ShowFactory()
    {
        if(!Facotory.activeInHierarchy)
        {
            Facotory.SetActive(true);
            FactoryDevContainer.SetActive(true);
        }      
    }
    [ContextMenu("CloseFloorCollider")]
    public void CloseBoxCollider()
    {
        FloorController[] floors = transform.GetComponentsInChildren<FloorController>();
        Debug.Log(floors.Length);
        foreach(FloorController floor in floors)
        {
            BoxCollider collider = floor.GetComponent<BoxCollider>();
            if (collider) collider.enabled = false;
            else Debug.Log("BoxCollider is null:"+floor.gameObject.name);
        }
    }
    /// <summary>
    /// 隐藏工厂模型
    /// </summary>
    public void HideFacotry()
    {
        if (Facotory.activeInHierarchy)
        {
            Facotory.SetActive(false);
            FactoryDevContainer.SetActive(false);
        }          
    }

    /// <summary>
    /// 创建厂区设备
    /// </summary>
    public void CreateFactoryDev()
    {
        //添加门控制脚本
        InitDoorAccessModelAdd();

        //创建区域设备
        if (SystemSettingHelper.deviceSetting.LoadParkDevWhenEnter)
        {
            CreateParkDevs();
        }
    }

    /// <summary>
    /// 创建厂区设备
    /// </summary>
    [ContextMenu("CreateParkDevs")]
    private void CreateParkDevs()
    {
        //Debug.LogError("IsFactory dev create:"+IsDevCreate);
        if (IsDevCreate) return;
        IsDevCreate = true;
        //RoomFactory.Instance.CreateDepDev(NodeID, FactoryDevContainer, RoomFactory.DevType.DepDev);
        RoomFactory.Instance.CreateDepDev(this);
    }

    private void InitDoorAccessModelAdd()
    {
        if (transform.GetComponent<DoorAccessModelAdd>() == null)
        {
            //初始化门的控制脚本
            DoorAccessModelAdd modelAdd = gameObject.AddComponent<DoorAccessModelAdd>();
            modelAdd.AddDoorAccessManage();
        }
    }

    /// <summary>
    /// 根据设备位置，判断是否在区域内
    /// </summary>
    /// <param name="pos"></param>
    public int GetDevDepId(Vector3 pos)
    {
        foreach(DepNode item in ChildNodes)
        {
            DepController depController = item as DepController;
            if(depController&&depController.IsInDepField(pos))
            {
                return item.NodeID;
            }
        }
        return NodeID;
    }

    /// <summary>
    /// 建筑底下所有楼层地板
    /// </summary>
    [ContextMenu("CreateAllFloorCube")]
    public void CreateAllFloorCube()
    {
        FloorController[] sloorControllers = GetComponentsInChildren<FloorController>();
        foreach (FloorController sloorController in sloorControllers)
        {
            sloorController.CreateFloorCube();
        }
    }

    /// <summary>
    /// 设置所有建筑的Collider是否启用
    /// </summary>
    public void SetAllColliderEnable(bool isEnable)
    {
        if (colliders == null) return;
        foreach (Collider collider in colliders)
        {
            collider.enabled = isEnable;
        }
    }


    /// <summary>
    /// 设置所有建筑的Collider是否启用
    /// </summary>
    public void SetAllColliderIgnoreRaycastOP(bool isIgnore)
    {
        GetColliders();
        SetAllColliderIgnoreRaycast(isIgnore);
    }

    /// <summary>
    /// 设置所有建筑的Collider是否启用
    /// </summary>
    public void SetAllColliderIgnoreRaycast(bool isIgnore)
    {
        if (colliders == null) return;
        if (isIgnore)
        {
            foreach (Collider collider in colliders)
            {
                try
                {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Floor")) continue;
                }
                catch
                {
                    int i = 0;
                }
                if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) continue;
                if (collider.gameObject.layer == LayerMask.NameToLayer("Railing")) continue;
                //collider.gameObject.layer = LayerMask.NameToLayer(Layers.IgnoreRaycast);
                if (!layerDic.ContainsKey(collider.gameObject))
                {
                    layerDic.Add(collider.gameObject,collider.gameObject.layer);
                    collider.gameObject.layer = LayerMask.NameToLayer(Layers.IgnoreRaycast);
                }
            }
        }
        else
        {
            if (layerDic == null) return;
            foreach(KeyValuePair<GameObject,int>value in layerDic)
            {
                if (value.Key == null) continue;
                value.Key.layer = value.Value;
            }
            layerDic.Clear();
            //foreach (Collider collider in colliders)
            //{
            //    if (collider.gameObject.layer == LayerMask.NameToLayer("Floor")) continue;
            //    collider.gameObject.layer = LayerMask.NameToLayer(Layers.Default);
            //}
        }
    }

    /// <summary>
    /// 显示CAD图纸
    /// </summary>
    public void ShowCAD()
    {
        SetCAD_Active(true);
        SetTerrain_Active(false);
    }

    /// <summary>
    /// 隐藏CAD图纸
    /// </summary>
    public void HideCAD()
    {
        SetCAD_Active(false);
        SetTerrain_Active(true);
    }

    /// <summary>
    /// CAD图纸是否启用
    /// </summary>
    public void SetCAD_Active(bool isActive)
    {
        planeCAD.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// CAD图纸是否启用
    /// </summary>
    public void SetTerrain_Active(bool isActive)
    {
        terrain.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 展开楼层(Editor)状态下
    /// </summary>
    [ContextMenu("ExpandInEditor")]

    public void ExpandInEditor()
    {
        Debug.Log("FactoryDepManager.ExpandInEditor");
        var buildings=GameObject.FindObjectsOfType<BuildingController>();
        foreach (BuildingController building in buildings)
        {
            building.ExpandInEditor();
        }
    }

    [ContextMenu("CollapseInEditor")]
    public void CollapseInEditor()
    {
        Debug.Log("FactoryDepManager.CollapseInEditor");
        var buildings = GameObject.FindObjectsOfType<BuildingController>();
        foreach (BuildingController building in buildings)
        {
            building.CollapseInEditor();
        }
    }
}
