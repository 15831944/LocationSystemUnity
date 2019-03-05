using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MapBuilding : MonoBehaviour {
    /// <summary>
    /// 建筑ID
    /// </summary>
    public string BuildingName;

    /// <summary>
    /// 建筑ID
    /// </summary>
    public int BuildingId;
    /// <summary>
    /// 对应区域
    /// </summary>
    public DepNode Node;
    /// <summary>
    /// 楼层列表
    /// </summary>
    public List<MapFloor> FloorList;
	// Use this for initialization
	void Start () {
		
	}
    public void InitFloor()
    {
        foreach(var item in Node.ChildNodes)
        {
            //Topologies.Find(Topo => Topo.Name == item.NodeName);
            MapFloor floor = FloorList.Find( Floor => Floor.FloorName==item.NodeName);
            if(floor!=null)
            {
                floor.FloorNode = item;
                floor.InitRoom();
            }           
        }
    }
    /// <summary>
    /// 当前选中的楼层
    /// </summary>
    private MapFloor CurrentFloor;
    /// <summary>
    /// 选中楼层
    /// </summary>
    /// <param name="Floor"></param>
    public void SelectFloor(DepNode Floor)
    {
        gameObject.SetActive(true);
        foreach(var item in FloorList)
        {
            if (item.FloorNode == null) continue;
            if(item.FloorNode.NodeID==Floor.NodeID)
            {
                CurrentFloor = item;
                item.ShowFloor();
                if(MapLoadManage.Instance)
                {
                    MapLoadManage.Instance.ShowMapPageSwitch(item,FloorList);
                }
            }
        }
    }
    /// <summary>
    /// 取消选中
    /// </summary>
    public void DisSelect()
    {
        gameObject.SetActive(false);
        CurrentFloor.HideFloor();
        CurrentFloor = null;
    }
}
