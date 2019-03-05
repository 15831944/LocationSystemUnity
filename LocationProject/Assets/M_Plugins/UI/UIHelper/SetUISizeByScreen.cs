using UnityEngine;
using System.Collections;

/// <summary>
/// 设置UI的大小与屏幕一样大
/// </summary>
[ExecuteInEditMode]
public class SetUISizeByScreen : MonoBehaviour {
    public RectTransform rect;
	// Use this for initialization
	void Start () {
        rect = GetComponent<RectTransform>();

    }

	// Update is called once per frame
	void Update () {
        //print("111");
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }
        if (rect)
        {
            rect.sizeDelta = new Vector3(Screen.width, Screen.height);
        }
	}
}
