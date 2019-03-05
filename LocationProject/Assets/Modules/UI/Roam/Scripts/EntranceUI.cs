using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceUI : MonoBehaviour {

    public Camera Cam;
    public Canvas canvasT;
    public GameObject tagUi;
	void Start () {
       

    }
	
	// Update is called once per frame
	void Update () {
        EntranceFollowUI();
    }
    public void EntranceFollowUI()
    {
        Vector3 p= UGUIFollowTarget.WorldToUIWithIgnoreCanvasScaler(Cam ,canvasT ,tagUi .transform .position );
        this.transform.GetComponent<RectTransform>().localPosition = p;
    }
}
