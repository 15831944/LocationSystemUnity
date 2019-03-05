using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTopCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    /// <summary>
    /// 漫游人物退出建筑
    /// </summary>
    public void PersonExit()
    {
        DevSubsystemManage fpsManager = DevSubsystemManage.Instance;
        if (fpsManager.IsFPSInBuilding()) RoamManage.Instance.EntranceIndoor(false);  
    }
    public void PersonEnter()
    {
        DevSubsystemManage fpsManager = DevSubsystemManage.Instance;
        if (fpsManager.IsFPSInBuilding()) RoamManage.Instance.EntranceIndoor(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<CharacterController>() != null)
        {
            BuildingTopColliderManage manager = BuildingTopColliderManage.Instance;
            if(manager&&!manager.IsColliderExist(this))
            {
                manager.TopColliders.Add(this);
                BuildingTopColliderManage.IsInBuildingRoof = true;
                PersonExit();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<CharacterController>() != null)
        {
            BuildingTopColliderManage manager = BuildingTopColliderManage.Instance;
            if (manager)
            {
                if (manager.IsColliderExist(this)) manager.TopColliders.Remove(this);
                if (manager.TopColliders.Count == 0) BuildingTopColliderManage.IsInBuildingRoof = false;
                PersonEnter();
            }
            
        }
    }
}
