using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobsEditing : MonoBehaviour {
    public static JobsEditing Instance;
    public Text IDText;
    public InputField JobName;
    public Button CloseBut;
    public GameObject JobEditWindow;
    public Button EnsureBut;
    public Button CancelBut;
    Post CreatPost;
    
    void Start () {
        Instance = this;
        CreatPost = new Post() ;
        CloseBut.onClick.AddListener(()=>
        {
            EditPersonnelInformation.Instance.ShowJobInfo();
            CloseJobEditWindow();
           
        });
        EnsureBut.onClick.AddListener(GetModifyJobEditData);
    }
	public void GetJobEditData(Post post)
    {
        CreatPost = post;
        JobName.text = post.Name.ToString();
        IDText.text = "<color=#60D4E4FF>Id：</color>" + post.Id.ToString();
       
    }
    public void GetModifyJobEditData()
    {
        CreatPost.Name = JobName.text;
        SaveJobEditData(CreatPost);
        CloseJobEditWindow();
    }
    public void SaveJobEditData(Post post)
    {
        CommunicationObject.Instance.EditPost(post);
    }

    public void ShowJobEditWindow()
    {
        JobEditWindow.SetActive(true);
    }
    public void CloseJobEditWindow()
    {
        JobEditWindow.SetActive(false);
    }
	void Update () {
		
	}
}
