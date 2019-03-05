using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoadManage : MonoBehaviour {
    /// <summary>
    /// 地图加载管理
    /// </summary>
    public static MapLoadManage Instance;
    /// <summary>
    /// 切页管理
    /// </summary>
    public MapSwitch SwitchManager;
    /// <summary>
    /// 建筑列表
    /// </summary>
    public List<MapBuilding> BuildingList;
    /// <summary>
    /// 当前选中大楼
    /// </summary>
    private MapBuilding CurrentBuilding;
	// Use this for initialization
	void Awake () {
        Instance = this;
        //InitBuildingId();
    }

    void Start()
    {
        InitBuildingId();
    }
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 绑定建筑ID
    /// </summary>
    private void InitBuildingId()
    {
        Log.Info("InitBuildingId");
        RoomFactory factory = RoomFactory.Instance;
        if (factory)
        {
            foreach (var item in BuildingList)
            {
                DepNode node = factory.GetDepNode(item.BuildingName);
                if (node == null)
                {
                    Debug.LogError(string.Format("Building not find! Id:{0},Name:{1}", item.BuildingId,
                        item.BuildingName));
                }
                else
                {
                    item.Node = node;
                    item.InitFloor();
                }
            }
        }
    }
    /// <summary>
    /// 显示小地图
    /// </summary>
    /// <param name="node"></param>
    public bool ShowBuildingMap(DepNode node)
    {
        if (node.ParentNode == null)
        {
            return false;
        }
        bool flag=false;
        DisSelectLast();
        foreach (var item in BuildingList)
        {
            if (item.Node == null) continue;
            if(item.Node.NodeName==node.ParentNode.NodeName)
            {
                CurrentBuilding = item;
                item.SelectFloor(node);
                flag = true;
            }
        }
        return flag;
    }
    /// <summary>
    /// 取消上一个的选中
    /// </summary>
    public void DisSelectLast()
    {
        if(CurrentBuilding!=null)
        {
            SwitchManager.Hide();
            CurrentBuilding.DisSelect();
            CurrentBuilding = null;
        }
    }
    /// <summary>
    /// 显示楼层切页
    /// </summary>
    public void ShowMapPageSwitch(MapFloor currentFloor, List<MapFloor> FloorList)
    {
        SwitchManager.ShowMapSwitch(currentFloor,FloorList);
    }
}
