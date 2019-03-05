using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UGUI_LineChartToolTip : MonoBehaviour {

    public Text X;
    public Text Y;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show(string Xstr,string Ystr)
    {
        X.text = Xstr;
        Y.text = Ystr;
    }
}
