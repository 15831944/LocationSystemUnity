using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 一键收起树接口
/// </summary>
public class TreeControlViewManager : MonoBehaviour {

    public static TreeControlViewManager Instance;

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
       
    }
    //public void ShowJobInfo()
    //{
    //    EditPersonnelInformation.Instance.RefreshEditJobInfo();
    //}
    //public void ShowDepartmentInfo()
    //{
    //    DepartmentList.Instance.ShowDepartmentListUI();
    //    DepartmentList.Instance.GetDepartmentListData();
    //}
    /// <summary>
    /// 收起树
    /// </summary>
    public void ResTractTree(ChangeTreeView tree) //一键收起所有节点
    {
        // Debug.Log("输出树：" + Tree.Nodes);
        if (tree.Nodes != null)
        {
            foreach (var item in tree.Nodes)
            {
                if (item.IsExpanded)
                {
                    item.IsExpanded = false; //收起根节点
                    FindChildNodes(item);
                }
            }
        }
        else
        {
            Debug.LogError("TopoTree is null...");
        }

    }
    /// <summary>
    /// 递归寻找寻找子节点
    /// </summary>
    /// <returns></returns>
    public void FindChildNodes(TreeNode<TreeViewItem> GoTree)
    {
        if (GoTree.TotalNodesCount != 0)
        {
            foreach (var childitem in GoTree.Nodes)
            {
                if (childitem.IsExpanded)
                {
                    childitem.IsExpanded = false;//关闭子节点
                    FindChildNodes(childitem);
                }
            }
        }
        return;
    }
    /// <summary>
    /// 刷新树时图标旋转
    /// </summary>
    public void RefreshIconRotate(Image targetImg)  
    {
        if (OperationTree.Instance.AreaTree.activeInHierarchy)
            OperationTree.Instance.FindAreaTreeObject.Tree.ResizeContent();
        if (OperationTree.Instance.DepartTree.activeInHierarchy)
            OperationTree.Instance.FindDepartTreeObject.Tree.ResizeContent();
        targetImg.transform.DOLocalRotate(new Vector3(0,0, 360), 1f, RotateMode.WorldAxisAdd);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
