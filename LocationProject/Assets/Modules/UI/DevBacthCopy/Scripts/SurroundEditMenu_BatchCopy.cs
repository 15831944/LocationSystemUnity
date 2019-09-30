using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Location.WCFServiceReferences.LocationServices;
using Assets.M_Plugins.Helpers.Utils;
using RTEditor;

public class SurroundEditMenu_BatchCopy : MonoBehaviour
{
    public static SurroundEditMenu_BatchCopy Instacne;

    /// <summary>
    /// 要批量复制的设备
    /// </summary>
    private GameObject CopyObj;
    /// <summary>
    /// 要批量复制的设备的原始旋转角度
    /// </summary>
    public Vector3 objOriginEulerAngles;
    /// <summary>
    /// 是否可以批量复制
    /// </summary>
    public static bool IsCanCopy = false;
    /// <summary>
    /// 上一次批量复制是否已结束，或是否还未进行批量复制
    /// </summary>
    private bool IsEndCanCopy = true;
    /// <summary>
    /// 是否是点下OnPress
    /// </summary>
    private bool IsOnPressed = false;
    /// <summary>
    /// floor层的值
    /// </summary>
    private int maskValue;
    /// <summary>
    /// 复制方向
    /// </summary>
    public Vector3 CopyDirection = Vector3.right;//TODO:要设置为可调
    /// <summary>
    /// 复制间距
    /// </summary>
    private float spaceDistance = 0.01f;
    /// <summary>
    /// 设备的宽度
    /// </summary>
    private float DeviceWidth;
    /// <summary>
    /// 要复制的模型名称
    /// </summary>
    public string modelName;
    /// <summary>
    /// 模型
    /// </summary>
    private GameObject model;
    /// <summary>
    /// 所有复制出的机柜
    /// </summary>
    public List<GameObject> list;
    /// <summary>
    /// 线的预设
    /// </summary>
    public GameObject LinePrefab;//线（LineRender）
    /// <summary>
    /// 线的预设
    /// </summary>
    private GameObject LineAxis;//线（LineRender）
    /// <summary>
    /// 线的预设
    /// </summary>
    private GameObject LineMove;//线（LineRender）
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    /// <summary>
    /// 当前选中的设备（设备+复制设备）
    /// </summary>
    private List<GameObject> SelectObjs=new List<GameObject>();
    // Use this for initialization
    void Start()
    {
        Instacne = this;
        maskValue = LayerMask.GetMask(Layers.Floor);
        //UIEventListener.Get(CopyButton).onPress = OnPress_Copy;
        //CopyButton.onClick.AddListener(OnPress_Copy);
        InitAngleLine();
    }

    //private void OnEnable()
    //{
    //    //Init();
    //}

