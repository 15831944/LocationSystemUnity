using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoTicketSystemUITweener : MonoBehaviour
{
    public static TwoTicketSystemUITweener Instance;
    /// <summary>
    /// 两票系统
    /// </summary>
    public GameObject TwoTicketSystemUI;
    public Tween TwoTicketSystemCloseTween;
    public Tween TwoTicketSystemOpenTween;

    public CanvasGroup TwoTicketSystemGroup;

    public Tween TwoTicketSystemCloseDisappear;
    public Tween TwoTicketSystemOpenAppear;
    void Start()
    {
        Instance = this;
        TwoTicketSystemGroup = TwoTicketSystemUI.transform.GetComponent<CanvasGroup>();
    }
    public void TwoTicketSystemUITweenerCloseTween()
    {
        TwoTicketSystemCloseTween = TwoTicketSystemUI.transform.GetComponent<RectTransform>().DOLocalMoveX(-1198f, 1f);
        TwoTicketSystemCloseDisappear = DOTween.To(() => TwoTicketSystemGroup.alpha, x => TwoTicketSystemGroup.alpha = x, 0, 1f);
    }
    public void TwoTicketSystemUITweenerOpenTween()
    {
        TwoTicketSystemOpenTween = TwoTicketSystemUI.transform.GetComponent<RectTransform>().DOLocalMoveX(-780f, 1);
        TwoTicketSystemOpenAppear = DOTween.To(() => TwoTicketSystemGroup.alpha, x => TwoTicketSystemGroup.alpha = x, 1, 1f);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
