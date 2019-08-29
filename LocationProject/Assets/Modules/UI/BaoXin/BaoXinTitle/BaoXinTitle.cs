using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaoXinTitle : MonoBehaviour {
    public static BaoXinTitle Instance;
    public GameObject Title;
	// Use this for initialization
	void Awake () {
        Instance = this;
	}
	
    public void Show()
    {
        if (Title != null) Title.SetActive(true);
    }

    public void Close()
    {
        if (Title != null) Title.SetActive(false);
    }
	
}
