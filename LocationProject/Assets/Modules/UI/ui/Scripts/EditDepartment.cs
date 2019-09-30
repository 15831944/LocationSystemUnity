using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditDepartment : MonoBehaviour
{
    public static EditDepartment Instance;
    Department CurrentDepartment;
    List<Department> departmentList;
    public Button CloseBut;
    public Button EnsureBut;
    public InputField DepartmentName;
    public SuperiorDepartmentDropdown SuperiorDepDro;
    public GameObject EditDepWindow;
    public bool EditOrAdd;
    int value = 0;
    GameObject Obj;
    GridLayoutGroup Grid;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        EnsureBut.onClick.AddListener(() =>
        {
            GetSaveDepartmentData();
        });
        CloseBut.onClick.AddListener(() =>
        {
            CloseOrShowEditDepWindow(false);
            EditCurrentDepartmentInfo();
            if (EditOrAdd)
            {

                DepartmentList.Instance.ShowAndCloseDepartmentListUI(true );
                //DepartmentList.Instance.GetDepartmentListData();
            }
            else
            {
             //   AddDepartmentList.Instance.GetDepartmentListData();
                AddDepartmentList.Instance.ShowAndCloseAddDepartmentListUI(true );
            }
        });
    }
    public void GetDepartmentData(Department dep, List<Department> depList, GridLayoutGroup grid,GameObject obj)
    {
        Obj = obj;
        Grid = grid;
        SuperiorDepDro.SetDropdownData(dep, depList);
        CurrentDepartment = new Department();
        CurrentDepartment = dep;
        DepartmentName.text = dep.Name;
        departmentList = new List<Department>();
        departmentList.AddRange(depList);
        if (string.IsNullOrEmpty(dep.ParentId.ToString()))
        {
            SuperiorDepDro.SuperiorDepDropdown.value = 0;
            SuperiorDepDro.SuperiorDepDropdown.captionText.text = "--";
        }
        else
        {
            for (int i = 0; i < depList.Count; i++)
            {
                if (dep.ParentId == depList[i].Id)
                {
                    value = i;
                    SuperiorDepDro.SuperiorDepDropdown.value = i;
                    SuperiorDepDro.SuperiorDepDropdown.captionText.text = depList[i].Name;
                }
            }
        }
    }
    public void GetSaveDepartmentData()
    {

        if (string.IsNullOrEmpty(DepartmentName.text))
        {
            UGUIMessageBox.Show("部分必填信息不完整，请补充完整再进行提交！", "确定", "", null, null, null);
            return;
        }
        if (CurrentDepartment.Name == DepartmentName.text && value == SuperiorDepDro.SuperiorDepDropdown.value)

            return;
        else if (CurrentDepartment.Name == DepartmentName.text && value != SuperiorDepDro.SuperiorDepDropdown.value)
        {
            if (SuperiorDepDro.SuperiorDepDropdown.value == 0)
            {
                CurrentDepartment.ParentId = null;
            }
            else
            {
                Department ParentDep1 = departmentList.Find(i => i.Name == SuperiorDepDro.SuperiorDepDropdown.captionText.text);
                CurrentDepartment.ParentId = ParentDep1.Id;
            }

        }
        else if (CurrentDepartment.Name != DepartmentName.text)
        {
            Department CurrentNewDepart = departmentList.Find(i => i.Name == DepartmentName.text);
            if (CurrentNewDepart != null)
            {
                UGUIMessageBox.Show("部门已存在！", null, null);
                return;
            }
            else
            {

                CurrentDepartment.Name = DepartmentName.text;
               
                if (SuperiorDepDro.SuperiorDepDropdown.value == 0)
                {
                    CurrentDepartment.ParentId = null;
                }
                else
                {
                    Department ParentDep = departmentList.Find(i => i.Name == SuperiorDepDro.SuperiorDepDropdown.captionText.text);
                    CurrentDepartment.ParentId = ParentDep.Id;
                }


            }
        }

        bool IsSuccessful = CommunicationObject.Instance.EditDepartment(CurrentDepartment);
        if (IsSuccessful)
        {
            PersonnelTreeManage.Instance.areaDivideTree.ShowAreaDivideTree(PersonnelTreeManage.Instance.areaDivideTree.departmentDivideTree.ShowDepartmentDivideTree);
            // PersonnelTreeManage.Instance.departmentDivideTree.GetTopoTree();//刷新部门树
            UGUIMessageBox.Show("编辑部门信息成功！", "确定", "", null, null, null);
        }
        else
        {
            UGUIMessageBox.Show("编辑部门信息失败！", "确定", "", null, null, null);
        }
    }
    public void EditCurrentDepartmentInfo()
    {

        int k = Obj.transform.GetSiblingIndex();
        Transform line = Grid.transform.GetChild(k);
        line.GetChild(0).GetComponent<Text>().text = CurrentDepartment.Name;
        if (CurrentDepartment.ParentId == null)
        {
            line.GetChild(1).GetComponent<Text>().text = "--";
        }else
        {
            line.GetChild(1).GetComponent<Text>().text = CurrentDepartment.ParentId.ToString();
        }
       
    }
    public void CloseOrShowEditDepWindow(bool b)
    {
        EditDepWindow.SetActive(b);
    }
    void Update()
    {

    }
}
