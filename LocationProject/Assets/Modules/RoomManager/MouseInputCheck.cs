using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputCheck : MonoBehaviour {
    /// <summary>
    /// 建筑是否展开
    /// </summary>
    private bool IsBuildingOpen;
	// Use this for initialization
	void Start () {
        SceneEvents.BuildingOpenCompleteAction += OnBuildingOpen;
        SceneEvents.BuildingStartCloseAction += OnBuildingClose;
    }
	
	// Update is called once per frame
	void Update () {
        CloseBuildingCheck();
    }
    /// <summary>
    /// 建筑展开
    /// </summary>
    private void OnBuildingOpen()
    {
        //Debug.LogError("BuildingOpen");
        IsBuildingOpen = true;
    }
    /// <summary>
    /// 建筑关闭
    /// </summary>
    private void OnBuildingClose()
    {
        //Debug.LogError("BuildingClose");
        IsBuildingOpen = false;
    }
    /// <summary>
    /// 检测是否关闭建筑
    /// </summary>
    private void CloseBuildingCheck()
    {
        if (!IsBuildingOpen) return;
        if (BuildingController.isTweening||IsClickUGUIorNGUI.Instance.isOverUI)return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitInfo=Physics.RaycastAll(ray, float.MaxValue);
            bool value = false;
            for(int i=0;i<hitInfo.Length;i++)
            {
                FloorController floor = hitInfo[i].transform.GetComponent<FloorController>();
                BuildingFloor topFloor = hitInfo[i].transform.GetComponent<BuildingFloor>();
                if (floor!=null||topFloor!=null)
                {
                    value = true;
                    break;
                }
            }
            if(!value)
            {
                if (FactoryDepManager.currentDep is BuildingController)
                {
                    BuildingController controller = FactoryDepManager.currentDep as BuildingController;
                    controller.CloseFloor();
                }
            }
        }
    }
}
