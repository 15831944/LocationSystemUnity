using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchTweener : MonoBehaviour {

    public GameObject Search;
    public Tween SearchAppearTween;
    public Tween SearchMoveTween;
    public CanvasGroup SearchGroup;
    void Start () {
        SearchGroup = Search.transform.GetComponent<CanvasGroup>();
    }

    public void SearchTween()
    {
        SearchMoveTween = Search.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        SearchAppearTween = DOTween.To(() => SearchGroup.alpha, x => SearchGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
