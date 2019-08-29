using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;
using System;
using HighlightingSystem;

public class DepNode : MonoBehaviour
{
    public string Tag = "";

    public string NodeKKS;
    public string NodeName;
    /// <summary>
    /// 对应区域ID
    /// </summary>
    public int NodeID;

    [System.NonSerialized]  private PhysicalTopology _topoNode;
    public PhysicalTopology TopoNode
    {
        get { return _topoNode; }
        //set
        //{
        //    SetTopoNode(value);
        //}
    }

    public T GetParentNode<T>() where T : DepNode
    {
        if(this is T)
        {
            return (T)this;
        }
        if (ParentNode != null)
        {
            if (ParentNode is T)
            {
                return (T)ParentNode;
            }
            else
            {
                return ParentNode.GetParentNode<T>();
            }
        }
        return null;
    }

    /// <summary>
    /// 找到相关的节点（ID相同的）
    /// </summary>
    /// <param name="focusNode"></param>
    /// <returns></returns>
    public DepNode FindRelationNode(DepNode node)
    {
        if (node == null)
        {
            //Debug.LogError("DepNode.FindRelationNode node==null ");
            return this;//漫游进入建筑，没有focusNode
        }
        DepNode rNode = null;
        if (this.NodeID == node.NodeID)
        {
            rNode = this;
        }
        else
        {
            if(ChildNodes!=null)
                foreach (DepNode child in ChildNodes)
                {
                    rNode = child.FindRelationNode(node);
                    if (rNode != null)
                    {
                        return rNode;
                    }
                }
        }
        return rNode;
    }

    public void SetTopoNode(PhysicalTopology node)
    {
        _topoNode = node;
        if (node != null)
        {
            //Debug.Log("DepNode.SetTopoNode:" + node.Name);
            NodeID = node.Id;
            NodeName = node.Name;
            NodeKKS = node.KKS;
            HaveTopNode = true;
        }
        else
        {
            HaveTopNode = false;
        }
    }

    public void SetTopoNodeEx(PhysicalTopology node)
    {
        SetTopoNode(node);
        if (node.Children != null)
        {

        }
    }

    [ContextMenu("RefreshChildrenNodes")]
    public virtual void RefreshChildrenNodes()
    {
        //var childNodes = this.gameObject.GetComponentsInChildren<DepNode>();
        //ChildNodes = new List<DepNode>();
        //ChildNodes.AddRange(childNodes);
        ChildNodes = new List<DepNode>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            var depNode = child.GetComponent<DepNode>();
            if (depNode)
            {
                ChildNodes.Add(depNode);
                depNode.ParentNode = this;
            }
            else
            {
                //处理J6J11的情况
                for (int j = 0; j < child.childCount; j++)
                {
                    var subChild = child.GetChild(j);
                    var depNode2 = subChild.GetComponent<DepNode>();
                    if (depNode2)
                    {
                        ChildNodes.Add(depNode2);
                        depNode2.ParentNode = this;
                    }
                }
            }
        }

