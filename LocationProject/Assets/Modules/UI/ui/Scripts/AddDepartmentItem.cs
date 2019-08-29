using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDepartmentItem : MonoBehaviour
{

    public Text DepartName;
    public Text SuperiorDepartments;
   
    public Button DeleteBut;


    void Start()
    {

    }
    public void ShowDepartmentItemInfo(Department dep, List<Department> depList)
    {

        DepartName.text = dep.Name.ToString();
        if (string.IsNullOrEmpty(dep.ParentId.ToString()))
        {
            SuperiorDepartments.text = "四会电厂";
        }
        else
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
       
        DeleteBut.onClick.AddListener(() =>
        {
            DeleteDepartmentItem(dep);
            AddPersonnel.Instance.RefreshDepartmentInfo();

        });
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
        Department depart = CommunicationObject.Instance.GetDepartment(CurrentDep.Id);
        if (depart != null)
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
            UGUIMessageBox.Show("删除部门成功！", "确定", "",
           () => {
               EditPersonnelInformation.Instance.RefreshEditDepartData();

           }, null,null);

        }
        else
        {
            UGUIMessageBox.Show("删除部门失败！" ,"确定", "", null, null, null);
        }
    }
    /// <summary>
    /// 如果该部门下有子部门或者有人，不能删除
    /// </summary>
    public void SaveFailureInfo()
    {

        UGUIMessageBox.Show("当前部门已关联多个人员信息，不能删除！", "确定", "", null, null, null);
    }

    }
