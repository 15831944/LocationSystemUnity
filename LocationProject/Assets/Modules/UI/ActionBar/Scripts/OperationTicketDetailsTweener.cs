using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationTicketDetailsTweener : MonoBehaviour {
    public static OperationTicketDetailsTweener Instance;
    /// <summary>
    /// 操作票信息
    /// </summary>
    public GameObject OperationTicketDetails;

    public Tween OperationTicketCloseTween;
    public Tween OperationTicketOpenTween;

    public CanvasGroup OperationTicketGroup;

    public Tween OperationTicketCloseDisappear;
    public Tween OperationTicketOpenAppear;
    void Start () {
        Instance = this;
        OperationTicketGroup = OperationTicketDetails.transform.GetComponent<CanvasGroup>();
    }
	public void OperationTicketDetailsCloseTween()
    {
        OperationTicketCloseTween = OperationTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(738f, 1f);
        OperationTicketCloseDisappear = DOTween.To(() => OperationTicketGroup.alpha, x => OperationTicketGroup.alpha = x, 0, 1f);
    }
    public void OperationTicketDetailsOpernTween()
    {
        OperationTicketOpenTween = OperationTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(0f, 1f);
        OperationTicketOpenAppear = DOTween.To(() => OperationTicketGroup.alpha, x => OperationTicketGroup.alpha = x, 1, 1f);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
