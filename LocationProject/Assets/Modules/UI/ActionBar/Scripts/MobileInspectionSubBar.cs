using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionSubBar : MonoBehaviour {
    public static MobileInspectionSubBar Instance;
    public GameObject Bar;
    /// <summary>
    /// 历史
    /// </summary>
    public Toggle historyToggle;

    // Use this for initialization
    void Start () {
        Instance = this;
        InitToggleMethod();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 初始化Toggle绑定方法
    /// </summary>
    private void InitToggleMethod()
    {
        historyToggle.onValueChanged.AddListener(HistoryToggle_OnValueChanged);
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        Bar.SetActive(true);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        Bar.SetActive(false);
    }

    /// <summary>
    /// 设置显示隐藏
    /// </summary>
    public void SetShoworHide(bool isActive)
    {
        if (isActive)
        {
            Show();
        }
        else
        {
            ClearToggles();
            Hide();
        }
    }

    /// <summary>
    /// 退出子系统
    /// </summary>
    public void Exit()
    {
        if (historyToggle.isOn)
        {
            historyToggle.isOn = false;
        }
    }

    /// <summary>
    /// 设置历史Toggle
    /// </summary>
    public void SetHistoryToggle(bool ison)
    {
        if (historyToggle.isOn != ison)
        {
            historyToggle.isOn = ison;
        }
    }

    /// <summary>
    /// 历史Toggle值改变
    /// </summary>
    /// <param name="ison"></param>
    public void HistoryToggle_OnValueChanged(bool ison)
    {
        ActionBarManage.Instance.ChangeImage(ison, historyToggle);
        if (ison)
        {
            MobileInspectionHistory_N.Instance.Show();
        }
        else
        {
            MobileInspectionHistory_N.Instance.SetContentActive(false);
        }

        //MobileInspectionHistory.Instance.SetWindowActive(ison);
    }

    /// <summary>
    /// 清除Toggles
    /// </summary>
    public void ClearToggles()
    {
        historyToggle.isOn = false;
    }
}
