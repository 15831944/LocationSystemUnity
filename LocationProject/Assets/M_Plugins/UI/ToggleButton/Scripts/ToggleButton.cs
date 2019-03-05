using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 此脚本挂在Toggle下
/// </summary>
public class ToggleButton : MonoBehaviour {
    private Toggle toggle;
    /// <summary>
    /// 正常图片
    /// </summary>
    public Image NormalImage;
    ///// <summary>
    ///// 选中图片
    ///// </summary>
    //public Image SelectImage;

    // Use this for initialization
    void Start () {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(Toggle_Changed);
        Toggle_Changed(toggle.isOn);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle_Changed(bool ison)
    {
        if (ison)
        {
            NormalImage.enabled = false;
        }
        else
        {
            NormalImage.enabled = true;
        }
    }
}
