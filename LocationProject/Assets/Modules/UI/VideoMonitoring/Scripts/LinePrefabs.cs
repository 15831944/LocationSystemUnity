using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinePrefabs : MonoBehaviour {

    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    List<string> lineexample = new List<string>();
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public void InstantiateLine( )
    {
       // if (DataList == null) return;

      //  for (int i = 0; i < DataList.Count; i++)
       // {
            GameObject o = Instantiate(TemplateInformation);
            o.SetActive(true);
            //  OperationalLibrary tableRowItem = o.GetComponent<OperationalLibraryManage>();
            o.transform.parent = grid.transform;
            o.transform.localScale = Vector3.one;
            o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);


      //  }

    }
}
