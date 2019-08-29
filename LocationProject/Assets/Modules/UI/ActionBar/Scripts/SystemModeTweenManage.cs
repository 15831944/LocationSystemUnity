using DG.Tweening;
using RTEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemModeTweenManage : MonoBehaviour
{
    public static SystemModeTweenManage Instance;
    public Sequence TweenSequence;

    public GameObject PerPosObj;
    private Tween PerPosAppearT;
    private Tween PerPosMoveT;
    public CanvasGroup PerPosGroup;

    public GameObject DevObj;
    Tween DevAppearT;
    Tween DevMoveT;
    public CanvasGroup DevGroup;

    public GameObject InspectionObj;//移动巡检
    Tween InspectionAppearT;
    Tween InspectionMoveT;
    public CanvasGroup InspectionGroup;

    public GameObject TwoVotesObj;//两票
    Tween TwoVotesAppearT;
    Tween TwoVotesMoveT;
    public CanvasGroup TwoVotesGroup;

    public GameObject SearchObj;
    Tween SearchAppearT;
    Tween SearchMoveT;
    public CanvasGroup SearchGroup;

    public GameObject PerAlarmObj;
    Tween PerAlarmAppearT;
    Tween PerAlarmMoveT;
    public CanvasGroup PerAlarmGroup;

    public GameObject HistoryObj;
    Tween HistoryAppearT;
    Tween HistoryMoveT;
    public CanvasGroup HistoryGroup;

    public GameObject EditAreaObj;
    Tween EditAreaAppearT;
    Tween EditAreaMoveT;
    public CanvasGroup EditAreaGroup;

    public GameObject CardObj;
    Tween CardAppearT;
    Tween CardMoveT;
    public CanvasGroup CardGroup;



    public CanvasGroup SmallMapGroup;
    public GameObject SmallMap;
    public Tween SmallMapAppearTween;
    public Tween SmallMapMoveTween;

    public GameObject PerTreeObj;
    Tween PerTreeAppearT;
    Tween PerTreeMoveT;
    public CanvasGroup PerTreeGroup;

    public GameObject startOut;
    public CanvasGroup startOutGroup;
    public Tween startOutAppearTween;
    public Tween startOutMoveTween;

    public GameObject PushAlarm;
    public CanvasGroup PushAlarmGroup;
    public Tween PushAlarmAppearTween;
    public Tween PushAlarmMoveTween;

    public GameObject ParkObj;
    public CanvasGroup ParkGroup;
    Tween ParkMoveT;
    Tween ParkAppearT;

    public GameObject Action;
    Tween ActionAppearT;

    public GameObject FunctionSwitchBar;
    public Tween FunctionSwitchAppearT;
    public CanvasGroup FunctionSwitchGroup;

    public GameObject ModdleTog;
    public CanvasGroup ModdleTogGroup;
    private Tween ModdleTogAppearT;
    private Tween ModdleTogMoveT;

    public GameObject ModdleImage;
    private Tween ModdleScaleTween;
    private Tween ModdleColorTween;

    public Text ViewMode;
    private Tween ViewModeTween;
    public Text Feature;
    private Tween FeatureTween;


    void Start()
    {

    }
    private void Awake()
    {
        Instance = this;
        SetStartTween();
    }
    public bool IsStartTween = true;


    public void StartTweener()
    {

        TweenSequence = DOTween.Sequence();
        TweenSequence.Append(ViewModeTween);
        TweenSequence.Join(FeatureTween);
        TweenSequence.Join(ActionAppearT);

        TweenSequence.Join(InspectionAppearT);

        TweenSequence.Join(InspectionMoveT);

        TweenSequence.Join(SearchMoveT);

        TweenSequence.Insert(0.025f, ModdleTogMoveT);
        TweenSequence.Join(SearchAppearT);

        TweenSequence.Insert(0.1f, PerTreeMoveT);
        TweenSequence.Join(ParkMoveT);
        TweenSequence.Join(SmallMapMoveTween);
        TweenSequence.Join(startOutMoveTween);
        TweenSequence.Join(PushAlarmMoveTween);



        TweenSequence.Insert(0.125f, PerTreeAppearT);
        TweenSequence.Join(ParkAppearT);
        TweenSequence.Join(SmallMapAppearTween);
        TweenSequence.Join(startOutAppearTween);
        TweenSequence.Join(PushAlarmAppearTween);


        TweenSequence.Insert(0.15f, TwoVotesMoveT);
        TweenSequence.Join(PerAlarmMoveT);

        TweenSequence.Insert(0.175f, PerAlarmAppearT);
        TweenSequence.Join(TwoVotesAppearT);

        TweenSequence.Insert(0.2f, DevMoveT);
        TweenSequence.Join(HistoryMoveT);

        TweenSequence.Insert(0.225f, DevAppearT);
        TweenSequence.Join(HistoryAppearT);

        TweenSequence.Insert(0.25f, PerPosMoveT);
        TweenSequence.Join(EditAreaMoveT);


        TweenSequence.Insert(0.275f, PerPosAppearT);
        TweenSequence.Join(EditAreaAppearT);

        TweenSequence.Insert(0.3f, CardMoveT);

        TweenSequence.Insert(0.325f, CardAppearT);

        TweenSequence.Insert(0.35f, FunctionSwitchAppearT);

        TweenSequence.SetAutoKill(false);
        TweenSequence.Pause();
        TweenSequence.Rewind(false);


    }
    public void ModdleTween()
    {
        ModdleTogAppearT = DOTween.To(() => ModdleTogGroup.alpha, x => ModdleTogGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);
        ModdleScaleTween = ModdleImage.transform.GetComponent<RectTransform>().DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.6f).SetLoops(-1, LoopType.Yoyo);
        ModdleColorTween = ModdleImage.transform.GetComponent<Image>().DOColor(new Color(200 / 255f, 200 / 255f, 200 / 255f, 200 / 255f), 0.48f).SetEase(Ease.OutBack);
    }
    public void KillModdleTween()
    {
        ModdleScaleTween.Kill();

        ModdleScaleTween.Rewind(false);
        ModdleColorTween.Kill();

        ModdleColorTween.Rewind(false);
        // ModdleTogGroup.alpha = 1;
    }
    public void SetStartTween()
    {
        PerPosMoveT = PerPosObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        PerPosAppearT = DOTween.To(() => PerPosGroup.alpha, x => PerPosGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        DevMoveT = DevObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        DevAppearT = DOTween.To(() => DevGroup.alpha, x => DevGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        TwoVotesMoveT = TwoVotesObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        TwoVotesAppearT = DOTween.To(() => TwoVotesGroup.alpha, x => TwoVotesGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        InspectionMoveT = InspectionObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        InspectionAppearT = DOTween.To(() => InspectionGroup.alpha, x => InspectionGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        SearchMoveT = SearchObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        SearchAppearT = DOTween.To(() => SearchGroup.alpha, x => SearchGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        PerAlarmMoveT = PerAlarmObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        PerAlarmAppearT = DOTween.To(() => PerAlarmGroup.alpha, x => PerAlarmGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        HistoryMoveT = HistoryObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        HistoryAppearT = DOTween.To(() => HistoryGroup.alpha, x => HistoryGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        EditAreaMoveT = EditAreaObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        EditAreaAppearT = DOTween.To(() => EditAreaGroup.alpha, x => EditAreaGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        CardMoveT = CardObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        CardAppearT = DOTween.To(() => CardGroup.alpha, x => CardGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        SmallMapAppearTween = DOTween.To(() => SmallMapGroup.alpha, x => SmallMapGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);
        SmallMapMoveTween = SmallMap.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(183f, 0f), 0.5f).SetEase(Ease.OutBack);

        PerTreeAppearT = PerTreeObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(20f, 0f), 0.5f).SetEase(Ease.OutBack);
        PerTreeMoveT = DOTween.To(() => PerTreeGroup.alpha, x => PerTreeGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        startOutMoveTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(2.5f, 0f), 0.5f).SetEase(Ease.OutBack);
        startOutAppearTween = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        PushAlarmMoveTween = PushAlarm.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-180f, 250f), 0.5f).SetEase(Ease.OutBack);
        PushAlarmAppearTween = DOTween.To(() => PushAlarmGroup.alpha, x => PushAlarmGroup.alpha = x, 1, 0.48f).SetEase(Ease.OutBack);

        ParkMoveT = ParkObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(85f, 294f), 0.48f).SetEase(Ease.OutBack);
        ParkAppearT = DOTween.To(() => ParkGroup.alpha, x => ParkGroup.alpha = x, 1, 0.5f).SetEase(Ease.OutBack);

        ViewModeTween = ViewMode.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f), 0.5f).SetEase(Ease.OutBack);
        FeatureTween = Feature.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f), 0.5f).SetEase(Ease.OutBack);
        ModdleTogMoveT = ModdleTog.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, -14.6f), 0.48f).SetEase(Ease.OutBack);
        ActionAppearT = Action.transform.GetComponent<Image>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f), 0.48f).SetEase(Ease.OutBack);

        FunctionSwitchAppearT = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 1, 0.5f).SetEase(Ease.OutBack);
        StartTweener();
    }
    public void PersonTree_click()
    {
        SetPersonelTreeCanvasGroupState(true);
        //startOutGroup.interactable = true;
        //SmallMapGroup.interactable = true;
        //FunctionSwitchGroup.interactable = true;
        //PerPosGroup.interactable = true;
        //DevGroup.interactable = true;

        //TwoVotesGroup.interactable = true;
        //SearchGroup.interactable = true;
        //PerAlarmGroup.interactable = true;
        //HistoryGroup.interactable = true;
        //EditAreaGroup.interactable = true;
        //CardGroup.interactable = true;
    }

    public void PersonTree_normal()
    {
        SetPersonelTreeCanvasGroupState(false);
        //startOutGroup.interactable = false ;
        //SmallMapGroup.interactable = false;
        //FunctionSwitchGroup.interactable = false;
        //PerPosGroup.interactable = false;
        //DevGroup.interactable = false;

        //TwoVotesGroup.interactable = false;
        //SearchGroup.interactable = false;
        //PerAlarmGroup.interactable = false;
        //HistoryGroup.interactable = false;
        //EditAreaGroup.interactable = false;
        //CardGroup.interactable = false;
    }

    /// <summary>
    /// 设置人员树Canvas状态
    /// </summary>
    /// <param name="isOn"></param>
    private void SetPersonelTreeCanvasGroupState(bool isOn)
    {
        SetCanvasGroupState(startOutGroup, isOn);
        SetCanvasGroupState(SmallMapGroup, isOn);
        SetCanvasGroupState(FunctionSwitchGroup, isOn);
        SetCanvasGroupState(PerPosGroup, isOn);
        SetCanvasGroupState(DevGroup, isOn);
        SetCanvasGroupState(TwoVotesGroup, isOn);
        SetCanvasGroupState(SearchGroup, isOn);
        SetCanvasGroupState(PerAlarmGroup, isOn);
        SetCanvasGroupState(HistoryGroup, isOn);
        SetCanvasGroupState(EditAreaGroup, isOn);
        SetCanvasGroupState(CardGroup, isOn);
        SetCanvasGroupState(InspectionGroup, isOn);
    }

    /// <summary>
    /// 设置CanvasGruop状态
    /// </summary>
    /// <param name="group"></param>
    /// <param name="isOn"></param>
    private void SetCanvasGroupState(CanvasGroup group, bool isOn)
    {
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }
}
