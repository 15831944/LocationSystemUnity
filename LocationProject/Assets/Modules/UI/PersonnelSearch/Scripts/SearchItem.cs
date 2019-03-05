using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SearchItem : MonoBehaviour {

    public GameObject ui;
    GameObject currentUI;
    void Start () {
		
	}

    public void Update()
    {
        GameObject currentUI = EventSystem.current.currentSelectedGameObject;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ui == currentUI)
            {
                DataPaging.Instance.SetPerFindData_Click();
            }
                
        }
    }
}
