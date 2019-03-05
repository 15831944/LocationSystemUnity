using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTextColor : MonoBehaviour
{
    public Text PersonName;
    public Text PersonNum;
    Color HighColor = new Color(254 / 255f, 209 / 255f, 109 / 255f, 255 / 255f);
    Color NormalColor = new Color(108 / 255f, 237 / 255f, 253 / 255f, 255 / 255f);
    void Start () {
		
	}

    /// <summary>
    /// 正常字体的颜色
    /// </summary>
    public void NormalTextColor()
    {
        PersonName.transform.GetComponent<Text>().color = NormalColor;
        PersonNum.transform.GetComponent<Text>().color = NormalColor;
    }
    /// <summary>
    /// 点击字体的颜色
    /// </summary>
    public void ClickTextColor()
    {
        PersonName.transform.GetComponent<Text>().color = HighColor;
        PersonNum.transform.GetComponent<Text>().color = HighColor;
    }
}
