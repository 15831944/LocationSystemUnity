using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonitorRange;
using HighlightingSystem;
using DG.Tweening;
using System;
using System.Linq;
using UnityStandardAssets.Characters.FirstPerson;
using TModel.Location.Data;

public class DoorAccessItem : MonoBehaviour {

    /// <summary>
    /// 是否单门
    /// </summary>
    [HideInInspector]
    public bool IsSingleDoor;
    /// <summary>
    /// 是否卷闸门
    /// </summary>
    [HideInInspector]
    public bool IsRollingDoor;
    /// <summary>
    /// 当前门的状态
    /// </summary>
    public DoorAccessState CurrentDoorState;
    /// <summary>
    /// 门的ID
    /// </summary>
    public string DoorID
    {
        get
        {
            if (string.IsNullOrEmpty(_doorID)) SetDoorID();
            return _doorID;
        }
    } 
    /// <summary>
    /// 门所在区域ID
    /// </summary>
    public int? ParentID
    {
        get
        {
            if (_parentID == null) GetDepId();   
            return _parentID;
        }
        set { _parentID = value; }
    }
    /// <summary>
    /// 门禁控制脚本
    /// </summary>
    public List<DoorAccessDevController> DoorAccessList = new List<DoorAccessDevController>();

    /// <summary>
    /// 高亮物体
    /// </summary>
    private GameObject RenderObj;
    /// <summary>
    /// 高亮材质
    /// </summary>
    private Renderer CubeRender;
    /// <summary>
    /// 门的动画部分
    /// </summary>
    private DoorAccessItem_Tween TweenPart;
    /// <summary>
    /// 左门
    /// </summary>
    public GameObject DoorLeft;
    /// <summary>
    /// 右门
    /// </summary>
    public GameObject DoorRight;
    /// <summary>
    /// 门是否平行于X轴
    /// </summary>
    private bool IsDoorInXField;
    /// <summary>
    /// 门的ID
    /// </summary>
    private string _doorID;
    /// <summary>
    /// 门所在区域ID
    /// </summary>
    private int? _parentID = null;
    /// <summary>
    /// 门的Collider
    /// </summary>
    private BoxCollider DoorCollider;
    /// <summary>
    /// 门所属的建筑
    /// </summary>
    private BuildingController doorBuilding;
    // Use this for initialization
    void Start () {
              
    }
    #region Public Method
    