    /// <summary>
    /// 开始批量复制
    /// </summary>
    /// <param name="dev"></param>
    public void Open(DevNode dev)
    {
        CopyObj = dev.gameObject;
        //CopyButton.gameObject.SetActive(true);
        OnPress_Copy();
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void CloseEdit()
    {
        SetCopyCubeState(false);
        CopyObj.transform.eulerAngles = objOriginEulerAngles;
        IsEndCanCopy = true;
        Invoke("SetIsCanCopyFalse", 0.1f);
        for (int i = 0; i < list.Count; i++)
        {
            DestroyImmediate(list[i]);
        }
        CloseUI();
        SetDevEditWindow(true);
        list.Clear();
    }
    /// <summary>
    /// 设置编辑时，部分UI的状态
    /// </summary>
    /// <param name="isShow"></param>
    private void SetDevEditWindow(bool isShow)
    {
        if(isShow)
        {
            if (ObjectAddListManage.IsEditMode)
            {
                ObjectAddListManage.Instance.SetWindowState(true);
            }
            StartOutManage.Instance.Show();
        }
        else
        {
            ObjectAddListManage.Instance.SetWindowState(false);
            StartOutManage.Instance.Hide();
            DeviceEditUIManager.Instacne.Close();
            CloseUI();
            //DeviceEditUIManager.Instacne.Show(); 复制完设备，自己会显示
        }    
    }
    /// <summary>
    /// 初始化角度相关信息
    /// </summary>
    private void InitAngleLine()
    {
        LineAxis = Instantiate(LinePrefab) as GameObject;
        LineAxis.name = "LineAxis";
        LineAxis.SetActive(false);
        LineMove = Instantiate(LinePrefab) as GameObject;
        LineMove.name = "LineMove";
        LineMove.SetActive(false);
        window.gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化信息
    /// </summary>
    public void Init()
    {
        //CopyObj =...;
        if(CopyObj == null)
        {
            Debug.LogError("CopyObj is null...");
            return;
        }
        //CopyObj = dev;
        list = new List<GameObject>();
        DeviceWidth = CopyObj.GetSize().x;//设备的宽度
        modelName = CopyObj.GetComponent<DevNode>().Info.ModelName;
        model = ModelIndex.Instance.Get(modelName);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCanCopy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsOnPressed) return;

                IsEndCanCopy = true;
                //IsCanCopy = false;
                Invoke("SetIsCanCopyFalse", 0.1f);

                SaveData();//保存

                SetSelectObjInfo();

                //Vector3 center = RoomObject.Current.CalculateSelectObjsCenter(RoomObject.Current.SelectObjs);
                //GameObject centerObj = RoomObject.Current.GetSelectObjsCenterObj(center);
                //RoomObject.Current.SaveParent(RoomObject.Current.SelectObjs, centerObj);
                SetCopyCubeState(false);
                SelectObjsHighlighter();
                CloseUI();
                SetDevEditWindow(true);
                list.Clear();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CloseEdit();
            }
            else
            {
                return;
            }
        }
    }

    private void SetIsCanCopyFalse()
    {
        IsCanCopy = false;
    }

    /// <summary>
    /// 关闭一些UI
    /// </summary>
    public void CloseUI()
    {
        //CopyButton.gameObject.SetActive(false);
        //CommonUIHelperScene.Instance.SetEditToolBarRight(false);
        //if (RoomDevInfoPanel.Instance) RoomDevInfoPanel.Instance.Hide();
    }

    /// <summary>
    /// 给复制的设备，添加脚本并初始化
    /// </summary>
    private void SetSelectObjInfo()
    {
        //Todo:加DevNode 放置DevContainer...
        SelectObjs.Clear();
        SelectObjs.Add(CopyObj);
        SelectObjs.AddRange(list);
    }


    /// <summary>
    /// 所有选中的设备高亮
    /// </summary>
    private void SelectObjsHighlighter()
    {
        foreach (GameObject g in SelectObjs)
        {
            if (g == null) continue;
            DevNode roomDev = g.GetComponent<DevNode>();
            if (roomDev)
                roomDev.HighlightOn();
            if(g.GetComponent<BoxCollider>()!=null)g.GetComponent<BoxCollider>().enabled = true;
        }
    }

    /// <summary>
    /// 保存数据信息
    /// </summary>
    private void SaveData()
    {
        CommunicationObject service = CommunicationObject.Instance;
        if (service == null) return;
        DevNode modelTemp = CopyObj.GetComponent<DevNode>();
        if (modelTemp == null) return;
        modelTemp.Info.Pos.RotationY = CopyObj.transform.eulerAngles.y;
        service.ModifyDevPos(modelTemp.Info.Pos);
        if (list != null && list.Count != 0)
        {
            //DataAccessController.Instance.ModifyDevInRoomEx(model);     
            List<DevInfo> devinfoList = new List<DevInfo>();
            for (int i = 0; i < list.Count; i++)
            {
                GameObject devModel = list[i];
                DevInfo devInfo = GetDevInfo(modelTemp.Info, i);
                devModel.transform.parent = CopyObj.transform.parent;
                devModel.layer = CopyObj.layer;
                DevNode dev = SetDevController(devInfo, devModel);
                if (dev == null) continue;
                DevPos pos = GetDevPos(dev, devInfo.DevID);
                devInfo.Pos = pos;
                devinfoList.Add(devInfo);
            }
            List<DevInfo> infoList = service.AddDevInfo(devinfoList);
            SetDevInfoId(devinfoList, infoList);
            SaveDevSubInfo(list, service);
        }       
        AddSelectionDevs(list);
    }
    /// <summary>
    /// 设置设备Id(int->数据库自动生成)
    /// </summary>
    /// <param name="old"></param>
    /// <param name="newDevInfoes"></param>
    private void SetDevInfoId(List<DevInfo>old,List<DevInfo>newDevInfoes)
    {
        foreach(var dev in old)
        {
            DevInfo info = newDevInfoes.Find(i=>i.DevID==dev.DevID);
            if (info != null)
            {
                dev.Id = info.Id;
            }
        }
    }
    /// <summary>
    /// 清除选中设备
    /// </summary>
    private void ClearSelection()
    {
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection)
        {
            selection.ClearSelection(false);
        }
    }
    /// <summary>
    /// 选中创建的设备
    /// </summary>
    /// <param name="dev"></param>
    public void AddSelectionDevs(List<GameObject> devList)
    {
        List<GameObject> devIncludeModel = new List<GameObject>();
        if(devList!=null&&devList.Count!=0) devIncludeModel.AddRange(devList);
        devIncludeModel.Add(CopyObj);
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection)
        {
            selection.SetSelectedObjects(devIncludeModel, false);
            RefleshGizmoPosition();
        }
    }
    /// <summary>
    /// 刷新设备编辑Gizmo位置
    /// </summary>
    public void RefleshGizmoPosition()
    {
        Gizmo activeGizmo = EditorGizmoSystem.Instance.ActiveGizmo;
        if (activeGizmo == null)
        {
            Debug.Log("Active Gizmo is null...");
            return;
        }
        activeGizmo.transform.position = EditorObjectSelection.Instance.GetSelectionWorldCenter();
    }

    void OnPress_Copy()
    {
        IsOnPressed = true;
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) return;
        ClearSelection();
        SetDevEditWindow(false);
        Init();
        if (IsCanCopy == false)
        {
            print("DeviceBatchCopy>>>>>>>>>>>>>>OnPress");
            IsCanCopy = true;
            IsEndCanCopy = false;
            objOriginEulerAngles = CopyObj.transform.eulerAngles;
            StartCoroutine(RealtimeCopyDevice());
        }
        //0.1秒后将IsOnPressed设置为false，因为OnPress万后会立马进入Update()中的Input.GetMouseButtonDown(0)，所以使用了IsOnPressed标志
        Invoke("SetIsOnPressed", 0.1f);
    }


    private void SetIsOnPressed()
    {
        IsOnPressed = false;
        print("OnPress_Copy>>>>>>>>>>IsOnPressed:" + IsOnPressed);
    }

    void OnClick()
    {
        //print("DeviceBatchCopy>>>>>>>>>>>>>>OnClick");
        //if(Input.getmous)
    }

    /// <summary>
    /// 获取鼠标碰撞到地面的点
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFloorColliderPoint()
    {
        Camera cam = CameraSceneManager.Instance.MainCamera;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, maskValue))
        {
            return hit.point;
        }
        return CopyObj.transform.position;
    }

    /// <summary>
    /// 获取设备要复制的长度
    /// </summary>
    /// <returns></returns>
    public float GetObjToMouseLength(Vector3 point)
    {
        Vector3 tranpos = new Vector3(CopyObj.transform.position.x, 0, CopyObj.transform.position.z);
        Vector3 pointpos = new Vector3(point.x, 0, point.z);
        CopyDirection = pointpos - tranpos;
        float Length = Vector3.Distance(tranpos, pointpos);
        return Length;
    }


    ///// <summary>
    ///// 沿某一方向，一定间距，批量复制设备
    ///// </summary>
    ///// <param name="length"></param>
    //public void RealtimeCreateDevice(float length)
    //{
    //    float len = DeviceWidth + spaceDistance;
    //    if (length > len)//如果长度超过一个设备的宽度+间距，才需要复制
    //    {
    //        int num = Convert.ToInt32(length / len);
    //        int n=num-list
    //        for(int i=0;)
    //        //ModelResource.GetRoomDev(modelName);
    //    }
    //}
    public GameObject CopyCube;
    /// <summary>
    /// 沿某一方向，一定间距，批量复制设备
    /// </summary>
    /// <param name="length"></param>
    IEnumerator RealtimeCopyDevice()
    {
        //协程会因为Active被设为false而终止
        yield return null;
        while (true)
        {
            yield return null;
            if (IsEndCanCopy)
            {
                LineAxis.SetActive(false);
                LineMove.SetActive(false);
                window.gameObject.SetActive(false);
                yield break;
            }
            if (IsCanCopy)//
            {
                InitCopyCube();
                SetCopyCubeState(true);
                Vector3 point = GetFloorColliderPoint();
                //print("RealtimeCreateDevice>>>>>point:" + point);
                float length = GetObjToMouseLength(point);

                StartCoroutine(SetAngleLine(point));

                CopyObj.transform.right = CopyDirection;
                float len = DeviceWidth + spaceDistance;
                //print("RealtimeCreateDevice>>>>>>>length:" + length + ">>>len:" + len);
                //if (length > len)//如果长度超过一个设备的宽度+间距，才需要复制
                //{
                CopyDeviceOP(length, len);
                //}
            }
        }
    }
    /// <summary>
    /// 设置复制用的平面，和设备在同一水平线
    /// </summary>
    private void InitCopyCube()
    {
        if(CopyCube==null)
        {
            CopyCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            CopyCube.transform.localScale = new Vector3(100,0.5f,100);            
            CopyCube.AddCollider();
            MeshRenderer render = CopyCube.GetComponent<MeshRenderer>();
            if (render) render.enabled = false;
            CopyCube.layer = LayerMask.NameToLayer("Floor");
            CopyCube.transform.parent = transform;
            CopyCube.transform.name = "BactchCopyRayPlane";
        }
    }
    /// <summary>
    /// 设置复制用参考平面的状态
    /// </summary>
    /// <param name="isOn"></param>
    private void SetCopyCubeState(bool isOn)
    {
        if(CopyCube!=null)
        {
            if (CopyCube.activeSelf == isOn) return;
            CopyCube.SetActive(isOn);
            if (isOn&& CopyCube != null) CopyCube.transform.position = CopyObj.transform.position;
        }
    }
    /// <summary>
    /// 设置角度线
    /// </summary>
    public IEnumerator SetAngleLine(Vector3 targetPos)
    {
        yield return null;

        float lineLength = 2;

        float height = CopyObj.transform.position.y + CopyObj.GetSize().y / 2;
        height = height + 0.2f;//上移一点
        Vector3 oriPos = new Vector3(CopyObj.transform.position.x, height, CopyObj.transform.position.z);
        targetPos = new Vector3(targetPos.x, height, targetPos.z);
        LineAxis.SetActive(true);
        LineMove.SetActive(true);

        LineRenderer lineRenderer1 = LineAxis.GetComponent<LineRenderer>();
        lineRenderer1.positionCount=2;
        Vector3 oriAxisPos = new Vector3(oriPos.x + lineLength, height, oriPos.z);
        lineRenderer1.SetPosition(0, oriAxisPos);
        lineRenderer1.SetPosition(1, oriPos);
        //lineRenderer1.SetPosition(2, targetPos);
        lineRenderer1.startWidth = 0.03f;
        lineRenderer1.endWidth = 0.03f;

        Vector3 targetPosNew = oriPos + (targetPos - oriPos).normalized * lineLength;
        LineRenderer lineRenderer2 = LineMove.GetComponent<LineRenderer>();
        lineRenderer2.positionCount = 2;
        lineRenderer2.SetPosition(0, oriPos);
        lineRenderer2.SetPosition(1, targetPosNew);
        lineRenderer2.startWidth = 0.03f;
        lineRenderer2.endWidth = 0.03f;

        window.gameObject.SetActive(true);
        Camera cam = CameraSceneManager.Instance.MainCamera;
        Vector3 pos = cam.WorldToScreenPoint(targetPosNew);
        //print("AngleText"+pos);
        window.transform.localPosition = new Vector3(pos.x - (Screen.width / 2), pos.y - (Screen.height / 2), 0);
        window.GetComponentInChildren<Text>().text = "旋转" + Math.Round(CopyObj.transform.rotation.eulerAngles.y) + "°,已复制" + list.Count + "个";
    }

    /// <summary>
    /// 复制机柜操作
    /// </summary>
    /// <param name="length"></param>
    /// <param name="len"></param>
    private void CopyDeviceOP(float length, float len)
    {
        int num = Mathf.FloorToInt(length / len);//两距离间可以复制多少个机柜
        int alreadyNum = list.Count;//已经复制多少个机柜
        int needNum = num - alreadyNum;//还需要复制多少个机柜
        //print((length / len) + "---num:" + num + ">>>>>alreadyNum:" + alreadyNum + ">>>>>needNum:" + needNum);
        if (needNum > 0)
        {
            for (int i = 1; i <= needNum; i++)
            {
                GameObject modelClone = Instantiate(model) as GameObject;
                modelClone.transform.parent = CopyObj.transform;
                modelClone.transform.localPosition = Vector3.zero;
                modelClone.transform.localEulerAngles = Vector3.zero;
                modelClone.transform.localPosition = new Vector3((alreadyNum + i) * len,
                    modelClone.transform.localPosition.y, modelClone.transform.localPosition.z);
                BoxCollider colloder = modelClone.GetComponent<BoxCollider>();
                //modelClone.tag = Tags.Cabinet;
                if(colloder == null&&modelClone.GetComponent<MeshCollider>()!=null)
                {
                    modelClone.RemoveComponent<MeshCollider>();
                    modelClone.AddCollider();
                }
                if (modelClone.GetComponent<BoxCollider>() != null)
                {
                    modelClone.GetComponent<BoxCollider>().enabled = false;
                }
                list.Add(modelClone);
            }
        }
        else
        {
            for (int i = -1; i >= needNum; i--)
            {
                GameObject modelClone = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                DestroyImmediate(modelClone);
            }
        }
    }
    #region 获取设备数据
    /// <summary>
    /// 保存其他信息
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="service"></param>
    private void SaveDevSubInfo(List<GameObject>copyDevList, CommunicationObject service)
    {
        List<Dev_CameraInfo> cameraInfoList = new List<Dev_CameraInfo>();
        foreach(var devObjT in copyDevList)
        {
            DevNode devInfoT = devObjT.GetComponent<DevNode>();
            if (devInfoT == null) continue;
            DevInfo devInfo = devInfoT.Info;
            if (TypeCodeHelper.IsCamera(devInfo.TypeCode.ToString()))
            {
                Dev_CameraInfo cameraInfoT = new Dev_CameraInfo();
                cameraInfoT.Ip = "127.0.0.1";
                cameraInfoT.DevInfoId = devInfo.Id;
                cameraInfoT.UserName = "admin";
                cameraInfoT.PassWord = "12345";
                cameraInfoT.Port = 80;
                cameraInfoT.CameraIndex = 0;
                cameraInfoT.Local_DevID = devInfo.Abutment_DevID;
                cameraInfoT.ParentId = devInfo.ParentId;
                //CameraDevController camera = devInfoT as CameraDevController;         
                //Dev_CameraInfo value = service.AddCameraInfo(cameraInfo);
                cameraInfoList.Add(cameraInfoT);
                //Debug.Log("Add cameraInfo value:" + value == null);
            }
        }
        if (cameraInfoList.Count != 0) service.AddCameraInfo(cameraInfoList);
    }
    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <param name="copyDev"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private DevInfo GetDevInfo(DevInfo copyDev, int index)
    {
        DevInfo dev = new DevInfo();
        dev.DevID = Guid.NewGuid().ToString();
        dev.IP = "";
        dev.CreateTime = DateTime.Now;
        dev.ModifyTime = DateTime.Now;
        dev.Name = string.Format("{0} {1}", copyDev.Name, index);
        dev.ModelName = copyDev.ModelName;
        dev.Status = 0;
        dev.ParentId = copyDev.ParentId;
        dev.TypeCode = copyDev.TypeCode;
        dev.UserName = copyDev.UserName;
        return dev;
    }
    /// <summary>
    /// 获取设备CAD位置信息
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    private DevPos GetDevPos(DevNode devNode,string devId)
    {
        if (devNode == null)
        {
            Debug.LogError(String.Format("devNode is null:{0}", devId));
            return null;
        }
        else
        {
            DevPos posInfo = new DevPos();
            posInfo.DevID = devId;
            //Vector3 pos = model.transform.localPosition;
            Vector3 CadPos = UnityPosToCad(devNode.transform, devNode);
            posInfo.PosX = CadPos.x;
            posInfo.PosY = CadPos.y;
            posInfo.PosZ = CadPos.z;
            Vector3 rotation = devNode.transform.eulerAngles;
            posInfo.RotationX = rotation.x;
            posInfo.RotationY = rotation.y;
            posInfo.RotationZ = rotation.z;
            Vector3 scale = devNode.transform.localScale;
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
    private DevNode SetDevController(DevInfo Info,GameObject copyModel)
    {
        FactoryDepManager depManager = FactoryDepManager.Instance;
        if (depManager)
        {
            copyModel.AddCollider();
            if (Info.ParentId == depManager.NodeID)
            {
                copyModel.transform.parent = depManager.FactoryDevContainer.transform;
                return DevControllerAdd(Info, copyModel, depManager);
            }
            else
            {
                FloorController floor = FactoryDepManager.currentDep as FloorController;
                if (floor && Info.ParentId == floor.NodeID)
                {
                    if (floor.RoomDevContainer != null) return InitRoomDevParent(copyModel,floor, floor.RoomDevContainer, Info);
                }
                else
                {
                    if (floor != null)
                    {
                        DepNode room = floor.ChildNodes.Find(item => item.NodeID == Info.ParentId);
                        if (room != null && room as RoomController)
                        {
                            RoomController controller = room as RoomController;
                            return InitRoomDevParent(copyModel,controller, controller.RoomDevContainer, Info);
                        }
                        else
                        {
                            if (floor.RoomDevContainer != null) return InitRoomDevParent(copyModel,floor, floor.RoomDevContainer, Info);
                        }
                    }
                    else
                    {
                        RoomController roomController = FactoryDepManager.currentDep as RoomController;
                        if (roomController) return InitRoomDevParent(copyModel,roomController, roomController.RoomDevContainer, Info);
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
    private DevNode DevControllerAdd(DevInfo devInfo, GameObject modelTemp, DepNode parentNode)
    {
        string typeCode = devInfo.TypeCode.ToString();
        if (TypeCodeHelper.IsBorderAlarmDev(typeCode))
        {
            BorderDevController borderDev = modelTemp.AddMissingComponent<BorderDevController>();
            InitDevInfo(borderDev, devInfo, parentNode);
            return borderDev;
        }
        else if (TypeCodeHelper.IsCamera(typeCode))
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
    private DevNode InitRoomDevParent(GameObject copyModel,DepNode dep, GameObject devContainer, DevInfo info)
    {
        copyModel.transform.parent = devContainer.transform;
        if (TypeCodeHelper.IsCamera(info.TypeCode.ToString()))
        {
            CameraDevController roomDev = copyModel.AddMissingComponent<CameraDevController>();
            return InitDevInfo(roomDev, info, dep);
        }
        else
        {
            RoomDevController roomDev = copyModel.AddMissingComponent<RoomDevController>();
            return InitDevInfo(roomDev, info, dep);
        }
    }
    /// <summary>
    /// 初始化设备信息
    /// </summary>
    /// <param name="roomDev"></param>
    /// <param name="info"></param>
    /// <param name="parentNode"></param>
    private DevNode InitDevInfo(DevNode devController, DevInfo info, DepNode parentNode)
    {
        devController.Info = info;
        devController.ParentDepNode = parentNode;
        if (RoomFactory.Instance)
        {
            RoomFactory.Instance.SaveDepDevInfo(parentNode, devController, info);
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
        if (!devNode.IsLocal())//不在楼层内
        {
            pos = LocationManager.GetCadVector(dev.position);
        }
        else if (devNode != null)
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

    #endregion
}
