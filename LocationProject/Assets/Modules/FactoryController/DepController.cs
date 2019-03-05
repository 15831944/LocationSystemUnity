using HighlightingSystem;
using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepController : DepNode {

    /// <summary>
    /// 区域设备存放处
    /// </summary>
    public GameObject DepDevContainer;
	// Use this for initialization
	void Awake () {
        depType = DepType.Dep;
	}
    private void Start()
    {

    }
    /// <summary>
    /// 判断位置是否在区域内
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsInDepField(Vector3 pos)
    {
        return false;
        //if (depCollider == null) return false; 
        //Bounds bounds = depCollider.bounds;
        //bool rendererIsInsideTheBox = bounds.Contains(pos);
        //return rendererIsInsideTheBox;
    }
    /// <summary>
    /// 载入区域设备
    /// </summary>
    public void LoadDepDev()
    {
        //RoomFactory.Instance.CreateDepDev(NodeID,DepDevContainer,RoomFactory.DevType.DepDev);
    }

    /// <summary>
    /// 显示人员定位
    /// </summary>
    private void ShowLocation()
    {
        ActionBarManage menu = ActionBarManage.Instance;
        if(menu && menu.PersonnelToggle.isOn)
        {
            menu.ShowLocation();
        }      
    }
    /// <summary>
    /// 取消上一个区域的选中
    /// </summary>
    private void DisSelectLastDep()
    {
        if(FactoryDepManager.currentDep!=null)
        {
            FactoryDepManager.currentDep.IsFocus = false;
        }
    }
    /// <summary>
    /// 返回园区
    /// </summary>
    private void BackToFactory()
    {
        IsFocus = false;
        HideDep();
        CameraSceneManager.Instance.ReturnToDefaultAlign();
        FactoryDepManager.currentDep = FactoryDepManager.Instance;
        SceneEvents.OnDepNodeChanged(this,FactoryDepManager.currentDep);
        SceneBackButton.Instance.Hide();
    }
    /// <summary>
    /// 打开区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void OpenDep(Action onComplete = null, bool isFocusT = true)
    {
        ShowFactory();
        DisSelectLastDep();
        if (isFocusT)
        {
            IsFocus = true;
            FocusOn(onComplete);
        }
        else
        {
            if (onComplete != null)
            {
                onComplete();
            }
        }
        foreach (var building in ChildNodes)
        {
            building.HighlightOn(false);
        }
        DepNode lastDep = FactoryDepManager.currentDep;
        FactoryDepManager.currentDep = this;
        SceneEvents.OnDepNodeChanged(lastDep, FactoryDepManager.currentDep);
    }
    /// <summary>
    /// 关闭区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void HideDep(Action onComplete = null)
    {
        IsFocus = false;
        foreach (var building in ChildNodes)
        {
            building.HighLightOff();
        }
    }
    /// <summary>
    /// 聚焦区域
    /// </summary>
    public override void FocusOn(Action onFocusComplete=null)
    {
        IsFocus = true;
        CameraSceneManager.Instance.ReturnToDefaultAlign(onFocusComplete,()=> 
        {
            if (RoomFactory.Instance) RoomFactory.Instance.SetDepFoucusingState(false);
        });
    }
    /// <summary>
    /// 取消区域聚焦
    /// </summary>
    /// <param name="onFocusComplete"></param>
    public override void FocusOff(Action onFocusComplete = null)
    {
        IsFocus = false;
        CameraSceneManager.Instance.ReturnToDefaultAlign(onFocusComplete);
    }
    /// <summary>
    /// 显示厂区建筑
    /// </summary>
    private void ShowFactory()
    {
        FactoryDepManager manager = FactoryDepManager.Instance;
        if (manager) manager.ShowFactory();
    }
}
