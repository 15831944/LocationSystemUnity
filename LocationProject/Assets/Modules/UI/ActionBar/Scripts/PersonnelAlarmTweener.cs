using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonnelAlarmTweener : MonoBehaviour {

    public GameObject PerAlarm;
    public Tween PerAlarmAppearTween;
    public Tween PerAlarmMoveTween;
    public CanvasGroup PerAlarmGroup;
    void Start () {
        PerAlarmGroup = PerAlarm.transform.GetComponent<CanvasGroup>();
    }

    public void PersonnelAlarmTween()
    {
        PerAlarmMoveTween = PerAlarm.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        PerAlarmAppearTween = DOTween.To(() => PerAlarmGroup.alpha, x => PerAlarmGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
