using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PersonnelSearchTweener : MonoBehaviour {
    public static PersonnelSearchTweener Instance;
    /// <summary>
    /// 详细信息动画
    /// </summary>
    public Tweener openTweener;
    /// <summary>
    /// 详细信息界面
    /// </summary>
    public Transform InfoPanel;
    Vector3 size = new Vector3(1f, 1f, 1);

    public Tweener MinPersonnelTweener;
    public Transform MinPersonnelUI;

   
 
    
    void Start()
    {
        Instance = this;
        CreateSpreadTweener();
        CreaterMinTweener();
       
        
    }
    /// <summary>
    /// 创建详细信息动画
    /// </summary>
    public void CreateSpreadTweener()
    {
        openTweener = InfoPanel.DOScale(size, 0.24f);
        openTweener.SetEase(Ease.OutBack);
        openTweener.SetAutoKill(false);
        openTweener.Pause();
       
    }
    /// <summary>
    /// 点击定位缩小动画
    /// </summary>
    public void CreaterMinTweener()
    {
        MinPersonnelTweener = MinPersonnelUI.DOScale(Vector3.zero, 0.24f);
        MinPersonnelTweener.SetEase(Ease.InSine);
        MinPersonnelTweener.SetAutoKill(false);
        MinPersonnelTweener.Pause();
    }
    /// <summary>
    /// 点击按钮，搜索界面恢复
    /// </summary>
    public void MaxBut_Click()
    {
        DataPaging.Instance.ShowpersonnelSearchWindow();
        MinPersonnelTweener.PlayBackwards();
       

    }
    /// <summary>
    /// 点击按钮搜索界面关闭
    /// </summary>
    public void CloseBut_Click()
    {

    //    ShowMinWindow(false);
        MinPersonnelTweener.PlayForward();
        DataPaging.Instance.ClosepersonnelSearchWindow();
    }
  
    
    
}