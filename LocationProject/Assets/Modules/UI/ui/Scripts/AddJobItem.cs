using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddJobItem : MonoBehaviour {

    public Text JobName;
    public Button DeleteBut;
    Post CurrentPost;
    List<Personnel> PersonnelList;
    string CurrentInputKey;
    string CurrentInputValue;
    void Start()
    {
        DeleteBut.onClick.AddListener(() =>
        {
            DeletePost();
        });
    }
    public void ShowJobItemInfo(Post post, List<Personnel> personnelList, string inputKey, string inputValue)
    {
        CurrentPost = new Post();
        CurrentPost = post;
        PersonnelList = new List<Personnel>();
        PersonnelList.AddRange(personnelList);
        CurrentInputKey = inputKey;
        CurrentInputValue = inputValue;
        JobName.text = post.Name.ToString();     
    }

    public void DeletePost()
    {

        Personnel CurrentPersonnel = PersonnelList.Find(m => !string.IsNullOrEmpty(m.Pst) && m.Pst.Trim() == CurrentPost.Name.Trim());
        if (CurrentPersonnel != null)
        {
            UGUIMessageBox.Show("当前岗位已关联多个人员信息，不能删除！", "确定", "", null, null, null);
        }
        else
        {

            bool IsSuccessful = CommunicationObject.Instance.DeletePost(CurrentPost.Id);
            if (IsSuccessful)
            {
                UGUIMessageBox.Show("删除岗位信息成功！", "确定", "",
              () => {
                  AddDeletePostInfo();
                  AddPersonnel.Instance.RefreshAddJobs();
                 
              }, null, null);

            }
            else
            {
                UGUIMessageBox.Show("删除岗位信息失败！",
                 null, null);
            }
        }
    }
     public void AddDeletePostInfo()
    {
        AddJobList.Instance.ShowAndClosePostInfo(true);
        AddJobList.Instance.JobData.RemoveAll(item => item.Id == CurrentPost.Id);
        AddJobList.Instance.ScreenList.RemoveAll(item => item.Id == CurrentPost.Id);
        AddJobList.Instance.JobSelected.text = CurrentInputKey;
        AddJobList.Instance.ShowAddPostInfo();

        double pageNum = Math.Ceiling((double)(AddJobList.Instance.ScreenList.Count) / 10);
        if (int.Parse(CurrentInputValue) > pageNum && AddJobList.Instance.ScreenList.Count != 0)
        {
            AddJobList.Instance.pegeNumText.text = pageNum.ToString();
        }
        else if (AddJobList.Instance.ScreenList.Count == 0)
        {

            AddJobList.Instance.pegeNumText.text = "1";

        }
        else
        {
            AddJobList.Instance.pegeNumText.text = CurrentInputValue;
        }
        AddJobList.Instance.InputJobPage(AddJobList.Instance.pegeNumText.text);

    }

}

