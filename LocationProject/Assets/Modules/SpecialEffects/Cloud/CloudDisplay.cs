using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDisplay : MonoBehaviour {
    /// <summary>
    /// 云
    /// </summary>
    public GameObject Cloud;
	// Use this for initialization
	void Start () {
        SceneEvents.FullViewStateChange += OnFullViewChange;
	}
	

    private void OnFullViewChange(bool isOn)
    {
        if (Cloud == null) return;
        Cloud.SetActive(!isOn);
    }
}
