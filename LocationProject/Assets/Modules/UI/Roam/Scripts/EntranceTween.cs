using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntranceTween : MonoBehaviour {
    public static EntranceTween instance;
    /// <summary>
    /// 第一个入口上下摆动的动画
    /// </summary>
    public Tweener FirstTweener;
    /// <summary>
    /// 第二个入口上下摆动的动画
    /// </summary>
    public Tweener SecoundTweener;
    /// <summary>
    /// 第三个入口上下摆动的动画
    /// </summary>
    public Tweener ThirdTweener;
    /// <summary>
    /// 第四个入口上下摆动的动画
    /// </summary>
    public Tweener ForthTweener;
    /// <summary>
    /// 第五个入口上下摆动的动画
    /// </summary>
    public Tweener FiveTweener;

    /// <summary>
    /// 第一个入口
    /// </summary>
    public Transform FirstTransform;
    /// <summary>
    /// 第二个入口
    /// </summary>
    public Transform SecoundTransform;
    /// <summary>
    /// 第三个入口
    /// </summary>
    public Transform ThirdTransform;
    /// <summary>
    /// 第四个入口
    /// </summary>
    public Transform ForthTransform;
    /// <summary>
    /// 第五个入口
    /// </summary>
    public Transform FiveTransform;

  

    void Start () {
        instance = this;
        CreateFirstTweener();
        CreateSecoundTweener();
        CreateThirdTweener();
        CreateForthTweener();
        CreateFiveTweener();
    
        
    }
	/// <summary>
    /// 第一个入口上下摆动动画
    /// </summary>
	public void CreateFirstTweener()
    {
        FirstTweener = FirstTransform.GetComponent<RectTransform>().DOLocalMoveY(10, 0.38f);
        FirstTweener.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        FirstTweener.Pause();
        FirstTweener.SetAutoKill(false);
      }
    /// <summary>
    /// 第二个入口上下摆动的动画
    /// </summary>
    public void CreateSecoundTweener()
    {
        SecoundTweener = SecoundTransform.GetComponent<RectTransform>().DOLocalMoveY(10, 0.38f);
        SecoundTweener.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        SecoundTweener.Pause();
        SecoundTweener.SetAutoKill(false);
    }
    /// <summary>
    /// 第三个入口上下摆动的动画
    /// </summary>
    public void CreateThirdTweener()
    {
        ThirdTweener = ThirdTransform.GetComponent<RectTransform>().DOLocalMoveY(10, 0.38f);
        ThirdTweener.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        ThirdTweener.Pause();
        ThirdTweener.SetAutoKill(false);
    }
    /// <summary>
    /// 第四个入口上下摆动的动画
    /// </summary>
    public void CreateForthTweener()
    {
        ForthTweener = ForthTransform.GetComponent<RectTransform>().DOLocalMoveY(10, 0.38f);
        ForthTweener.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        ForthTweener.Pause();
        ForthTweener.SetAutoKill(false);
    }
    /// <summary>
    /// 第五个入口上下摆动的动画
    /// </summary>
    public void CreateFiveTweener()
    {
        FiveTweener = FiveTransform.GetComponent<RectTransform>().DOLocalMoveY(10, 0.38f);
        FiveTweener.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        FiveTweener.Pause();
        FiveTweener.SetAutoKill(false);
    }
    /// <summary>
    /// 鼠标放上ui界面时播放的动画
    /// </summary>
    /// <param name="obj"></param>
    public void SetButtonEnterTweener(Transform obj)
    {
        if (obj == FirstTransform)
        {
            FirstTransform.GetComponent<RectTransform>().DOLocalMoveY(50, 0.6f).SetEase(Ease.InOutSine).OnComplete(FirstTweener.PlayForward);
        }
        else if (obj == SecoundTransform)
        {
            SecoundTransform.GetComponent<RectTransform>().DOLocalMoveY(50, 0.6f).SetEase(Ease.InOutSine).OnComplete(SecoundTweener.PlayForward);
        }
        else if (obj == ThirdTransform)
        {
            ThirdTransform.GetComponent<RectTransform>().DOLocalMoveY(50, 0.6f).SetEase(Ease.InOutSine).OnComplete(ThirdTweener.PlayForward);
        }
        else if (obj == ForthTransform)
        {
            ForthTransform.GetComponent<RectTransform>().DOLocalMoveY(50, 0.6f).SetEase(Ease.InOutSine).OnComplete(ForthTweener.PlayForward);
        }
        else if (obj ==FiveTransform)
        {
            FiveTransform.GetComponent<RectTransform>().DOLocalMoveY(50, 0.6f).SetEase(Ease.InOutSine).OnComplete(FiveTweener.PlayForward);
        }
    }
    /// <summary>
    /// 鼠标离开时播放的动画
    /// </summary>
    /// <param name="obj"></param>
   public void SetButtonExitTweener(Transform obj)
    {
        if (obj ==FirstTransform)
        {
            DOTween.Pause(FirstTransform.GetComponent<RectTransform>());
           FirstTransform.GetComponent<RectTransform>().DOLocalMoveY(0, 0.38f).SetEase(Ease.OutSine);
           instance.FirstTweener.Pause();
        }
        else if (obj ==SecoundTransform)
        {
            DOTween.Pause(SecoundTransform.GetComponent<RectTransform>());
           SecoundTransform.GetComponent<RectTransform>().DOLocalMoveY(0, 0.38f).SetEase(Ease.OutSine);
            SecoundTweener.Pause();
        }
        else if (obj ==ThirdTransform)
        {
            DOTween.Pause(ThirdTransform.GetComponent<RectTransform>());
           ThirdTransform.GetComponent<RectTransform>().DOLocalMoveY(0, 0.38f).SetEase(Ease.OutSine);
           ThirdTweener.Pause();
        }
        else if (obj ==ForthTransform)
        {
            DOTween.Pause(ForthTransform.GetComponent<RectTransform>());
            ForthTransform.GetComponent<RectTransform>().DOLocalMoveY(0, 0.38f).SetEase(Ease.OutSine);
            ForthTweener.Pause();
        }
        else if (obj ==FiveTransform)
        {
            DOTween.Pause(FiveTransform.GetComponent<RectTransform>());
            FiveTransform.GetComponent<RectTransform>().DOLocalMoveY(0, 0.38f).SetEase(Ease.OutSine);
            FiveTweener.Pause();

        }
    }
    public void CloseRoamTween()
    {
        FirstTweener.Pause();
        SecoundTweener.Pause();
        ThirdTweener.Pause();
        ForthTweener.Pause();
        FiveTweener.Pause();
    }
}
