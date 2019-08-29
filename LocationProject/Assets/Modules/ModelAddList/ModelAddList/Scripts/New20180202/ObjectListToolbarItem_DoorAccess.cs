using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.M_Plugins.Helpers.Utils;
using RTEditor;

public class ObjectListToolbarItem_DoorAccess : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    /// <summary>
    /// 提示信息
    /// </summary>
    public GameObject tipUI;

    LayerMask mask;

    private GameObject model;

    private string ModelName;

    private string TypeCode;

    private Vector3 mouseScreenPos;

    public static bool IsDragToInitDeviceEnd = true;//是否拖动生成设备是否结束

    private static bool IsCanSetModelPosition = true;//是否可以设置设备的位置

    private bool IsCanInstantiateModel = false; //是否可以生成模型
    /// <summary>
    /// TitleObj名称
    /// </summary>
    private string TitleObjName = "TitleObj";
    private string ModelPosListName = "ModelPos";

    private string errorMsg = "门禁已经存在，请删除原设备后添加！";
    /// <summary>
    /// 环境是否改变（拖门禁时，高亮所有门）
    /// </summary>
    private bool IsEnvironmentChange;
    /// <summary>
    /// 射线碰撞到的门
    /// </summary>
    private DoorAccessItem DoorItem;
    // Use this for initialization
    void Start()
    {
        mask = LayerMask.GetMask("Floor");
    }

    private Transform modelParent;//刚创建模型的父物体
    /// 回调函数：拖动生成
    /// </summary>
    /// <param name="obj"></param>
    private void DragInstantiateFun(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            Debug.LogError("拖动获取不到模型");
            return;
        }

        GameObject g = obj as GameObject;
        ModelIndex.Instance.Add(g, ModelName);//2016_09_13 cww:添加到缓存中
        GameObject o = Instantiate(g);
        model = o;
        model.AddCollider();
        //modelParent = o.transform.parent;
        model.SetActive(false);
        o.layer = LayerMask.NameToLayer("DepDevice");
        GetTitleObj(model);
        ChangeEnvironmentOnBegin();
    }
    /// <summary>
    /// 展示设备模型位置
    /// </summary>
    /// <param name="titleObj"></param>
    private void DisplayModelPos(GameObject titleObj)
    {
        //if (titleObj == null) return;
        //Camera mainCamera = GetMainCamera();
        //if (mainCamera == null) return;
        //GameObject name = UGUIFollowManage.Instance.CreateItem(DevicePosUI.Instance.DevicePosUIPrefab, titleObj, ModelPosListName, null, false, true);
    }
    /// <summary>
    /// 获取主相机
    /// </summary>
    /// <returns></returns>
    private Camera GetMainCamera()
    {
        CameraSceneManager manager = CameraSceneManager.Instance;
        if (manager && manager.MainCamera != null)
        {
            return manager.MainCamera;
        }
        else
        {
            Debug.LogError("CameraSceneManager.MainCamera is null!");
            return null;
        }
    }
    /// <summary>
    /// 获取设备下的TitleObj
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    private GameObject GetTitleObj(GameObject device)
    {
        Transform titleObj = device.transform.Find(TitleObjName);
        if (titleObj == null)
        {
            GameObject titleObjTemp = new GameObject();
            titleObjTemp.transform.parent = device.transform;
            float objHeight = device.transform.gameObject.GetSize().y / 2 + 0.2f;
            titleObjTemp.transform.localPosition = new Vector3(0, objHeight, 0);
            return titleObjTemp;
        }
        else
        {
            return titleObj.gameObject;
        }
    }
    /// <summary>
    /// 开始拖拽模型，改变场景环境
    /// </summary>
    private void ChangeEnvironmentOnBegin()
    {
        if (TypeCodeHelper.IsDoorAccess(ModelName))
        {
            Debug.Log("Add Door Access...");
            DepNode currentDep = FactoryDepManager.currentDep;
            if (currentDep == null) return;
            if (currentDep.Doors != null)
            {
                IsEnvironmentChange = true;
                currentDep.Doors.SetDoorsRenderEnable(true);
                Rigidbody DoorRig = model.AddMissingComponent<Rigidbody>();
                DoorRig.useGravity = false;
                DoorRig.isKinematic = true;
                model.AddMissingComponent<ObjectModel_DoorAccess>();
            }
        }
    }
    /// <summary>
    /// 恢复环境
    /// </summary>
    private void RecoverEnvironment()
    {
        if (!IsEnvironmentChange) return;
        DepNode currentDep = FactoryDepManager.currentDep;
        if (currentDep == null) return;
        if (currentDep.Doors != null)
        {
            IsEnvironmentChange = false;
            currentDep.Doors.SetDoorsRenderEnable(false);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsCanInstantiateModel)
        {
            mouseScreenPos = Input.mousePosition;

            AssetbundleGet.Instance.GetObj(ModelName, AssetbundleGetSuffixalName.prefab, DragInstantiateFun);

            IsCanInstantiateModel = false;

            IsDragToInitDeviceEnd = false;
        }
        Debug.Log("OnBeginDrag!");
    }
    public bool Ishit;
    public void OnDrag(PointerEventData eventData)
    {
        if (model != null && IsCanSetModelPosition)
        {
            if (IsClickUGUIorNGUI.Instance.isOverUI)
            {
                if (model.activeInHierarchy)
                    model.SetActive(false);
            }
            else
            {
                if (!model.activeInHierarchy)
                    model.SetActive(true);
            }
            GiveRayF();
        }
    }
    private void GiveRayF()
    {
        if (model != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Vector3 screenSpace;
            if (Physics.Raycast(ray, out hit, float.MaxValue, mask.value))
            {
                float normalY = hit.point.y + model.transform.gameObject.GetSize().y / 2;
                model.transform.position = new Vector3(hit.point.x, normalY, hit.point.z); ;
            }
            else
            {
                if (model.activeInHierarchy)
                {
                    model.SetActive(false);
                    Debug.Log("Ray can't find floor...");
                }
            }
            if(Physics.Raycast(ray,out hit,float.MaxValue))
            {
                DoorAccessItem item = hit.transform.GetComponent<DoorAccessItem>();
                if (item == null&&hit.transform.childCount>0) item = hit.transform.GetChild(0).GetComponent<DoorAccessItem>();
                if (item)
                {
                    ClearLastDoor();
                    DoorItem = item;
                    ObjectModel_DoorAccess access = model.GetComponent<ObjectModel_DoorAccess>();
                    access.IsAccessInDoor = true;
                    access.DoorItem = item;
                    item.ConstantOn(Color.green);
                }
                else
                {
                    ClearLastDoor();
                }
            }
            else
            {
                ClearLastDoor();
            }


        }
    }
    /// <summary>
    /// 清除门的信息
    /// </summary>
    private void ClearLastDoor()
    {
        if (DoorItem == null) return;
        ObjectModel_DoorAccess access = model.GetComponent<ObjectModel_DoorAccess>();
        access.IsAccessInDoor = false;
        access.DoorItem = null;
        DoorItem.ConstantOff();
        DoorItem = null;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Reset();
        if (TypeCodeHelper.IsDoorAccess(ModelName))
        {
            RecoverEnvironment();
            ObjectModel_DoorAccess access = model.GetComponent<ObjectModel_DoorAccess>();
            if (!access.IsAccessInDoor|| mouseScreenPos == Input.mousePosition || IsClickUGUIorNGUI.Instance.isOverUI)
            {
                DestroyImmediate(model);
                return;
            }
            else
            {
                access.DoorItem.CloseAllDoor();
                List<GameObject> DoorAccess = access.CreateDoorAccess();
                AddSave(DoorAccess,access.DoorItem.DoorID,access.DoorItem.ParentID);
                DestroyImmediate(model);
              //  ShowParkData();
            }
        }
    }
    /// <summary>
    /// Show ParkData
    /// </summary>
    private void ShowParkData()
    {
        DepNode dep;
        if (FactoryDepManager.currentDep is DepController || FactoryDepManager.currentDep is BuildingController)
        {
            dep = FactoryDepManager.Instance;
        }
        else
        {
            dep = FactoryDepManager.currentDep;
        }
        ParkInformationManage.Instance.TitleText.text = dep.NodeName.ToString();
        ParkInformationManage.Instance.RefreshParkInfo(dep.NodeID);
    }
    #region 数据保存部分
    public void AddSave(List<GameObject>DoorAccessList,string doorID,int? areaId)
    {
        List<DevNode> doorList = new List<DevNode>();
        //List<Dev_DoorAccess> doorInfos = new List<Dev_DoorAccess>();
        if(IsDoorAccessAdd(DoorAccessList))
        {
            UGUIMessageBox.Show(errorMsg);
            for(int i=DoorAccessList.Count-1;i>=0;i--)
            {
                DestroyImmediate(DoorAccessList[i]);
            }
            return;
        }
        foreach(var door in DoorAccessList)
        {
            DoorAccessDevController controller;
            Dev_DoorAccess doorInfo;

            GetDoorAccessData(door,doorID,areaId,out controller,out doorInfo);
            doorList.Add(controller);
            //doorInfos.Add(doorInfo);
        }
        ShowEditUI(doorList);
        CommunicationObject service = CommunicationObject.Instance;
        if(service)
        {
            //bool value = service.AddDoorAccess(doorInfos);
            foreach(var dev in doorList)
            {
                DoorAccessDevController doorDev=dev as DoorAccessDevController;
                if(doorDev)
                {
                    doorDev.DoorAccessInfo=service.AddDoorAccess(doorDev.DoorAccessInfo);
                    if(doorDev.DoorAccessInfo!=null)doorDev.Info = doorDev.DoorAccessInfo.DevInfo;
                }
            }                    
        }
    }
    /// <summary>
    /// 门禁是否已经添加
    /// </summary>
    /// <param name="devModel"></param>
    /// <returns></returns>
    private bool IsDoorAccessAdd(List<GameObject> DoorAccessList)
    {
        if(DoorAccessList!=null&&DoorAccessList.Count>0)
        {
            GameObject devModel = DoorAccessList[0];
            ObjectModel_DoorAccess access = devModel.GetComponent<ObjectModel_DoorAccess>();
            if (access == null) return true;
            if (access.DoorItem != null)
            {
                if (access.DoorItem.DoorAccessList != null && access.DoorItem.DoorAccessList.Count > 0) return true;
            }
        }        
        return false;
    }
    /// <summary>
    /// 获取门禁所有信息
    /// </summary>
    private void GetDoorAccessData(GameObject door, string doorID, int? areaId,out DoorAccessDevController controller,out Dev_DoorAccess doorAccessInfo)
    {
        DevInfo devInfo = GetDevInfo(areaId);
        controller = SetDevController(door, devInfo, areaId);
        DevPos pos = GetDevPos(devInfo.DevID,door);
        devInfo.Pos = pos;
        doorAccessInfo = GetDoorAccessInfo(devInfo, doorID, areaId);
        doorAccessInfo.DevInfo = devInfo;
        controller.DoorAccessInfo = doorAccessInfo;
    }
    
    /// <summary>
    /// 显示设备编辑界面
    /// </summary>
    private void ShowEditUI(List<DevNode>devNodes)
    {
        //DevNode devNode = dev.GetComponent<DevNode>();
        DeviceEditUIManager editManager = DeviceEditUIManager.Instacne;
        if (editManager && devNodes.Count!=0)
        {
            editManager.ShowMultiDev(devNodes);
        }
        SetDevSelection(GetDevs(devNodes));
    }
    /// <summary>
    /// 获取设备物体
    /// </summary>
    /// <param name="devs"></param>
    /// <returns></returns>
    private List<GameObject>GetDevs(List<DevNode>devs)
    {
        List<GameObject> objList = new List<GameObject>();
        foreach(var item in devs)
        {
            if(item.gameObject!=null)
            objList.Add(item.gameObject);
        }
        return objList;
    }
    /// <summary>
    /// 选中当前设备
    /// </summary>
    private void SetDevSelection(List<GameObject> devList)
    {
        if (EditorCamera.Instance)
        {
            foreach (var item in devList)
            {
                EditorCamera.Instance.AdjustObjectVisibility(item);
            }
            EditorObjectSelection.Instance.ClearSelection(false);
            EditorObjectSelection.Instance.SetSelectedObjects(devList, false);
        }
    }
    /// <summary>
    /// 获取门禁信息
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="doorID"></param>
    /// <param name="parentID"></param>
    /// <returns></returns>
    private Dev_DoorAccess GetDoorAccessInfo(DevInfo devInfo,string doorID,int? parentID)
    {
        Dev_DoorAccess access = new Dev_DoorAccess()
        {
            DevID = devInfo.DevID,
            DoorId = doorID,
            ParentId = parentID
        };
        return access;
    }
    /// <summary>
    /// 生成设备信息
    /// </summary>
    /// <returns></returns>
    private DevInfo GetDevInfo(int? parentId)
    {
        DevInfo dev = new DevInfo();
        dev.DevID = Guid.NewGuid().ToString();
        dev.IP = "";
        dev.CreateTime = DateTime.Now;
        dev.ModifyTime = DateTime.Now;
        dev.Name = ModelName;
        dev.ModelName = ModelName;
        dev.Status = 0;
        dev.ParentId = parentId;
        try
        {
            dev.TypeCode = int.Parse(TypeCode);
        }
        catch (Exception e)
        {
            dev.TypeCode = 0;
        }
        dev.UserName = "admin";
        return dev;
    }
    /// <summary>
    /// 获取parentID
    /// </summary>
    /// <returns></returns>
    private int GetPID(GameObject dev)
    {
        FactoryDepManager dep = FactoryDepManager.Instance;
        DepNode currentDep = FactoryDepManager.currentDep;
        int devPid;
        if (currentDep == null) return dep.NodeID;
        if (currentDep as FloorController)
        {
            FloorController floor = currentDep as FloorController;
            Transform lastParent = dev.transform.parent;
            dev.transform.parent = floor.RoomDevContainer.transform;
            devPid = floor.GetDevBoundId(dev.transform.localPosition);
            dev.transform.parent = lastParent;
        }
        else if (currentDep as DepController || currentDep as BuildingController)
        {
            //区域和大楼，默认算厂区设备
            return dep.NodeID;
        }
        else
        {
            devPid = currentDep.NodeID;
        }
        return devPid;
    }
    /// <summary>
    /// 获取设备CAD位置信息
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    private DevPos GetDevPos(string devId,GameObject devModel)
    {
        DevNode devNode = devModel.GetComponent<DevNode>();
        if (devNode == null)
        {
            Debug.LogError(String.Format("devNode is null:{0} {1}", ModelName, devId));
            return null;
        }
        else
        {
            DevPos posInfo = new DevPos();
            posInfo.DevID = devId;
            //posInfo.ParentId = devNode.Info.ParentId;
            //Vector3 pos = model.transform.localPosition;
            Vector3 CadPos = UnityPosToCad(devModel.transform, devNode);
            posInfo.PosX = CadPos.x;
            posInfo.PosY = CadPos.y;
            posInfo.PosZ = CadPos.z;
            Vector3 rotation = devModel.transform.eulerAngles;
            posInfo.RotationX = rotation.x;
            posInfo.RotationY = rotation.y;
            posInfo.RotationZ = rotation.z;
            Vector3 scale = devModel.transform.localScale;
            posInfo.ScaleX = scale.x;
            posInfo.ScaleY = scale.y;
            posInfo.ScaleZ = scale.z;
            return posInfo;
        }
    }
    /// <summary>
    /// 获取门禁存放处
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private GameObject GetDevContainer(DepNode dep)
    {
        if(dep as BuildingController)
        {
            BuildingController building = dep as BuildingController;
            GameObject container = building.DevContainer;
            return container;
        }else if(dep as FloorController)
        {
            FloorController floor = dep as FloorController;
            GameObject container = floor.RoomDevContainer;
            return container;
        }
        else
        {
            return FactoryDepManager.Instance.FactoryDevContainer;
        }
    }
    /// <summary>
    /// 设置设备的Controller
    /// </summary>
    /// <param name="devModel"></param>
    /// <param name="devInfo"></param>
    /// <param name="parentID"></param>
    private DoorAccessDevController SetDevController(GameObject devModel,DevInfo devInfo,int? parentID)
    {
        ObjectModel_DoorAccess access = devModel.GetComponent<ObjectModel_DoorAccess>();
        DepNode node = RoomFactory.Instance.GetDepNodeById((int)parentID);
        devModel.transform.parent = GetDevContainer(node).transform;
        DoorAccessDevController controller = devModel.AddMissingComponent<DoorAccessDevController>();
        controller.DevId = devInfo.DevID;
        if(RoomFactory.Instance)
        {
            RoomFactory.Instance.SaveDepDevInfo(node,controller,devInfo);
        }
        if (access)
        {
            controller.DoorItem = access.DoorItem;
            access.DoorItem.AddDoorAccess(controller);
        }
        devModel.RemoveComponent<ObjectModel_DoorAccess>();
        return controller;    
    }
    /// <summary>
    /// unity位置转换cad位置
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="devNode"></param>
    /// <returns></returns>
    public Vector3 UnityPosToCad(Transform dev, DevNode devNode)
    {
        Vector3 pos;
        if (devNode as DepDevController)
        {
            pos = LocationManager.GetCadVector(dev.position);
        }
        else if (devNode as RoomDevController|| devNode as DoorAccessDevController)
        {
            pos = UnityLocalPosToCad(dev.localPosition);
        }
        else
        {
            Debug.Log("Controller not find..");
            pos = Vector3.zero;
        }
        return pos;
    }
    /// <summary>
    /// UnityLocalPos转CADPos
    /// </summary>
    /// <param name="localPos"></param>
    /// <returns></returns>
    private Vector3 UnityLocalPosToCad(Vector3 localPos)
    {
        Vector3 tempPos;
        LocationManager manager = LocationManager.Instance;
        if (manager.LocationOffsetScale.y == 0)
        {
            tempPos = new Vector3(localPos.x * manager.LocationOffsetScale.x, localPos.y * manager.LocationOffsetScale.x, localPos.z * manager.LocationOffsetScale.z);
        }
        else
        {
            tempPos = new Vector3(localPos.x * manager.LocationOffsetScale.x, localPos.y * manager.LocationOffsetScale.y, localPos.z * manager.LocationOffsetScale.z);
        }
        return tempPos;
    }
    //private static void AfterAddRoomDev(GameObject newObj)
    //{
    //    RoomDevController roomDev = ControllerHelper.AddRoomDevController(newObj);
    //    //roomDev.Select(true);
    //    roomDev.NewSelect();
    //    SurroundEditMenu.Instance.AfreshShowMenu();
    //    RoomDevInfoPanel.SetInfo(newObj);
    //    FollowControlManager.Instance.ModifyRoomDevTitle(roomDev.Model);
    //}
    #endregion
    /// <summary>
    /// 重置状态
    /// </summary>
    void Reset()
    {
        //Debug.Log("Reset:"+ModelName);
        IsDragToInitDeviceEnd = true;
        IsCanSetModelPosition = true;
        IsCanInstantiateModel = true;
    }
    /// <summary>
    /// 设置模型名称
    /// </summary>
    /// <param name="name"></param>
    public void SetModelName(string name, string typeCode)
    {
        Reset();
        ModelName = name;
        TypeCode = typeCode;
    }
    /// <summary>
    /// 设置聚焦时提示信息
    /// </summary>
    public void SetFocusTipText(string name)
    {
        FindTipUI();
        Text t = tipUI.GetComponentInChildren<Text>(true);
        t.text = name;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tipUI != null)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) return;
            tipUI.SetActive(true);
            tipUI.transform.SetParent(ObjectListController.Instance.transform);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsCanInstantiateModel = true;
        if (tipUI != null)
        {
            tipUI.transform.SetParent(transform);
            tipUI.SetActive(false);
        }
    }
    /// <summary>
    /// 寻找提示框界面元素
    /// </summary>
    public void FindTipUI()
    {
        if (tipUI == null)
        {
            int childcount = transform.childCount;
            for (int i = 0; i < childcount; i++)
            {
                if (transform.GetChild(i).name == "tip")
                {
                    tipUI = transform.GetChild(i).gameObject;
                }
            }

        }
    }
}
