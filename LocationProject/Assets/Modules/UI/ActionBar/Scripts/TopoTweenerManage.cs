using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopoTweenerManage : MonoBehaviour {
    public static TopoTweenerManage Instance;
    public GameObject parkObj;//统计信息
    public CanvasGroup parkInfoGroup;
     Tween parkInfoMoveTween;
    Tween parkInfoAppearTween;

    public CanvasGroup SmallMapGroup;//小地图
    public GameObject SmallMap;
     Tween SmallMapAppearTween;
     Tween SmallMapMoveTween;

    public GameObject startOut;//开始结束
    public CanvasGroup startOutGroup;
     Tween startOutAppearTween;
     Tween startOutMoveTween;

 //   public CanvasGroup actionBarGroup;//操作栏
    public GameObject actionBar;
     Tween actionBarAppearTween;

    public GameObject topoTree;//设备树
     Tween topoTreeAppearTween;
     Tween topoTreeMoveTween;
    public CanvasGroup topoTreeGroup;

    public GameObject FunctionSwitchBar;
     Tween FunctionSwitchAppearTween;
    public CanvasGroup FunctionSwitchGroup;

    public GameObject personnelPos;//人员
     Tween PerPosAppearTween;
     Tween PerPosMoveTween;
    public  CanvasGroup PerPosGroup;

    public GameObject DevPos;//设备
     Tween DevPosAppearTween;
     Tween DevPosMoveTween;
    public  CanvasGroup DevPosGroup;

    public GameObject MobilePatrol;
     Tween MobilePatrolAppearTween ;//移动巡检
    Tween MobilePatrolMoveTween ;
    public  CanvasGroup MobilePatrolGroup ;

    public GameObject TwoVotes;
     Tween TwoVotesAppearTween;//两票
     Tween TwoVotesMoveTween ;
    public  CanvasGroup TwoVotesGroup ;

    public GameObject Roam;//漫游
     Tween RoamAppearTween;
     Tween RoamMoveTween;
    public CanvasGroup RoamGroup;

    public GameObject Query;//查询
     Tween QueryAppearTween;
     Tween QueryMoveTween;
    public CanvasGroup QueryGroup;

    public GameObject DevAlarm;//设备告警
     Tween DevAlarmAppearTween;
     Tween DevAlarmMoveTween;
    public CanvasGroup DevAlarmGroup;

    public GameObject DevEditor;//设备编辑
     Tween DevEditorAppearTween;
     Tween DevEditorMoveTween;
    public CanvasGroup DevEditorGroup;

    public Text ViewMode;
     Tween ViewModeTween;
    public Text Feature;
    private Tween FeatureTween;

    //  public GameObject ModdleTog;//中间按钮
    //  public Tween  ModdleTogMoveTween;

    public Sequence TopoTweenSequence;

    public GameObject PushAlarm;
    public CanvasGroup PushAlarmGroup;
    public Tween PushAlarmAppearTween;
    public Tween PushAlarmMoveTween;

    void Start () {
        Instance = this;

    }
    private void Awake()
    {
        TopoTweenSequence = DOTween.Sequence();
        SetTopoTween();
    }
 
	public void SetTopoTween()
    {
        ViewModeTween = ViewMode.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 0/ 255f), 0.5f).SetEase(Ease.OutBack);
        FeatureTween = Feature.GetComponent<Text>().DOColor(new Color(255 / 255f, 255 / 255f, 255 / 255f, 0 / 255f), 0.5f).SetEase(Ease.OutBack);

        parkInfoAppearTween = parkObj.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(260f, 294f), 0.48f).SetEase(Ease.OutBack);
        parkInfoMoveTween = DOTween.To(() => parkInfoGroup.alpha, x => parkInfoGroup.alpha = x, 0, 0.5f).SetEase(Ease.OutBack);

        topoTreeMoveTween = topoTree.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-350f, 100f), 0.48f).SetEase(Ease.OutBack);
        topoTreeAppearTween = DOTween.To(() => topoTreeGroup.alpha, x => topoTreeGroup.alpha = x, 0, 1f);

        SmallMapAppearTween = DOTween.To(() => SmallMapGroup.alpha, x => SmallMapGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);
        SmallMapMoveTween = SmallMap.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-340f, 0f),2f).SetEase(Ease.OutBack);

        startOutMoveTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-220f, 0f), 0.5f).SetEase(Ease.OutBack);
        startOutAppearTween = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        PushAlarmMoveTween = PushAlarm.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(180f, 250f), 0.5f).SetEase(Ease.OutBack);
        PushAlarmAppearTween = DOTween.To(() => PushAlarmGroup.alpha, x => PushAlarmGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);


        FunctionSwitchAppearTween = DOTween.To(() => FunctionSwitchGroup.alpha, x => FunctionSwitchGroup.alpha = x, 0, 0.5f).SetEase(Ease.OutBack);

        actionBarAppearTween = actionBar.transform.GetComponent<Image>().DOColor(new Color(0 / 255f, 0 / 255f, 0 / 255f, 0 / 255f), 0.48f).SetEase(Ease.OutBack);

      //  ModdleTogMoveTween = ModdleTog.transform.GetComponent<RectTransform>().DOMoveY(-2f, 0.5f).SetEase(Ease.OutBack);

        PerPosMoveTween = personnelPos.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        PerPosAppearTween = DOTween.To(() => PerPosGroup.alpha, x => PerPosGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        DevPosMoveTween = DevPos.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        DevPosAppearTween = DOTween.To(() => DevPosGroup.alpha, x => DevPosGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        MobilePatrolMoveTween = MobilePatrol.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        MobilePatrolAppearTween = DOTween.To(() => MobilePatrolGroup.alpha, x => MobilePatrolGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        TwoVotesMoveTween = TwoVotes.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        TwoVotesAppearTween = DOTween.To(() => TwoVotesGroup.alpha, x => TwoVotesGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        RoamMoveTween = Roam.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        RoamAppearTween = DOTween.To(() => RoamGroup.alpha, x => RoamGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        QueryMoveTween = QueryGroup.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        QueryAppearTween = DOTween.To(() => QueryGroup.alpha, x => QueryGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        DevAlarmMoveTween = DevAlarm.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        DevAlarmAppearTween = DOTween.To(() => DevAlarmGroup.alpha, x => DevAlarmGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);

        DevEditorMoveTween = DevEditor.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 2f), 0.5f).SetEase(Ease.OutBack);
        DevEditorAppearTween = DOTween.To(() => DevEditorGroup.alpha, x => DevEditorGroup.alpha = x, 0, 0.48f).SetEase(Ease.OutBack);
      
        TopoTweenSequence.Append(actionBarAppearTween);

        TopoTweenSequence.Join(ViewModeTween);
        TopoTweenSequence.Join(FeatureTween);

        TopoTweenSequence.Join(MobilePatrolMoveTween);//移动

        TopoTweenSequence.Join(FunctionSwitchAppearTween);

        TopoTweenSequence.Join(RoamMoveTween);//漫游
        TopoTweenSequence.Join(SmallMapMoveTween);//小地图
        TopoTweenSequence.Insert(0.025f, MobilePatrolAppearTween);
        TopoTweenSequence.Join(RoamAppearTween);

        TopoTweenSequence.Insert(0.1f, topoTreeMoveTween);//树
        TopoTweenSequence.Join(parkInfoMoveTween);//统计
        
        TopoTweenSequence.Join(startOutMoveTween);//开始
        TopoTweenSequence.Join(PushAlarmMoveTween);

        TopoTweenSequence.Insert(0.125f, topoTreeAppearTween);
        TopoTweenSequence.Join(parkInfoAppearTween);
        TopoTweenSequence.Join(SmallMapAppearTween);
        TopoTweenSequence.Join(startOutAppearTween);
        TopoTweenSequence.Join(PushAlarmAppearTween);

        TopoTweenSequence.Insert(0.15f, TwoVotesMoveTween);//两票
        TopoTweenSequence.Join(QueryMoveTween);//查询

        TopoTweenSequence.Insert(0.175f, QueryAppearTween);
        TopoTweenSequence.Join(TwoVotesAppearTween);

        TopoTweenSequence.Insert(0.2f, DevPosMoveTween);//设备
        TopoTweenSequence.Join(DevAlarmMoveTween);//设备告警

        TopoTweenSequence.Insert(0.225f, DevPosAppearTween);
        TopoTweenSequence.Join(DevAlarmAppearTween);

        TopoTweenSequence.Insert(0.25f, PerPosMoveTween);//人员
        TopoTweenSequence.Join(DevEditorMoveTween);//设备编辑

        TopoTweenSequence.Insert(0.275f, PerPosAppearTween);
        TopoTweenSequence.Join(DevEditorAppearTween);

        

        TopoTweenSequence.SetAutoKill(false);
        TopoTweenSequence.Pause();
      //  TopoTweenSequence.Rewind(true );
    }
	public void TopoTree_click()
    {
        SetTopoTreeCanvasGroupState(true);

        //startOutGroup.interactable = true;
        //SmallMapGroup.interactable = true;
        //FunctionSwitchGroup.interactable = true;
        //PerPosGroup.interactable = true;
        //DevPosGroup.interactable = true;
        //MobilePatrolGroup.interactable = true;
        //TwoVotesGroup.interactable = true;
        //RoamGroup.interactable = true;
        //QueryGroup.interactable = true;
        //DevAlarmGroup.interactable = true;
        //DevEditorGroup.interactable = true;

    }
    public void TopoTree_normal()
    {
        SetTopoTreeCanvasGroupState(false);

        //startOutGroup.interactable = false ;
        //SmallMapGroup.interactable = false;
        //FunctionSwitchGroup.interactable = false;
        //PerPosGroup.interactable = false;
        //DevPosGroup.interactable = false;
        //MobilePatrolGroup.interactable = false;
        //TwoVotesGroup.interactable = false;
        //RoamGroup.interactable = false;
        //QueryGroup.interactable = false;
        //DevAlarmGroup.interactable = false;
        //DevEditorGroup.interactable = false;
    }
    /// <summary>
    /// 设置拓朴树Canvas状态
    /// </summary>
    /// <param name="isOn"></param>
    private void SetTopoTreeCanvasGroupState(bool isOn)
    {
        SetCanvasGroupState(startOutGroup, isOn);
        SetCanvasGroupState(SmallMapGroup, isOn);
        SetCanvasGroupState(FunctionSwitchGroup, isOn);
        SetCanvasGroupState(PerPosGroup, isOn);
        SetCanvasGroupState(DevPosGroup, isOn);
        SetCanvasGroupState(MobilePatrolGroup, isOn);
        SetCanvasGroupState(TwoVotesGroup, isOn);
        SetCanvasGroupState(RoamGroup, isOn);
        SetCanvasGroupState(QueryGroup, isOn);
        SetCanvasGroupState(DevAlarmGroup, isOn);
        SetCanvasGroupState(DevEditorGroup, isOn);
    }

    /// <summary>
    /// 设置CanvasGruop状态
    /// </summary>
    /// <param name="group"></param>
    /// <param name="isOn"></param>
    private void SetCanvasGroupState(CanvasGroup group,bool isOn)
    {
        group.interactable = isOn;
        group.blocksRaycasts = isOn;
    }
}
