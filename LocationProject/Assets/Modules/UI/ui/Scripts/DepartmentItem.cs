using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepartmentItem : MonoBehaviour {

    public Text DepartName;
    public Text SuperiorDepartments;    
    public Button DeleteBut;
    public Button DetailBut;
    private int DepID;
    Department CurrentDepartment;
    List<Department> departmentList;
    GridLayoutGroup Grid;
    string CurrentInputKey;
    string CurrentInputValue;
    
    void Start () {
        DeleteBut.onClick.AddListener(() =>
        {
            DeleteDepartmentItem(CurrentDepartment);
        });
        DetailBut.onClick.AddListener(() =>
      {
          EditDepartment.Instance.EditOrAdd = true;
          DepartmentList.Instance.ShowAndCloseDepartmentListUI(false );
          EditDepartment.Instance.CloseOrShowEditDepWindow(true);
          EditDepartment.Instance.GetDepartmentData(CurrentDepartment, departmentList, Grid,this .gameObject );
          
      });
    }
    public void ShowDepartmentItemInfo(Department dep, List<Department> depList, GridLayoutGroup grid,string inputKey,string inputValue)
    {
        Grid = grid;
        CurrentInputKey = inputKey;
        CurrentInputValue = inputValue;
        CurrentDepartment = new Department();
        departmentList = new List<Department>();
        departmentList.AddRange(depList);
        CurrentDepartment = dep;
        DepID = dep.Id;
        DepartName.text = dep.Name.ToString();
        if (string.IsNullOrEmpty(dep.ParentId.ToString ()))
        {
            SuperiorDepartments.text = "--";
        }else
        {
            int id = (int)dep.ParentId;
            foreach (var per in depList)
            {
                if (id == per.Id)
                {
                    SuperiorDepartments.text = per.Name.ToString();
                }
            }
        }
     
     
    }
    /// <summary>
    /// 删除某一个部门
    /// </summary>
	public void DeleteDepartmentItem(Department CurrentDep)
    {

        GetDeleteDepartmentInfo(CurrentDep);

    }
    /// <summary>
    /// 得到删除部门的详细信息
    /// </summary>
    /// <param name="CurrentDep"></param>
    public void GetDeleteDepartmentInfo(Department CurrentDep)
    {
        Department depart= CommunicationObject.Instance.GetDepartment(CurrentDep.Id);
        if (depart !=null)
        {
            JudgeDeleteDepartmentInfo(depart);
        }
    }
    public void JudgeDeleteDepartmentInfo(Department CurrentDep)
    {
        if (CurrentDep.Children == null && CurrentDep.LeafNodes != null)
        {
            if (CurrentDep.LeafNodes.Length == 0)
            {
                SavedepartmentDeleteInfo(CurrentDep);
            }
            else
            {
                SaveFailureInfo();
            }
        }
        else if (CurrentDep.LeafNodes != null && CurrentDep.Children != null)
        {
            if (CurrentDep.LeafNodes.Length == 0 && CurrentDep.Children.Length == 0)
            {
                SavedepartmentDeleteInfo(CurrentDep);
            }
            else
            {
                SaveFailureInfo();
            }
        }
        else if (CurrentDep.LeafNodes == null && CurrentDep.Children != null)
        {
            if (CurrentDep.Children.Length == 0)
            {
                SavedepartmentDeleteInfo(CurrentDep);
            }
            else
            {
                SaveFailureInfo();
            }
        }

        else if (CurrentDep.LeafNodes == null && CurrentDep.Children == null)
        {
            SavedepartmentDeleteInfo(CurrentDep);
        }
    }
   /// <summary>
   /// 删除部门
   /// </summary>
    public void SavedepartmentDeleteInfo(Department CurrentDep)
    {
        bool IsSuccessful = CommunicationObject.Instance.DeleteDepartment(CurrentDep.Id);
        
        if (IsSuccessful)
        {
            PersonnelTreeManage.Instance.departmentDivideTree.ReshDeleteDepartTree(CurrentDep);
            UGUIMessageBox.Show("删除部门信息成功！", "确定", "",
           () => {
               //   EditPersonnelInformation.Instance.RefreshEditDepartData();
               EditDeleteDepartmentInfo(CurrentDep);

           }, null, null);

        }
        else
        {
            UGUIMessageBox.Show("删除部门信息失败！", "确定", "", null, null, null);
        }
    }
    public void EditDeleteDepartmentInfo(Department currentDep)
    {
        DepartmentList.Instance.ShowAndCloseDepartmentListUI(true);
        DepartmentList.Instance.DepartList.RemoveAll (item=> item.Id== currentDep.Id);
        DepartmentList.Instance.ScreenList.RemoveAll(item => item.Id == currentDep.Id);
        DepartmentList.Instance.DepSelected.text = CurrentInputKey;
        DepartmentList.Instance.ShowEditDepartmentInfo();
        double pageNum = Math.Ceiling((double)(DepartmentList.Instance.ScreenList.Count) / 10);
        if (int.Parse(CurrentInputValue) > pageNum && DepartmentList.Instance.ScreenList.Count != 0)
        {
            DepartmentList.Instance.pegeNumText.text = pageNum.ToString();
        }
        else if (DepartmentList.Instance.ScreenList.Count == 0)
        {

            DepartmentList.Instance.pegeNumText.text = "1";

        }
        else
        {
            DepartmentList.Instance.pegeNumText.text = CurrentInputValue;
        }
        DepartmentList.Instance.InputDepartmentPage(DepartmentList.Instance.pegeNumText.text);
        PersonnelTreeManage.Instance.departmentDivideTree.GetTopoTree();
    }
    /// <summary>
    /// 如果该部门下有子部门或者有人，不能删除
    /// </summary>
    public void SaveFailureInfo()
    {

        UGUIMessageBox.Show("当前部门已关联多个人员信息，不能删除！", "确定", "", null, null, null);

    }
    // Update is called once per frame
    void Update () {
		
	}
}
