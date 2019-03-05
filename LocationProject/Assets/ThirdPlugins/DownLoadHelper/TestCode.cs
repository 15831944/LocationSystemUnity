using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCode : MonoBehaviour {

    public Button GetPathBtn;
	// Use this for initialization
	void Start () {
        GetPathBtn.onClick.AddListener(OnButtonClick);

    }
	
	private void OnButtonClick()
    {
        string path = OpenDialogFile.GetDownLoadPath();
        Debug.Log(path);
    }
}
