using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UGUI_PieChartDemo : MonoBehaviour {

    public UGUI_PieChart pieChart;
    public UGUI_PieChartDoTween pieChartDoTween;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void On_Click()
    {
        List<float> fs = new List<float>();
        fs.Add(0.2f);
        fs.Add(0.3f);
        fs.Add(0.1f);
        fs.Add(0.4f);
        pieChart.Show(fs);
    }

    public void On_Click2()
    {
        List<float> fs = new List<float>();
        fs.Add(0.6f);
        fs.Add(0.3f);
        fs.Add(0.1f);

        pieChart.Show(fs);
    }

    public void On_Click3_new()
    {
        List<float> fs = new List<float>();
        fs.Add(0.6f);
        fs.Add(0.3f);
        fs.Add(0.1f);

        pieChartDoTween.Show(fs);
    }

    public void On_Click4_new()
    {
        List<float> fs = new List<float>();
        fs.Add(0.45f);
        fs.Add(0.3f);
        fs.Add(0.1f);
        fs.Add(0.05f);
        fs.Add(0.1f);
        pieChartDoTween.Show(fs);
    }

    public void On_Click5_new()
    {
        List<float> fs = new List<float>();
        //fs.Add(0.45f);
        //fs.Add(0.3f);
        //fs.Add(0.1f);
        //fs.Add(0.05f);
        //fs.Add(0.1f);
        pieChartDoTween.Show(fs);
    }
}
