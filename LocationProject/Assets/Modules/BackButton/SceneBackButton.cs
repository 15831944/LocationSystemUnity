using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneBackButton : MonoBehaviour {
    public static SceneBackButton Instance;
    /// <summary>
    /// 返回按钮
    /// </summary>
    public Button backButton;
    /// <summary>
    /// 返回按钮触发方法
    /// </summary>
    private Action backAction;
	// Use this for initialization
	void Awake () {
        Instance = this;
        //backButton.onClick.AddListener(Back);
	}
    /// <summary>
    /// 返回
    /// </summary>
    private void Back()
    {
        //if (backAction != null) backAction();
    }
    /// <summary>
    /// 显示返回按钮
    /// </summary>
    /// <param name="_backAction"></param>
    public void Show(Action _backAction)
    {
        //backButton.gameObject.SetActive(true);
        //backAction = _backAction;
    }
    /// <summary>
    /// 关闭返回按钮
    /// </summary>
    public void Hide()
    {
        //backButton.gameObject.SetActive(false);
        //backAction = null;
    }
}