    /// <summary>
    /// 查找另一个门禁
    /// </summary>
    /// <param name="currentDev"></param>
    /// <returns></returns>
    public DoorAccessDevController GetAnotherAccess(DoorAccessDevController currentDev)
    {
        DoorAccessDevController dev = DoorAccessList.FirstOrDefault(door=>door.Info.DevID!=currentDev.Info.DevID);
        return dev;
    }
    /// <summary>
    /// 添加门禁
    /// </summary>
    /// <param name="doorAccess"></param>
    public void AddDoorAccess(DoorAccessDevController doorAccess)
    {
        DoorAccessDevController dev = DoorAccessList.Find(i=>i.DevId==doorAccess.DevId);
        if (dev == null) DoorAccessList.Add(doorAccess);
    }
    /// <summary>
    /// 删除门禁
    /// </summary>
    /// <param name="doorAccess"></param>
    public void RemoveDoorAccess(DoorAccessDevController doorAccess)
    {
        if(DoorAccessList.Contains(doorAccess))
        {
            DoorAccessList.Remove(doorAccess);
        }
    }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="isSingleDoor">是否单门</param>
    /// <param name="firstDoor">单门/左门</param>
    /// <param name="secondDoor">右侧门</param>
    public void Init(bool isSingleDoor,GameObject firstDoor,GameObject secondDoor)
    {
        IsSingleDoor = isSingleDoor;
        SetDoorID();       
        SetDoors(isSingleDoor,firstDoor,secondDoor);
        AddBoxCollider();
        TweenPart = gameObject.AddMissingComponent<DoorAccessItem_Tween>();
        TweenPart.SetDoor(isSingleDoor,firstDoor,secondDoor);
        if (firstDoor)
        {
            MeshCollider collider=firstDoor.AddMissingComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = true;
        }
        if (secondDoor)
        {
            MeshCollider collider = secondDoor.AddMissingComponent<MeshCollider>();
            collider.convex = true;
            collider.isTrigger = true;
        }
    }
    public void InitRollingDoor()
    {
        IsRollingDoor = true;
        DoorCollider =(BoxCollider)gameObject.AddCollider();
        if (DoorCollider) DoorCollider.isTrigger = true;
    }
    /// <summary>
    /// 3D响应推送状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeDoorState(DoorAccessState state)
    {
        CurrentDoorState = state;
        if(state.Abutment_CardState=="开")
        {
            OpenDoor();
        }
        else
        {
            CloseDoor(false);
        }
    }
    /// <summary>
    /// 开门动画
    /// </summary>
    /// <param name="onComplete"></param>
    public void OpenDoor(Action onComplete=null)
    {      
        if (!IsRollingDoor)
        {
            if (TweenPart == null) return;
            TweenPart.OpenDoor(onComplete);
        }
        else
        {
            MeshRenderer renderT = transform.GetComponent<MeshRenderer>();
            if (renderT) renderT.enabled = false;
            if (onComplete != null) onComplete();
        }
    }
    /// <summary>
    /// 关门动画
    /// </summary>
    /// <param name="isImmediately"></param>
    /// <param name="onComplete"></param>
    public void CloseDoor(bool isImmediately,Action onComplete=null)
    {
        if (!IsRollingDoor)
        {
            if (TweenPart == null) return;
            TweenPart.CloseDoor(isImmediately,onComplete);
        }
        else
        {
            MeshRenderer renderT = transform.GetComponent<MeshRenderer>();
            if (renderT) renderT.enabled = true;
            if (onComplete != null) onComplete();
        }
    }
    /// <summary>
    /// 设置Render
    /// </summary>
    /// <param name="isOn"></param>
    public void SetRendererEnable(bool isOn)
    {
        if (CubeRender != null)
        {
            RenderObj.SetActive(isOn);
        }
    }
    /// <summary>
    /// 开启高亮
    /// </summary>
    /// <param name="flashColor"></param>
    /// <param name="frequency">Flashing frequency (times per second)</param>
    public void ConstantOn(Color NormalColor)
    {
        //SetRendererEnable(true);
        Highlighter h = RenderObj.AddMissingComponent<Highlighter>();
        h.ConstantOn(NormalColor);
    }
    /// <summary>
    /// 关闭闪烁
    /// </summary>
    public void ConstantOff()
    {
        if (RenderObj.GetComponent<Highlighter>() != null)
        {
            Highlighter h = RenderObj.AddMissingComponent<Highlighter>();
            h.ConstantOff();
            //SetRendererEnable(false);
        }
        else
        {
            Debug.LogError(string.Format("{0} Highlighter is null", gameObject.name));
        }
    }
    /// <summary>
    /// 设置单门门禁
    /// </summary>
    /// <param name="doorTransform"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public List<GameObject> SetDoubleDoorAccess(Transform doorTransform, GameObject model)
    {
        return AddDoubleDoorAccess(doorTransform,model);
    }
    /// <summary>
    /// 设置双门门禁
    /// </summary>
    /// <param name="doorTransform"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public List<GameObject> SetSingleDoorAccess(Transform doorTransform, GameObject model)
    {
        return AddSingleDoorAccess(doorTransform,model);
    }
    /// <summary>
    /// 门Collider扩大缩小的值
    /// </summary>
    private float colliderEnlargeSize = 4f;
    /// <summary>
    /// 卷闸门扩大的值
    /// </summary>
    private float rollingEnlargeSize = 6f;

    /// <summary>
    /// 是否以及调大了碰撞体，防止一直变大。
    /// </summary>
    public bool IsLargeCollder = false;

