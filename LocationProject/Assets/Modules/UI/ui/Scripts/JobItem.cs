using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobItem : MonoBehaviour {

    public Text JobName;
    List<Personnel> PersonnelList;
    public Button DeleteBut;
    Post CurrentPost;
    string CurrentInputKey;
    string CurrentInputValue;
    void Start () {
        DeleteBut.onClick.AddListener(() =>
        {
            DeletePost(CurrentPost, PersonnelList);
        });
    }
	
	public void ShowJobItemInfo(Post post, List<Personnel> personnelList, string inputKey, string inputValue)
    {
        CurrentInputKey = inputKey;
        CurrentInputValue = inputValue;
        CurrentPost = new Post() ;
        CurrentPost = post;
        JobName.text = post.Name.ToString();
        PersonnelList = new List<Personnel>();
        PersonnelList.AddRange(personnelList);
      
    }
	public void DeletePost(Post post, List<Personnel> personnelList)
    {
        Personnel CurrentPersonnel = PersonnelList.Find(m => !string.IsNullOrEmpty(m.Pst)&& m.Pst.Trim ()== post.Name.Trim ());
        if (CurrentPersonnel != null)
        {
                UGUIMessageBox.Show("当前岗位已关联多个人员信息，不能删除！", "确定", "", null , null, null);
        }
        else
        {
    
            bool IsSuccessful = CommunicationObject.Instance.DeletePost(post.Id);
            if (IsSuccessful)
            {
                UGUIMessageBox.Show("删除岗位信息成功！", "确定", "",
              () => {
                  EditDeletePostInfo();
                //  EditPersonnelInformation.Instance.RefreshEditJobInfo();
              }, null, null);

            }
            else
            {
                UGUIMessageBox.Show("删除岗位信息失败！",
                 null, null);
            }
        }

    }
    public void EditDeletePostInfo()
    {
        JobList.Instance.ShowAndCloseEditPostWindow(true);
        JobList.Instance.JobData.RemoveAll(item => item.Id == CurrentPost.Id);
        JobList.Instance.ScreenList.RemoveAll(item => item.Id == CurrentPost.Id);
        JobList.Instance.JobSelected.text = CurrentInputKey;
        JobList.Instance.ShowPostInfo();

        double pageNum = Math.Ceiling((double)(JobList.Instance.ScreenList.Count) / 10);
        if (int.Parse(CurrentInputValue) > pageNum && JobList.Instance.ScreenList.Count != 0)
        {
            JobList.Instance.pegeNumText.text = pageNum.ToString();
        }
        else if (JobList.Instance.ScreenList.Count == 0)
        {

            JobList.Instance.pegeNumText.text = "1";

        }
        else
        {
            JobList.Instance.pegeNumText.text = CurrentInputValue;
        }
        JobList.Instance.InputJobPage(JobList.Instance.pegeNumText.text);
        
    }
    void Update () {
		
	}
}
