using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
public class BuildingFloor : MonoBehaviour
{
    /// <summary>
    /// 建筑管理部分
    /// </summary>
    private BuildingController controller;
    /// <summary>
    /// 是否不需要操作的顶层
    /// </summary>
    private bool isTopFloor=true;
    void Start()
    {
        if(transform.GetComponent<FloorController>()!=null)
        {
            isTopFloor = false;
        }
        //DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        //trigger.onClick = OnClick;
        //controller = transform.GetComponentInParent<BuildingController>();
    }
    private void OnClick()
    {
        //if (!isTopFloor||IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) return;
        //if (controller&&!BuildingController.isTweening)
        //    controller.CloseFloor(false);
    }
}
