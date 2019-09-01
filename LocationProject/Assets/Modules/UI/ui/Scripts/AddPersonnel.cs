using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPersonnel : MonoBehaviour
{
    public static AddPersonnel Instance;
    public InputField Name;
    public Text TagName;
    public Button TagBut;
    public Text BornTime;
    public InputField WorkNum;
    public InputField Phone;

    public Dropdown job;
    public Toggle UseTog;
    public GameObject AddPerWindow;
    public PersonnelSex personnelSex;
    public Button CloseBut;
    public Button DepartBut;
    public Button JobBut;

    public Text departText;
    public JobsManagement JobManagements;
    public Button EnsureBut;
     public Button CancelBut;
    [System.NonSerialized]
    public Personnel CreatPersonnel;
    [System.NonSerialized]
    List<Personnel> PerList;
    public int CurrentId;


    void Start()
    {
        Instance = this;
        if(CancelBut)
            CancelBut.onClick.AddListener(() =>
            {
                CloseAddPerWindow();
                DataPaging.Instance.ShowpersonnelSearchWindow();
                DataPaging.Instance.StartPerSearchUI();
            });
        if(CloseBut)
            CloseBut.onClick.AddListener(() =>
            {
                CloseAddPerWindow();
                DataPaging.Instance.ShowpersonnelSearchWindow();
                DataPaging.Instance.StartPerSearchUI();
            });
        if(DepartBut)
            DepartBut.onClick.AddListener(() =>
            {
                AddDepartmentData();
            });
        if(JobBut)
            JobBut.onClick.AddListener(() =>
            {
                GetAddJobsData();
                CloseAddPerWindow();
            });
        if(EnsureBut)
            EnsureBut.onClick.AddListener(() =>
            {
                AddPersonnelData();
            });
        if(TagBut)
            TagBut.onClick.AddListener(AddTagCardInfo);
    }
    public void AddTagCardInfo()
    {

        AddEditCardRoleInfo.Instance.addTagDropdown.GetTagData();
        AddEditCardRoleInfo.Instance.ShowEditTagInfoWindow();
        AddEditCardRoleInfo.Instance.GetTagData(PerList);

        CloseAddPerWindow();
    }
    /// <summary>
    /// 打开添加部门信息列表
    /// </summary>
    public void AddDepartmentData()
    {
        AddDepartmentList.Instance.GetDepartmentListData();
        //AddDepartment.Instance.IsAdd = true;
        //departmentManagement.DepartManagement();
        //AddDepartmentList.Instance.GetDepartmentListData(departmentManagement.DepartList);
        AddDepartmentList.Instance.ShowDepartmentListUI();
    }
    /// <summary>
    /// 打开岗位信息界面并且刷新数据
    /// </summary>
	public void GetAddJobsData()
    {
        JobManagements.GetJobsManagementData();
        AddJobList.Instance.GetJobListData(JobManagements.JobsList);
        AddJobList.Instance.ShowJobListWindow();
    }



    public void AddPersonnelData()
    {
        CreatPersonnel = new Personnel();

        CreatPersonnel.Name = Name.text;
        CreatPersonnel.Sex = personnelSex.PerSexDropdownItem.captionText.text;
        DateTime BirthTime = Convert.ToDateTime(BornTime.text);
        CreatPersonnel.BirthDay = BirthTime;

        CreatPersonnel.WorkNumber = WorkNum.text;
        if (string.IsNullOrEmpty(Phone.text))
        {
            CreatPersonnel.PhoneNumber = "";
        }
        else
        {
            CreatPersonnel.PhoneNumber = Phone.text;
        }

        if (CurrentId == 0)
        {
            CreatPersonnel.ParentId = 1;
        }
        else
        {
            CreatPersonnel.ParentId = CurrentId;
        }

        CreatPersonnel.Pst = JobManagements.JobsDropdownItem.captionText.text;
        CreatPersonnel.Enabled = UseTog.isOn;

        SavePersonnelInfo(CreatPersonnel, () =>
         {
         });


    }
    public void GetAddPersonnelInfo(Personnel person)
    {
        if (string.IsNullOrEmpty(WorkNum.text) || string.IsNullOrEmpty(Name.text))
        {
            UGUIMessageBox.Show("人员必填信息不完整，请补充完整在进行提交！", null, null);
        }
        else
        {
            Personnel Per = PerList.Find(i => i.WorkNumber == WorkNum.text);
            if (Per == null)
            {
                person.Name = Name.text;
                person.WorkNumber = WorkNum.text;
                int IsAddPer = CommunicationObject.Instance.AddPerson(person);
                person.Id = IsAddPer;
                if (!string.IsNullOrEmpty(person.Name))
                {
                    UGUIMessageBox.Show("人员信息已添加！", "确定", "",
                        () =>
                        {
                            CloseAddPerWindow();
                            DataPaging.Instance.ShowpersonnelSearchWindow();
                            DataPaging.Instance.StartPerSearchUI();
                            PersonnelTreeManage.Instance.departmentDivideTree.GetTopoTree();
                        }, null, null);

                }
                else
                {
                    UGUIMessageBox.Show("人员信息已添加失败！", "确定", "", null, null, null);
                }
            }
            else
            {
                UGUIMessageBox.Show("该工号已存在！", "确定", "", null, null, null);
            }
        }
    }

    public void SavePersonnelInfo(Personnel p, Action action = null)
    {
        Tag chooseTag = AddEditCardRoleInfo.Instance.ChooseTag;
        if (chooseTag != null)
        {
            p.TagId = chooseTag.Id;
            p.Tag = chooseTag;
            chooseTag.IsActive = true;
            bool IsTag = CommunicationObject.Instance.EditTag(chooseTag);
        }
        //int perId=    CommunicationObject.Instance.AddPerson(p);
        //p.Id = perId;
        GetAddPersonnelInfo(p);
        if (action != null) action();
    }
    public void ShowAddPerWindow()
    {
        DataEmpty();
        GetDepartmentInfo();
        AddPerWindow.SetActive(true);
        DataPaging.Instance.ClosepersonnelSearchUI();
        // GetDepartmentInfo();
    }
    public void ShowAddPerWindow(List<Personnel> Per)
    {
        PerList = new List<Personnel>();
        PerList.AddRange(Per);
        DataEmpty();
        GetDepartmentInfo();
        AddPerWindow.SetActive(true);
        DataPaging.Instance.ClosepersonnelSearchUI();
        // GetDepartmentInfo();
    }

    public void DataEmpty()
    {

        Name.text = "";
        TagName.text = "";
        WorkNum.text = "";
        Phone.text = "";
        departText.text = "未绑定";
        JobManagements.JobsDropdownItem.captionText.text = "未绑定";
        UseTog.isOn = false;
    }
    public void CloseAddPerWindow()
    {
        AddPerWindow.SetActive(false);
        AddEditCardRoleInfo.Instance.ChooseTag = null;
        if (AddDepartmentTreeViewManger.Instance.DepBut.isOn)
        {
            AddDepartmentTreeViewManger.Instance.DepBut.isOn = false;
        }
    }
    public void GetDepartmentInfo()
    {
        personnelSex.PerSexDropdownItem.value = 0;
        personnelSex.PerSexDropdownItem.captionText.text = "男";
    }

    public void RefreshAddJobs()
    {
        JobManagements.GetJobsManagementData();
        AddJobList.Instance.GetJobListData(JobManagements.JobsList);
        AddJobList.Instance.ShowJobListWindow();

    }

    /// <summary>
    /// 在添加部门情况下，删除部门并且刷新数据
    /// </summary>
    public void RefreshDepartmentInfo()
    {
        //departmentManagement.DepartManagement();
        //departmentManagement.ShowFirstInfo();
        //AddDepartmentList.Instance.GetDepartmentListData(departmentManagement.DepartList);
    }

    void Update()
    {

    }
}
