using System;
using System.Collections;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;

public static class SceneEvents
{
    #region 区域拓扑树选择事件
    public static event Action<PhysicalTopology, PhysicalTopology> TopoNodeChanged;

    public static PhysicalTopology TopoRootNode;

    public static PhysicalTopology TopoNode;

    public static void OnTopoNodeChanged(PhysicalTopology argNew)
    {
        PhysicalTopology lastDep = TopoNode;
        TopoNode = argNew;
        OnTopoNodeChanged(lastDep, TopoNode);

        //OnDepNodeChanged(DepNode, argNew);
    }
    public static void OnTopoNodeChanged(PhysicalTopology argOld, PhysicalTopology argNew)
    {
        if (TopoNodeChanged != null)
        {
            TopoNodeChanged(argOld, argNew);
        }
    }
    #endregion

    #region 区域类型事件
    /// <summary>
    /// 区域开始切换时（在摄像头移动视角之前）
    /// </summary>
    public static event Action<DepNode, DepNode> DepNodeChangeStart;
    /// <summary>
    /// 当前区域节点发生变化，一般是拓扑树点击或者导航栏按钮点击触发
    /// </summary>
    public static event Action<DepNode, DepNode> DepNodeChanged;
    /// <summary>
    /// 当前区域节点
    /// </summary>
    public static DepNode DepNode;
    /// <summary>
    /// 大楼楼层开始展开触发事件
    /// </summary>
    public static event Action BuildingOpenStartAction;
    /// <summary>
    /// 大楼楼层展开完毕触发事件
    /// </summary>
    public static event Action BuildingOpenCompleteAction;
    /// <summary>
    /// 大楼楼层开始合拢触发事件
    /// </summary>
    public static event Action BuildingStartCloseAction;
    /// <summary>
    /// 大楼楼层合拢完毕触发事件
    /// </summary>
    public static event Action BuildingCloseCompleteAction;

    /// <summary>
    /// 区域创建完成事件（设备加载完成）
    /// </summary>
    public static event Action<DepNode> OnDepCreateComplete;
    /// <summary>
    /// 区域创建完成事件（设备加载完成）
    /// </summary>
    /// <param name="dep">区域</param>
    public static void OnDepCreateCompleted(DepNode dep)
    {
        if(OnDepCreateComplete!= null)
        {
            OnDepCreateComplete(dep);
        }
    }
    /// <summary>
    /// 区域开始切换
    /// </summary>
    /// <param name="argOld"></param>
    /// <param name="argNew"></param>
    public static void OnDepNodeChangeStart(DepNode argOld,DepNode argNew)
    {
        if(DepNodeChangeStart!=null)
        {
            DepNodeChangeStart(argOld,argNew);
        }
    }
    /// <summary>
    /// 区域节点发生变化
    /// </summary>
    /// <param name="argOld"></param>
    /// <param name="argNew"></param>
    public static void OnDepNodeChanged(DepNode argOld, DepNode argNew)
    {
        if (DepNodeChanged != null)
        {
            DepNodeChanged(argOld, argNew);
        }
        DepNode = argNew;
    }
    /// <summary>
    /// 区域节点发生变化
    /// </summary>
    /// <param name="argNew"></param>
    public static void OnDepNodeChanged(DepNode argNew)
    {
        DepNode lastDep = FactoryDepManager.currentDep;
        FactoryDepManager.currentDep = argNew;
        OnDepNodeChanged(lastDep, FactoryDepManager.currentDep);

        //OnDepNodeChanged(DepNode, argNew);
    }

    /// <summary>
    /// 大楼楼层展开触发
    /// </summary>
    public static void OnBuildingOpenStartAction()
    {
        if (BuildingOpenStartAction != null)
        {
            BuildingOpenStartAction();
        }
    }
    /// <summary>
    /// 大楼楼层展开完毕触发
    /// </summary>
    public static void OnBuildingOpenCompleteAction()
    {
        if (BuildingOpenCompleteAction != null)
        {
            BuildingOpenCompleteAction();
        }
    }

    /// <summary>
    /// 大楼楼层开始关闭触发
    /// </summary>
    public static void OnBuildingStartCloseAction()
    {
        if (BuildingStartCloseAction != null)
        {
            BuildingStartCloseAction();
        }
    }

    /// <summary>
    /// 大楼楼层关闭结束触发
    /// </summary>
    public static void OnBuildingCloseCompleteAction()
    {
        if (BuildingCloseCompleteAction != null)
        {
            BuildingCloseCompleteAction();
        }
    }

    #endregion
    #region 首页切换事件
    public delegate void FullViewPartChangeDel(FullViewPart part);
    /// <summary>
    /// 全景展示，区域切换事件
    /// </summary>
    public static event FullViewPartChangeDel FullViewPartChange;

    public delegate void ViewChangeDel(bool isFullView);
    /// <summary>
    /// 进入/离开全景展示模式的事件
    /// </summary>
    public static event ViewChangeDel FullViewStateChange;

    /// <summary>
    /// 首页模式改变
    /// </summary>
    /// <param name="part"></param>
    public static void OnFullViewPartChange(FullViewPart part)
    {
        if (FullViewPartChange != null)
        {
            FullViewPartChange(part);
        }
    }
    /// <summary>
    /// 进入/退出首页
    /// </summary>
    /// <param name="isFullView"></param>
    public static void OnFullViewStateChange(bool isFullView)
    {
        if(FullViewStateChange != null)
        {
            FullViewStateChange(isFullView);
        }
    }
    #endregion
}
