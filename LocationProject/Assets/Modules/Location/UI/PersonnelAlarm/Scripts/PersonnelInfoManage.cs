using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelInfoManage : MonoBehaviour {
    /// <summary>
    ///环形动画
    /// </summary>
    public Tweener RingTweener;
    /// <summary>
    /// 环形图片
    /// </summary>
    public CircleImage RingImage;
  
    /// <summary>
    /// 符号动画
    /// </summary>
    public Tweener symbolTweener;
    /// <summary>
    /// "+"图片
    /// </summary>
    public Transform symbolImage;
    /// <summary>
    /// "+"旋转角度
    /// </summary>
    Vector3 rotation = new Vector3(0, 0, -45);
   /// <summary>
   /// 环形改变的颜色
   /// </summary>
    Color ClickRing=new Color(255 / 255f, 255 / 255f, 255 / 255f, 204 / 255f);
    /// <summary>
    /// 非人员告警按钮
    /// </summary>
    public Toggle PersonnelToggle;
    /// <summary>
    /// 非人员告警界面
    /// </summary>
    public GameObject Window;
    public   CircleImage circleImage;
    void Start () {
        CreateSymbolTweener();
        CreateRingColorTweener();
        
    }
	/// <summary>
    /// 创建“+”动画
    /// </summary>
	public void CreateSymbolTweener()
    {
        symbolTweener = symbolImage.DORotate(rotation, 0.24f);
        symbolTweener.SetAutoKill(false);
        symbolTweener.Pause();
        symbolTweener.SetEase(Ease.InOutBack);

       
        
        
    }
    /// <summary>
    /// 创建环形颜色动画
    /// </summary>
    public void CreateRingColorTweener()
    {
        RingTweener = RingImage.DOColor(ClickRing, 0.24f);
        RingTweener.SetAutoKill(false);
        RingTweener.Pause();
        RingTweener.SetEase(Ease.InOutBack);
    }
   
    public void ShowPersonnelWindow()
    {
        if (PersonnelToggle.isOn ==true )
        {
            Window.SetActive(true);
            symbolTweener.PlayForward();
            RingTweener.PlayForward();
            circleImage = circleImage.GetComponent<CircleImage>();
            Tween RingWide = DOTween.To(()=> circleImage.thickness, x=> circleImage.thickness=x, 20, 0.24f);
        }
        else
        {
            Window.SetActive(false);
            symbolTweener.PlayBackwards();
            RingTweener.PlayBackwards();
             circleImage = circleImage.GetComponent<CircleImage>();
            Tween RingWide = DOTween.To(() => circleImage.thickness, x => circleImage.thickness = x, 8, 0.24f);
        }
    }
}
