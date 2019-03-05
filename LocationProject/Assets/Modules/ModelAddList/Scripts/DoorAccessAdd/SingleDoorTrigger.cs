using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SingleDoorTrigger : MonoBehaviour {

    private DoorAccessItem DoorItem;

    public void InitDoorItem(DoorAccessItem door)
    {
        DoorItem = door;
    }
    void OnTriggerEnter(Collider other)
    {
        FirstPersonController person = other.gameObject.GetComponent<FirstPersonController>();
        if (person != null)
        {
            if(DoorItem)DoorItem.OnFPSEnter(person.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        FirstPersonController person = other.gameObject.GetComponent<FirstPersonController>();
        if (person != null)
        {
            if (DoorItem) DoorItem.OnFPSExit(person.gameObject);
        }
    }
}
