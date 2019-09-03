using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUI_LineChartYValue : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    public void DateY(int Num)
    {
        int TextCount = transform.childCount;
        if (Num == 0)
        {
            for (int i = 0; i < TextCount; i++)
            {
                Text t = transform.GetChild(i).GetComponentInChildren<Text>();
                t.text = "";
            }
        }
        else
        {
            int differenceValue = (int)Math.Ceiling((double)Num / (double)TextCount);
            for (int i = 0; i < TextCount; i++)
            {
                Text t = transform.GetChild(i).GetComponentInChildren<Text>();
                t.text = (differenceValue * (TextCount - i)).ToString();
            }
        }


    }
    // Update is called once per frame
    void Update()
    {

    }
}
