using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullViewTweenCircleController : MonoBehaviour {

    /// <summary>
    /// 圆环UI部分
    /// </summary>
    public FullViewTweenCircleUI CircleUI;
    // Use this for initialization
    void Awake()
    {
        InitCircleRotateTween();
        InitCircleSequence();

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartTween();
        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    PauseTween();
        //}
    }
    /// <summary>
    /// 停止所有动画
    /// </summary>
    public void PauseTween()
    {
        StopAllCoroutines();
        CircleUI.RecoverState();
        CirclePartSequence.Pause();
        PauseCircleTween();
    }
    /// <summary>
    /// 开始圆环部分动画
    /// </summary>
    public void StartTween(Action action=null)
    {
        if (CircleUI)
        {
            TwinkleObj(CircleUI.VerticalLine, 0, 0.02f, () =>
            {
                if(FullViewTweenController.IsTweening||!FullViewTweenController.displayFullViewTween)
                {
                    CircleUI.HorizontalLine.SetActive(true);
                    CirclePartSequence.OnComplete(() =>
                    {
                        if (FullViewTweenController.IsTweening || !FullViewTweenController.displayFullViewTween)
                        {
                            ShowCirclePart(() =>
                            {
                                PlayCircleRotateTween();
                                if (action != null) action();
                            });
                        }                            
                    }).Restart();
                }
            });
        }
    }
    /// <summary>
    /// 立刻完成所有动画
    /// </summary>
    public void CompleteTween()
    {
        StopAllCoroutines();
        ResetTwinkle();
        //恢复初始
        CirclePartSequence.Rewind();
        CircleFillAmount1.Rewind();
        CircleFillAmount2.Rewind();
        //立即结束
        CirclePartSequence.Complete();
        CircleFillAmount1.Complete();
        CircleFillAmount2.Complete();
        CircleUI.Circle2.transform.localScale = Vector3.one;
        PlayCircleRotateTween();
    }
    /// <summary>
    /// 大圆旋转一圈所需时间
    /// </summary>
    private float CircleRotateTime = 20f;
    /// <summary>
    /// 小圆线，摇摆一次所需时间
    /// </summary>
    private float CircleShakeTime = 2f;
    /// <summary>
    /// 圆环动画1
    /// </summary>
    private Tween CircleTween1;
    /// <summary>
    /// 圆环动画2
    /// </summary>
    private Tween CircleTween2;
    /// <summary>
    /// 圆环动画3
    /// </summary>
    private Tween CircleTween3;

    private Tween CircleFillAmount1;
    private Tween CircleFillAmount2;
    /// <summary>
    /// 初始化图片旋转动画
    /// </summary>
    private void InitCircleRotateTween()
    {
        CircleTween1 = CircleUI.Circle3.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 360), CircleRotateTime, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        CircleTween2 = CircleUI.Circle4.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 45), CircleShakeTime).SetLoops(-1, LoopType.Yoyo);
        CircleTween3 = CircleUI.Circle6.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, -360), CircleRotateTime, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

        CircleFillAmount1= CircleUI.Circle5.GetComponent<Image>().DOFillAmount(1, 0.5f).SetEase(Ease.Linear);
        CircleFillAmount2= CircleUI.Circle2.GetComponent<Image>().DOFillAmount(1, 0.5f).SetEase(Ease.Linear);
        CircleFillAmount1.SetAutoKill(false);
        CircleFillAmount2.SetAutoKill(false);
        CircleFillAmount1.Pause();
        CircleFillAmount2.Pause();
    }
    /// <summary>
    /// 播放图片旋转动画
    /// </summary>
    private void PlayCircleRotateTween()
    {
        CircleTween1.Restart();
        CircleTween2.Restart();
        CircleTween3.Restart();
    }
    /// <summary>
    /// 暂停图片旋转动画
    /// </summary>
    private void PauseCircleTween()
    {
        CircleTween1.Pause();
        CircleTween2.Pause();
        CircleTween3.Pause();
    }
    /// <summary>
    /// 圆环区域的动画序列
    /// </summary>
    private Sequence CirclePartSequence;
    /// <summary>
    /// 水平线最终角度
    /// </summary>
    private Vector3 HorizontalVec = new Vector3(0, 0, 90);
    /// <summary>
    /// 十字线旋转所需时间
    /// </summary>
    private float HorizontalRotateTime = 0.28f;
    /// <summary>
    /// 初始化动画序列
    /// </summary>
    private void InitCircleSequence()
    {
        if (CircleUI)
        {
            CirclePartSequence = DOTween.Sequence();
            RectTransform rec = CircleUI.HorizontalLine.GetComponent<RectTransform>();
            Tween first = rec.DOLocalRotate(HorizontalVec, HorizontalRotateTime).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                //Debug.LogError("First Finish!");
                TwinkleObj(CircleUI.SmallLeftLine, 0.16f, 0.04f);
                TwinkleObj(CircleUI.SmallRightLine, 0.16f, 0.04f);
            });
            Tween second = CircleUI.SmallLine.transform.DOScale(Vector3.one, 0.5f);
            //中间x小短线部分
            Tween third = CircleUI.SmallLeftLine.transform.DOLocalRotate(new Vector3(0, 0, 75), 0.1f).SetEase(Ease.OutCubic);
            Tween forth = CircleUI.SmallRightLine.transform.DOLocalRotate(new Vector3(0, 0, -75), 0.1f).SetEase(Ease.OutCubic);

            Tween fifth = CircleUI.SmallLeftLine.transform.DOLocalRotate(new Vector3(0, 0, 15), 0.16f).SetEase(Ease.InOutBack);
            Tween sixth = CircleUI.SmallRightLine.transform.DOLocalRotate(new Vector3(0, 0, -15), 0.16f).SetEase(Ease.InOutBack);

            Tween seven = CircleUI.SmallLeftLine.transform.DOLocalRotate(new Vector3(0, 0, 45), 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                //Debug.LogError("Small triangle setActive!");
                CircleUI.SmallLineTriangle.SetActive(true);
            });
            Tween eight = CircleUI.SmallRightLine.transform.DOLocalRotate(new Vector3(0, 0, -45), 0.1f).SetEase(Ease.OutCubic);
            //菱形线FillAmount
            Tween Diamond1 = CircleUI.DiamondLine1.DOFillAmount(1, 0.5f);
            Tween Diamond2 = CircleUI.DiamondLine2.DOFillAmount(1, 0.5f);
            Tween Diamond3 = CircleUI.DiamondLine3.DOFillAmount(1, 0.5f);
            Tween Diamond4 = CircleUI.DiamondLine4.DOFillAmount(1, 0.5f);
            //小短线对应三角形部分
            Tween nine = CircleUI.SmallLineTriangle.transform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.OutCubic);
            //Tween加入动画序列
            CirclePartSequence.Append(first);
            CirclePartSequence.Append(second);
            //小短线
            CirclePartSequence.Append(third);
            CirclePartSequence.Join(forth);
            CirclePartSequence.Append(fifth);
            CirclePartSequence.Join(sixth);
            CirclePartSequence.Append(seven);
            CirclePartSequence.Join(eight);
            //菱形线FillAmount
            CirclePartSequence.Join(Diamond1);
            CirclePartSequence.Join(Diamond2);
            CirclePartSequence.Join(Diamond3);
            CirclePartSequence.Join(Diamond4);
            //小三角形动画
            CirclePartSequence.Join(nine);


            CirclePartSequence.SetAutoKill(false);
            CirclePartSequence.Pause();
        }

    }
    /// <summary>
    /// 显示圆环区域部分
    /// </summary>
    /// <param name="onComplete"></param>
    private void ShowCirclePart(Action onComplete = null)
    {
        StartCoroutine(showCirclePartCorutine(onComplete));
    }
    IEnumerator showCirclePartCorutine(Action action = null)
    {
        //CircleUI.Circle5.GetComponent<Image>().DOFillAmount(1, 0.5f).SetEase(Ease.Linear);
        CircleFillAmount1.Restart();
        yield return null;
        CircleUI.Circle5.SetActive(true);
        CircleUI.Circle2.SetActive(true);
        yield return TwinkeWaitTime;
        //CircleUI.Circle2.GetComponent<Image>().DOFillAmount(1, 0.5f).SetEase(Ease.Linear);
        CircleFillAmount2.Restart();
        yield return new WaitForSeconds(0.1f);
        CircleUI.Circle5.SetActive(false);
        CircleUI.Circle2.SetActive(false);
        yield return TwinkeWaitTime;
        CircleUI.Circle2.transform.localScale = new Vector3(0.93f, 0.93f, 1);
        CircleUI.Circle5.SetActive(true);
        CircleUI.Circle2.SetActive(true);
        yield return TwinkeWaitTime;
        CircleUI.Circle5.SetActive(false);
        CircleUI.Circle2.SetActive(false);
        yield return TwinkeWaitTime;
        CircleUI.Circle2.transform.localScale = new Vector3(0.96f, 0.96f, 1);
        CircleUI.Circle5.SetActive(true);
        CircleUI.Circle2.SetActive(true);
        yield return TwinkeWaitTime;
        CircleUI.Circle5.SetActive(false);
        CircleUI.Circle2.SetActive(false);
        yield return TwinkeWaitTime;
        CircleUI.Circle2.transform.localScale = Vector3.one;
        CircleUI.Circle5.SetActive(true);
        CircleUI.Circle2.SetActive(true);
        yield return TwinkeWaitTime;

        CircleUI.Circle6.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        CircleUI.Circle4.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        CircleUI.Circle3.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        CircleUI.Circle1.SetActive(true);
        CircleUI.Circle6.SetActive(false);
        CircleUI.Circle4.SetActive(false);
        yield return new WaitForSeconds(0.015f);
        CircleUI.Circle1.SetActive(false);
        CircleUI.Circle6.SetActive(true);
        CircleUI.Circle4.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        CircleUI.Circle1.SetActive(true);
        CircleUI.Circle6.SetActive(false);
        CircleUI.Circle4.SetActive(false);
        yield return new WaitForSeconds(0.015f);
        CircleUI.Circle1.SetActive(false);
        CircleUI.Circle6.SetActive(true);
        CircleUI.Circle4.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        CircleUI.Circle1.SetActive(true);
        CircleUI.Circle6.SetActive(true);
        CircleUI.Circle4.SetActive(true);
        if (action != null) action();
    }
    /// <summary>
    /// 重置所有闪烁
    /// </summary>
    private void ResetTwinkle()
    {
        
        if (!CircleUI.Circle1.activeInHierarchy) CircleUI.Circle1.SetActive(true);
        if (!CircleUI.Circle2.activeInHierarchy) CircleUI.Circle2.SetActive(true);
        if (!CircleUI.Circle3.activeInHierarchy) CircleUI.Circle3.SetActive(true);

        if (!CircleUI.Circle4.activeInHierarchy) CircleUI.Circle4.SetActive(true);
        if (!CircleUI.Circle5.activeInHierarchy) CircleUI.Circle5.SetActive(true);
        if (!CircleUI.Circle6.activeInHierarchy) CircleUI.Circle6.SetActive(true);

        if (!CircleUI.VerticalLine.activeInHierarchy) CircleUI.VerticalLine.SetActive(true);
        if (!CircleUI.HorizontalLine.activeInHierarchy) CircleUI.HorizontalLine.SetActive(true);

        if (!CircleUI.SmallRightLine.activeInHierarchy) CircleUI.SmallLeftLine.SetActive(true);
        if (!CircleUI.SmallLeftLine.activeInHierarchy) CircleUI.SmallLeftLine.SetActive(true);
        if (!CircleUI.SmallLineTriangle.activeInHierarchy) CircleUI.SmallLineTriangle.SetActive(true);
        //if (!CircleUI.Circle9) CircleUI.Circle1.SetActive(true);
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
