using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class RoamBuildingCollider : MonoBehaviour {

    public string BuildingName;

    private BuildingController building;
	// Use this for initialization
	void Start () {
		
	}
    private void InitBuilding()
    {
        if (building != null) return;
        DepNode depT = RoomFactory.Instance.GetDepNode(BuildingName);
        if(depT is BuildingController)
        {
            building = depT as BuildingController;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        FirstPersonController person = other.gameObject.GetComponent<FirstPersonController>();
        if (person != null)
        {
            InitBuilding();
            if(building!=null)
            {
                building.ShowBuildingDev(false);
            }
        }
    }
}
