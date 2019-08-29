using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddJobItem : MonoBehaviour {

    public Text JobName;
   
    public Button DeleteBut;
    void Start()
    {

    }
    public void ShowJobItemInfo(Post post)
    {
        JobName.text = post.Name.ToString();
       
        DeleteBut.onClick.AddListener(() =>
      {
          UGUIMessageBox.Show("删除岗位信息！",
     () =>
     {
         bool IsSuccessful = CommunicationObject.Instance.DeletePost(post.Id);
         if (IsSuccessful)
         {
             UGUIMessageBox.Show("删除部门信息成功！",
             null, null);
             AddPersonnel.Instance.RefreshAddJobs();
         }
         else
         {
             UGUIMessageBox.Show("删除部门信息失败！",
              null, null);
         }
         
     }, null);
        
      });
    }
    

}
