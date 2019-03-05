using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 测试
/// </summary>
public class UGUI_3DFollowTestDemo : MonoBehaviour {

    /// <summary>
    /// 物体列表
    /// </summary>
    public List<GameObject> objs;
    public GameObject uiPrefab;

	// Use this for initialization
	void Start () {
        CreateFlags();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreateFlags()
    {
        foreach (GameObject o in objs)
        {
            UGUI_3DFollowManage.Instance.CreateItem(uiPrefab, o, Camera.main.gameObject, new Vector3(0, o.GetGlobalSize().y / 2, 0), "CreateFlagsDemo");
        }
    }
}
