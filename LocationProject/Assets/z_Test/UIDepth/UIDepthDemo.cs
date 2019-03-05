using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDepthDemo : MonoBehaviour {

    public GameObject prafebUI;

    public List<UGUIFollowTarget> uifollows;

    public List<Color> colors;

    public bool IsCheck;

	// Use this for initialization
	void Start () {
        uifollows = new List<UGUIFollowTarget>();
        colors = new List<Color>();
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.blue);
        colors.Add(Color.yellow);
        colors.Add(Color.white);
        colors.Add(Color.gray);
        CreateUIS();
        Create500();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1)&& IsCheck)
        {
            Sortt();
        }
	}

    public void CreateUIS()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(obj, Vector3.zero);
            GameObject objt = UGUIFollowManage.Instance.CreateItem(prafebUI, targetTagObj, "LocationNameUI",null,false,false,true);
            UGUIFollowTarget u = objt.GetComponent<UGUIFollowTarget>();
            Image image = objt.GetComponent<Image>();
            image.color = colors[i];
            uifollows.Add(u);
        } 
    }

    [ContextMenu("Sortt")]
    public void Sortt()
    {
        UGUIFollowManage.Instance.Sort("LocationNameUI");
    }

    public void Create500()
    {
        for (int i = 0; i < 500; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(0.1f * i, 1, 5);
            obj.transform.SetParent(transform);
            GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(obj, Vector3.zero);
            GameObject objt = UGUIFollowManage.Instance.CreateItem(prafebUI, targetTagObj, "LocationNameUI",null, false, false, true);
            UGUIFollowTarget u = objt.GetComponent<UGUIFollowTarget>();
            Image image = objt.GetComponent<Image>();
            image.color = colors[i% colors.Count];
            uifollows.Add(u);
        }
    }
}
