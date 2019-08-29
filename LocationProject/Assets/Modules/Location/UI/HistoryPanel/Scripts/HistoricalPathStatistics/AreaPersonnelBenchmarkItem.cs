using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaPersonnelBenchmarkItem : MonoBehaviour {

    public Text TimeText;
    public Text LocationDataText;
    public Toggle LineTog;

    public void ShowAreaPersonnelBenchmarkInfo(PositionList info)
    {
        TimeText.text = info.Name;
        LocationDataText.text = info.Count.ToString();

    }
}
