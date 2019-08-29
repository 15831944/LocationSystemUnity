using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManage : MonoBehaviour {

    public static UIManage Instance;

    /// <summary>
    /// 是否显示新历史轨迹查询界面
    /// </summary>
    public bool isShowNewHistoryWindow;

	// Use this for initialization
	void Start () {
        Instance = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
