using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposeFollowTarget : MonoBehaviour {

    public bool isInit;
    private string gruopName;

    public GameObject followUI;
	// Use this for initialization
	void Start () {
		
	}

    //private void OnDisable()
    //{
    //    if (followUI != null) followUI.SetActive(false);
    //}
    //private void OnEnable()
    //{
    //    if (followUI != null) followUI.SetActive(true);
    //}

    public void SetInfo(string groupNameT,GameObject followUITemp=null)
    {
        followUI = followUITemp;
        gruopName = groupNameT;
        isInit = true;
    }

    private void OnDestroy()
    {
        if(isInit&&UGUIFollowManage.Instance)
        {
            UGUIFollowManage.Instance.RemoveUIbyTarget(gruopName, gameObject);
        }
    }
}
