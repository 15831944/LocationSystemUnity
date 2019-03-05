using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemModeTweenManage : MonoBehaviour
{
    public static SystemModeTweenManage Instance;
    private Sequence TweenSequence;
    public ActionBarTweenManage actionBarTweenManage;//操作栏
    public MobilePatrolTweener mobilePatrolTweener;//移动巡检
    public CoordinateConfigurationTweener coordinateConfigurationTweener;//坐标系设置
    public BenchmarkingTweener benchmarkingTweener;//设置基准点
    public TwoVotesTweener twoVotesTweener;//两票系统
    public EditAreaTweener editAreaTweener;//编辑监控区域
    public HistoricalPathTweener historicalPathTweener;//历史路径
    public DevicePositionTweener devicePositionTweener;//设备管理
    public PersonnelAlarmTweener personnelAlarmTweener;//人员告警
    public PersonnelPositionTweener personnelPositionTweener;//人员定位
    public SearchTweener searchTweener;//人员搜索
    public ModdleToggle moddleToggle;//操作栏中间按钮
    public PersonnelTreeTweener personnelTree;//人员树
    public parkInformationTweener ParkInfo;//统计信息
   

    private Tween PerTreeAppearTween;
    private Tween PerTreeMoveTween;
    private CanvasGroup personnelTreeGroup;

    private CanvasGroup parkInfoGroup;
    private Tween parkInfoMoveTween;
    private Tween parkInfoAppearTween;

    private Tween ModdleTogAppearTween;
    private Tween ModdleTogMoveTween;
    private CanvasGroup ModdleTogGroup;

    private Tween SearchAppearTween;
    private Tween SearchMoveTween;
    private CanvasGroup SearchGroup;

    private Tween PerPosAppearTween;
    private Tween PerPosMoveTween;
    private CanvasGroup PerPosGroup;

    private Tween PerAlarmAppearTween;
    private Tween PerAlarmMoveTween;
    private CanvasGroup PerAlarmGroup;

    private Tween DevPosAppearTween;
    private Tween DevPosMoveTween;
    private CanvasGroup DevPosGroup;

    private Tween HistoricalPathAppearTween;
    private Tween HistoricalPathMoveTween;
    private CanvasGroup HistoricalPathGroup;

    private Tween EditAreaAppearTween;
    private Tween EditAreaMoveTween;
    private CanvasGroup EditAreaGroup;

    private Tween TwoVotesAppearTween;
    private Tween TwoVotesMoveTween;
    private CanvasGroup TwoVotesGroup;

    private Tween BenchmarkingAppearTween;
    private Tween BenchmarkingMoveTween;
    private CanvasGroup BenchmarkingGroup;

    private Tween CoordinateConfigurationAppearTween;
    private Tween CoordinateConfigurationMoveTween;
    private CanvasGroup CoordinateConfigurationGroup;

    private Tween MobilePatrolAppearTween;
    private Tween MobilePatrolMoveTween;
    private CanvasGroup MobilePatrolGroup;

    private CanvasGroup actionBarGroup;
    private Tween actionBarMoveTween;
    private Tween actionBarAppearTween;

    public GameObject  FunctionSwitchBar;
    public Tween FunctionSwitchAppearTween;
    public CanvasGroup FunctionSwitchGroup;

    public CanvasGroup SmallMapGroup;
    public GameObject  SmallMap;
    public Tween SmallMapAppearTween;
    public Tween SmallMapMoveTween;

    public GameObject startOut;
    public CanvasGroup startOutGroup;
    public Tween startOutAppearTween;
    public Tween startOutMoveTween;

    void Start()
    {
        Instance = this;

        FunctionSwitchGroup = FunctionSwitchBar.GetComponent<CanvasGroup>();
        PerTreeAppearTween = personnelTree.PerTreeOpenAppear;
        PerTreeMoveTween = personnelTree.PerTreeOpenTween;
        personnelTreeGroup = personnelTree.personnelTreeGroup;

        parkInfoGroup = ParkInfo.parkInfoGroup;
        parkInfoMoveTween = ParkInfo.parkInfoOpenAppear;
        parkInfoAppearTween = ParkInfo.parkInfoOpenTween;

        ModdleTogAppearTween = moddleToggle.ModdleTogAppearTween;
        ModdleTogMoveTween = moddleToggle.ModdleTogMoveTween;
        ModdleTogGroup = moddleToggle.ModdleTogGroup;

        SearchAppearTween = searchTweener.SearchAppearTween;
        SearchMoveTween = searchTweener.SearchMoveTween;
        SearchGroup = searchTweener.SearchGroup;

        PerPosAppearTween = personnelPositionTweener.PerPosAppearTween;
        PerPosMoveTween = personnelPositionTweener.PerPosMoveTween;
        PerPosGroup = personnelPositionTweener.PerPosGroup;

        PerAlarmAppearTween = personnelAlarmTweener.PerAlarmAppearTween;
        PerAlarmMoveTween = personnelAlarmTweener.PerAlarmMoveTween;
        PerAlarmGroup = personnelAlarmTweener.PerAlarmGroup;

        DevPosAppearTween = devicePositionTweener.DevPosAppearTween;
        DevPosMoveTween = devicePositionTweener.DevPosMoveTween;
        DevPosGroup = devicePositionTweener.DevPosGroup;

        HistoricalPathAppearTween = historicalPathTweener.HistoricalPathAppearTween;
        HistoricalPathMoveTween = historicalPathTweener.HistoricalPathMoveTween;
        HistoricalPathGroup = historicalPathTweener.HistoricalPathGroup;

        EditAreaAppearTween = editAreaTweener.EditAreaAppearTween;
        EditAreaMoveTween = editAreaTweener.EditAreaMoveTween;
        EditAreaGroup = editAreaTweener.EditAreaGroup;

        TwoVotesAppearTween = twoVotesTweener.TwoVotesAppearTween;
        TwoVotesMoveTween = twoVotesTweener.TwoVotesMoveTween;
        TwoVotesGroup = twoVotesTweener.TwoVotesGroup;

        BenchmarkingAppearTween = benchmarkingTweener.BenchmarkingAppearTween;
        BenchmarkingMoveTween = benchmarkingTweener.BenchmarkingMoveTween;
        BenchmarkingGroup = benchmarkingTweener.BenchmarkingGroup;

        CoordinateConfigurationAppearTween = coordinateConfigurationTweener.CoordinateConfigurationAppearTween;
        CoordinateConfigurationMoveTween = coordinateConfigurationTweener.CoordinateConfigurationMoveTween;
        CoordinateConfigurationGroup = coordinateConfigurationTweener.CoordinateConfigurationGroup;

        MobilePatrolAppearTween = mobilePatrolTweener.MobilePatrolAppearTween;
        MobilePatrolMoveTween = mobilePatrolTweener.MobilePatrolMoveTween;
        MobilePatrolGroup = mobilePatrolTweener.MobilePatrolGroup;

        actionBarGroup = actionBarTweenManage.actionBarGroup;
        actionBarAppearTween = actionBarTweenManage.actionBarOpenAppear;

     

}

    public void StartTweener()
    {
        TweenSequence = DOTween.Sequence();
        
        PerTreeMoveTween = personnelTree.personnelTree.transform.GetComponent<RectTransform>().DOMoveX(190f, 0.5f).SetEase(Ease.OutBack);
        PerTreeAppearTween = DOTween.To(() => personnelTreeGroup.alpha, x => personnelTreeGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        parkInfoAppearTween = ParkInfo.parkInfo.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(80f, 294f), 0.48f).SetEase(Ease.OutBack);
        parkInfoMoveTween = DOTween.To(() => parkInfoGroup.alpha, x => parkInfoGroup.alpha = x, 1, 0.5f).SetEase(Ease.OutBack);

        ModdleTogMoveTween = moddleToggle.ModdleTog.transform.GetComponent<RectTransform>().DOMoveY(60f, 0.5f).SetEase(Ease.OutBack);
        ModdleTogAppearTween = DOTween.To(() => ModdleTogGroup.alpha, x => ModdleTogGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        SearchMoveTween = searchTweener.Search.transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        SearchAppearTween = DOTween.To(() => SearchGroup.alpha, x => SearchGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        PerPosMoveTween = personnelPositionTweener.PerPos.transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        PerPosAppearTween = DOTween.To(() => PerPosGroup.alpha, x => PerPosGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        PerAlarmMoveTween = personnelAlarmTweener.PerAlarm.transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        PerAlarmAppearTween = DOTween.To(() => PerAlarmGroup.alpha, x => PerAlarmGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        DevPosMoveTween = devicePositionTweener.DevPos .transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        DevPosAppearTween = DOTween.To(() => DevPosGroup.alpha, x => DevPosGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        HistoricalPathMoveTween = historicalPathTweener.HistoricalPath .transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        HistoricalPathAppearTween = DOTween.To(() => HistoricalPathGroup.alpha, x => HistoricalPathGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        EditAreaMoveTween = editAreaTweener.EditArea.transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        EditAreaAppearTween = DOTween.To(() => EditAreaGroup.alpha, x => EditAreaGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        TwoVotesMoveTween = twoVotesTweener.TwoVotes .transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        TwoVotesAppearTween = DOTween.To(() => TwoVotesGroup.alpha, x => TwoVotesGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        BenchmarkingMoveTween = benchmarkingTweener.Benchmarking.transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        BenchmarkingAppearTween = DOTween.To(() => BenchmarkingGroup.alpha, x => BenchmarkingGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        CoordinateConfigurationMoveTween = coordinateConfigurationTweener.CoordinateConfiguration.transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        CoordinateConfigurationAppearTween = DOTween.To(() => CoordinateConfigurationGroup.alpha, x => CoordinateConfigurationGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        MobilePatrolMoveTween = mobilePatrolTweener.MobilePatrol .transform.GetComponent<RectTransform>().DOMoveY(55, 0.5f).SetEase(Ease.OutBack);
        MobilePatrolAppearTween = DOTween.To(() => MobilePatrolGroup.alpha, x => MobilePatrolGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        actionBarAppearTween = actionBarTweenManage.actionBar.transform.GetComponent<Image>().DOColor(new Color (255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f), 0.48f).SetEase(Ease.OutBack);
       // actionBarMoveTween = actionBarTweenManage.actionBar.transform.GetComponent<RectTransform>().DOMoveY(70, 0.5f).SetEase(Ease.OutBack);

        SmallMapAppearTween= DOTween.To(() => SmallMapGroup.alpha, x => SmallMapGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);
        SmallMapMoveTween = SmallMap.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(183f, 0f), 0.5f).SetEase(Ease.OutBack);

        startOutMoveTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(2.5f, 0f), 0.5f).SetEase(Ease.OutBack);
        startOutAppearTween = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        FunctionSwitchAppearTween = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 1, 0.5f).SetEase(Ease.OutBack);

        TweenSequence.Append(actionBarAppearTween);
        //TweenSequence.Join(actionBarMoveTween);
        //TweenSequence.Join(ModdleTogMoveTween);
        TweenSequence.Join(ModdleTogAppearTween);
        TweenSequence.Join(MobilePatrolMoveTween);
     
        TweenSequence.Join(SearchMoveTween);

        TweenSequence.Insert(0.025f,MobilePatrolAppearTween);
        TweenSequence.Join(SearchAppearTween);

        TweenSequence.Insert(0.1f, PerTreeMoveTween);
        TweenSequence.Join(parkInfoMoveTween);
        TweenSequence.Join(SmallMapMoveTween);
        TweenSequence.Join(startOutMoveTween);

        TweenSequence.Insert(0.125f,PerTreeAppearTween);
        TweenSequence.Join(parkInfoAppearTween);
        TweenSequence.Join(SmallMapAppearTween);
        TweenSequence.Join(startOutAppearTween);

        TweenSequence.Insert(0.15f, TwoVotesMoveTween);
        TweenSequence.Join(PerAlarmMoveTween);

        TweenSequence.Insert(0.175f,PerAlarmAppearTween);
        TweenSequence.Join(TwoVotesAppearTween);

        TweenSequence.Insert(0.2f, TwoVotesMoveTween);
        TweenSequence.Join(PerAlarmMoveTween);

        TweenSequence.Insert(0.225f,TwoVotesAppearTween);
        TweenSequence.Join(PerAlarmAppearTween);

        TweenSequence.Insert(0.25f, DevPosMoveTween);
        TweenSequence.Join(HistoricalPathMoveTween);

        TweenSequence.Insert(0.275f,DevPosAppearTween);
        TweenSequence.Join(HistoricalPathAppearTween);

        TweenSequence.Insert(0.3f, PerPosMoveTween);
        TweenSequence.Join(EditAreaMoveTween);

        TweenSequence.Insert(0.325f,PerPosAppearTween);
        TweenSequence.Join(EditAreaAppearTween);

        TweenSequence.Insert(0.35f, BenchmarkingMoveTween);

        TweenSequence.Insert(0.375f,BenchmarkingAppearTween);

        TweenSequence.Insert(0.4f, CoordinateConfigurationMoveTween);

        TweenSequence.Insert(0.425f,CoordinateConfigurationAppearTween);

        TweenSequence.Insert(0.45f,FunctionSwitchAppearTween);

        



    }
    void Update()
    {

    }
}
