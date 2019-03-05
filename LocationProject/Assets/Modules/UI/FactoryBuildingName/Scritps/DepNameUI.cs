using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepNameUI : MonoBehaviour {
    public static DepNameUI Instance;
    public GameObject Window;
    public Text NameText;

    private Canvas canvas;
    private CanvasScaler canvasScaler;
    private bool isShow;
    private Vector3 offset = new Vector3(10,-15,0);
    // Use this for initialization
    void Start () {
        Instance = this;
        if (canvas == null)
        {
            canvas = gameObject.GetComponentInParent<Canvas>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (isShow) SetPosition();
	}
    /// <summary>
    /// 显示区域名称
    /// </summary>
    /// <param name="depName"></param>
    public void Show(string depName)
    {
        if (UGUITooltip.Instance == null) return;
        Window.SetActive(true);
        isShow = true;
        //SetPosition();
        NameText.text = depName;
    }
    /// <summary>
    /// 关闭显示界面
    /// </summary>
    public void Close()
    {
        isShow = false;
        Window.SetActive(false);
    }
    /// <summary>
    /// 设置位置
    /// </summary>
    public void SetPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        //Vector3 panelPos = new Vector3(screenPos.x - (Screen.width / 2) + offset.x, screenPos.y - (Screen.height / 2) + offset.y);
        Vector3 panelPos = UGUIFollowTarget.ScreenToUI(Camera.main, canvasScaler, screenPos);
        Vector3 pos = UGUITooltip.Instance.AdjustOutBorder(panelPos);
        Window.transform.localPosition = pos+ offset;
    }
}
