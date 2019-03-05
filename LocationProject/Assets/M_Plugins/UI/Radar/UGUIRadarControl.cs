using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UGUIRadarControl : MonoBehaviour {

    ////背景
    //public UGUIRadar backRadar0;
    //public UGUIRadar backRadar1;
    //内容
    public UGUIRadar contentRadar0;
    public UGUIRadar contentRadar1;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetValue(List<float> values)
    {
        contentRadar0.SetValue(values);
        contentRadar1.SetValue(values);
    }
}
