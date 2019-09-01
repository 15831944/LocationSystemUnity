using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingImporter : MonoBehaviour {

    [Tooltip("上一个建筑物，用于参考摄像头角度")]
    public BuildingController lastController;

    [Tooltip("建筑父物体")]
    public DepNode ParentNode;

    [Tooltip("添加子物体的MeshCollider")]
    public List<GameObject> MeshColliderParent;
	// Use this for initialization
	void Start () {
		
	}
    [ContextMenu("AddMeshCollider")]
    public void AddMeshCollider()
    {
        if (MeshColliderParent == null || MeshColliderParent.Count == 0)
        {
            Debug.LogError("MeshList is null...");
            return;
        }
        else
        {
            foreach(var child in MeshColliderParent)
            {
                AddChildMesh(child);
            }
        }
    }
    [ContextMenu("SetBuildingScripts")]
    public void SetBuildingScripts()
    {
        BuildingController building = transform.GetComponent<BuildingController>();
        if(building.GetComponent<Collider>()==null)building.gameObject.AddCollider();
        building.depType = DepType.Building;
        building.NodeObject = building.gameObject;
        SetFocusAngleAndCollider(lastController,building);
        if(ParentNode != null)
        {
            ReplaceParentNode(ParentNode,building);
        }
        else
        {
            Debug.LogError(string.Format("{0} parentNode is null...", building.NodeName));
        }
        AddChildNode(building);
        AddBuildingTween(building);

        Debug.LogError("请检查FloorCube、StaticDev等...");
    }
    /// <summary>
    /// 添加动画
    /// </summary>
    /// <param name="building"></param>
    private void AddBuildingTween(DepNode building)
    {
        BuildingFloorTweening tween = building.gameObject.AddMissingComponent<BuildingFloorTweening>();
        if(building is BuildingController)
        {
            BuildingController bt = building as BuildingController;
            bt.FloorTween = tween;
        }
        tween.OffsetPerFloor = 5f;
        tween.TweenTime = 1f;
        if (tween.FloorList == null) tween.FloorList = new List<GameObject>();
        foreach(var floor in building.ChildNodes)
        {
            if (floor == building) continue;
            if (!tween.FloorList.Contains(floor.gameObject)) tween.FloorList.Add(floor.gameObject);
        }
        BuildingFloor topFloor = building.GetComponentInChildren<BuildingFloor>(false);
        if(topFloor!=null)
        {
            if(topFloor.GetComponent<Collider>()==null)
            {
                Collider collider = topFloor.gameObject.AddCollider();
                collider.enabled = false;
            }           
            if(!tween.FloorList.Contains(topFloor.gameObject))tween.FloorList.Add(topFloor.gameObject);
        }
    }
    /// <summary>
    /// 绑定父节点
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="building"></param>
    private void ReplaceParentNode(DepNode parentNode, DepNode building)
    {
        building.ParentNode = parentNode;
        if (parentNode.ChildNodes == null) parentNode.ChildNodes = new List<DepNode>();
        DepNode last = parentNode.ChildNodes.Find(i => i!=null&&i.NodeName == building.NodeName);
        if (last != null) parentNode.ChildNodes.Remove(last);
        parentNode.ChildNodes.Add(building);
        Debug.Log(string.Format("{0} parentNode is : {1}", building.NodeName, building.ParentNode.NodeName));
    }
    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="building"></param>
    private void AddChildNode(DepNode building)
    {
        if (building.ChildNodes == null) building.ChildNodes = new List<DepNode>();
        FloorController[] floorSum = building.transform.GetComponentsInChildren<FloorController>();
        if (floorSum != null && floorSum.Length != 0)
        {
            foreach(var floor in floorSum)
            {
                if (floor == building) continue;
                if(!building.ChildNodes.Contains(floor)) building.ChildNodes.Add(floor);
                floor.ParentNode = building;
                floor.depType = DepType.Floor;
                floor.NodeObject = floor.gameObject;
                if(floor.GetComponent<Collider>()==null)
                {
                    Collider collider = floor.gameObject.AddCollider();
                    collider.enabled = false;
                    collider.isTrigger = true;
                }   
                if(floor is FloorController)
                {
                    FloorController fController = floor as FloorController;
                    fController.CreateFloorCube();
                }
                SetFocusAngleAndCollider(lastController,floor);
            }
        }
        else
        {
            Debug.LogError("Child node is null...");
        }
    }
    /// <summary>
    /// 设置对焦角度
    /// </summary>
    /// <param name="lastDep"></param>
    /// <param name="dep"></param>
    private void SetFocusAngleAndCollider(DepNode lastDep,DepNode dep)
    {
        if(lastDep != null)
        {
            if(lastDep.NodeName==dep.NodeName)
            {
                if(lastDep is BuildingController)
                {
                    BuildingController building = lastDep as BuildingController;
                    BuildingController currentBuilding = dep as BuildingController;
                    currentBuilding.angleFocus = building.angleFocus;
                    currentBuilding.angleRange = building.angleRange;
                    currentBuilding.disRange = building.disRange;
                    currentBuilding.camDistance = building.camDistance;
                    currentBuilding.AreaSize = building.AreaSize;
                    CopyBoxColliderSize(lastDep,dep);
                }
                else if(lastDep is FloorController)
                {
                    FloorController building = lastDep as FloorController;
                    FloorController currentBuilding = dep as FloorController;
                    currentBuilding.angleFocus = building.angleFocus;
                    currentBuilding.angleRange = building.angleRange;
                    currentBuilding.disRange = building.disRange;
                    currentBuilding.camDistance = building.camDistance;
                    currentBuilding.AreaSize = building.AreaSize;
                    CopyBoxColliderSize(lastDep, dep);
                }
            }else if(lastDep.ChildNodes!=null)
            {
                foreach(var item in lastDep.ChildNodes)
                {
                    SetFocusAngleAndCollider(item,dep);
                }
            }
            else
            {
                Debug.LogError(dep.gameObject.name+" not find in lastDep...");
            }
        }
    }
    private void CopyBoxColliderSize(DepNode lastDep, DepNode dep)
    {
        BoxCollider collider = lastDep.gameObject.GetComponent<BoxCollider>();
        BoxCollider newCollider = dep.gameObject.GetComponent<BoxCollider>();
        if(collider!=null&&newCollider!=null)
        {
            newCollider.center = collider.center;
            newCollider.size = collider.size;
        }
    }
    /// <summary>
    /// 添加子物体meshCollider
    /// </summary>
    /// <param name="obj"></param>
    private void AddChildMesh(GameObject obj)
    {
        if (obj == null) return;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render!= null)
        {
            if (!render.enabled) render.enabled = true;
            MeshCollider collider = obj.AddMissingComponent<MeshCollider>();
            collider.enabled = true;
        }
        foreach(Transform child in obj.transform)
        {
            AddChildMesh(child.gameObject);
        }
    }
}
