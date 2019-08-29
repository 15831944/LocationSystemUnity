using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingFollowUI : MonoBehaviour {

    /// <summary>
    /// 当前展开弹窗的UI
    /// </summary>
    public static BuildingFollowUI CurrentMonitor;

    /// <summary>
    /// 漂浮框
    /// </summary>
    public GameObject Content;
    /// <summary>
    /// 建筑名称
    /// </summary>
    public Text Name;
    /// <summary>
    /// 建筑信息
    /// </summary>
    public Text Info;
    /// <summary>
    /// 建筑信息Button
    /// </summary>
    public Toggle UIToggle;
    /// <summary>
    /// 界面是否显示
    /// </summary>
    private bool isShow;
	// Use this for initialization
	void Start () {
        UIToggle.onValueChanged.AddListener(OnUIClick);
    }

    private void OnEnable()
    {
        UGUIFollowTarget follow = transform.GetComponent<UGUIFollowTarget>();
        if (follow&&follow.Target)
        {
            bool isEnable = follow.Target.activeInHierarchy;
            if (!isEnable) gameObject.SetActive(false);         //加载精细模型时，不显示
        }
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="Name">建筑名称</param>
    /// <param name="Area">建筑面积 m²</param>
    /// <param name="Height">建筑高度 m</param>
    /// <param name="FloorNum">建筑层数</param>
    public void SetUIInfo(string Name,string Area,string Height,string FloorNum)
    {
        string BuildingInfo =string.Format("面积 {0}m² \\ 高度 {1}m \\层数 {2}",Area,Height,FloorNum);
        this.Name.text = Name;
        this.Info.text = BuildingInfo;   
    }
    private void OnUIClick(bool isOn)
    {
        if(isOn)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    /// <summary>
    /// 显示建筑信息
    /// </summary>
    public void Show()
    {
        if (CurrentMonitor != null && CurrentMonitor != this) CurrentMonitor.UIToggle.isOn=false;
        CurrentMonitor = this;
        isShow = true;
        Content.SetActive(true);
    }
    /// <summary>
    /// 隐藏建筑信息
    /// </summary>
    public void Hide()
    {
        isShow = false;
        CurrentMonitor = null;
        Content.SetActive(false);
    }
}
