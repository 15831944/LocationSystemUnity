using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeviceTemperatureItem : MonoBehaviour {
    public Slider TemperatureSlider;
    public Text TemperatureValue;
    public Text DeviceName;
	// Use this for initialization
	void Start () {
		
	}
	/// <summary>
    /// 初始化
    /// </summary>
    /// <param name="value"></param>
    /// <param name="devName"></param>
    public void InitItem(float value,float maxValue,string devName)
    {
        TemperatureValue.text = value.ToString("f1") + "℃";
        TemperatureSlider.value = value / maxValue;
        DeviceName.text = devName;
    }
}
