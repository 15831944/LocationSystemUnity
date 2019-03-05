using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadarDataDemo : MonoBehaviour {

    public UGUIRadarControl control;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void On_Click1()
    {
        List<float> list = new List<float>();
        list.Add(0.2f);
        list.Add(0.3f);
        list.Add(0.4f);
        control.SetValue(list);
    }

    public void On_Click2()
    {
        List<float> list = new List<float>();
        list.Add(0.7f);
        list.Add(0.9f);
        list.Add(0.3f);
        control.SetValue(list);
    }

    public void On_Click3()
    {
        List<float> list = new List<float>();
        list.Add(0.5f);
        list.Add(0.5f);
        list.Add(0.5f);
        control.SetValue(list);

    }
}
