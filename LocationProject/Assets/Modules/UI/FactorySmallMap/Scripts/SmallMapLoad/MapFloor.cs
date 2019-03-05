using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapFloor : MonoBehaviour {
    /// <summary>
    /// 当前楼层数
    /// </summary>
    public int FloorNum;
    /// <summary>
    /// 楼层ID
    /// </summary>
    public string FloorName;
    /// <summary>
    /// 楼层对应节点
    /// </summary>
    public DepNode FloorNode;
    /// <summary>
    /// 房间列表
    /// </summary>
    public List<MapRoom> RoomList;
	// Use this for initialization
	void Start () {
        SetRoomInfo();
        
    }
   
    /// <summary>
    /// 设置机房信息
    /// </summary>
    private void SetRoomInfo()
    {
        foreach(var room in RoomList)
        {
            room.SetParentInfo(this);
        }
    }
	/// <summary>
    /// 取消另一个机房的选中
    /// </summary>
    /// <param name="roomName"></param>
    public void DisSelectLastRoom(string roomName)
    {
        foreach(var room in RoomList)
        {
            if(room.RoomName!= roomName&&room.IsSelect)
            {
                room.Deselect();
            }
        }
    }
    /// <summary>
    /// 关闭楼层UI
    /// </summary>
    public void ShowFloor()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 显示楼层UI
    /// </summary>
    public void HideFloor()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 初始化房间信息
    /// </summary>
    public void InitRoom()
    {
        foreach (var item in FloorNode.ChildNodes)
        {
            //Topologies.Find(Topo => Topo.Name == item.NodeName);
            MapRoom room = RoomList.Find(roomT => roomT.RoomName == item.NodeName);
            if (room != null)
            {
                room.RoomNode = item;
            }
        }
    }

    /// <summary>
    /// 添加楼层下房间
    /// </summary>
    [ContextMenu("AddChild")]
    public void AddChildRoom()
    {
        foreach(Transform child in transform)
        {
            MapRoom room = child.GetComponent<MapRoom>();
            if(room!=null)
            {
                if (RoomList == null) RoomList = new List<MapRoom>();
                if(!RoomList.Contains(room))RoomList.Add(room);
            }
        }
    }

}
