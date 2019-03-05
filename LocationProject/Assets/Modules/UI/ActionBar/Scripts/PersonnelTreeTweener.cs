using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonnelTreeTweener : MonoBehaviour {
    public static PersonnelTreeTweener Instance;
    /// <summary>
    /// 人员树
    /// </summary>
    public GameObject personnelTree;

    public  Tween PerTreeCloseTween;
    public  Tween PerTreeOpenTween;
    public  CanvasGroup personnelTreeGroup;

    public  Tween PerTreeCloseDisappear;
    public  Tween PerTreeOpenAppear;
   
    void Start () {
        Instance = this;
        personnelTreeGroup = personnelTree.transform.GetComponent<CanvasGroup>();
    }

  
    public void PersonnelTreeCloseTween()
    {
        PerTreeCloseTween = personnelTree.transform.GetComponent<RectTransform>().DOMoveX(-183f, 1f);

        PerTreeCloseDisappear = DOTween.To(() => personnelTreeGroup.alpha, x => personnelTreeGroup.alpha = x, 0, 1f);
       
    }
    public void PersonnelTreeOpernTween()
    {
        PerTreeOpenTween = personnelTree.transform.GetComponent<RectTransform>().DOMoveX(190f, 1f);
        PerTreeOpenAppear = DOTween.To(() => personnelTreeGroup.alpha, x => personnelTreeGroup.alpha = x, 1, 1f);
    }
    }
