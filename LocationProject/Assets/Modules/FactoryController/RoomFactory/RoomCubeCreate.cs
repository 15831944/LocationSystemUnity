using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class RoomCubeCreate : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SceneEvents.OnDepCreateComplete += OnDepCreate;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDepCreate(DepNode dep)
    {
        if (CreateDeps.Contains(dep)) return;
        CreateDeps.Add(dep);
        GameObject roomDevContainer = null;
        if (dep as RoomController)
        {
            RoomController room = dep as RoomController;
            roomDevContainer = room.RoomDevContainer;
            List<PhysicalTopology> blocks = TryGetRoomBlocks(room);
            //if (blocks != null)
            //{
            //    Debug.Log("Blcok Count:" + blocks);
            //    StartCoroutine(CreateBlocks(blocks, roomDevContainer));
            //}

            CommunicationObject.Instance.GetPointsByPid(room.NodeID, (points) =>
             {
                 if (points != null)
                 {
                     Debug.Log("Blcok Count:" + points);
                     StartCoroutine(CreateBlocks(points, roomDevContainer));
                 }
             });

        }
        else if(dep as FloorController)
        {
            FloorController floor = dep as FloorController;
            roomDevContainer = floor.RoomDevContainer;
            //List<PhysicalTopology> blocks = TryGetRoomBlocks(floor);
            //if (blocks != null)
            //{
            //    Debug.Log("Blcok Count:" + blocks);
            //    StartCoroutine(CreateBlocks(blocks,roomDevContainer));
            //}
            CommunicationObject.Instance.GetPointsByPid(floor.NodeID, (points) =>
            {
                if (points != null)
                {
                    Debug.Log("Blcok Count:" + points);
                    StartCoroutine(CreateBlocks(points, roomDevContainer));
                }
            });
        }
    }
    private string blockName = "Block";
    private float blockHeight = 4f;

    private List<DepNode> CreateDeps = new List<DepNode>();//已经创建柱子的区域
    /// <summary>
    /// 获取区域下的柱子节点
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private List<PhysicalTopology> TryGetRoomBlocks(DepNode dep)
    {
        List<PhysicalTopology> blockList = new List<PhysicalTopology>();
        TopoTreeManager manager = TopoTreeManager.Instance;
        if(manager)
        {
            TreeNode<TreeViewItem> node = manager.TryGetAreaNode(dep.NodeID);
            if(node!=null)
            {
                PhysicalTopology depPhysic = node.Item.Tag as PhysicalTopology;
                if(depPhysic!=null)
                {
                    foreach (var item in depPhysic.Children)
                    {
                        if (item.Name == blockName)
                        {
                            blockList.Add(item);
                        }
                    }
                }               
            }
        }         
        return blockList.Count == 0 ? null : blockList; ;
    }

    private IEnumerator CreateBlocks(AreaPoints[] blockList, GameObject devContainer)
    {
        foreach (var block in blockList)
        {
            var points = block.Points;
            if (points.Length == 4)
            {
                Vector3 pos = Vector3.zero;
                float width = 0;
                float length = 0;
                GetCADBlockSize(points, ref pos, ref width, ref length);
                Debug.Log(string.Format("Pos:{0} width:{1} length:{2}", pos, width, length));

                GameObject blockObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blockObj.transform.parent = devContainer.transform;
                blockObj.transform.localScale = new Vector3(width, blockHeight, length);
                blockObj.transform.localPosition = pos;
                blockObj.transform.localEulerAngles = Vector3.zero;
            }
            yield return null;
        }
        Debug.LogError("Block Create Complete,Block Count:" + blockList.Length);
    }

    //private IEnumerator CreateBlocks(List<PhysicalTopology> blockList,GameObject devContainer)
    //{
    //    foreach(var block in blockList)
    //    {
    //        Bound initBound = block.InitBound;
    //        if (initBound != null && initBound.Points != null)
    //        {
    //            Point[] points = initBound.Points;
    //            if (points.Length == 4)
    //            {
    //                Vector3 pos = Vector3.zero;
    //                float width = 0;
    //                float length = 0;
    //                GetCADBlockSize(points, ref pos, ref width, ref length);
    //                Debug.Log(string.Format("Pos:{0} width:{1} length:{2}", pos, width, length));

    //                GameObject blockObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //                blockObj.transform.parent = devContainer.transform;
    //                blockObj.transform.localScale = new Vector3(width,blockHeight,length);
    //                blockObj.transform.localPosition = pos;
    //                blockObj.transform.localEulerAngles = Vector3.zero;
    //            }
    //        }
    //        yield return null;
    //    }
    //    Debug.LogError("Block Create Complete,Block Count:"+blockList.Count);  
    //}
    private void GetCADBlockSize(Point1[] pointGroup,ref Vector3 pos,ref float width,ref float length)
    {
        Vector3 locationScale = LocationManager.Instance.LocationOffsetScale; 

        float maxX=float.MinValue;
        float maxY = float.MinValue;
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        foreach(var point in pointGroup)
        {
            if (point.X < minX) minX = point.X;
            else if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            else if (point.Y > maxY) maxY = point.Y;
        }
        width = (maxX - minX);
        length = (maxY - minY);
        pos = new Vector3(minX+width/2,0,minY+length/2);
        width /= locationScale.x;
        length /= locationScale.z;
        pos = LocationManager.CadToUnityPos(pos,true);
    }
}
