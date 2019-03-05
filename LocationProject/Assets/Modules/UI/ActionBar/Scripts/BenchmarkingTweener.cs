using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchmarkingTweener : MonoBehaviour {

    public GameObject Benchmarking;
    public Tween BenchmarkingAppearTween;
    public Tween BenchmarkingMoveTween;
    public CanvasGroup BenchmarkingGroup;
    void Start () {
        BenchmarkingGroup = Benchmarking.transform.GetComponent<CanvasGroup>();
    }

    public void EditAreaTween()
    {
        BenchmarkingMoveTween = Benchmarking.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        BenchmarkingAppearTween = DOTween.To(() => BenchmarkingGroup.alpha, x => BenchmarkingGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
