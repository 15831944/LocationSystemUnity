using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobItem : MonoBehaviour {

    public Text JobName;
    List<Personnel> PersonnelList;
    public Button DeleteBut;
    void Start () {
		
	}
	public void ShowJobItemInfo(Post post, List<Personnel> personnelList)
    {
        JobName.text = post.Name.ToString();
        PersonnelList = new List<Personnel>();
        PersonnelList.AddRange(personnelList);
        DeleteBut.onClick.AddListener(() =>
       {
           DeletePost(post, personnelList);
       });
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
                UGUIMessageBox.Show("删除部门信息成功！", "确定", "",
              () => {
                  EditPersonnelInformation.Instance.RefreshEditJobInfo();
              }, null, null);

            }
            else
            {
                UGUIMessageBox.Show("删除部门信息失败！",
                 null, null);
            }
        }

    }

    void Update () {
		
	}
}
