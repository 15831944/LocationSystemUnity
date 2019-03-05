using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MapRoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    /// <summary>
    /// 房间节点
    /// </summary>
    [HideInInspector]
    public DepNode RoomNode;
    /// <summary>
    /// 房间名称
    /// </summary>
    public string RoomName;
    /// <summary>
    /// 房间图片
    /// </summary>
    public Image roomImage;
    /// <summary>
    /// 是否选中
    /// </summary>
    [HideInInspector]
    public bool IsSelect;
    /// <summary>
    /// 当前所在楼层
    /// </summary>
    private MapFloor Floor;
    // Use this for initialization
    void Start()
    {
        SceneEvents.DepNodeChanged += OnDepNodeChange;
    }
    private void OnDepNodeChange(DepNode last, DepNode current)
    {
        if (RoomNode == null) return;
        if(current.NodeID==RoomNode.NodeID)
        {
            Select();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowToolTip(true);
        SetImageAlpha(1);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        DeselectOtherRoom();
        Select();
        OpenRoom();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ShowToolTip(false);
        if(!IsSelect)
        {
            SetImageAlpha(0.3f);
        }
    }
    /// <summary>
    /// 设置父物体信息
    /// </summary>
    /// <param name="mapFloor"></param>
    public void SetParentInfo(MapFloor mapFloor)
    {
        Floor = mapFloor;
    }
    /// <summary>
    /// 取消其他房间选中
    /// </summary>
    private void DeselectOtherRoom()
    {
        if(Floor)
        {
            Floor.DisSelectLastRoom(RoomName);
        }
    }
    /// <summary>
    /// 打开机房
    /// </summary>
    private void OpenRoom()
    {
        RoomFactory factory = RoomFactory.Instance;
        if(factory&&RoomNode!=null)
        {
            factory.FocusNode(RoomNode);
        }
    }
    /// <summary>
    /// 设置图片颜色
    /// </summary>
    /// <param name="alpha"></param>
    private void SetImageAlpha(float alpha)
    {
        if (roomImage == null) return;
        Color tempColor = roomImage.color;
        tempColor.a = alpha;
        roomImage.color = tempColor;
    }
    /// <summary>
    /// 是否显示ToolTip
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowToolTip(bool isShow)
    {
        if (string.IsNullOrEmpty(RoomName)) return;
        if (isShow)
        {
            if (UGUITooltip.Instance)
                UGUITooltip.Instance.ShowTooltip(RoomName);
        }
        else
        {
            if (UGUITooltip.Instance)
                UGUITooltip.Instance.Hide();
        }
    }
    /// <summary>
    /// 选中当前图片
    /// </summary>
    public void Select()
    {
        IsSelect = true;
        SetImageAlpha(1f);
    }
    /// <summary>
    /// 取消图片选中
    /// </summary>
    public void Deselect()
    {
        IsSelect = false;
        SetImageAlpha(0.3f);
    }
    void OnDisable()
    {
        Deselect();
    }
    void OnDestroy()
    {
        SceneEvents.DepNodeChanged -= OnDepNodeChange;
    }
}
