using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PersonnelTreeManage : MonoBehaviour
{
    public static PersonnelTreeManage Instance;
    public GameObject Window;
    public DepartmentDivideTree departmentDivideTree;

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
    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        Invoke("InitTree", 1f);//不延迟1秒，会导致加载拓扑树异常
        //InitTree();

        AreaDivideToggle.onValueChanged.AddListener(areaDivideTree.ShowAreaDivideWindow);
        areaDivideTree.ShowAreaDivideWindow(true);
        DepartmentDivideToggle.onValueChanged.AddListener(departmentDivideTree.ShowDepartmentWindow);
    }

    public void InitTree()
    {
        //departmentDivideTree.ShowDepartmentDivideTree();
        //areaDivideTree.ShowAreaDivideTree(null);

        areaDivideTree.ShowAreaDivideTree(departmentDivideTree.ShowDepartmentDivideTree);//做成有先后关系的
    }

    public void ClosePersonnelWindow()
    {
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
        if (areaDivideTree.AreaWindow==true)
        {
            areaDivideTree.StartRefreshAreaPersonnel();
        }
       
    }


    /// <summary>
    /// PersonNode类型转化oPersonal
    /// </summary>
    public PersonNode PersonnelToPersonNode(Personnel personnelT)
    {
        PersonNode nodeT = areaDivideTree.PersonList.Find((item) => item.Id == personnelT.Id);
        return nodeT;
    }
}
