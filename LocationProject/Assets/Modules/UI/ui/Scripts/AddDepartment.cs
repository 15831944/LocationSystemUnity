using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDepartment : MonoBehaviour
{

    public static AddDepartment Instance;
    public GameObject AddDepartmentWindow;
    public InputField DepartmentName;
    public AddEditDepDropdown addEditDepDropdown;
    public Button CloseBut;
    public Button EnsureBut;
   
    [System.NonSerialized]
    List<Department> departmentList;
    [System.NonSerialized]
    Department addDepartment;
    public bool IsAdd;
    int id;
    void Start()
    {
        Instance = this;
        addDepartment = new Department();
        CloseBut.onClick.AddListener(() =>
        {
            CloseAddDepartmentWindow();
            if (IsAdd == true)
            {
                AddPersonnel.Instance.AddDepartmentData();
            }
            else
            {
                EditPersonnelInformation.Instance.ShowDepartmentInfo();
            }             
        });     
        EnsureBut.onClick.AddListener(() =>
        {
            GetModifyDepartmentData();
        });
    }
    /// <summary>
    /// 保存添加部门信息
    /// </summary>
    public void GetModifyDepartmentData()
    {      
        if (string .IsNullOrEmpty (DepartmentName.text))
        {
            UGUIMessageBox.Show("部分必填信息不完整，请补充完整再进行提交！", "确定", "", null, null, null);
            return;
        }
        addDepartment.Name = DepartmentName.text;
        Department CurrentDepart = departmentList.Find(i => i.Name == DepartmentName.text);
        if (CurrentDepart != null)
        {
            UGUIMessageBox.Show("部门已存在！", null, null);
            return;
        }
        else
        {
            foreach (var dep in addEditDepDropdown.DepartList)
            {
                if (dep.Name == addEditDepDropdown.DepartmentDropdownItem.captionText.text)
                {
                    addDepartment.ParentId = dep.Id;
                }
            }
            int DepartID = CommunicationObject.Instance.AddDepartment(addDepartment);
            addDepartment.Id = DepartID;
            if (!string .IsNullOrEmpty (addDepartment.Name))
            {
                UGUIMessageBox.Show("新建部门成功！", "确定", "", ()=> 
                {
                    PersonnelTreeManage.Instance.departmentDivideTree.ReshAddDepartTree(addDepartment);
                    CloseAddDepartmentWindow(() =>
                    {
                        if (IsAdd == true)
                        {
                            //  AddPersonnel.Instance.AddDepartmentData();
                            AddDepartmentList.Instance.ShowAndCloseAddDepartmentListUI(true);

                            AddDepartmentList.Instance.DepartList.Insert(0, addDepartment);
                            AddDepartmentList.Instance.ScreenList.Insert(0, addDepartment);
                            AddDepartmentList.Instance.DepSelected.text = "";
                            AddDepartmentList.Instance.ShowAddDepartmentInfo();
                        }
                        else
                        {
                            DepartmentList.Instance.ShowAndCloseDepartmentListUI(true);
                            DepartmentList.Instance.DepartList.Insert(0, addDepartment);
                            DepartmentList.Instance.ScreenList.Insert(0, addDepartment);
                            DepartmentList.Instance.DepSelected.text = "";
                            DepartmentList.Instance.ShowEditDepartmentInfo();
                          

                            // EditPersonnelInformation.Instance.ShowDepartmentInfo();
                        }
                    });
                }, null, null);     
            }
           else
            {
                UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
            }
        }

        
    }
    public void GetDepartmentList(List<Department> depart)
    {
        departmentList = new List<Department>();
        departmentList.AddRange(depart);
    }
    /// <summary>
    /// 打开添加部门信息UI
    /// </summary>
    public void ShowAddDepartmentWindow()
    {

        AddDepartmentWindow.SetActive(true);
        addEditDepDropdown.DepartManagement();
    }
    /// <summary>
    /// 关闭添加部门信息UI
    /// </summary>
    /// <param name="action"></param>
    public void CloseAddDepartmentWindow(Action action = null)
    {
        DepartmentName.text = "";
        AddDepartmentWindow.SetActive(false);
        if (action != null) action();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
