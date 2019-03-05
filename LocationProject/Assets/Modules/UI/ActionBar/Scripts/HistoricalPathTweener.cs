using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoricalPathTweener : MonoBehaviour {

    public GameObject HistoricalPath;
    public Tween HistoricalPathAppearTween;
    public Tween HistoricalPathMoveTween;
    public CanvasGroup HistoricalPathGroup;
    void Start () {
        HistoricalPathGroup = HistoricalPath.transform.GetComponent<CanvasGroup>();
    }

    public void HistoricalPathTween()
    {
        HistoricalPathMoveTween = HistoricalPath.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        HistoricalPathAppearTween = DOTween.To(() => HistoricalPathGroup.alpha, x => HistoricalPathGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
