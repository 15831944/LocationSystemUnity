using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectListToolbarItem_Cabinet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 提示信息
    /// </summary>
    public GameObject tipUI;

    LayerMask mask;

    private GameObject model;

    private string ModelName;

    private Vector3 mouseScreenPos;

    public static bool IsDragToInitDeviceEnd = true;//是否拖动生成设备是否结束

    private static bool IsCanSetModelPosition = true;//是否可以设置设备的位置

    private bool IsCanInstantiateModel = false; //是否可以生成模型
    // Use this for initialization
    void Start () {
        mask = LayerMask.GetMask("Floor");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private Transform modelParent;//刚创建模型的父物体
    /// 回调函数：拖动生成
    /// </summary>
    /// <param name="obj"></param>
    private void DragInstantiateFun(Object obj)
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
        modelParent = o.transform.parent;
        model.SetActive(false);
        //o.AddMissingComponent<BoxCollider>();
        //o.AddMissingComponent<RoomDevMouseDrag>();
        //o.AddMissingComponent<RoomDevController>();
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
            //AddSave();
        }
    }
    #region 数据保存部分
    ///// <summary>
    ///// 保存
    ///// </summary>
    //public void AddSave()
    //{
    //    RoomObject.Current.InitCabinetGroupObjectCollection();//必须先初始化一下，不然空机房添加的第一个机柜会一次添加两个物体

    //    Vector3 floorPointT = new Vector3(model.transform.position.x, 0, model.transform.position.z);
    //    Vector3 relatPos = RoomObject.Current.GetRelativePositionRound(floorPointT);
    //    CabinetObject cabinet = new CabinetObject(model, relatPos, ModelName, " ");
    //    cabinet.CreateObjectF(model);
    //    Room room = GlobalDataManager.Instance.GetRoom(RoomObject.Current.Id);
    //    string cabinetName = ModelName;
    //    if (room != null)
    //    {
    //        cabinetName = room.GetNewCabinetName();
    //    }
    //    model.AddMissingComponent<CabinetModel>();

    //    DataAccessController.Instance.AddCabinetInRoom(ModelName, relatPos, model, cabinetName, RoomObject.Current.Id, () =>
    //    {
    //        AfterAddRoomDev(model);
    //        model = null;
    //    }, () => {
    //        DestroyImmediate(model);
    //        model = null;
    //    });
    //}


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
    public static void Reset()
    {
        IsDragToInitDeviceEnd = true;
        IsCanSetModelPosition = true;
    }
    /// <summary>
    /// 设置模型名称
    /// </summary>
    /// <param name="name"></param>
    public void SetModelName(string name)
    {
        Reset();
        ModelName = name;
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
