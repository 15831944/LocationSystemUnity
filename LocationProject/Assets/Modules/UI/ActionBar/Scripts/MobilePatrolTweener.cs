using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePatrolTweener : MonoBehaviour {

    public GameObject MobilePatrol;
    public Tween MobilePatrolAppearTween;
    public Tween MobilePatrolMoveTween;
    public CanvasGroup MobilePatrolGroup;
    void Start () {
        MobilePatrolGroup = MobilePatrol.transform.GetComponent<CanvasGroup>();
    }


    public void MobilePatrolTween()
    {
        MobilePatrolMoveTween = MobilePatrol.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        MobilePatrolAppearTween = DOTween.To(() => MobilePatrolGroup.alpha, x => MobilePatrolGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
