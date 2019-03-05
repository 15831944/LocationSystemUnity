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
    /// <summary>
    /// 当前所属区域
    /// </summary>
    public static DepNode currentDep;
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
            ChildNodes.Add(dep);
        }
        colliders = GetComponentsInChildren<Collider>().ToList();
    }
    void Start()
    {
        
    }
    private void Update()
    {

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
                if (dep.NodeID != parentId)
                {
                    dep.gameObject.SetActive(false);
                    depHideList.Add(dep);
                }
                else
                {
                    foreach (DepNode building in dep.ChildNodes)
                    {
                        if (building.NodeID != currentBuilding.NodeID)
                        {
                            building.gameObject.SetActive(false);
                            depHideList.Add(building);
                        }
                    }
                }
            }
            if (OtherBuilding != null)
            {
                Debug.Log("OtherBuilding.SetActive false...");
                OtherBuilding.SetActive(false);
            }
            else
            {
                Debug.Log("OtherBuilding is null...");
            }
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
            if (!dep.gameObject.activeInHierarchy)
            {
                dep.gameObject.SetActive(true);
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
        DepNode last = currentDep;
        currentDep = this;
        SceneEvents.OnDepNodeChanged(last, currentDep);
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
        //Debug.LogError("IsFactory dev create:"+IsDevCreate);
        if (IsDevCreate) return;
        IsDevCreate = true;
        if(transform.GetComponent<DoorAccessModelAdd>()==null)
        {
            //初始化门的控制脚本
            DoorAccessModelAdd modelAdd = gameObject.AddComponent<DoorAccessModelAdd>();
            modelAdd.AddDoorAccessManage();
        }
        //RoomFactory.Instance.CreateDepDev(NodeID, FactoryDevContainer, RoomFactory.DevType.DepDev);
        RoomFactory.Instance.CreateDepDev(this);
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
    public void SetAllColliderIgnoreRaycast(bool isIgnore)
    {
        if (colliders == null) return;
        if (isIgnore)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Floor")) continue;
                //collider.gameObject.layer = LayerMask.NameToLayer(Layers.IgnoreRaycast);
                if(!layerDic.ContainsKey(collider.gameObject))
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
}
