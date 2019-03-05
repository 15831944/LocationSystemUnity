using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopoTreeTweener : MonoBehaviour {
    public static TopoTreeTweener Instance;
    /// <summary>
    /// 设备树
    /// </summary>
    public GameObject topoTree;
    public Tween topoTreeCloseTween;
    public Tween topoTreeCloseDisappear;
    public CanvasGroup topoTreeGroup;
    public Tween topoTreeOpenTween;
    public Tween topoTreeOpenAppear;
    // Use this for initialization
    void Start()
    {
        Instance = this ;
        topoTreeGroup = topoTree.transform.GetComponent<CanvasGroup>();

    }

   
    public void TopoTreeCloseTween()
    {
        
        topoTreeCloseTween = topoTree.transform.GetComponent<RectTransform>().DOMoveX(-349f, 1f);
        topoTreeCloseDisappear = DOTween.To(() => topoTreeGroup.alpha, x => topoTreeGroup.alpha = x, 0, 1f);
    }
    public void TopoTreeOpernTween()
    {
       
        topoTreeOpenTween = topoTree.transform.GetComponent<RectTransform>().DOMoveX(20f, 1f);
        topoTreeOpenAppear = DOTween.To(() => topoTreeGroup.alpha, x => topoTreeGroup.alpha = x, 1, 1f);
    }
}
