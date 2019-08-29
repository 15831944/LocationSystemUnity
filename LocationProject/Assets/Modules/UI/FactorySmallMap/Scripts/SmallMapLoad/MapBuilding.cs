using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    public void SetNode(DepNode node)
    {
        Node = node;
        if (node != null)
        {
            BuildingId = node.NodeID;
        }
        else
        {
            Debug.LogError(string.Format("Building not find! Id:{0},Name:{1}", BuildingId,
                       BuildingName));
        }
    }

    /// <summary>
    /// 楼层列表
    /// </summary>
    public List<MapFloor> FloorList;
	// Use this for initialization
	void Start () {
		
	}
    public void InitFloor()
    {
        if (Node == null)
        {
            Debug.LogError("MapBuilding.InitFloor Node == null");
            return;
        }
        if (Node.ChildNodes == null)
        {
            Node.RefreshChildrenNodes();
        }
        if (Node.ChildNodes != null)
            foreach (var item in Node.ChildNodes)
            {
                if (item == null) continue;
                //Topologies.Find(Topo => Topo.Name == item.NodeName);
                MapFloor floor = FloorList.Find(Floor => Floor.FloorName == item.NodeName);
                if (floor != null)
                {
                    floor.FloorNode = item;
                    floor.InitRoom();
                }
                else
                {
                    Debug.LogError("InitFloor floor == null : " + item.NodeName);
                }
            }
    }

    internal void ReplaceDepNode(DepNode depNode)
    {
        if (depNode == null)
        {
            Debug.LogError("MapBuilding.ReplaceDepNode depNode == null");
            return;
        }
        //if (Node == null)
        //{
        //    Debug.LogError("MapBuilding.ReplaceDepNode Node == null : BuildingName :" + BuildingName);
        //    //return;
        //}
        if (BuildingId == depNode.NodeID)
        {
            //Node = depNode;
            SetNode(depNode);

            foreach (var item in FloorList)
            {
                item.ReplaceDepNode(depNode.ChildNodes);
            }
            if (CurrentFloor)
            {
                CurrentFloor.ReplaceDepNode(depNode.ChildNodes);
            }
        }
        else
        {
            Debug.LogWarning(string.Format("MapBuilding.ReplaceDepNode BuildingId != depNode.NodeID Id1={0},Name1={1};ID2={2},Name2={3}", BuildingId, BuildingName, depNode.NodeID, depNode.NodeName));
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
