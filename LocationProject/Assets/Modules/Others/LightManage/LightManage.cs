using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManage : MonoBehaviour {

    public static LightManage Instance;
    public Light mainlight;//场景主灯光

	// Use this for initialization
	void Start () {
        Instance = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 摄制组是否开启主灯光
    /// </summary>
    public void SetMainlight(bool b)
    {
        mainlight.gameObject.SetActive(b);
    }
}
