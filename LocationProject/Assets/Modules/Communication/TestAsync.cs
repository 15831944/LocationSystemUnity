using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAsync : MonoBehaviour {

    public bool TopoTree;
    public bool DepartmentTree;
    public bool PersonTree;
    public bool AreaStatistics;
    public bool Tags;
    // Use this for initialization
    void Start()
    {
        Test();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        GetTopoTree();
        GetDepartmentTree();
        GetPersonTree();
        GetAreaStatistics();
        GetTagsAsync();
    }

    [ContextMenu("GetTags")]
    private void GetTagsAsync()
    {
        if (Tags)
            CommunicationObject.Instance.GetTags(result =>
            {

            });
    }

    [ContextMenu("GetAreaStatistics")]
    private void GetAreaStatistics()
    {
        if (AreaStatistics)
            CommunicationObject.Instance.GetAreaStatistics(2, result =>
            {

            });
    }

    [ContextMenu("GetPersonTree")]
    private void GetPersonTree()
    {
        if (PersonTree)
            CommunicationObject.Instance.GetPersonTree(result =>
            {

            });
    }

    [ContextMenu("GetDepartmentTree")]
    private void GetDepartmentTree()
    {
        if (DepartmentTree)
            CommunicationObject.Instance.GetDepartmentTree(result =>
            {

            });
    }

    [ContextMenu("GetTopoTree")]
    private void GetTopoTree()
    {
        if (TopoTree)
            CommunicationObject.Instance.GetTopoTree((result) =>
            {

            });
    }

    // Update is called once per frame
    void Update () {
		
	}
}
