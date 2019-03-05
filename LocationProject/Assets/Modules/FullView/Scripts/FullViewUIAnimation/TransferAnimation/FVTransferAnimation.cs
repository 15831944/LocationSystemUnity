using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FVTransferAnimation : MonoBehaviour {
    public static FVTransferAnimation Instance;
    public GameObject ImageContent;
    public Image Circle1;
    public Image Circle2;
    public Image Circle3;

    public Image CircleIn1;
    public Image CircleIn2;
    public Image CircleIn3;
	// Use this for initialization
	void Start () {
        Instance = this;
        InitSequence();
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetKeyDown(KeyCode.S))
  //      {
  //          TransferSeq.Restart();
  //      }
	}
    private Sequence TransferSeq;
    private void InitSequence()
    {
        TransferSeq = DOTween.Sequence();
 
        Tween Out1 = Circle1.transform.DOScale(Vector3.one,0.8f).SetEase(Ease.OutBack );
        //Tween Out2 = Circle2.transform.DOScale(Vector3.one,0.8f).SetEase(Ease.OutBack).OnStart(()=> 
        //{
        //    Circle2.gameObject.SetActive(true);
        //});
        Tween Out3 = Circle3.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack).OnStart(() =>
        {
            Circle3.gameObject.SetActive(true);
        });

        Tween In1 = CircleIn1.transform.DOScale(Vector3.zero, 0.8f).SetEase(Ease.OutCubic);
        Tween In2 = CircleIn2.transform.DOScale(Vector3.zero,0.8f).SetEase(Ease.OutCubic);
        Tween In3 = CircleIn3.transform.DOScale(Vector3.zero, 0.8f).SetEase(Ease.OutCubic);

        TransferSeq.Append(Out1);
        TransferSeq.Join(In1);
        TransferSeq.Join(In2);
        TransferSeq.Join(In3);

        //TransferSeq.Insert(0.2f,Out2);
        TransferSeq.Insert(0.1f,Out3);
       
        TransferSeq.OnStart(()=> 
        {
            Debug.Log("Sequence OnStart!");
            ImageContent.SetActive(true);
        });
        TransferSeq.OnRewind(() =>
        {
            Debug.Log("Sequence OnRewind!");
            ImageContent.SetActive(true);
        });
        TransferSeq.AppendCallback(()=> 
        {
            ImageContent.SetActive(false);
        });
        TransferSeq.SetAutoKill(false);
        TransferSeq.Pause();
    }
    /// <summary>
    /// 转场动画
    /// </summary>
    /// <param name="onComplete">动画完成时间</param>
    public void DoTransferAnimation(Action onComplete)
    {
        TransferSeq.OnComplete(()=> 
        {
            Debug.Log("Sequence complete!");
            if (onComplete != null) onComplete();
        }).Restart();
    }
}