    /// <summary>
    /// 扩大门的Collider
    /// </summary>
    [ContextMenu("EnlargeCollider")]
    public void EnlargeCollider()
    {
        if (DoorCollider == null) return;
        if (!DoorCollider.enabled) DoorCollider.enabled = true;
        if (IsLargeCollder) return;
        IsLargeCollder = true;
        float sizeEnlarge = IsRollingDoor ? rollingEnlargeSize : colliderEnlargeSize;
        if (IsDoorInXField)
        {
            Vector3 size = DoorCollider.size;
            size.z += sizeEnlarge;
            DoorCollider.size = size;
        }
        else
        {
            Vector3 size = DoorCollider.size;
            size.x += sizeEnlarge;
            DoorCollider.size = size;
        }

        //ConstantOn(Color.blue);
    }
    /// <summary>
    /// 恢复门的Collider
    /// </summary>
    [ContextMenu("RecoverCollider")]
    public void RecoverCollider()
    {
        if (DoorCollider == null) return;
        if (IsLargeCollder==false) return;
        IsLargeCollder = false;
        float sizeRecover = IsRollingDoor ? rollingEnlargeSize : colliderEnlargeSize;
        if (IsDoorInXField)
        {
            Vector3 size = DoorCollider.size;
            size.z -= sizeRecover;
            DoorCollider.size = size;
        }
        else
        {
            Vector3 size = DoorCollider.size;
            size.x -= sizeRecover;
            DoorCollider.size = size;
        }
        CloseAllDoor();
    }
    /// <summary>
    /// 关闭所有门
    /// </summary>
    [ContextMenu("CloseAllDoor")]
    public void CloseAllDoor()
    {
        if (DoorLeft != null) DoorLeft.transform.localEulerAngles = Vector3.zero;
        if (DoorRight != null) DoorRight.transform.localEulerAngles = Vector3.zero;
        if (IsRollingDoor) CloseDoor(true);
    }
    #endregion
    /// <summary>
    /// 判断门的中心点
    /// </summary>
    /// <param name="isSingleDoor"></param>
    /// <param name="firstDoor"></param>
    /// <param name="secondDoor"></param>
    private void SetDoors(bool isSingleDoor, GameObject firstDoor, GameObject secondDoor)
    {
        IsDoorInXField = IsInXField();
        if (IsSingleDoor)
        {
            if (IsDoorInLeft(firstDoor)) DoorLeft = firstDoor;
            else DoorRight = firstDoor;          
        }
        else
        {
            if (IsDoorInLeft(firstDoor))
            {
                DoorLeft = firstDoor;
                DoorRight = secondDoor;
            }
            else
            {
                DoorLeft = secondDoor;
                DoorRight = firstDoor;
            }
        }
    }
  
