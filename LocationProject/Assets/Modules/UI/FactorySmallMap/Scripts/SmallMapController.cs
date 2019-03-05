using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallMapController : MonoBehaviour {
    public static SmallMapController Instance;
    /// <summary>
    /// 界面窗体
    /// </summary>
    public GameObject Window;
    /// <summary>
    /// 导航栏生成部分
    /// </summary>
    public MapItemInit MapItemManager;
    /// <summary>
    /// 小地图展开部分
    /// </summary>
    public MapExpand MapExpandManager;
	// Use this for initialization
	void Awake () {
        Instance = this;
        SceneEvents.DepNodeChanged += OnDepNodeChange;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnDepNodeChange(DepNode LastNode,DepNode CurrentNode)
    {
        //if (!gameObject.activeInHierarchy) return;
        if (MapExpandManager)
        {
            MapExpandManager.ShowBg(CurrentNode);
        }
        ShowMapByDepNode(CurrentNode);       
    }
    /// <summary>
    /// 通过区域类型，显示小地图
    /// </summary>
    /// <param name="node"></param>
    public void ShowMapByDepNode(DepNode node)
    {
        MapItemManager.AddItems(node);
    }
    public void Hide()
    {
        if(Window.activeInHierarchy)
        {
            Window.SetActive(false);
        }
    }
    public void Show()
    {
        if (FullViewController.Instance.IsFullView) return;
        if (!Window.activeInHierarchy)
        {
            Window.SetActive(true);
        }
    }
    void OnDestroy()
    {
        SceneEvents.DepNodeChanged -= OnDepNodeChange;
    }
}
