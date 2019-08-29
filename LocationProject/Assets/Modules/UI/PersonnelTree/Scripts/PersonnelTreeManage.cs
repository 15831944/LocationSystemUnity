using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
public class PersonnelTreeManage : MonoBehaviour
{
    public static PersonnelTreeManage Instance;
    public GameObject Window;
    public DepartmentDivideTree departmentDivideTree;


    public List<Personnel> GetPersonnels()
    {
        if (departmentDivideTree)
        {
            return departmentDivideTree.personnels;
            //return departmentDivideTree.GetPersonnelFromServer();
        }
        else
        {
            return new List<Personnel>();
        }
    }

    public List<Personnel> GetAllPersonnels()
    {
        if (departmentDivideTree)
        {
            return departmentDivideTree.allPersonnels;
        }
        else
        {
            return new List<Personnel>();
        }
    }

    public Personnel GetTagPerson(int tagId)
    {
        return GetAllPersonnels().Find((item) => item.TagId == tagId);
    }

    public Personnel GetPerson(int id)
    {
        return GetAllPersonnels().Find((item) => item.Id == id);
    }

    public AreaDivideTree areaDivideTree;

    /// <summary>
    /// 收缩窗体动画
    /// </summary>
    private Tween ScaleWindowTween;
    /// <summary>
    /// 动画是否初始化
    /// </summary>
    private bool IsTweenInit;

    public Toggle AreaDivideToggle;
    public Toggle DepartmentDivideToggle;

    public Image DepartAndJob_Bg;//岗位管理和部门管理背景图
    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        //Invoke("InitTree", 1f);//不延迟1秒，会导致加载拓扑树异常

        AreaDivideToggle.onValueChanged.AddListener(areaDivideTree.ShowAreaDivideWindow);
        //areaDivideTree.ShowAreaDivideWindow(true);

        if (departmentDivideTree != null)
            DepartmentDivideToggle.onValueChanged.AddListener(departmentDivideTree.ShowDepartmentWindow);


    }


    /// <summary>
    /// 初始化人员和部门树
    /// </summary>
    public void InitTree()
    {
        try
        {
            if (departmentDivideTree != null)
            {
                areaDivideTree.ShowAreaDivideTree(departmentDivideTree.ShowDepartmentDivideTree);//做成有先后关系的
            }
            else
            {
                areaDivideTree.ShowAreaDivideTree(null);//做成有先后关系的
            }
            areaDivideTree.ShowAreaDivideWindow(true);
        }
        catch (Exception e)
        {
            Log.Error("Error: PersonnelTreeManage.InitTree->" + e.ToString());
        }

    }

    public void ClosePersonnelWindow()
    {
        if (departmentDivideTree != null)
            departmentDivideTree.ShowDepartmentWindow(false);
    }
    #region 窗体收缩动画部分
    /// <summary>
    /// 初始化动画
    /// </summary>
    private void InitTween()
    {
        IsTweenInit = true;
        RectTransform rect = transform.GetComponent<RectTransform>();
        Vector2 endValue = rect.sizeDelta - new Vector2(0, 280);
        ScaleWindowTween = transform.GetComponent<RectTransform>().DOSizeDelta(endValue, 0.3f);
        ScaleWindowTween.SetAutoKill(false);
        ScaleWindowTween.Pause();
    }
    /// <summary>
    /// 缩放窗体
    /// </summary>
    /// <param name="isExpand">是否扩大窗体</param>
    public void ScaleWindow(bool isExpand)
    {
        if (!IsTweenInit)
        {
            InitTween();
        }
        if (isExpand)
        {
            ScaleWindowTween.OnRewind(ResizeTree).PlayBackwards();
        }
        else
        {
            ScaleWindowTween.OnComplete(ResizeTree).PlayForward();
        }
    }
    /// <summary>
    /// 刷新树控件
    /// </summary>
    private void ResizeTree()
    {
        try
        {
            if (departmentDivideTree != null && areaDivideTree != null)
            {
                departmentDivideTree.Tree.ResizeContent();
                areaDivideTree.Tree.ResizeContent();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    #endregion
    /// <summary>
    /// 关闭设备拓朴树界面
    /// </summary>
    public void CloseWindow()
    {
        if (Window.activeInHierarchy)
            Window.SetActive(false);
        areaDivideTree.CloseeRefreshAreaPersonnel();
    }
    /// <summary>
    /// 打开设备拓朴树界面
    /// </summary>
    public void ShowWindow()
    {
        if (!Window.activeInHierarchy)
            Window.SetActive(true);
        if (AreaDivideToggle.isOn)
        {
            areaDivideTree.StartRefreshAreaPersonnel();
        }
        else
        {
            if (DepartmentDivideToggle.isOn)
            {
                departmentDivideTree.GetTopoTree();
            }
        }
    }


    /// <summary>
    /// PersonNode类型转化oPersonal
    /// </summary>
    public PersonNode PersonnelToPersonNode(Personnel personnelT)
    {
        PersonNode nodeT = areaDivideTree.FindPersonNode(personnelT.Id);
        return nodeT;
    }

    public void SelectPerson(Personnel person)
    {
        if (person == null)
        {
            Debug.LogError("PersonnelTreeManage.SelectPerson person==null");
            return;
        }
        if (departmentDivideTree)
            departmentDivideTree.Tree.SelectNodeByData(person.Id);
        if (areaDivideTree)
            areaDivideTree.Tree.SelectNodeByType(person.Id);
    }

    public void DeselectPerson(Personnel personnel)
    {
        if (departmentDivideTree)
            departmentDivideTree.Tree.DeselectNodeByData(personnel.Id);
        //PersonNode nodeT = PersonnelTreeManage.Instance.PersonnelToPersonNode(currentLocationFocusObj.personInfoUI.personnel.Id);
        if (areaDivideTree)
            areaDivideTree.Tree.AreaDeselectNodeByData(personnel.Id);
    }

    /// <summary>
    /// 找到区域树部分中的区域节点
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    public TreeNode<TreeViewItem> FindAreaNode(object areaId)
    {
        return areaDivideTree.Tree.AreaFindNodeById(areaId);
    }

    internal void RemovePersons(List<Tag> tags)
    {
        if (tags == null) return;
        foreach (Tag item in tags)
        {
            if (item.Pos == null || (item.Pos != null && item.Pos.IsHide))
            {
                var personId = item.PersonId;
                if (departmentDivideTree)
                {
                    TreeNode<TreeViewItem> node = departmentDivideTree.Tree.FindNodeByData(personId);
                    if (node != null)
                        node.Parent.Nodes.Remove(node);
                }
                if (areaDivideTree)
                {
                    TreeNode<TreeViewItem> node = areaDivideTree.Tree.FindNodeByData(personId);
                    if (node != null)
                        node.Parent.Nodes.Remove(node);
                }
            }
        }

    }
}
