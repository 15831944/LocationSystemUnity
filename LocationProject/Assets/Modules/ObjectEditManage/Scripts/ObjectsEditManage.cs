using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTEditor;
/// <summary>
/// 物体编辑管理控制脚本
/// </summary>
public class ObjectsEditManage : MonoBehaviour
{
    public static ObjectsEditManage Instance;
    /// <summary>
    /// 编辑部分（不使用时关闭，减少性能消耗）
    /// </summary>
    public GameObject EditPart;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        SceneEvents.DepNodeChanged += OnDepNodeChange;
        //EndEdit();
        //Invoke("EndEdit", 2f);
    }

    /// <summary>
    /// 开始编辑
    /// </summary>
    public void StartEdit()
    {
        if (!EditPart.activeInHierarchy)
            EditPart.SetActive(true);
    }
    /// <summary>
    /// 结束编辑
    /// </summary>
    public void EndEdit()
    {
        if (EditPart.activeInHierarchy)
            EditPart.SetActive(false);
    }

    /// <summary>
    /// 设置Translation，Rotation，Scale，VolumeScale是否可以编辑，是为了在不编辑状态时，按Q,W,E,R等键时不会影响到，开启编辑起始状态
    /// </summary>
    public void SetEditorGizmoSystem(bool isActive)
    {
        EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.Translation, isActive);
        EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.Rotation, isActive);
        EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.Scale, isActive);
        EditorGizmoSystem.Instance.SetGizmoTypeAvailable(GizmoType.VolumeScale, isActive);

        EditorGizmoSystem.Instance.ActiveGizmoType = GizmoType.Translation;
    }

    private void OnDepNodeChange(DepNode last, DepNode newDep)
    {
        if (ObjectAddListManage.IsEditMode)
        {
            ClearSelection();
            DeviceEditUIManager manager = DeviceEditUIManager.Instacne;
            if(manager)
            {
                manager.Close();
                manager.HideMultiDev();
                manager.SetEmptValue();
            }
            EditorCamera.Instance.SetObjectVisibilityDirty();
        }
    }
    /// <summary>
    /// 清除设备选中
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
    /// 设置可选择的Layer
    /// </summary>
    /// <param name="layerList"></param>
    public void SetSelectionLayer(List<int> layerList)
    {
        EditorObjectSelection editorSelection = EditorObjectSelection.Instance;
        if (editorSelection)
        {
            foreach (var item in layerList)
            {
                int layerBits = editorSelection.ObjectSelectionSettings.SelectableLayers;
                bool isSet = LayerHelper.IsLayerBitSet(layerBits, item);
                if (!isSet)
                {
                    editorSelection.ObjectSelectionSettings.SelectableLayers = LayerHelper.SetLayerBit(layerBits, item);
                }
            }
        }
    }
    /// <summary>
    /// 取消可选择的Layer
    /// </summary>
    /// <param name="layerList"></param>
    public void CloseSelectionLayer(List<int> layerList)
    {
        EditorObjectSelection editorSelection = EditorObjectSelection.Instance;
        if (editorSelection)
        {
            foreach (var item in layerList)
            {
                int layerBits = editorSelection.ObjectSelectionSettings.SelectableLayers;
                bool isSet = LayerHelper.IsLayerBitSet(layerBits, item);
                if (isSet)
                {
                    editorSelection.ObjectSelectionSettings.SelectableLayers = LayerHelper.ClearLayerBit(layerBits, item);
                }
            }
        }
    }
    #region 开启和关闭设备编辑

    private string DepDeviceName = "DepDevice";
    private string RoomDeviceName = "RoomDevice";
    /// <summary>
    /// 开启设备编辑
    /// </summary>
    public void OpenDevEdit()
    {
        SetSelectionLayer(GetDevLayers());
    }
    /// <summary>
    /// 关闭设备编辑
    /// </summary>
    public void CloseDevEdit()
    {
        CloseSelectionLayer(GetDevLayers());
    }
    /// <summary>
    /// 获取设备所在layer
    /// </summary>
    /// <returns></returns>
    private List<int> GetDevLayers()
    {
        List<int> devLayers = new List<int>()
        {
            LayerMask.NameToLayer(DepDeviceName),
            LayerMask.NameToLayer(RoomDeviceName)
        };
        return devLayers;
    }
    #endregion
}
