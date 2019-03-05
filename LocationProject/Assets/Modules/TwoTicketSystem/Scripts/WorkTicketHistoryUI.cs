using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkTicketHistoryUI : MonoBehaviour {

    public static WorkTicketHistoryUI Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button closeBtn;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseBtn_OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 是否显示传统
    /// </summary>
    public void SetWindowActive(bool isActive)
    {
        window.SetActive(isActive);
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    public void CloseBtn_OnClick()
    {
        MobileInspectionSubBar.Instance.SetHistoryToggle(false);
    }
}
