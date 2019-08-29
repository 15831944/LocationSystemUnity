using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TwoTicketSystemUITweenManage : MonoBehaviour {
    public static TwoTicketSystemUITweenManage Instance;

    public GameObject startOut;//开始结束
    public   CanvasGroup startOutGroup;
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

    public GameObject TwoVotesHistory;
    private Tween TwoVotesHistoryAppearTween;//两票历史
    private Tween TwoVotesHistoryMoveTween;
    public CanvasGroup TwoVotesHistoryGroup;

    public GameObject FunctionSwitchBar;
    public Tween FunctionSwitchAppearTween;
    public CanvasGroup FunctionSwitchGroup;

    public GameObject WorkTicketDetails;//两票列表
    public Tween WorkTicketMoveTween;
    public Tween WorkTicketAppearTween;
    public CanvasGroup WorkTicketGroup;

    public GameObject WorkTicketInfo;//两票详细信息
    public Tween WorkTicketInfoMoveTween;
    public Tween WorkTicketInfoAppearTween;
    public CanvasGroup WorkTicketInfoGroup;

    public Text ViewMode;
    private Tween ViewModeTween;
    public Text Feature;
    private Tween FeatureTween;

    public Sequence TwoVotesTweenSequence;
    void Start () {
        Instance = this;

    }

    private void Awake()
    {
        TwoVotesTweenSequence = DOTween.Sequence();
      //  SetTwoTicketSystemUITween();
    }
 
	public void SetTwoTicketSystemUITween()
    {
        ViewModeTween = ViewMode.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 0 / 255f), 0.5f).SetEase(Ease.OutBack);
        FeatureTween = Feature.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 0 / 255f), 0.5f).SetEase(Ease.OutBack);

        startOutMoveTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-220f, 0f), 0.5f).SetEase(Ease.OutBack);
        startOutAppearTween = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        FunctionSwitchAppearTween = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 0, 0.5f).SetEase(Ease.OutBack);

        actionBarAppearTween = actionBar.transform.GetComponent<Image>().DOColor(new Color(0 / 255f, 0 / 255f, 0 / 255f, 0 / 255f), 0.6f).SetEase(Ease.OutBack);

        WorkTicketMoveTween = WorkTicketDetails.transform.GetComponent<RectTransform>().DOLocalMoveX(-1149f, 1f);
        WorkTicketAppearTween = DOTween.To(() => WorkTicketGroup.alpha, x => WorkTicketGroup.alpha = x, 0, 1f);

        WorkTicketInfoMoveTween = WorkTicketInfo.transform.GetComponent<RectTransform>().DOLocalMoveX(-1149f, 1f);
        WorkTicketInfoAppearTween = DOTween.To(() => WorkTicketInfoGroup.alpha, x => WorkTicketInfoGroup.alpha = x, 0, 1f);

        PerPosMoveTween = personnelPos.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        PerPosAppearTween = DOTween.To(() => PerPosGroup.alpha, x => PerPosGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        DevPosMoveTween = DevPos.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        DevPosAppearTween = DOTween.To(() => DevPosGroup.alpha, x => DevPosGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        MobilePatrolMoveTween = MobilePatrol.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        MobilePatrolAppearTween = DOTween.To(() => MobilePatrolGroup.alpha, x => MobilePatrolGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        TwoVotesMoveTween = TwoVotes.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        TwoVotesAppearTween = DOTween.To(() => TwoVotesGroup.alpha, x => TwoVotesGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        TwoVotesHistoryMoveTween = TwoVotesHistory.transform.GetComponent<RectTransform>().DOMoveY(0, 0.5f).SetEase(Ease.OutBack);
        TwoVotesHistoryAppearTween = DOTween.To(() => TwoVotesHistoryGroup.alpha, x => TwoVotesHistoryGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);


        TwoVotesTweenSequence.Append(actionBarAppearTween);

        TwoVotesTweenSequence.Join(ViewModeTween);
        TwoVotesTweenSequence.Join(FeatureTween);

        TwoVotesTweenSequence.Join(startOutMoveTween);
        TwoVotesTweenSequence.Join(startOutAppearTween);

        TwoVotesTweenSequence.Join(WorkTicketMoveTween);
        TwoVotesTweenSequence.Join(WorkTicketAppearTween);

        TwoVotesTweenSequence.Join(WorkTicketInfoMoveTween);
        TwoVotesTweenSequence.Join(WorkTicketInfoAppearTween);

        TwoVotesTweenSequence.Join(MobilePatrolMoveTween);//移动
        TwoVotesTweenSequence.Join(TwoVotesAppearTween);
        TwoVotesTweenSequence.Join(FunctionSwitchAppearTween);



        TwoVotesTweenSequence.Insert(0.025f, MobilePatrolAppearTween);
        TwoVotesTweenSequence.Join(TwoVotesAppearTween);

        TwoVotesTweenSequence.Insert(0.1f, DevPosAppearTween);
        TwoVotesTweenSequence.Join(PerPosAppearTween);

        TwoVotesTweenSequence.Join(TwoVotesHistoryMoveTween);
        TwoVotesTweenSequence.Join(TwoVotesHistoryAppearTween);


        TwoVotesTweenSequence.Insert(0.125f, DevPosMoveTween);
        TwoVotesTweenSequence.Join(PerPosAppearTween);

        TwoVotesTweenSequence.SetAutoKill(false);
        TwoVotesTweenSequence.Pause();
        //TwoVotesTweenSequence.Rewind(false);

    }
    public void SetTwoVotes_click()
    {
        startOutGroup.interactable = true;
        PerPosGroup.interactable = true;
        DevPosGroup.interactable = true;
        MobilePatrolGroup.interactable = true;
        TwoVotesGroup.interactable = true;
        TwoVotesHistoryGroup.interactable = true;
        FunctionSwitchGroup.interactable = true;
        WorkTicketGroup.interactable = true; 
    }
    
    public void SetTwoVotes_normal()
    {
        startOutGroup.interactable = false;
        PerPosGroup.interactable = false;
        DevPosGroup.interactable = false;
        MobilePatrolGroup.interactable = false;
        TwoVotesGroup.interactable = false;
        TwoVotesHistoryGroup.interactable = false;
        FunctionSwitchGroup.interactable = false;
        WorkTicketGroup.interactable = false;
    }
    void Update () {
		
	}
}