        foreach (var item in ChildNodes)
        {
            item.RefreshChildrenNodes();//递归
        }
    }

    public bool IsRoom()
    {
        if (TopoNode == null)
        {
            Debug.LogError("IsRoom TopoNode == null");
            return false;
        }
        return TopoNode.Type == AreaTypes.机房 || TopoNode.Type == AreaTypes.范围;
    }

    public bool IsFloor()
    {
        if (TopoNode == null)
        {
            Debug.LogError("IsFloor TopoNode == null");
            return false;
        }
        return TopoNode.Type == AreaTypes.楼层;
    }

    /// <summary>
    /// 是否是大楼
    /// </summary>
    /// <returns></returns>
    public bool IsBuild()
    {
        if (TopoNode == null)
        {
            Debug.LogError("IsFloor TopoNode == null");
            return false;
        }
        return TopoNode.Type == AreaTypes.大楼;
    }

    public bool HaveTopNode;

    /// <summary>
    /// 区域类型
    /// </summary>
    public DepType depType;
    /// <summary>
    /// 是否被摄像头聚焦
    /// </summary>
    [HideInInspector]
    public bool IsFocus;
    /// <summary>
    /// 区域物体
    /// </summary>
    public GameObject NodeObject;
    /// <summary>
    /// 静态设备存放处
    /// </summary>
    [HideInInspector]
    public GameObject StaticDevContainer;
    /// <summary>
    /// 静态设备(建筑)
    /// </summary>
    public List<FacilityDevController> StaticDevList;

    /// <summary>
    /// 区域范围
    /// </summary>
    public MonitorRangeObject monitorRangeObject;
    /// <summary>
    /// 父节点
    /// </summary>
    public DepNode ParentNode;
    /// <summary>
    /// 子节点
    /// </summary>
    public List<DepNode> ChildNodes;

    /// <summary>
    /// 全部节点
    /// </summary>
    [HideInInspector]
    public List<DepNode> AllNodes;

    /// <summary>
    /// 地板方块，用于高层定位调整高度，设备编辑等
    /// </summary>
    public FloorCubeInfo floorCube;
    /// <summary>
    /// 区域设备是否创建
    /// </summary>
    [HideInInspector]
    public bool IsDevCreate;
    protected virtual void Start()
    {
        NodeObject = gameObject;
    }

    public bool HaveChildren()
    {
        if (ChildNodes == null) return false;
        if (ChildNodes.Count == 0) return false;
        return true;
    }

    public override string ToString()
    {
        return string.Format("name:{0},nodeId:{1},nodeName:{2},haveChildren:{3},topoNode:{4},depType:{5}", name, NodeID, NodeName,
            HaveChildren(), TopoNode != null, depType);
    }

    /// <summary>
    /// 设置该节点下的区域范围
    /// </summary>
    public virtual void SetMonitorRangeObject(MonitorRangeObject oT)
    {
        monitorRangeObject = oT;
    }
    /// <summary>
    /// 打开并聚焦区域
    /// </summary>
    public virtual void OpenDep(Action onComplete=null, bool isFocusT = true)
    {

    }
    /// <summary>
    /// 关闭区域，返回上一层
    /// </summary>
    public virtual void HideDep(Action onComplete=null)
    {

    }
    /// <summary>
    /// 聚焦区域
    /// </summary>
    /// <param name="onComplete"></param>
    public virtual void FocusOn(Action onComplete = null)
    {

    }
    /// <summary>
    /// 取消聚焦
    /// </summary>
    /// <param name="onComplete"></param>
    public virtual void FocusOff(Action onComplete = null)
    {

    }

    public virtual void Unload()
    {

    }

    #region 区域高亮
    /// <summary>
    /// 高亮设备
    /// </summary>
    /// <param name="isHighLightLastOff">是否关闭上一个物体的高亮</param>
    public virtual void HighlightOn(bool isHighLightLastOff=true)
    {       
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        Color colorConstant = Color.green;
        //SetOcculuderState(false);
        h.ConstantOnImmediate(colorConstant);
        HighlightManage manager = HighlightManage.Instance;
        if (manager&&isHighLightLastOff)
        {
            manager.SetHightLightDep(this);
        }
    }
    /// <summary>
    /// 设置遮挡
    /// </summary>
    /// <param name="isOn"></param>
    private void SetOcculuderState(bool isOn)
    {
        if(isOn)
        {
            HighlighterOccluder[] occulders = transform.GetComponentsInChildren<HighlighterOccluder>(false);
            foreach(HighlighterOccluder item in occulders)
            {
                Highlighter highLight = item.GetComponent<Highlighter>();
                if (highLight) highLight.OccluderOn();
            }
        }
        else
        {
            HighlighterOccluder[] occulders = transform.GetComponentsInChildren<HighlighterOccluder>(false);
            foreach (HighlighterOccluder item in occulders)
            {
                Highlighter highLight = item.GetComponent<Highlighter>();
                if (highLight)
                {
                    Debug.LogError("Occulder Off...");
                    highLight.OccluderOff();
                    highLight.ReinitMaterials();                    
                }
            }
        }
    }
    /// <summary>
    /// 取消高亮
    /// </summary>
    public virtual void HighLightOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //SetOcculuderState(true);
        h.ConstantOffImmediate();
    }
    #endregion
    #region DoorPart
    /// <summary>
    /// 区域下，所有门的管理
    /// </summary>
    [HideInInspector]
    public DepDoors Doors;
    public void InitDoor(DepDoors door)
    {
        door.DoorDep = this;
        Doors = door;
    }

    public bool IsUnload = false;

    public void SetIsUnload()
    {
        IsUnload = true;
        if(ChildNodes!=null)
            foreach (var item in ChildNodes)
            {
                item.SetIsUnload();
            }
    }

    public bool IsLoaded = false;

    public void SetIsLoaded()
    {
        IsLoaded = true;
        if (ChildNodes != null)
            foreach (var item in ChildNodes)
            {
                item.SetIsLoaded();
            }
    }

    #endregion

    private bool isInitBounds = false;

    private Bounds bounds;

    public virtual bool IsInBounds(Transform t)
    {
        if (isInitBounds == false)
        {
            bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
            isInitBounds = true;
        }

        return bounds.Contains(t.position);
    }
}