    /// <summary>
    /// 设置门的ID
    /// </summary>
    private void SetDoorID()
    {
        _doorID = transform.GetSiblingIndex().ToString();
    }
    /// <summary>
    /// 获取所在区域ID
    /// </summary>
	private void GetDepId()
    {
        DepDoors dep = transform.GetComponentInParent<DepDoors>();
        if(dep)
            _parentID = dep.DoorDep.NodeID;
    }
    private Vector3 SacleOffset = new Vector3(0.1f,0,0.1f);
    /// <summary>
    /// 添加门的Collider
    /// </summary>
    private void AddBoxCollider()
    {
        if(!IsSingleDoor)
        {
            Collider collider = transform.gameObject.AddCollider();
            collider.isTrigger = true;
            DoorCollider = transform.GetComponent<BoxCollider>();
        }
        else
        {
            SetSingleDoorParent();
        }   
        GameObject areaObj = CreateAreaObject(transform);
        areaObj.name = transform.name + "Render";
        //AreaObj.SetLayerAll(Layers.IgnoreRaycast);//Layers.Range
        Vector3 doorSize = transform.gameObject.GetSize();
        areaObj.transform.localScale = doorSize + SacleOffset;
        SetAreaObjPos(areaObj, doorSize);
        //AreaObj.transform.localPosition = Vector3.zero;
        areaObj.transform.localEulerAngles = Vector3.zero;
        CubeRender = areaObj.GetComponent<Renderer>();
        RenderObj = areaObj;
        //transform.gameObject.SetLayerAll(Layers.IgnoreRaycast);
    }
    /// <summary>
    /// 单门添加一个父物体
    /// </summary>
    /// <param name="singleDoor"></param>
    private void SetSingleDoorParent()
    {
        GameObject singleDoor = DoorLeft != null ? DoorLeft : DoorRight;
        GameObject obj = new GameObject();
        obj.transform.name = singleDoor.transform.name;
        int lastIndex = singleDoor.transform.GetSiblingIndex();
        obj.transform.parent = singleDoor.transform.parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localPosition = Vector3.zero;
        SingleDoorTrigger trigger = obj.AddMissingComponent<SingleDoorTrigger>();
        trigger.InitDoorItem(this);
        singleDoor.transform.parent = obj.transform;
        obj.transform.SetSiblingIndex(lastIndex);
        Collider collider = obj.AddCollider();
        collider.isTrigger = true;
        DoorCollider = obj.GetComponent<BoxCollider>();
    }
    /// <summary>
    /// 设置高亮材质位置
    /// </summary>
    /// <param name="areaObj"></param>
    private void SetAreaObjPos(GameObject areaObj,Vector3 doorSize)
    {
        if(!IsSingleDoor||IsRollingDoor)
        {
            areaObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            GameObject door = DoorLeft != null ? DoorLeft : DoorRight;
            if(IsDoorInXField)
            {
                if (IsDoorInLeft(door)) areaObj.transform.localPosition = new Vector3(-doorSize.x/2,0,0);
                else areaObj.transform.localPosition = new Vector3(doorSize.x / 2, 0, 0);
            }
            else
            {
                if(IsDoorInLeft(door)) areaObj.transform.localPosition = new Vector3(0, 0, doorSize.z/2);
                else areaObj.transform.localPosition = new Vector3(0, 0, -doorSize.z / 2);
            }
        }
    }
    /// <summary>
    /// 创建区域物体
    /// </summary>
    /// <returns></returns>
    private GameObject CreateAreaObject(Transform parent)
    {
        GameObject areaPrefab = MonitorRangeManager.Instance.areaPrefab;
        GameObject o;
        if (areaPrefab == null)
        {
            Debug.LogError("MonitorRangeManager.Instance.areaPrefab is null...");
            o= GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        else
        {
            o = Instantiate(areaPrefab);
            o.gameObject.RemoveComponent<BoxCollider>();
        }
        o.transform.SetParent(parent);
        return o;
    }
    void OnTriggerEnter(Collider other)
    {
        FirstPersonController person = other.gameObject.GetComponent<FirstPersonController>();
        if (person != null && person.gameObject.layer != LayerMask.NameToLayer("Person")) 
            //定位人员的Layer是Person，漫游人员没有设置是Default
        {
            OnFPSEnter(person.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        FirstPersonController person = other.gameObject.GetComponent<FirstPersonController>();
        if (person != null && person.gameObject.layer != LayerMask.NameToLayer("Person"))
        {
            OnFPSExit(person.gameObject);
        }
    }
    /// <summary>
    /// 门的中心点,是否在左边
    /// </summary>
    /// <param name="door"></param>
    /// <returns></returns>
    private bool IsDoorInLeft(GameObject door)
    {
        MeshFilter fliter = door.GetComponent<MeshFilter>();
        Bounds bounds = fliter.mesh.bounds;
        if (IsDoorInXField)
        {
            //Debug.Log(string.Format("x:{0}  y:{1}  z:{2}", bounds.center.x, bounds.center.y, bounds.center.z));
            if (bounds.center.x < 0) return true;
            else return false;
        }
        else
        {
            if (bounds.center.z >0) return true;
            else return false;
        }
    }

    /// <summary>
    /// 漫游人物进入
    /// </summary>
    /// <param name="person"></param>
    public void OnFPSEnter(GameObject person)
    {
        SetBuildingState(person,true);
        if (IsRollingDoor)
        {
            OpenDoor();
            return;
        }
        if(IsDoorInXField)
        {
            if (person.transform.position.z > transform.position.z)
            {
                //右门90  左-90
                if (DoorLeft != null) DoorLeft.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f);
                if (DoorRight!=null)DoorRight.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f);               
            }
            else
            {
                //右门-90 左90
                if (DoorLeft != null) DoorLeft.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f);
                if (DoorRight != null) DoorRight.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f);
            }
        }
        else
        {
            if (person.transform.position.x > transform.position.x)
            {
                //右门90  左-90
                if (DoorLeft != null) DoorLeft.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f);
                if (DoorRight != null) DoorRight.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f);               
            }
            else
            {
                //右门-90 左90
                if (DoorLeft != null) DoorLeft.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f);
                if (DoorRight != null) DoorRight.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f);
            }
        }
    }
    public void OnFPSExit(GameObject person)
    {
        SetBuildingState(person,false);
        if (IsRollingDoor)
        {
            CloseDoor(false);
            return;
        }
        if (DoorRight != null) DoorRight.transform.DOLocalRotate(Vector3.zero, 1f);
        if (DoorLeft != null) DoorLeft.transform.DOLocalRotate(Vector3.zero, 1f);
    }
    private void SetBuildingState(GameObject person,bool isEnter)
    {
        Debug.Log("DoorAccessItem.SetBuildingState isEnter:" + isEnter);
        if (doorBuilding == null) doorBuilding = transform.GetComponentInParent<BuildingController>();
        DevSubsystemManage manager = DevSubsystemManage.Instance;
        if (doorBuilding == null||manager==null) return;             
        if (isEnter)//进入门的范围
        {
            BuildingBox box = doorBuilding.GetComponent<BuildingBox>();
            if(box!=null)
            {
                box.LoadBuilding(dep =>
                {
                    LoadBuildingDev(doorBuilding);
                });
            }
            else
            {
                LoadBuildingDev(doorBuilding);
            }                     
        }
        else//离开门的范围
        {
            var isExist = manager.IsBuildingExist(doorBuilding);
            if (!isExist)
            {
                DevSubsystemManage.Instance.SetTriggerBuilding(doorBuilding, false);
                //doorBuilding.ShowBuildingDev(false);
                RoamManage.Instance.SetLight(false);
            }           
        }
    }
    /// <summary>
    /// 加载建筑内设备
    /// </summary>
    /// <param name="doorBuilding"></param>
    private void LoadBuildingDev(BuildingController doorBuilding)
    {
        doorBuilding.ShowBuildingDev(true);
        RoamManage.Instance.SetLight(true);
        var isExist = DevSubsystemManage.Instance.IsBuildingExist(doorBuilding);//人是否已经在建筑里面了

        if (!isExist)//进入门的范围，但是还没踏入建筑时，加载建筑内的设备。
        {
            //Debug.LogError(string.Format("{0} enter", doorBuilding.NodeName));
            DevSubsystemManage.Instance.SetTriggerBuilding(doorBuilding, true);  //设置已经进入建筑了，进入门的范围则算是进入建筑了                               
        }
        else //先进入建筑的范围，再进入门的范围
        {

        }
        //进入门的范围则算是进入建筑了
        if (!doorBuilding.IsDevCreate)
        {
            doorBuilding.IsDevCreate = true;
            //RoomFactory.Instance.CreateDepDev(doorBuilding, true);//进入建筑，创建设备。
            doorBuilding.LoadDevices(() =>
            {
                RoomFactory.Instance.CreateDepDev(doorBuilding, true);
            });
        }
    }

    /// <summary>
    /// 门是否在X轴上
    /// </summary>
    /// <returns></returns>
    private bool IsInXField()
    {
        MeshRenderer render;
        render = transform.GetComponentInChildren<MeshRenderer>();
        if (render == null) return false;
        else return IsInXField(render);
    }
    /// <summary>
    /// 门是否在X轴上
    /// </summary>
    /// <param name="renderT"></param>
    /// <returns></returns>
    private bool IsInXField(MeshRenderer renderT)
    {       
        Vector3 boundSize = renderT.bounds.size;       
        Vector3 sizeT = renderT.bounds.size;
        bool isXRotation;
        isXRotation = sizeT.x > sizeT.z ? true : false;
        return isXRotation;
    }
    #region CreateDoorAccess
        /// <summary>
        /// 创建门禁设备
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
    public List<GameObject>CreateDoorAccess(GameObject model)
    {
        if(IsSingleDoor||IsRollingDoor)
        {
            return SetSingleDoorAccess(transform,model);
        }
        else
        {
            return SetDoubleDoorAccess(transform,model);
        }
    }
    /// <summary>
    /// 设备高度，以门中心点为基准
    /// </summary>
    private float DoorAccessHeight = 0.1f;
    /// <summary>
    /// 设备到墙的距离
    /// </summary>
    private float DisToWall = 0.07f;
    /// <summary>
    /// 卷闸门的宽度0.7,超过0.6判定为卷闸门
    /// </summary>
    private float RollingDoor = 0.6f;

    private Vector3 CubeScale = Vector3.one;
    private Vector3 RightAngleX = new Vector3(0, 180, 0);
    private Vector3 LeftAngleX = Vector3.zero;

    private Vector3 RightAngleZ = new Vector3(0, 90, 0);
    private Vector3 LeftAngleZ = new Vector3(0, 270, 0);
    /// <summary>
    /// 设置双门门禁位置
    /// </summary>
    private List<GameObject> AddDoubleDoorAccess(Transform doorTransform,GameObject model)
    {
        MeshRenderer renderT = doorTransform.GetChild(0).GetComponent<MeshRenderer>();
        if (renderT == null)
        {
            Debug.Log(string.Format("{0} MeshRender is null", doorTransform.name));
            return null;
        }
        //Vector3 boundSize = renderT.bounds.size;
        //if (boundSize.x > RollingDoor || boundSize.z > RollingDoor)
        //{
        //    Debug.Log("Is rolling door " + doorTransform.name);
        //    return null;
        //}
        List<GameObject> objList = new List<GameObject>();
        Vector3 sizeT = renderT.bounds.size;
        bool isXRotation;
        bool isZRotation;
        //Debug.Log(string.Format("Vector3.Forward : {0} Transform.Forward : {1}",Vector3.forward,tr));
        isXRotation = sizeT.x > sizeT.z ? true : false;
        isZRotation = !isXRotation;
        //Debug.Log("IsXRotation:"+isXRotation+"  IsZRotation:"+isZRotation);

        GameObject obj = Instantiate(model);
        GameObject obj2 = Instantiate(model);
        if (isXRotation)
        {
            float offset = -(sizeT.x + 0.1f);
            obj.transform.parent = doorTransform;
            obj.transform.localScale = CubeScale;
            obj.transform.localEulerAngles = RightAngleX;
            obj.transform.localPosition = new Vector3(offset, DoorAccessHeight, DisToWall);

            obj2.transform.parent = doorTransform;
            obj2.transform.localScale = CubeScale;
            obj2.transform.localEulerAngles = LeftAngleX;
            obj2.transform.localPosition = new Vector3(-offset, DoorAccessHeight, -DisToWall);
        }
        else
        {
            float offset = -(sizeT.z + 0.1f);
            obj.transform.parent = doorTransform;
            obj.transform.localScale = CubeScale;
            obj.transform.localEulerAngles = RightAngleZ;
            obj.transform.localPosition = new Vector3(-DisToWall, DoorAccessHeight, offset);

            obj2.transform.parent = doorTransform;
            obj2.transform.localScale = CubeScale;
            obj2.transform.localEulerAngles = LeftAngleZ;
            obj2.transform.localPosition = new Vector3(DisToWall, DoorAccessHeight, -offset);
        }
        objList.Add(obj);
        objList.Add(obj2);
        return objList;
    }
    /// <summary>
    /// 设置单门门禁位置
    /// </summary>
    private List<GameObject> AddSingleDoorAccess(Transform doorTransform,GameObject model)
    {
        //判断墙的方向
        MeshRenderer rendererT = doorTransform.GetComponent<MeshRenderer>();
        if (rendererT == null)
        {
            Debug.Log(string.Format("{0} MeshRender is null", doorTransform.name));
            return null;
        }
        //Vector3 boundSize = rendererT.bounds.size;
        //if (boundSize.x > RollingDoor || boundSize.z > RollingDoor)
        //{
        //    Debug.Log("Is rolling door " + doorTransform.name);
        //    return null;
        //}
        List<GameObject> objList = new List<GameObject>();
        Vector3 sizeT = rendererT.bounds.size;
        bool isXRotation;
        bool isZRotation;
        isXRotation = sizeT.x > sizeT.z ? true : false;
        isZRotation = !isXRotation;

        //判断墙的中心点（偏左/偏右）
        bool isLeftSide = false;
        MeshFilter fliterT = doorTransform.GetComponent<MeshFilter>();
        foreach (var item in fliterT.mesh.vertices)
        {
            //Debug.Log(item);
            if (isXRotation && item.x <= -0.1f)
            {
                isLeftSide = true;
                break;
            }
            else if (isZRotation && item.z <= -0.1f)
            {
                isLeftSide = true;
                break;
            }
        }

        //设置门禁位置
        GameObject obj = Instantiate(model);
        GameObject obj2 = Instantiate(model);
        if (isXRotation)
        {
            if(IsRollingDoor)
            {
                float offsetT = (sizeT.x / 2 + 0.1f);
                Vector3 localPos1 = new Vector3(offsetT, DoorAccessHeight, DisToWall);
                SetDoorPos(obj,doorTransform,CubeScale,RightAngleX,localPos1);
                Vector3 localPos2 = new Vector3(-offsetT, DoorAccessHeight, DisToWall);
                SetDoorPos(obj,doorTransform,CubeScale,LeftAngleX,localPos2);
            }
            else
            {
                float offset = isLeftSide ? -(sizeT.x + 0.1f) : -0.1f;
                SetDoorPos(obj,doorTransform,CubeScale,RightAngleX, new Vector3(offset, DoorAccessHeight, DisToWall));                
                Vector3 localPos = isLeftSide ? new Vector3(-offset - sizeT.x, DoorAccessHeight, -DisToWall) : new Vector3(-offset + sizeT.x, DoorAccessHeight, -DisToWall);
                SetDoorPos(obj2, doorTransform, CubeScale, LeftAngleX, localPos);
            }          
        }
        else
        {
            if (IsRollingDoor)
            {
                float offsetT = (sizeT.z / 2 + 0.1f);
                Vector3 localPos1 = new Vector3(-DisToWall, DoorAccessHeight, offsetT);
                SetDoorPos(obj, doorTransform, CubeScale, RightAngleX, localPos1);
                Vector3 localPos2 = new Vector3(DisToWall, DoorAccessHeight, -offsetT);
                SetDoorPos(obj, doorTransform, CubeScale, LeftAngleX, localPos2);
            }
            else
            {
                float offset = isLeftSide ? -(sizeT.z + 0.1f) : -0.1f;
                Vector3 localPosFirst = new Vector3(-DisToWall, DoorAccessHeight, offset);
                SetDoorPos(obj, doorTransform, CubeScale, RightAngleZ, localPosFirst);
                Vector3 localPos = isLeftSide ? new Vector3(DisToWall, DoorAccessHeight, -offset - sizeT.z) : new Vector3(0.1f, DisToWall, -offset + sizeT.z);
                SetDoorPos(obj2, doorTransform, CubeScale, LeftAngleZ, localPos);
            }                 
        }
        objList.Add(obj);
        objList.Add(obj2);
        return objList;
    }
    /// <summary>
    /// 设置门禁的位置和父物体
    /// </summary>
    /// <param name="door">门禁</param>
    /// <param name="parentObj">门禁父物体</param>
    /// <param name="localScale">缩放</param>
    /// <param name="localAngles">角度</param>
    /// <param name="localPos">位置</param>
    private void SetDoorPos(GameObject door,Transform parentObj,Vector3 localScale,Vector3 localAngles,Vector3 localPos)
    {
        door.transform.parent = parentObj;
        door.transform.localScale = localScale;
        door.transform.localEulerAngles = localAngles;
        door.transform.localPosition = localPos;
    }
    #endregion
}
