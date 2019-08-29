using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
//using Location.WCFServiceReferences.LocationCallbackServices;
using UIWidgets;

public class OperationTree : MonoBehaviour {
    public static OperationTree Instance;
    //private Button btn_Retract; //收起按钮
    //private Button btn_ReFresh; //刷新树按钮
    //public Button DepartManagerView; //部门管理
    //public Button JobManagerView;//岗位管理
    public Button DepartmentBut; //部门管理
    public Button JobBut;//岗位管理 
    public Button Area_Retract; //区域收起
    public Button Area_ReFresh;
    public Image Img_AreaReFresh;
    public Button Dep_Retract;//部门刷新
    public Button Dep_ReFresh;
    //private Image Img_AreaReFresh; //区域树刷新图标
    public Image Img_DepFresh; //部门树刷新图标
    public Toggle tog;//在线人员刷新

    //public GameObject DepartView;//部门管理界面
    //public GameObject JobView;//岗位管理界面
    public GameObject DepartTree; //部门树
    public GameObject AreaTree;//区域树
    [HideInInspector]
    public AreaDivideTree FindAreaTreeObject; //找到创建的区域树
    [HideInInspector]
    public DepartmentDivideTree FindDepartTreeObject;  //找到创建的部门树
                                                     

    private void Awake()
    {
        Instance = this;
    }
    void Start () {
        FindAreaTreeObject = PersonnelTreeManage.Instance.areaDivideTree; //找到一棵区域树
        FindDepartTreeObject = PersonnelTreeManage.Instance.departmentDivideTree;//找到一颗部门树
        //btn_Retract = GameObject.Find("ReTract_Button").GetComponent<Button>();
        //btn_ReFresh = GameObject.Find("Refresh_Button").GetComponent<Button>();
        //Img_ReFresh = GameObject.Find("Refresh_Button/Image").GetComponent<Image>();
        //tog = GameObject.Find("HideOffLinePeople_Toggle").GetComponent<Toggle>();
        Area_Retract.onClick.AddListener(RetractOnclick);
        Area_ReFresh.onClick.AddListener(RefreshTree);
        DepartmentBut.onClick.AddListener(ShowDepartmentInfo);
        JobBut.onClick.AddListener(ShowJobInfo);

        Dep_Retract.onClick.AddListener(RetractOnclick);//部门收起树事件绑定
        Dep_ReFresh.onClick.AddListener(RefreshTree);//部门刷新树时间绑定
        tog.onValueChanged.AddListener((bool isOn) => HideOffLinePeople(isOn));//部门人员在线刷新
	}
    /// <summary>
    /// 岗位管理界面调取
    /// </summary>
    public void ShowJobInfo()
    {
        EditPersonnelInformation.Instance.RefreshEditJobInfo();
    }
    /// <summary>
    /// 部门管理界面调取
    /// </summary>
    public void ShowDepartmentInfo()
    {
        DepartmentList.Instance.ShowDepartmentListUI();
        DepartmentList.Instance.GetDepartmentListData();
    }
    /// <summary>
    /// 绑定收起树方法
    /// </summary>
    public void RetractOnclick()
    {
        if (FindAreaTreeObject.Tree != null && AreaTree.activeInHierarchy)
            TreeControlViewManager.Instance.ResTractTree(FindAreaTreeObject.Tree);
        if (FindDepartTreeObject.Tree != null && DepartTree.activeInHierarchy)
            TreeControlViewManager.Instance.ResTractTree(FindDepartTreeObject.Tree);
    }
    /// <summary>
    /// 刷新树
    /// </summary>
    public void RefreshTree()
    {
        
        if (AreaTree.activeInHierarchy)
        {
            PersonnelTreeManage.Instance.areaDivideTree.RefreshPersonnel(); //刷新区域树
            TreeControlViewManager.Instance.RefreshIconRotate(Img_AreaReFresh); //旋转刷新图标
        }
            
        if (DepartTree.activeInHierarchy)
        {
            PersonnelTreeManage.Instance.departmentDivideTree.GetTopoTree();//刷新部门树
            TreeControlViewManager.Instance.RefreshIconRotate(Img_DepFresh); //旋转刷新图标
        }
            
       

    }
    /// <summary>
    /// 隐藏不能定位的人员
    /// </summary>
    public void HideOffLinePeople(bool isOn)
    {
        if(isOn)
        {
            PersonnelTreeManage.Instance.departmentDivideTree.HideOffLinePerson();
        }
        else
        {
            PersonnelTreeManage.Instance.departmentDivideTree.ShowAllPerson();
        }
        
    }
    
    // Update is called once per frame
    void Update () {
		
	}
}
