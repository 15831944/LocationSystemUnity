using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryViewController : MonoBehaviour
{

    //public GameObject FactoryViewUI;

	// Use this for initialization
	void Awake () {
        SceneEvents.FullViewStateChange += Instance_OnViewChange;
    }

    private void Instance_OnViewChange(bool isFullView)
    {
        if (isFullView)
        {
            Hide();
            //ActionBarManage.Instance.CurrentState = ViewState.None;
        }
        else
        {
            ActionBarManage.Instance.CurrentState = ViewState.人员定位;
            Show();
            SwitchToFactory();
        }
    }
    /// <summary>
    /// 进入厂区，显示相关界面
    /// </summary>
    public void Show()
    {
        SmallMapController.Instance.Show();
        StartOutManage.Instance.Show();
        ActionBarManage.Instance.Show();
    }
    /// <summary>
    /// 返回首页，隐藏界面
    /// </summary>
    public void Hide()
    {
        SmallMapController.Instance.Hide();
        if (StartOutManage.Instance != null)
        {
            StartOutManage.Instance.Hide();
        }

        if (ActionBarManage.Instance)
        {
            ActionBarManage.Instance.Hide();
        }
        if (FunctionSwitchBarManage.Instance)
        {
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
        }
    }
    /// <summary>
    /// 区域切换响应事件
    /// </summary>
    private void SwitchToFactory()
    {
        FactoryDepManager manager = FactoryDepManager.Instance;
        if (manager)
        {
            manager.OpenDep();
        }
    }
    private void OnDestroy()
    {
        SceneEvents.FullViewStateChange -= Instance_OnViewChange;
    }
    
}
