using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Toggle字体颜色变化
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleTextColor : MonoBehaviour {

    /// <summary>
    /// Toggle.ison==false时，字体颜色
    /// </summary>
    public Color toggleFalseColor;
    /// <summary>
    /// Toggle.ison==true时，字体颜色
    /// </summary>
    public Color toggleTrueColor;

    private Toggle toggle;
    private Text txt;

    // Use this for initialization
    void Start () {
        toggle = GetComponent<Toggle>();
        txt = GetComponentInChildren<Text>();
        toggle.onValueChanged.AddListener(Toggle_ValueChanged);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle_ValueChanged(bool b) 
    {
        if (b)
        {
            txt.color = toggleTrueColor;
        }
        else
        {
            txt.color = toggleFalseColor;
        }
    }
}
