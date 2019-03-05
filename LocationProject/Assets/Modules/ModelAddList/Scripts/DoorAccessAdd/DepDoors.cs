using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepDoors : MonoBehaviour {
    /// <summary>
    /// 区域下门的集合
    /// </summary>
    public List<DoorAccessItem> DoorList=new List<DoorAccessItem>();
    /// <summary>
    /// 门所在区域
    /// </summary>
    public DepNode DoorDep;
	// Use this for initialization
	void Start () {
		
	}
	/// <summary>
    /// 设置门的高亮材质
    /// </summary>
	public void SetDoorsRenderEnable(bool isOn)
    {
        foreach(var door in DoorList)
        {
            door.SetRendererEnable(isOn);
        }
    }
    /// <summary>
    /// 根据区域下门的ID,获取门
    /// </summary>
    /// <param name="doorId"></param>
    /// <returns></returns>
    public DoorAccessItem GetDoorItem(string doorId)
    {
        DoorAccessItem door = DoorList.Find(i=>i.DoorID==doorId);
        return door;
    }
}
