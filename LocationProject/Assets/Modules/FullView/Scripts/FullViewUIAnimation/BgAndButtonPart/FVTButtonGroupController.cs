using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class FVTButtonGroupController : MonoBehaviour {
    /// <summary>
    /// 按钮组UI部分
    /// </summary>
    public FVTButtonGroupUI UIPart;
    // Use this for initialization
    void Awake () {
        InitSequence();
    }
    private void Start()
    {
        //HideAllImage();
    }
    public void StartTween(Action action=null)
    {
        TwinkleObj(UIPart.BgGrid.gameObject, 0.5f, 0.03f);
        BtnGroupSequence.OnComplete(()=> 
        {
            if (action != null) action();
        }).Restart();
    }
    public void PauseTween()
    {
        StopAllCoroutines();
        BtnGroupSequence.Pause();
        UIPart.RecoverState();
    }
    /// <summary>
    /// 立刻完成所有动画
    /// </summary>
    public void CompleteTween()
    {
        StopAllCoroutines();
        BtnGroupSequence.Rewind();
        BtnGroupSequence.Complete();
        //ResetTwinkle();
    }
    /// <summary>
    /// 按钮组动画序列
    /// </summary>
    private Sequence BtnGroupSequence; 
    /// <summary>
    /// 初始化动画序列
    /// </summary>
    private void InitSequence()
    {
        BtnGroupSequence = DOTween.Sequence();
        Tween BtnBgTween = UIPart.ButtonGroupBg.GetComponent<RectTransform>().DOScaleY(1, 0.3f).OnComplete(()=> 
        {
            //if(FullViewTweenController.IsTweening || !FullViewTweenController.displayFullViewTween)
            //{
            //    TwinkleObj(UIPart.FullFactoryImage.gameObject,0.1f,0.02f);
            //    TwinkleObj(UIPart.FactoryImage.gameObject, 0.1f, 0.02f);
            //    TwinkleObj(UIPart.BoilerImage.gameObject, 0.1f, 0.02f);
            //    TwinkleObj(UIPart.WaterImage.gameObject, 0.1f, 0.02f);
            //    TwinkleObj(UIPart.GasEnergyImage.gameObject, 0.1f, 0.02f);
            //    TwinkleObj(UIPart.LifeAreaImage.gameObject, 0.1f, 0.02f);
            //}
        }).SetEase(Ease.OutCubic);
        Tween ImageTween1 = UIPart.FullFactoryImage.DOFade(1,0.1f).SetLoops(4,LoopType.Restart);
        Tween ImageTween2 = UIPart.FactoryImage.DOFade(1, 0.1f).SetLoops(4, LoopType.Restart);
        Tween ImageTween3 = UIPart.BoilerImage.DOFade(1, 0.1f).SetLoops(4, LoopType.Restart);
        Tween ImageTween4 = UIPart.WaterImage.DOFade(1, 0.1f).SetLoops(4, LoopType.Restart);
        Tween ImageTween5 = UIPart.GasEnergyImage.DOFade(1, 0.1f).SetLoops(4, LoopType.Restart);
        Tween ImageTween6 = UIPart.LifeAreaImage.DOFade(1, 0.1f).SetLoops(4, LoopType.Restart);

        Tween GridBgTween = UIPart.BgGrid.transform.DOScale(Vector3.one,0.2f);
        //按钮和文本框
        float showTextTime = 0.3f;
        Tween FullFactory = UIPart.FullFactoryText.DOText(GetTextString(UIPart.FullFactoryText),showTextTime);
        Tween FactoryTween = UIPart.FactoryText.DOText(GetTextString(UIPart.FactoryText), showTextTime);      
        Tween BoilerTween = UIPart.BoilerText.DOText(GetTextString(UIPart.BoilerText), showTextTime);
        Tween WaterTween = UIPart.WaterText.DOText(GetTextString(UIPart.WaterText), showTextTime);
        Tween GasEnergyTween = UIPart.GasEnergyText.DOText(GetTextString(UIPart.GasEnergyText), showTextTime);
        Tween LifeAreaTween = UIPart.LifeAreaText.DOText(GetTextString(UIPart.LifeAreaText), showTextTime).OnComplete(()=> 
        {
            Debug.Log("Button Group Tween Complete!");
            //Invoke("ShowImage",0.5f);
        });


        BtnGroupSequence.Append(BtnBgTween);
        BtnGroupSequence.Join(ImageTween1);
        BtnGroupSequence.Join(ImageTween2);
        BtnGroupSequence.Join(ImageTween3);
        BtnGroupSequence.Join(ImageTween4);
        BtnGroupSequence.Join(ImageTween5);
        BtnGroupSequence.Join(ImageTween6);
        BtnGroupSequence.Join(GridBgTween);
        //按钮和文本框
        BtnGroupSequence.Append(FullFactory);
        BtnGroupSequence.Join(FactoryTween);
        BtnGroupSequence.Join(BoilerTween);
        BtnGroupSequence.Join(WaterTween);
        BtnGroupSequence.Join(GasEnergyTween);
        BtnGroupSequence.Join(LifeAreaTween);
        //BtnGroupSequence.AppendCallback(ShowImage);

        BtnGroupSequence.SetAutoKill(false);
        BtnGroupSequence.Pause();
    }
    /// <summary>
    /// 隐藏所有Toggle照片
    /// </summary>
    private void HideAllImage()
    {
        UIPart.FactoryImage.gameObject.SetActive(false);
        UIPart.WaterImage.gameObject.SetActive(false);
        UIPart.BoilerImage.gameObject.SetActive(false);
        UIPart.GasEnergyImage.gameObject.SetActive(false);
        UIPart.LifeAreaImage.gameObject.SetActive(false);
        UIPart.FullFactoryImage.gameObject.SetActive(false);
    }
    /// <summary>
    /// 获取并清除文本框
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private string GetTextString(Text t)
    {
        string value = t.text;
        t.text = "";
        return value;
    }
    private void ShowImage()
    {
        //Debug.Log("Show Toggle Image!");
        UIPart.FullFactoryImage.GetComponentInParent<Toggle>().enabled = false;
        UIPart.FactoryImage.GetComponentInParent<Toggle>().enabled = false;
        UIPart.BoilerImage.GetComponentInParent<Toggle>().enabled = false;
        UIPart.WaterImage.GetComponentInParent<Toggle>().enabled = false;
        UIPart.GasEnergyImage.GetComponentInParent<Toggle>().enabled = false;
        UIPart.LifeAreaImage.GetComponentInParent<Toggle>().enabled = false;

        UIPart.FullFactoryImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
        UIPart.FactoryImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
        UIPart.BoilerImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
        UIPart.WaterImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
        UIPart.GasEnergyImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
        UIPart.LifeAreaImage.transform.GetChild(0).GetComponent<Image>().enabled = true;

        UIPart.FullFactoryImage.GetComponentInParent<Toggle>().enabled = true;
        UIPart.FactoryImage.GetComponentInParent<Toggle>().enabled = true;
        UIPart.BoilerImage.GetComponentInParent<Toggle>().enabled = true;
        UIPart.WaterImage.GetComponentInParent<Toggle>().enabled = true;
        UIPart.GasEnergyImage.GetComponentInParent<Toggle>().enabled = true;
        UIPart.LifeAreaImage.GetComponentInParent<Toggle>().enabled = true;
    }
    /// <summary>
    /// 重置所有Twinkle
    /// </summary>
    private void ResetTwinkle()
    {
        if (!UIPart.BgGrid.gameObject.activeInHierarchy) UIPart.BgGrid.gameObject.SetActive(true);
        if (!UIPart.BoilerImage.gameObject.activeInHierarchy) UIPart.BoilerImage.gameObject.SetActive(true);
        if (!UIPart.WaterImage.gameObject.activeInHierarchy) UIPart.WaterImage.gameObject.SetActive(true);
        if (!UIPart.GasEnergyImage.gameObject.activeInHierarchy) UIPart.GasEnergyImage.gameObject.SetActive(true);
        if (!UIPart.LifeAreaImage.gameObject.activeInHierarchy) UIPart.LifeAreaImage.gameObject.SetActive(true);
        if (!UIPart.FactoryImage.gameObject.activeInHierarchy) UIPart.FactoryImage.gameObject.SetActive(true);
        if (!UIPart.FullFactoryImage.gameObject.activeInHierarchy) UIPart.FullFactoryImage.gameObject.SetActive(true);
    }

    #region Helper Part
    /// <summary>
    /// 闪烁时间
    /// </summary>
    private WaitForSeconds TwinkeWaitTime = new WaitForSeconds(0.02f);
    /// <summary>
    /// 物体闪烁
    /// </summary>
    private void TwinkleObj(GameObject obj, float waitTime, float gapTime = 0.02f, Action action = null)
    {      
        StartCoroutine(twinkleCorutine(obj, waitTime, gapTime, action));
    }
    IEnumerator twinkleCorutine(GameObject obj, float waitTime, float gapTime, Action action = null)
    {
        WaitForSeconds TwinkeGapTime = new WaitForSeconds(gapTime);
        yield return new WaitForSeconds(waitTime);
        obj.SetActive(false);
        yield return TwinkeGapTime;
        obj.SetActive(true);
        yield return TwinkeGapTime;
        obj.SetActive(false);
        yield return TwinkeGapTime;
        obj.SetActive(true);
        yield return TwinkeGapTime;
        obj.SetActive(false);
        yield return TwinkeGapTime;
        obj.SetActive(true);
        if (action != null) action();
    }
    #endregion
}
