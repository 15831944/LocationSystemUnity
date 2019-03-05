using UnityEngine;
using System.Collections;
//using U3D.DataAccess.DAs;
using Location.WCFServiceReferences.LocationServices;
using System.Collections.Generic;
using RTEditor;
using Assets.M_Plugins.Helpers.Utils;
/// <summary>
/// 物体添加列表管理
/// </summary>
public class ObjectAddListManage : MonoBehaviour {

    public static ObjectAddListManage Instance;
    /// <summary>
    /// 是否在设备编辑状态
    /// </summary>
    public static bool IsEditMode;
    /// <summary>
    /// 窗口界面
    /// </summary>
    public GameObject window;
    /// <summary>
    /// 物体添加列表信息
    /// </summary>
    public List<ObjectAddList_Type> info;


    // Use this for initialization
    void Start () {
        Instance = this;
        SceneEvents.DepNodeChanged += OnDepNodeChange;
        //InitObjectAddList();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    private void OnDepNodeChange(DepNode old,DepNode newDep)
    {
        if(IsEditMode)
        {
            SetDepBorderDevEnable(old,false);
            SetDepBorderDevEnable(newDep, true);
        }
    }
    /// <summary>
    /// 设置区域下，边界告警设备的显示和隐藏
    /// </summary>
    /// <param name="dep"></param>
    /// <param name="isEnable"></param>
    private void SetDepBorderDevEnable(DepNode dep,bool isEnable)
    {
        if (dep == null) return;
        List<DevNode> dev = RoomFactory.Instance.GetDepDevs(dep);
        if (dev == null || dev.Count == 0) return;
        List<DevNode> borderDevList = dev.FindAll(item => TypeCodeHelper.IsBorderAlarmDev(item.Info.TypeCode.ToString()));
        if (borderDevList == null || borderDevList.Count == 0) return;
        foreach (var item in borderDevList)
        {
            if (item.isAlarm) continue;
            BorderDevController borderDev = item as BorderDevController;
            borderDev.SetRendererEnable(isEnable);
        }
    }
    private void OnEnable()
    {
        //InitObjectAddList();
    }

    /// <summary>
    /// 初始化物体添加列表
    /// </summary>
    public void InitObjectAddList()
    {
        ThreadManager.Run(() =>
        {
            info = GetObjectAddListInfo();

        }, () =>
        {
            //if (RoomState.IsObjectMode)
            //{
            //    ObjectListToolbar.Instance.SetObjectToolbar(false);
            //}
            //else
            //{
            //    ObjectListToolbar.Instance.SetObjectToolbar(true);
            //}
            ObjectListToolbar.Instance.SetObjectToolbar(true);

            ObjectListToolbar.Instance.powerDeviceToggle.isOn = true;
            ObjectListToolbar.Instance.PowerDeviceToggle_ValueChanged(true);
        }, "获取设备数据");
    }

    /// <summary>
    /// 获取物体添加列表的信息
    /// </summary>
    public List<ObjectAddList_Type> GetObjectAddListInfo()
    {
        CommunicationObject access = CommunicationObject.Instance;
        List<ObjectAddList_Type> modelList =access.GetModelAddList();
        return modelList;
    }

    /// <summary>
    /// 根据大类名称获取大类相关信息
    /// </summary>
    /// <returns></returns>
    public ObjectAddList_Type GetObjectAddListTypeInfoByName(string name)
    {
        ObjectAddList_Type objectAddList_Type = info.Find((item) => item.typeName == name);
        return objectAddList_Type;
    }

    public void Show()
    {
        window.SetActive(true);
        SetEditMode(true);
        Reflesh();
    }

    public void Hide()
    {
        window.SetActive(false);
        SetEditMode(false);
    }
    /// <summary>
    /// 显示\关闭窗口
    /// </summary>
    /// <param name="isOn"></param>
    public void SetWindowState(bool isOn)
    {
        window.SetActive(isOn);
    }
    /// <summary>
    /// 设置编辑模式
    /// </summary>
    /// <param name="isOn">是否编辑模式</param>
    private void SetEditMode(bool isOn)
    {
        EditorObjectSelection objectEditor = EditorObjectSelection.Instance;
        if (isOn)
        {
            IsEditMode = true;
            SetDepBorderDevEnable(FactoryDepManager.currentDep, true);
        }
        else
        {
            IsEditMode = false;
            //EditorObjectSelection objectEditor = EditorObjectSelection.Instance;
            if (objectEditor)
            {
                if(DeviceEditUIManager.Instacne)DeviceEditUIManager.Instacne.HideMultiDev();
                objectEditor.ClearSelection(false);
                DeviceEditUIManager.Instacne.Close();
            }
            SetDepBorderDevEnable(FactoryDepManager.currentDep, false);
        }
    }
    /// <summary>
    /// 刷新列表
    /// </summary>
    public void Reflesh()
    {
        if (window.activeInHierarchy)
        {
            InitObjectAddList();
        }
    }
}
