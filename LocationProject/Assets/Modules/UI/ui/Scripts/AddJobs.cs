using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddJobs : MonoBehaviour
{
    public static AddJobs Instance;
    public InputField JobName;
    public Button CloseBut;
    public GameObject JobEditWindow;
    public Button EnsureBut;

    Post CreatPost;
    public bool isAdd;
    private List<Post> PostList;
    void Start()
    {
        Instance = this;
        CreatPost = new Post();
        CloseBut.onClick.AddListener(() =>
        {
            if (isAdd)
            {
                AddPersonnel.Instance.GetAddJobsData();
            }
            else
            {
                EditPersonnelInformation.Instance.RefreshEditJobInfo();
            }
            CloseJobEditWindow();
        });
        EnsureBut.onClick.AddListener(() =>
        {
            GetModifyJobEditData();         
        });
    }
    /// <summary>
    /// 保存添加岗位信息
    /// </summary>
    public void GetModifyJobEditData()
    {
        CreatPost = new Post();
        CreatPost.Name = JobName.text;
        if (string.IsNullOrEmpty(JobName.text))
        {
            UGUIMessageBox.Show("部分必填信息不完整，请补充完整再进行提交！", "确定", "", null, null, null);
        }
        else
        {
            Post CurrentPost = PostList.Find(m => m.Name == JobName.text);
            if (CurrentPost != null)
            {
                UGUIMessageBox.Show("岗位已存在！",
               null, null);
            }
            else
            {
                SaveJobEditData(CreatPost);
            }
        }
    }
    public void SaveJobEditData(Post post)
    {
        int PostID = CommunicationObject.Instance.AddPost(post);
        post.Id = PostID;
        if (string.IsNullOrEmpty(post.Name))
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
        }
        else
        {
            UGUIMessageBox.Show("新建部门成功！", "确定", "",
                () =>
                {
                    CloseJobEditWindow();
                    if (isAdd)
                {
                        //  AddPersonnel.Instance.GetAddJobsData();
                        AddJobList.Instance.ShowAndClosePostInfo(true);
                        AddJobList.Instance.JobData.Insert(0, post);
                        AddJobList.Instance.ScreenList.Insert(0, post);
                        AddJobList.Instance.JobSelected.text = "";
                        AddJobList.Instance.ShowAddPostInfo();
                    }
                else
                {
                        //EditPersonnelInformation.Instance.ShowJobInfo();
                        JobList.Instance.ShowAndCloseEditPostWindow(true);
                        JobList.Instance.JobData.Insert(0, post);
                        JobList.Instance.ScreenList.Insert(0, post);
                        JobList.Instance.JobSelected.text = "";
                        JobList.Instance.ShowPostInfo();
                    }
              
                }, null, null);
        }

    }
    public void GetPostList(List<Post> info)
    {
        PostList = new List<Post>();
        if (PostList.Count != 0)
        {
            PostList.Clear();
        }
        PostList.AddRange(info);
    }
    public void ShowJobEditWindow()
    {
        JobEditWindow.SetActive(true);
    }
    public void CloseJobEditWindow(Action action = null)
    {
        JobName.text = "";
        JobEditWindow.SetActive(false);
        if (action != null) action();
    }
}
