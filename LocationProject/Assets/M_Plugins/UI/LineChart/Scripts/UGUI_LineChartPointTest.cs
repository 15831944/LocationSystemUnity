using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UGUI_LineChartPointTest : UGUI_LineChartPoint
{

	// Use this for initialization
	void Start () {
        base.Start();
	
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    public override void UpdateData()
    {
        base.UpdateData();
        int i = transform.GetSiblingIndex();
        //NewBehaviourScriptqqqqqqqqqqq.Instance.strList[i];
        //if (UGUI_LineChartDemo.Instance.intList.Count > i)
        //{
        //SetData("x:" + i.ToString(), "y:" + UGUI_LineChartDemo.Instance.intList[i]);
        //}
        SetData("x:" + i.ToString(), "y:" + lineChart.valueList[i]);
    }



}
