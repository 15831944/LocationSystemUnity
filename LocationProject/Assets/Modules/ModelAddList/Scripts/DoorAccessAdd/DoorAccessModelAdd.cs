using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccessModelAdd : MonoBehaviour {
    /// <summary>
    /// 门集合的名称
    /// </summary>
    private static string DoorContainerName = "Doors";
    /// <summary>
    /// 卷闸门的宽度>1,超过1判定为卷闸门  受Scale影响，后续得修改
    /// </summary>
    private static float RollingDoor = 1f;
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
        //StartCoroutine(FindDoorListInChildCoroutine(transform));

        FindDoorListInChild(transform);
    }
    IEnumerator FindDoorListInChildCoroutine(Transform childTransform)
    {
        foreach (Transform child in childTransform)
        {
            if (child.name.Contains(DoorContainerName))
            {
                //Debug.Log(child.name);
                DepDoors depDoors=AddChildDoorManage(child);
                foreach (Transform child2 in childTransform)//主厂房的墙壁下面也有门
                {
                    if (child2.name.Contains("_Wall"))
                    {
                        AddWallDoors(depDoors,child2);
                    }
                }
            }
            else
            {
                StartCoroutine(FindDoorListInChildCoroutine(child));
            }
            yield return null;
        }
    }

    void FindDoorListInChild(Transform childTransform)
    {
        foreach (Transform child in childTransform)
        {
            if (child.name.Contains(DoorContainerName))
            {
                //Debug.Log(child.name);
                DepDoors depDoors = AddChildDoorManage(child);
                foreach (Transform child2 in childTransform)//主厂房的墙壁下面也有门
                {
                    if (child2.name.Contains("_Wall"))
                    {
                        AddWallDoors(depDoors, child2);
                    }
                }
            }
            else
            {
                FindDoorListInChild(child);
            }
        }
    }

    public static void InitDoorControl(Transform childTransform)
    {
        foreach (Transform child in childTransform)
        {
            if (child.name.Contains(DoorContainerName))
            {
                DepDoors depDoors = AddChildDoorManage(child);

                foreach (Transform child2 in childTransform)//主厂房的墙壁下面也有门
                {
                    if (child2.name.Contains("_Wall"))
                    {
                        AddWallDoors(depDoors, child2);
                    }
                }
            }
            else
            {
                InitDoorControl(child);
            }
        }
    }


    private static DoorAccessItem GetDoorAccessItem(Transform child)
    {
        if (!child.name.ToLower().Contains("door"))
        {
            return null;
        }
        DoorAccessItem item = null;
        if (child.childCount == 0)
        {
            item = child.gameObject.AddMissingComponent<DoorAccessItem>();
            if (IsNormalDoor(false, child))
            {
                GameObject leftDoor = child.gameObject;
                item.Init(true, leftDoor, null);
            }
            else
            {
                item.InitRollingDoor();
            }
        }
        else if (child.childCount == 2)
        {
            item = child.gameObject.AddMissingComponent<DoorAccessItem>();
            GameObject leftDoor = child.GetChild(0).gameObject;
            GameObject rightDoor = child.GetChild(1).gameObject;
            item.Init(false, leftDoor, rightDoor);
        }

        var colliders = child.FindComponentsInChildren<MeshCollider>();
        foreach (MeshCollider collider in colliders)
        {
            //collider.enabled = false;//防止妨碍进入
            //GameObject.Destroy(collider);//防止妨碍进入
            collider.convex = true;
            collider.isTrigger = true;
        }
        return item;
    }

    /// <summary>
    /// 添加门控制脚本
    /// </summary>
    private static DepDoors AddChildDoorManage(Transform DoorContainer)
    {
        DepDoors depDoors = DoorContainer.gameObject.AddMissingComponent<DepDoors>();
        DepNode node = DoorContainer.GetComponentInParent<DepNode>();
        node.InitDoor(depDoors);
        foreach (Transform child in DoorContainer)
        {
            DoorAccessItem item = GetDoorAccessItem(child);
            if (item != null)
            {
                depDoors.DoorList.Add(item);
            }
        }
        return depDoors;
    }

    /// <summary>
    /// 添加门控制脚本
    /// </summary>
    private static DepDoors AddWallDoors(DepDoors depDoors, Transform DoorContainer)
    {
        foreach (Transform child in DoorContainer)
        {
            DoorAccessItem item = GetDoorAccessItem(child);
            if (item != null)
            {
                depDoors.DoorList.Add(item);
            }
        }
        return depDoors;
    }
    /// <summary>
    /// 是否单双门(剔除铁皮门)
    /// </summary>
    /// <param name="isDoubleDoor"></param>
    /// <param name="doorTransform"></param>
    /// <returns></returns>
    private static bool IsNormalDoor(bool isDoubleDoor, Transform doorTransform)
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
