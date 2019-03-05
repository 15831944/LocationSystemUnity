using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class FVTPhotoController : MonoBehaviour {
    /// <summary>
    /// 照片区域按钮区域
    /// </summary>
    public FVTPhotoUI UIPart;
	// Use this for initialization
	void Awake () {
        InitSequence();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void StartTween(Action action=null)
    {
        StartTwinkleAnimation();
        BtnGroupSequence.OnComplete(()=> 
        {
            if (action != null) action();
        }).Restart();
    }
    public void StopTween()
    {
        BtnGroupSequence.Pause();
        UIPart.RecoverState();
    }
    public void CompleteTween()
    {
        StopAllCoroutines();
        ResetTwinkle();
        BtnGroupSequence.Rewind();
        BtnGroupSequence.Complete();
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

        //3D模块区域
        Tween showThreeD = UIPart.ThreeDAreaImage.transform.DOScaleX(1,0.3f);
        Tween ThreeDBottomLine = UIPart.ThreeDBottomLine.transform.DOScaleX(1,0.3f);

        Tween PhotoAreaBG = UIPart.PhotoAreaBg.transform.DOScaleX(1,0.5f);

        Tween introduceTitle = UIPart.introduceTitleText.DOText(GetTextString(UIPart.introduceTitleText),0.2f);
        Tween introduceContent = UIPart.introduceContentText.DOText(GetTextString(UIPart.introduceContentText),0.5f);

        //最后的三根装饰线动画
        Tween circleTopToBottom = UIPart.ExtensionLine.transform.DOScaleY(1,0.3f);
        Tween leftToRight = UIPart.LeftToRightLine.transform.DOScaleX(1,0.3f);
        Tween photoTopToBottom = UIPart.TopToBottomLine.transform.DOScaleY(1,0.3f);
        //三个按钮的文字效果
        //Tween posText = UIPart.PosBtnText.DOText(GetTextString(UIPart.PosBtnText),0.5f);
        //Tween projectScaleText = UIPart.projectSacelBtnText.DOText(GetTextString(UIPart.projectSacelBtnText), 0.5f);
        Tween enterFactoryText = UIPart.enterFactoryBtnText.DOText(GetTextString(UIPart.enterFactoryBtnText), 0.5f);

        BtnGroupSequence.AppendInterval(0.3f);
        BtnGroupSequence.Append(showThreeD);
        BtnGroupSequence.Join(ThreeDBottomLine);
        BtnGroupSequence.Join(PhotoAreaBG);
        //按钮文本部分
        //BtnGroupSequence.Join(posText);
        //BtnGroupSequence.Join(projectScaleText);
        BtnGroupSequence.Join(enterFactoryText);

        //标题栏文本
        BtnGroupSequence.Join(introduceTitle);
        BtnGroupSequence.Join(introduceContent);
        //最后三根线部分
        BtnGroupSequence.Append(circleTopToBottom);
        BtnGroupSequence.Join(leftToRight);
        BtnGroupSequence.Join(photoTopToBottom);

        BtnGroupSequence.SetAutoKill(false);
        BtnGroupSequence.Pause();
    }
    /// <summary>
    /// 执行闪烁动画
    /// </summary>
    private void StartTwinkleAnimation()
    {
        TwinkleObj(UIPart.ThreeDBottomLine,0.3f);
        TwinkleObj(UIPart.ThreeDAreaImage,0.3f);
        TwinkleObj(UIPart.PhotoAreaBg,0.3f);

        float waitTime = 0.5f;
        float gapTime = 0.03f;
        TwinkleObj(UIPart.circleTopDecration, waitTime, gapTime);
        TwinkleObj(UIPart.photoTopDecration, waitTime, gapTime);
        TwinkleObj(UIPart.circleLeftDecration, waitTime, gapTime);

        TwinkleObj(UIPart.AreaInfo, waitTime, gapTime);
        TwinkleObj(UIPart.enterFacotoryBtn, waitTime, gapTime);
        TwinkleObj(UIPart.appExitButton,waitTime,gapTime);

        TwinkleObj(UIPart.PhotoControlPart, waitTime, gapTime);
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
    /// <summary>
    /// 重置所有Twinkle
    /// </summary>
    private void ResetTwinkle()
    {
        if (!UIPart.ThreeDBottomLine.activeInHierarchy) UIPart.ThreeDBottomLine.SetActive(true);
        if (!UIPart.ThreeDAreaImage.activeInHierarchy) UIPart.ThreeDAreaImage.SetActive(true);
        if (!UIPart.PhotoAreaBg.activeInHierarchy) UIPart.PhotoAreaBg.SetActive(true);

        if (!UIPart.circleTopDecration.activeInHierarchy) UIPart.circleTopDecration.SetActive(true);
        if (!UIPart.photoTopDecration.activeInHierarchy) UIPart.photoTopDecration.SetActive(true);
        if (!UIPart.circleLeftDecration.activeInHierarchy) UIPart.circleLeftDecration.SetActive(true);

        if (!UIPart.AreaInfo.activeInHierarchy) UIPart.AreaInfo.SetActive(true);
        if (!UIPart.enterFacotoryBtn.activeInHierarchy) UIPart.enterFacotoryBtn.SetActive(true);
        if (!UIPart.appExitButton.activeInHierarchy) UIPart.appExitButton.SetActive(true);
        if (!UIPart.PhotoControlPart.activeInHierarchy) UIPart.PhotoControlPart.SetActive(true);
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
