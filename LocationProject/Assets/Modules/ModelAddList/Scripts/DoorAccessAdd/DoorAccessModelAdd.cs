using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccessModelAdd : MonoBehaviour {
    /// <summary>
    /// 门集合的名称
    /// </summary>
    private string DoorContainerName = "Doors";
    /// <summary>
    /// 卷闸门的宽度>1,超过1判定为卷闸门  受Scale影响，后续得修改
    /// </summary>
    private float RollingDoor = 1f;
    /// <summary>
    /// 是否初始化
    /// </summary>
    private bool IsInit;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    #region AddDoorAccessItem

    /// <summary>
    /// 添加门的控制脚本
    /// </summary>
    [ContextMenu("AddDoorAccessItem")]
    public void AddDoorAccessManage()
    {
        if (IsInit) return;
        IsInit = true;
        StartCoroutine(FindDoorListInChild(transform));
    }
    IEnumerator FindDoorListInChild(Transform childTransform)
    {
        foreach (Transform child in childTransform)
        {
            if (child.name.Contains(DoorContainerName))
            {
                //Debug.Log(child.name);
                AddChildDoorManage(child);
            }
            else
            {
                StartCoroutine(FindDoorListInChild(child));
            }
            yield return null;
        }
    }
    /// <summary>
    /// 添加门控制脚本
    /// </summary>
    private void AddChildDoorManage(Transform DoorContainer)
    {
        DepDoors depDoors = DoorContainer.gameObject.AddMissingComponent<DepDoors>();
        DepNode node = DoorContainer.GetComponentInParent<DepNode>();
        node.InitDoor(depDoors);
        foreach (Transform child in DoorContainer)
        {
            if (child.childCount == 0)
            {
                DoorAccessItem item = child.gameObject.AddMissingComponent<DoorAccessItem>();
                if (IsNormalDoor(false, child))
                {                  
                    GameObject leftDoor = child.gameObject;
                    item.Init(true, leftDoor, null);
                    depDoors.DoorList.Add(item);
                }
                else
                {
                    item.InitRollingDoor();
                    depDoors.DoorList.Add(item);
                }
            }
            else if (child.childCount == 2)
            {
                DoorAccessItem item = child.gameObject.AddMissingComponent<DoorAccessItem>();
                GameObject leftDoor = child.GetChild(0).gameObject;
                GameObject rightDoor = child.GetChild(1).gameObject;
                item.Init(false, leftDoor, rightDoor);
                depDoors.DoorList.Add(item);
            }
        }
    }
    /// <summary>
    /// 是否单双门(剔除铁皮门)
    /// </summary>
    /// <param name="isDoubleDoor"></param>
    /// <param name="doorTransform"></param>
    /// <returns></returns>
    private bool IsNormalDoor(bool isDoubleDoor, Transform doorTransform)
    {
        MeshRenderer renderT;
        if (isDoubleDoor)
        {
            renderT = doorTransform.GetChild(0).GetComponent<MeshRenderer>();

        }
        else
        {
            renderT = doorTransform.GetComponent<MeshRenderer>();
        }
        if (renderT == null)
        {
            Debug.Log(string.Format("{0} MeshRender is null", doorTransform.name));
            return false;
        }
        Vector3 boundSize = renderT.bounds.size;
        if (boundSize.x > RollingDoor || boundSize.z > RollingDoor)
        {
            //Debug.Log("Is rolling door " + doorTransform.name);
            return false;
        }
        return true;
    }
    #endregion
}
