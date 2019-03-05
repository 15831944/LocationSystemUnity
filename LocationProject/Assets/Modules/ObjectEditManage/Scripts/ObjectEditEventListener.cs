using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTEditor;
using Location.WCFServiceReferences.LocationServices;
using System.Linq;
using Unity.Common.Utils;
using Assets.M_Plugins.Helpers.Utils;

public class ObjectEditEventListener : MonoBehaviour {

    private List<GameObject> selectedObjs = new List<GameObject>();//当前选中设备

    // Use this for initialization
    void Start()
    {
        BindingGizmosEvent();
    }
    /// <summary>
    /// 绑定设备编辑事件
    /// </summary>
    private void BindingGizmosEvent()
    {
        EditorGizmoSystem GizmoSystem = EditorGizmoSystem.Instance;
        if (GizmoSystem == null)
        {
            Debug.LogError("EditorGizmoSystem.Instance is null!");
        }
        else
        {
            //移动编辑事件
            GizmoSystem.TranslationGizmo.GizmoDragStart += TranslationGizmosStart;
            GizmoSystem.TranslationGizmo.GizmoDragUpdate += TranslationGizmosUpdate;
            GizmoSystem.TranslationGizmo.GizmoDragEnd += TranslationGizmosEnd;
            //旋转编辑事件
            GizmoSystem.RotationGizmo.GizmoDragStart += RotationGizmosStart;
            //GizmoSystem.RotationGizmo.GizmoDragUpdate += RotationGizmosUpdate;
            GizmoSystem.RotationGizmo.GizmoDragEnd += RotationGizmosEnd;
            //缩放编辑事件
            GizmoSystem.ScaleGizmo.GizmoDragStart += ScaleGizmosStart;
            //GizmoSystem.ScaleGizmo.GizmoDragUpdate += ScaleGizmosUpdate;
            GizmoSystem.ScaleGizmo.GizmoDragEnd += ScaleGizmosEnd;
        }
        if(EditorObjectSelection.Instance)
        {
            //物体选中和取消选中的事件
            EditorObjectSelection.Instance.SelectionChanged += OnEditObjectSelectionChange;
        }
    }
    /// <summary>
    /// 编辑物体，选中和取消选中的事件
    /// </summary>
    /// <param name="selectionChangedEventArgs"></param>
    public void OnEditObjectSelectionChange(ObjectSelectionChangedEventArgs selectionChangedEventArgs)
    {
        if (!ObjectAddListManage.IsEditMode) return;
        HideDevInfo();
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection.SelectedGameObjects == null || selection.SelectedGameObjects.Count == 0)
        {
            DeviceEditUIManager.Instacne.SetEmptValue();
            SurroundEditMenu_BatchCopy.Instacne.CloseUI();
        }
        else
        {
            ClearDevHighlight();
            selectedObjs = selection.SelectedGameObjects.ToList();
            List<DevNode> devs = GetDevNode(selectedObjs);
            ShowDevInfo(devs);
            //SetBatchCopyState(devs);
        }        
    }
    /// <summary>
    /// 清除静态设备高亮
    /// </summary>
    private void ClearDevHighlight()
    {
        if(HighlightManage.Instance)
        {
            HighlightManage.Instance.HighLightDevOff();
        }
    }
    /// <summary>
    /// 设置批量复制按钮
    /// </summary>
    /// <param name="devList"></param>
    private void SetBatchCopyState(List<DevNode>devList)
    {
        SurroundEditMenu_BatchCopy copyPart = SurroundEditMenu_BatchCopy.Instacne;
        if (copyPart)
        {
            if (devList.Count > 1) copyPart.CloseUI();
            else if (devList.Count == 1)
            {
                DevNode dev = devList[0];
                if (dev is RoomDevController || dev is DepDevController||!TypeCodeHelper.IsLocationDev(dev.Info.TypeCode.ToString()))
                {
                    if (ObjectAddListManage.IsEditMode) copyPart.Open(dev);
                }
                else
                {
                    copyPart.CloseUI();
                }
            }
        }
    }
  
    /// <summary>
    /// 获取设备
    /// </summary>
    /// <param name="devs"></param>
    /// <returns></returns>
    private List<DevNode>GetDevNode(List<GameObject>devs)
    {
        List<DevNode> devList = new List<DevNode>();
        foreach (var item in devs)
        {
            DevNode dev = item.GetComponent<DevNode>();
            if (dev == null) continue;
            devList.Add(dev);
        }
        return devList;
    }
    /// <summary>
    /// 关闭设备编辑界面
    /// </summary>
    private void HideDevInfo()
    {
        DeviceEditUIManager.Instacne.Close();
        DeviceEditUIManager.Instacne.HideMultiDev();
    }
    /// <summary>
    /// 显示设备编辑界面
    /// </summary>
    /// <param name="selectObjList"></param>
    private void ShowDevInfo(List<DevNode> selectObjList)
    {
        if (selectObjList.Count == 1)
        {
            DeviceEditUIManager.Instacne.Show(selectObjList[0]);
        }
        else if (selectObjList.Count > 1)
        {           
            ShowDevFollowUI(selectObjList);
        }       
    }
    /// <summary>
    /// 显示设备漂浮UI
    /// </summary>
    /// <param name="devList"></param>
    private void ShowDevFollowUI(List<DevNode>devList)
    {
        if (devList.Count == 1)
        {
            DeviceEditUIManager.Instacne.Show(devList[0]);
        }
        else if (devList.Count > 1)
        {
            DeviceEditUIManager.Instacne.ShowMultiDev(devList);
        }
    }

    private CommunicationObject Service;
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="ObjList"></param>
    private void ModifyDevPos(List<GameObject>ObjList)
    {
        if (Service == null)
        {
            Service = CommunicationObject.Instance;
        }
        List<DevPos> posList = new List<DevPos>();
        foreach (var Obj in ObjList)
        {
            DevNode Dev = Obj.GetComponent<DevNode>();
            if (Dev == null) continue;
            DevPos pos = CaculateDevPos(Dev,Obj);
            //Service.ModifyDevPos(pos);  
            posList.Add(pos);
        }
        if (posList.Count != 0)
        {
            Service.ModifyDevPosByList(posList);
        }
    }
    /// <summary>
    /// 更新设备位置信息
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="model"></param>
    private DevPos CaculateDevPos(DevNode devInfo,GameObject model)
    {
        DevPos posInfo = new DevPos();
        posInfo.DevID = devInfo.Info.DevID;
        Vector3 cadPos = GetCadPos(model,devInfo);
        posInfo.PosX = cadPos.x;
        posInfo.PosY = cadPos.y;
        posInfo.PosZ = cadPos.z;
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
    /// <summary>
    /// 获取CAD位置
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dev"></param>
    /// <returns></returns>
    private Vector3 GetCadPos(GameObject obj,DevNode dev)
    {
        //DeviceEdit editPart = DeviceEditUIManager.Instacne.EditPart;
        Vector3 cadPos;
        bool isLocalPos = !(dev.ParentDepNode==FactoryDepManager.Instance||dev is DepDevController);
        if (!isLocalPos)
        {
            cadPos = LocationManager.UnityToCadPos(obj.transform.position, false);
        }
        else
        {
            cadPos = LocationManager.UnityToCadPos(obj.transform.localPosition, true);
        }
        return cadPos;
    }
    #region TranslationGizmo
    /// <summary>
    /// 移动开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 移动更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosUpdate(Gizmo gizmo)
    {
        List<GameObject> controlObj = gizmo.ControlledObjects.ToList();
        if (controlObj.Count == 1)
        {
            GameObject modelT = controlObj[0];
            DevNode dev = modelT.GetComponent<DevNode>();
            if (dev == null||dev.Info==null) return;
            dev.Info.Pos = CaculateDevPos(dev, modelT);
            DeviceEditUIManager.Instacne.Show(dev);
        }
    }
    /// <summary>
    /// 移动结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void TranslationGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
    #region RotationGizmo
    /// <summary>
    /// 旋转开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 旋转更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// 旋转结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void RotationGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
    #region ScaleGizmo
    /// <summary>
    /// 缩放开始
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    /// <summary>
    /// 缩放更新
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosUpdate(Gizmo gizmo)
    {

    }
    /// <summary>
    /// 缩放结束
    /// </summary>
    /// <param name="gizmo"></param>
    private void ScaleGizmosEnd(Gizmo gizmo)
    {
        ModifyDevPos(gizmo.ControlledObjects.ToList());
        //foreach (var item in gizmo.ControlledObjects)
        //{
        //    Debug.Log(item.name);
        //}
    }
    #endregion
}
