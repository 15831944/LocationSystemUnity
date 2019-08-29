using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceUI : MonoBehaviour {

    public Camera Cam;
    public Canvas canvasT;
    public GameObject tagUi;
	void Start () {
        if (Cam == null)
        {
            Cam = Camera.main;
        }

    }
    bool isUseCanvasScalerT = true;
    int layerint = -1;
    bool changeChildActive = false;
    bool IsRayCheckCollision = false;
    // Update is called once per frame
    void Update () {
        EntranceFollowUI();
    }
    public void EntranceFollowUI()
    {
        // Vector3 p= UGUIFollowTarget.WorldToUIWithIgnoreCanvasScaler(Cam ,canvasT ,tagUi .transform .position );
        // this.transform.GetComponent<RectTransform>().localPosition = p;
        UGUIFollowTarget followTarget = UGUIFollowTarget.AddUGUIFollowTarget(this.gameObject  , tagUi, Cam, isUseCanvasScalerT, layerint);
        followTarget.SetTargetChildActive = changeChildActive;
        //followTarget.IsRayCheckCollision = IsRayCheckCollision;
        followTarget.SetIsRayCheckCollision(IsRayCheckCollision);
    }
}
