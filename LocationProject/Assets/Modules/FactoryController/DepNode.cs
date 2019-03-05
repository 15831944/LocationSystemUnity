using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;
using System;
using HighlightingSystem;

public class DepNode : MonoBehaviour
{
    public string NodeKKS;
    public string NodeName;
    /// <summary>
    /// 对应区域ID
    /// </summary>
    public int NodeID;


    private PhysicalTopology _topoNode;
    public PhysicalTopology TopoNode
    {
        get { return _topoNode; }
        set
        {
            _topoNode = value;
            if (value != null)
            {
                NodeID = value.Id;
                NodeName = value.Name;
                //if (value.Nodekks != null)
                //    NodeKKS = value.Nodekks.KKS;
                NodeKKS = value.KKS;
                //Log.Info("----------------------SetDepNode.TopoNode!!!!!!!!!!!!!!!!!!!!!!", this.ToString());

                HaveTopNode = true;
            }
            else
            {
                HaveTopNode = false;
            }
        }
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
    public Transform floorCube;
    /// <summary>
    /// 区域设备是否创建
    /// </summary>
    [HideInInspector]
    public bool IsDevCreate;
    protected virtual void Start()
    {

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
    #endregion
}
