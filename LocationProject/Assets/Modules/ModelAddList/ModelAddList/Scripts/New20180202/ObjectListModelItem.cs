using Assets.M_Plugins.Helpers.Utils;
using Location.WCFServiceReferences.LocationServices;
using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectListModelItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        //modelParent = o.transform.parent;
        model.SetActive(false);
        o.layer = LayerMask.NameToLayer("DepDevice");
        GetTitleObj(model);
        //DisplayModelPos(titleObj);
        //o.AddMissingComponent<BoxCollider>();
        //o.AddMissingComponent<RoomDevMouseDrag>();
        //o.AddMissingComponent<RoomDevController>();
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
        if(titleObj==null)
        {
            GameObject titleObjTemp = new GameObject();
            titleObjTemp.transform.parent = device.transform;
            float objHeight = device.transform.gameObject.GetSize().y / 2 + 0.2f;
            titleObjTemp.transform.localPosition = new Vector3(0,objHeight,0);
            return titleObjTemp;
        }
        else
        {
            return titleObj.gameObject;
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
                    model.SetActive(false);
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Reset();
        if (model != null && IsCanSetModelPosition)
        {
            if (mouseScreenPos == Input.mousePosition || IsClickUGUIorNGUI.Instance.isOverUI)
            {
                DestroyImmediate(model);
                return;
            }
            AddSave();
         //   ShowParkData();
        }
    }
    /// <summary>
    /// Show ParkData
    /// </summary>
    private void ShowParkData()
    {
        DepNode dep;
        if(FactoryDepManager.currentDep is DepController||FactoryDepManager.currentDep is BuildingController)
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
    /// <summary>
    /// 保存
    /// </summary>
    public void AddSave()
    {
        //ToDo:设备创建完成后，放置在合适位置
        CommunicationObject service= CommunicationObject.Instance;
        if(service)
        {
            DevInfo devInfo = GetDevInfo();
            DevNode dev = SetDevController(devInfo);
            if (dev == null) return;         
            DevPos pos = GetDevPos(devInfo.DevID);
            devInfo.Pos = pos;
            service.AddDevInfo(ref dev.Info);
            //Debug.LogError("DevID:"+ dev.Info.Id+"  DevName"+dev.gameObject.name);           
            SaveDevSubInfo(dev, service);
            ShowEditUI(model);
        }
        else
        {
            Debug.LogError("CommunicationObject.Instance is null!");
        }
    }
    /// <summary>
    /// 保存其他信息
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="service"></param>
    private void SaveDevSubInfo(DevNode devNode,CommunicationObject service)
    {
        DevInfo devInfo = devNode.Info;
        if (devInfo == null) return;
        if(TypeCodeHelper.IsLocationDev(devInfo.TypeCode.ToString()))
        {
            Archor archor = new Archor();
            archor.Code = "";
            archor.DevInfoId = devInfo.Id;
            archor.ParentId = (int)devInfo.ParentId;
            archor.X = 0;
            archor.Y = 0;
            archor.Z = 0;
            archor.Name = devInfo.Name;
            archor.Type = ArchorTypes.副基站;
            archor.IsAutoIp = true;
            archor.Ip = "";
            archor.ServerIp = "";
            archor.ServerPort = 0;
            archor.Power = 0;
            archor.AliveTime = 0;
            archor.Enable = IsStart.是;
            //archor.DevInfo = devInfo;
            bool value = service.AddArchor(archor);
            Debug.Log("Add archor value:"+value);
        }else if(TypeCodeHelper.IsCamera(devInfo.TypeCode.ToString()))
        {
            Dev_CameraInfo cameraInfo = new Dev_CameraInfo();
            cameraInfo.Ip = "";
            cameraInfo.DevInfoId = devInfo.Id;
            cameraInfo.UserName = "";
            cameraInfo.PassWord = "";
            cameraInfo.Port = 0;
            cameraInfo.CameraIndex = 0;
            cameraInfo.Local_DevID = devInfo.Abutment_DevID;
            cameraInfo.ParentId = devInfo.ParentId;
            CameraDevController camera = devNode as CameraDevController;
            //camera.SetCameraInfo(cameraInfo);            
            Dev_CameraInfo value = service.AddCameraInfo(cameraInfo);
            if(value!=null)
            {
                camera.SetCameraInfo(value);
            }
            Debug.Log("Add cameraInfo value:" + value==null);
        }
    }
    /// <summary>
    /// 显示设备编辑界面
    /// </summary>
    private void ShowEditUI(GameObject dev)
    {
        DevNode devNode = dev.GetComponent<DevNode>();
        DeviceEditUIManager editManager = DeviceEditUIManager.Instacne;
        if (editManager&&devNode)
        {
            editManager.Show(devNode);
        }
        SetDevSelection(dev);
    }
    /// <summary>
    /// 选中当前设备
    /// </summary>
    private void SetDevSelection(GameObject dev)
    {
        if (EditorCamera.Instance)
        {
            EditorCamera.Instance.AdjustObjectVisibility(dev);
            EditorObjectSelection.Instance.ClearSelection(false);
            List<GameObject> objList = new List<GameObject>();
            objList.Add(dev);
            EditorObjectSelection.Instance.SetSelectedObjects(objList, false);
        }
    }
    /// <summary>
    /// 生成设备信息
    /// </summary>
    /// <returns></returns>
    private DevInfo GetDevInfo()
    {
        DevInfo dev = new DevInfo();
        dev.DevID = Guid.NewGuid().ToString();
        dev.IP = "";
        dev.KKSCode = "";
        dev.CreateTime = DateTime.Now;
        dev.ModifyTime = DateTime.Now;
        dev.Name = ModelName;
        dev.ModelName = ModelName;
        dev.Status = 0;
        dev.ParentId = GetPID(model);
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
        }else if (currentDep as DepController || currentDep as BuildingController)
        {
            //区域和大楼，默认算厂区设备
            return dep.NodeID;
        }
        else
        {
            devPid = currentDep.NodeID;
        }
        //if (currentDep != null&& currentDep!=FactoryDepManager.Instance)
        //{
        //    if(currentDep as FloorController)
        //    {
        //        FloorController floorController = currentDep as FloorController;
        //        DevPID = floorController.GetDevNodeID(devPos);
        //    }
        //    else
        //    {
        //        DevPID = currentDep.NodeID;
        //    }           
        //}
        //else
        //{
        //    DevPID = dep.GetDevDepId(devPos);
        //}
        return devPid;
    }
    /// <summary>
    /// 获取设备CAD位置信息
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    private DevPos GetDevPos(string devId)
    {
        DevNode devNode = model.GetComponent<DevNode>();
        if (devNode == null)
        {
            Debug.LogError(String.Format("devNode is null:{0} {1}",ModelName,devId));
            return null;
        }
        else
        {
            DevPos posInfo = new DevPos();
            posInfo.DevID = devId;
            //Vector3 pos = model.transform.localPosition;
            Vector3 CadPos = UnityPosToCad(model.transform, devNode);
            posInfo.PosX = CadPos.x;
            posInfo.PosY = CadPos.y;
            posInfo.PosZ = CadPos.z;
            Vector3 rotation = model.transform.eulerAngles;
            posInfo.RotationX = rotation.x;
            posInfo.RotationY = rotation.y;
            posInfo.RotationZ = rotation.z;
            Vector3 scale = model.transform.localScale;
            posInfo.ScaleX = scale.x;
            posInfo.ScaleY = scale.y;
            posInfo.ScaleZ = scale.z;
            return posInfo;
        }
    }
    /// <summary>
    /// 设置设备的控制脚本
    /// </summary>
    /// <param name="Info"></param>
    private DevNode SetDevController(DevInfo Info)
    {
        FactoryDepManager depManager = FactoryDepManager.Instance;
        if(depManager)
        {
            model.AddCollider();
            if (Info.ParentId==depManager.NodeID)
            {
                model.transform.parent = depManager.FactoryDevContainer.transform;
                return DevControllerAdd(Info,model, depManager);
            }
            else
            {
                FloorController floor = FactoryDepManager.currentDep as FloorController;
                if (floor && Info.ParentId == floor.NodeID)
                {
                    if (floor.RoomDevContainer != null) return InitRoomDevParent(floor, floor.RoomDevContainer, Info);
                }
                else
                {
                    if (floor != null)
                    {
                        DepNode room = floor.ChildNodes.Find(item => item.NodeID == Info.ParentId);
                        if (room != null && room as RoomController)
                        {
                            RoomController controller = room as RoomController;
                            return InitRoomDevParent(controller, controller.RoomDevContainer, Info);
                        }
                        else
                        {
                            if (floor.RoomDevContainer != null) return InitRoomDevParent(floor, floor.RoomDevContainer, Info);
                        }
                    }
                    else
                    {
                        RoomController roomController = FactoryDepManager.currentDep as RoomController;
                        if (roomController) return InitRoomDevParent(roomController, roomController.RoomDevContainer,Info);                       
                    }
                    //Todo:保存到机柜
                    //Debug.Log("Check Dev PID:"+Info.ParentId);
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 添加设备控制脚本
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="modelTemp"></param>
    /// <returns></returns>
    private DevNode DevControllerAdd(DevInfo devInfo,GameObject modelTemp,DepNode parentNode)
    {
        string typeCode = devInfo.TypeCode.ToString();
        if (TypeCodeHelper.IsBorderAlarmDev(typeCode))
        {
            BorderDevController borderDev = modelTemp.AddMissingComponent<BorderDevController>();
            InitDevInfo(borderDev,devInfo, parentNode);
            return borderDev;
        }else if(TypeCodeHelper.IsCamera(typeCode))
        {
            CameraDevController cameraDev = modelTemp.AddMissingComponent<CameraDevController>();
            InitDevInfo(cameraDev, devInfo, parentNode);
            return cameraDev;
        }
        else
        {
            DepDevController controller = modelTemp.AddComponent<DepDevController>();
            InitDevInfo(controller, devInfo, parentNode);
            return controller;
        }
    }
    /// <summary>
    /// 初始化房间内设备
    /// </summary>
    /// <param name="room"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    private DevNode InitRoomDevParent(DepNode dep,GameObject devContainer,DevInfo info)
    {
        model.transform.parent = devContainer.transform;
        if(TypeCodeHelper.IsCamera(info.TypeCode.ToString()))
        {
            CameraDevController roomDev = model.AddMissingComponent<CameraDevController>();
            return InitDevInfo(roomDev, info, dep);
        }
        else
        {
            RoomDevController roomDev = model.AddMissingComponent<RoomDevController>();
            return InitDevInfo(roomDev, info, dep);
        }
    }
    /// <summary>
    /// 初始化设备信息
    /// </summary>
    /// <param name="roomDev"></param>
    /// <param name="info"></param>
    /// <param name="parentNode"></param>
    private DevNode InitDevInfo(DevNode devController, DevInfo info,DepNode parentNode)
    {
        devController.Info = info;
        devController.ParentDepNode = parentNode;
        if (RoomFactory.Instance)
        {
            RoomFactory.Instance.SaveDepDevInfo(parentNode,devController,info);
        }
        return devController;
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
        if (devNode.IsInPark())
        {
            pos = LocationManager.GetCadVector(dev.position);
        }
        else if (devNode!=null)
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
    public void SetModelName(string name,string typeCode)
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
        HideTipUI();
    }
    private void OnDestroy()
    {
        HideTipUI();
    }
    private void OnDisable()
    {
        HideTipUI();
    }
    /// <summary>
    /// 隐藏提示UI
    /// </summary>
    private void HideTipUI()
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
