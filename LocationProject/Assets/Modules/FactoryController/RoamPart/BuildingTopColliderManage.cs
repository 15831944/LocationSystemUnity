using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTopColliderManage : MonoBehaviour {

    public static BuildingTopColliderManage Instance;

    public List<BuildingTopCollider> TopColliders=new List<BuildingTopCollider>();

    public static bool IsInBuildingRoof;
    // Use this for initialization
    void Start () {
        Instance = this;
	}
    /// <summary>
    /// 漫游人物，是否在楼顶
    /// </summary>
    /// <returns></returns>
	public bool IsFPSInTopFloor()
    {
        if (TopColliders.Count == 0) return false;
        else return true;
    }
    /// <summary>
    /// Collider是否已经添加
    /// </summary>
    /// <param name="topCollider"></param>
    /// <returns></returns>
    public bool IsColliderExist(BuildingTopCollider topCollider)
    {
        if (TopColliders.Contains(topCollider)) return true;
        else return false;
    }
    /// <summary>
    /// 退出漫游时，清除信息
    /// </summary>
    public void Clear()
    {
        TopColliders.Clear();
        IsInBuildingRoof = false;
    }

}
