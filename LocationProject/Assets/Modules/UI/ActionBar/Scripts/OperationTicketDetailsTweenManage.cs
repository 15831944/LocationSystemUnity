using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationTicketDetailsTweenManage : MonoBehaviour {
    public static OperationTicketDetailsTweenManage Instance;

    public GameObject startOut;//开始结束
    public CanvasGroup startOutGroup;
    public Tween startOutAppearTween;
    public Tween startOutMoveTween;

    //   public CanvasGroup actionBarGroup;//操作栏
    public GameObject actionBar;
    public Tween actionBarAppearTween;

    public GameObject personnelPos;//人员
    private Tween PerPosAppearTween;
    private Tween PerPosMoveTween;
    public CanvasGroup PerPosGroup;

    public GameObject DevPos;//设备
    private Tween DevPosAppearTween;
    private Tween DevPosMoveTween;
    public CanvasGroup DevPosGroup;

    public GameObject MobilePatrol;
    private Tween MobilePatrolAppearTween;//移动巡检
    private Tween MobilePatrolMoveTween;
    public CanvasGroup MobilePatrolGroup;

    public GameObject TwoVotes;
    private Tween TwoVotesAppearTween;//两票
    private Tween TwoVotesMoveTween;
    public CanvasGroup TwoVotesGroup;

    public GameObject WorkTicketDetails;//两票列表
    public Tween WorkTicketMoveTween;
    public Tween WorkTicketAppearTween;
    public CanvasGroup WorkTicketGroup;

    public GameObject FunctionSwitchBar;
    public Tween FunctionSwitchAppearTween;
    public CanvasGroup FunctionSwitchGroup;

    public GameObject OperationTicketDetails;
    public Tween OperationTicketMoveTween;
    public Tween OperationTicketAppearTween;
    public CanvasGroup OperationTicketGroup;

    public Text ViewMode;
    private Tween ViewModeTween;
    public Text Feature;
    private Tween FeatureTween;

    public Sequence MobilePatrolTweenSequence;
    void Start () {
        Instance = this;

    }
    private void Awake()
    {
        MobilePatrolTweenSequence = DOTween.Sequence();
        SetFunctionSwitchBarTween();
    }
 
	public void SetFunctionSwitchBarTween()
    {

        ViewModeTween = ViewMode.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 0 / 255f), 0.5f).SetEase(Ease.OutBack);
        FeatureTween = Feature.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 0 / 255f), 0.5f).SetEase(Ease.OutBack);

        startOutMoveTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-220f, 0f), 0.5f).SetEase(Ease.OutBack);
        startOutAppearTween = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        FunctionSwitchAppearTween = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 0, 0.5f).SetEase(Ease.OutBack);

        actionBarAppearTween = actionBar.transform.GetComponent<Image>().DOColor(new Color(0 / 255f, 0 / 255f, 0 / 255f, 0 / 255f), 0.48f).SetEase(Ease.OutBack);

        PerPosMoveTween = personnelPos.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        PerPosAppearTween = DOTween.To(() => PerPosGroup.alpha, x => PerPosGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        DevPosMoveTween = DevPos.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        DevPosAppearTween = DOTween.To(() => DevPosGroup.alpha, x => DevPosGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        MobilePatrolMoveTween = MobilePatrol.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        MobilePatrolAppearTween = DOTween.To(() => MobilePatrolGroup.alpha, x => MobilePatrolGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        TwoVotesMoveTween = TwoVotes.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        TwoVotesAppearTween = DOTween.To(() => TwoVotesGroup.alpha, x => TwoVotesGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        OperationTicketMoveTween = OperationTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(738f, 1f);
        OperationTicketAppearTween = DOTween.To(() => OperationTicketGroup.alpha, x => OperationTicketGroup.alpha = x, 0, 1f);

        WorkTicketMoveTween = WorkTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(-1149f, 1f);
        WorkTicketAppearTween = DOTween.To(() => WorkTicketGroup.alpha, x => WorkTicketGroup.alpha = x, 0, 1f);



        MobilePatrolTweenSequence.Append(actionBarAppearTween);
        MobilePatrolTweenSequence.Join(WorkTicketMoveTween);
        MobilePatrolTweenSequence.Join(WorkTicketAppearTween);

        MobilePatrolTweenSequence.Join(ViewModeTween);
        MobilePatrolTweenSequence.Join(FeatureTween);

        MobilePatrolTweenSequence.Join(MobilePatrolMoveTween);//移动
        MobilePatrolTweenSequence.Join(TwoVotesAppearTween);
        MobilePatrolTweenSequence.Join(FunctionSwitchAppearTween);

        MobilePatrolTweenSequence.Insert(0.025f, MobilePatrolAppearTween);
        MobilePatrolTweenSequence.Join(TwoVotesAppearTween);

        MobilePatrolTweenSequence.Insert(0.1f, DevPosAppearTween);
        MobilePatrolTweenSequence.Join(PerPosAppearTween);

        MobilePatrolTweenSequence.Join(OperationTicketMoveTween);
        MobilePatrolTweenSequence.Join(OperationTicketAppearTween);

        MobilePatrolTweenSequence.Insert(0.125f, DevPosMoveTween);
        MobilePatrolTweenSequence.Join(PerPosAppearTween);

        MobilePatrolTweenSequence.SetAutoKill(false);
        MobilePatrolTweenSequence.Pause();
       // MobilePatrolTweenSequence.Rewind(false);
    }
    public void SetMobilePatrol_click()
    {
        startOutGroup.interactable = true;
        PerPosGroup.interactable = true;
        DevPosGroup.interactable = true;
        MobilePatrolGroup.interactable = true;
        TwoVotesGroup.interactable = true;
        OperationTicketGroup.interactable = true;
        FunctionSwitchGroup.interactable = true;
        WorkTicketGroup.interactable = true;
    }

    public void SetMobilePatrol_normal()
    {
        startOutGroup.interactable = false;
        PerPosGroup.interactable = false;
        DevPosGroup.interactable = false;
        MobilePatrolGroup.interactable = false;
        TwoVotesGroup.interactable = false;
        OperationTicketGroup.interactable = false;
        FunctionSwitchGroup.interactable = false;
        WorkTicketGroup.interactable = false;
    }
    void Update () {
		
	}
}
