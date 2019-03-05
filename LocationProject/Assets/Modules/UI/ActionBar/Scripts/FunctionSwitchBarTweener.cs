using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionSwitchBarTweener : MonoBehaviour {

    public static FunctionSwitchBarTweener Instance;
    public GameObject FunctionSwitch;

    public Tween FunctionSwitchCloseTween;
    public Tween FunctionSwitchOpenTween;

    public CanvasGroup FunctionSwitchGroup;

    public Tween FunctionSwitchCloseDisappear;
    public Tween FunctionSwitchOpenAppear;
    void Start()
    {
        Instance = this;
        FunctionSwitchGroup = FunctionSwitch.transform.GetComponent<CanvasGroup>();

    }

    public void FunctionSwitchBarCloseTween()
    {
        FunctionSwitchCloseTween = FunctionSwitch.transform.GetComponent<RectTransform>().DOMoveY(73, 1f);
        FunctionSwitchCloseDisappear = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 0, 1f);
    }
    public void FunctionSwitchBarOpernTween()
    {
        FunctionSwitchOpenTween = FunctionSwitch.transform.GetComponent<RectTransform>().DOMoveY(140, 1f);
        FunctionSwitchOpenAppear = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 1, 1f);
    }
}
