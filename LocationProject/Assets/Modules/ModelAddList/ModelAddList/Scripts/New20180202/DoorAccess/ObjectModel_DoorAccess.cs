using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectModel_DoorAccess : MonoBehaviour {
    /// <summary>
    /// 门禁是否碰撞到门
    /// </summary>
    public bool IsAccessInDoor;
    /// <summary>
    /// 门
    /// </summary>
    public DoorAccessItem DoorItem;
	// Use this for initialization
	void Start () {
		
	}
    /// <summary>
    /// 创建门禁设备
    /// </summary>
    /// <returns></returns>
    public List<GameObject>CreateDoorAccess()
    {
        return DoorItem.CreateDoorAccess(gameObject);
    }

}
