using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTicketDetailsTweener : MonoBehaviour {
    public static WorkTicketDetailsTweener Instance;
    /// <summary>
    /// 工作票详细信息
    /// </summary>
    public GameObject WorkTicketDetails;

    public Tween WorkTicketCloseTween;
    public Tween WorkTicketOpenTween;

    public CanvasGroup WorkTicketGroup;

    public Tween WorkTicketCloseDisappear;
    public Tween WorkTicketOpenAppear;
    void Start () {
        Instance = this;
        WorkTicketGroup = WorkTicketDetails.transform.GetComponent<CanvasGroup>();

    }
	public void WorkTicketDetailsCloseTween()
    {
        WorkTicketCloseTween = WorkTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(738f, 1f);
        WorkTicketCloseDisappear = DOTween.To(() => WorkTicketGroup.alpha, x => WorkTicketGroup.alpha = x, 0, 1f);
    }
    public void WorkTicketDetailsOpernTween()
    {
        WorkTicketOpenTween = WorkTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(0f, 1f);
        WorkTicketOpenAppear = DOTween.To(() => WorkTicketGroup.alpha, x => WorkTicketGroup.alpha = x, 1, 1f);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
