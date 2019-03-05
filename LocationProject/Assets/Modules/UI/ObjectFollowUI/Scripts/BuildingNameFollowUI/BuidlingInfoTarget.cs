using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuidlingInfoTarget : MonoBehaviour {
    /// <summary>
    /// 建筑名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 建筑面积
    /// </summary>
    public string Area;
    /// <summary>
    /// 建筑高度
    /// </summary>
    public string Height;
    /// <summary>
    /// 建筑层数
    /// </summary>
    public string FloorNum;

	// Use this for initialization
	void Start () {
        CreateFollowUI();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 设置楼层信息
    /// </summary>
    /// <param name="buildingName">建筑名称</param>
    /// <param name="area">建筑面积</param>
    /// <param name="height">建筑高度</param>
    /// <param name="floorNum">建筑层数</param>
    public void InitInfo(string buildingName,string area,string height,string floorNum)
    {
        //Name = buildingName;
        //Area = area;
        //Height = height;
        //FloorNum = floorNum;
        //CreateFollowUI();
    }
    /// <summary>
    /// 创建建筑信息UI
    /// </summary>
    private void CreateFollowUI()
    {
        //if(FactoryDepManager.currentDep!=null||FactoryDepManager.currentDep as DepController)
        //{
        //    Debug.Log("FollowUI is exist:"+Name);
        //    return;
        //}
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, Vector3.zero);
        FollowTargetManage controller = FollowTargetManage.Instance;
        if(controller != null&&controller.BuildingNameUIPrefab!=null)
        {
            if(UGUIFollowManage.Instance==null)
            {
                Debug.LogError("UGUIFollowManage.Instance==null");
                return;
            }
            Camera mainCamera = GetMainCamera();
            if (mainCamera == null) return;
            GameObject name = UGUIFollowManage.Instance.CreateItem(controller.BuildingNameUIPrefab, targetTagObj, controller.BuildingListName,null,false,true);
            BuildingFollowUI followUI = name.GetComponentInChildren<BuildingFollowUI>(false);
            if(followUI)
            {
                followUI.SetUIInfo(Name, Area, Height, FloorNum);               
            }
            //Transform Parent = name.transform.parent;
            ////创建完成后，整体隐藏
            //if (Parent!=null)
            //{
            //    if (Parent.gameObject.activeInHierarchy) Parent.gameObject.SetActive(false);
            //}           
            UGUIFollowManage.Instance.SetGroupUIbyName(controller.BuildingListName, false);
        }       
    }
    /// <summary>
    /// 获取主相机
    /// </summary>
    /// <returns></returns>
    private Camera GetMainCamera()
    {
        CameraSceneManager manager = CameraSceneManager.Instance;
        if(manager&&manager.MainCamera!=null)
        {
            return manager.MainCamera;
        }
        else
        {
            Debug.LogError("CameraSceneManager.MainCamera is null!");
            return null;
        }
    }
    private bool IsNameExist(string name)
    {

        return true;
    }
}
