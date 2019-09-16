using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobsManagement : MonoBehaviour {

   public   List<Post> JobsList;
    private List<Post> PostList;
    public Dropdown JobsDropdownItem;
    void Start () {
        JobsDropdownItem = GetComponent<Dropdown>();

    }
	public void GetJobsManagementData()
    {
        JobsList = new List<Post>();
        PostList = new List<Post>();
        PostList = CommunicationObject.Instance.GetJobsList();
        Post first = new Post();
        first.Name = "--";
        JobsList.Add(first);
        JobsList.AddRange(PostList);
     
        SetDropdownData(JobsList);
    }
    private void SetDropdownData(List<Post> showItem)
    {
        JobsDropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].Name.ToString();
            JobsDropdownItem.options.Add(tempData);
        }
        JobsDropdownItem.captionText.text = showItem[0].Name.ToString();
    }
    // Update is called once per frame
    void Update () {
		
	}
}
