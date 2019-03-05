using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TwoTicketSystem;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarTweenManage : MonoBehaviour
{

    private Sequence ActionBarSequence;
    public Toggle TweenToggle;
    /// <summary>
    /// 操作栏
    /// </summary>
    public GameObject actionBar;

    private Tween actionBarCloseTween;
    public  Tween actionBarOpenTween;

    public CanvasGroup actionBarGroup;

    private Tween actionBarCloseDisappear;
    public  Tween actionBarOpenAppear;

    public GameObject actionBarBg;
    public GameObject Obj;
    bool IsTween;
    public Sprite ClckImage;
    public Sprite ExitImage;
    // Use this for initialization
    void Start()
    {
        TweenToggle.onValueChanged.AddListener(ShowActionBarTween);
        GetCanvasGroups();
        Click_T0ggle(Obj);
    }
    public void GetCanvasGroups()
    {
        actionBarGroup = actionBar.transform.GetComponent<CanvasGroup>();
    }

    public void ShowActionBarTween(bool isOn)
    {
        if (IsTween) return;
        ActionBarSequence = DOTween.Sequence();
        actionBarBg.SetActive(isOn);
        if (isOn)
        {
            IsTween = true ;
            ActionBarCloseTween();

            if (ActionBarManage.Instance.PersonnelToggle.isOn  == isOn)
            {
                PersonnelTreeCloseAndDisappear();
            }
            else if (ActionBarManage.Instance.DevToggle.isOn  == isOn)
            {
                TopoTreeCloseAndDisappear();
            }
            else if (ActionBarManage.Instance.TwoVotesToggle.isOn  == isOn)
            {
                if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.工作票)
                {

                    WorkTicketDetailsCloseAndDisappear();

                }
                else if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.操作票)
                {
                    OperationTicketDetailsCloseAndDisappear();
                }

            }
            IsTween = false;
        }
        else
        {
            IsTween = true;
            ActionBarOpernTween();
            if (ActionBarManage.Instance.PersonnelToggle.isOn  == true )
            {
                PersonnelTreeRestoreAndAppear();
            }
            else if (ActionBarManage.Instance.DevToggle.isOn  == true)
            {
                TopoTreeRestoreAndAppear();
            }
            else if (ActionBarManage.Instance.TwoVotesToggle.isOn  == true)
            {
                if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.工作票)
                {

                    WorkTicketDetailsRestoreAndAppear();

                }
                else if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.操作票)
                {
                    OperationTicketDetailsRestoreAndAppear();
                }
            }
        }
        IsTween = false;
    }
    public void Click_T0ggle(GameObject obj)
    {
        EventTriggerListener objTog = EventTriggerListener.Get (obj);
        objTog.onEnter = UP_Toggle;
        objTog.onExit = Exit_toggle;
    }
    public void  UP_Toggle(GameObject obj)
    {
        if (TweenToggle.isOn == false)
        {
            obj.transform.GetChild(0).GetComponent<Text>().text = "展开UI";
           
        }
        else
        {
            obj.transform.GetChild(0).GetComponent<Text>().text = "收起UI";
        }
        obj.transform.GetChild(1).GetComponent<Image>().sprite = ClckImage;
    }
    public void Exit_toggle(GameObject obj)
    {
        obj.transform.GetChild(0).GetComponent<Text>().text = "  ";
        obj.transform.GetChild(1).GetComponent<Image>().sprite = ExitImage;
    }
    public void ActionBarCloseTween()
    {
        actionBarCloseTween = actionBar.transform.GetComponent<RectTransform>().DOScaleX(0, 1f);
        actionBarCloseDisappear = DOTween.To(() => actionBarGroup.alpha, x => actionBarGroup.alpha = x, 0, 1f);

    }
    public void ActionBarOpernTween()
    {
        actionBarOpenTween = actionBar.transform.GetComponent<RectTransform>().DOScaleX(1, 1f);
        actionBarOpenAppear = DOTween.To(() => actionBarGroup.alpha, x => actionBarGroup.alpha = x, 1, 1f);
    }
    public void PersonnelTreeCloseAndDisappear()
    {
        PersonnelTreeTweener.Instance.PersonnelTreeCloseTween();
        SmallMapTweener.Instance.ShowSmallMapCloseTween();
        parkInformationTweener.Instance.ParkInformationCloseTween();
        StartOutTweener.Instance . StartOutCloseTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarCloseTween();

        PersonnelTreeTweener.Instance.personnelTreeGroup.interactable = false;
        PersonnelTreeTweener.Instance.personnelTreeGroup.blocksRaycasts = false;

        SmallMapTweener.Instance.SmallMapGroup .interactable = false;
        SmallMapTweener.Instance.SmallMapGroup.blocksRaycasts = false;

        StartOutTweener.Instance.startOutGroup.interactable = false;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = false;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = false;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = false;
        ActionBarSequence.Join(actionBarCloseTween);
        ActionBarSequence.Join(actionBarCloseDisappear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseDisappear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseDisappear);

        ActionBarSequence.Join(PersonnelTreeTweener.Instance.PerTreeCloseTween);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoCloseTween);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapCloseTween);

        ActionBarSequence.Join(PersonnelTreeTweener.Instance.PerTreeCloseDisappear);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoCloseDisappear);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapCloseDisappear);
    }
    public void PersonnelTreeRestoreAndAppear()
    {
        PersonnelTreeTweener.Instance.PersonnelTreeOpernTween();
        SmallMapTweener.Instance.ShowSmallMapOpenTween();
        parkInformationTweener.Instance.ParkInformationOpernTween();
        StartOutTweener.Instance.StartOutOpernTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarOpernTween();

        PersonnelTreeTweener.Instance.personnelTreeGroup.interactable = true;
        PersonnelTreeTweener.Instance.personnelTreeGroup.blocksRaycasts = true;

        SmallMapTweener.Instance.SmallMapGroup.interactable = true;
        SmallMapTweener.Instance.SmallMapGroup.blocksRaycasts = true;

        StartOutTweener.Instance.startOutGroup.interactable = true;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = true;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = true;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = true;
        ActionBarSequence.Join(actionBarOpenTween);
        ActionBarSequence.Join(actionBarOpenAppear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenAppear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenAppear);

        ActionBarSequence.Join(PersonnelTreeTweener.Instance.PerTreeOpenTween);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoOpenTween);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapOpenTween);

        ActionBarSequence.Join(PersonnelTreeTweener.Instance.PerTreeOpenAppear);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoOpenAppear);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapOpenAppear);
    }
    public void TopoTreeCloseAndDisappear()
    {
        TopoTreeTweener.Instance.TopoTreeCloseTween();
        SmallMapTweener.Instance.ShowSmallMapCloseTween();
        parkInformationTweener.Instance.ParkInformationCloseTween();
        StartOutTweener.Instance.StartOutCloseTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarCloseTween();

        TopoTreeTweener.Instance.topoTreeGroup.interactable = false;
        TopoTreeTweener.Instance.topoTreeGroup.blocksRaycasts = false;

        SmallMapTweener.Instance.SmallMapGroup.interactable = false;
        SmallMapTweener.Instance.SmallMapGroup.blocksRaycasts = false;

        StartOutTweener.Instance.startOutGroup.interactable = false;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = false;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = false;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = false;
        ActionBarSequence.Join(actionBarCloseTween);
        ActionBarSequence.Join(actionBarCloseDisappear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseDisappear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseDisappear);

        ActionBarSequence.Join(TopoTreeTweener.Instance.topoTreeCloseTween);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoCloseTween);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapCloseTween);

        ActionBarSequence.Join(TopoTreeTweener.Instance.topoTreeCloseDisappear);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoCloseDisappear);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapCloseDisappear);
    }
    public void TopoTreeRestoreAndAppear()
    {
        TopoTreeTweener.Instance.TopoTreeOpernTween();
        SmallMapTweener.Instance.ShowSmallMapOpenTween();
        parkInformationTweener.Instance.ParkInformationOpernTween();
        StartOutTweener.Instance.StartOutOpernTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarOpernTween();

        TopoTreeTweener.Instance.topoTreeGroup.interactable = true;
        TopoTreeTweener.Instance.topoTreeGroup.blocksRaycasts = true;

        SmallMapTweener.Instance.SmallMapGroup.interactable = true;
        SmallMapTweener.Instance.SmallMapGroup.blocksRaycasts = true;

        StartOutTweener.Instance.startOutGroup.interactable = true;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = true;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = true;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = true;

        ActionBarSequence.Join(actionBarOpenTween);
        ActionBarSequence.Join(actionBarOpenAppear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenAppear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenAppear);

        ActionBarSequence.Join(TopoTreeTweener.Instance.topoTreeOpenTween);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoOpenTween);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapOpenTween);

        ActionBarSequence.Join(TopoTreeTweener.Instance.topoTreeOpenAppear);
        ActionBarSequence.Join(parkInformationTweener.Instance.parkInfoOpenAppear);
        ActionBarSequence.Join(SmallMapTweener.Instance.SmallMapOpenAppear);
    }
    public void WorkTicketDetailsCloseAndDisappear()
    {
        TwoTicketSystemUITweener.Instance.TwoTicketSystemUITweenerCloseTween();
        WorkTicketDetailsTweener.Instance.WorkTicketDetailsCloseTween();
        StartOutTweener.Instance.StartOutCloseTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarCloseTween();

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.interactable = false;
        WorkTicketDetailsTweener.Instance.WorkTicketGroup.interactable = false;

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.blocksRaycasts = false;
        WorkTicketDetailsTweener.Instance.WorkTicketGroup.blocksRaycasts = false;

        StartOutTweener.Instance.startOutGroup.interactable = false;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = false;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = false;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = false;

        ActionBarSequence.Join(actionBarCloseTween);
        ActionBarSequence.Join(actionBarCloseDisappear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseDisappear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseDisappear);

        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemCloseTween);
        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemCloseDisappear);

        ActionBarSequence.Join(WorkTicketDetailsTweener.Instance.WorkTicketCloseTween);
        ActionBarSequence.Join(WorkTicketDetailsTweener.Instance.WorkTicketCloseDisappear);
    }
    public void WorkTicketDetailsRestoreAndAppear()
    {
        TwoTicketSystemUITweener.Instance.TwoTicketSystemUITweenerOpenTween();
        WorkTicketDetailsTweener.Instance.WorkTicketDetailsOpernTween();
        StartOutTweener.Instance.StartOutOpernTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarOpernTween();

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.interactable = true;
        WorkTicketDetailsTweener.Instance.WorkTicketGroup.interactable = true;

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.blocksRaycasts = true;
        WorkTicketDetailsTweener.Instance.WorkTicketGroup.blocksRaycasts = true;

        StartOutTweener.Instance.startOutGroup.interactable = true;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = true;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = true;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = true;

        ActionBarSequence.Join(actionBarOpenTween);
        ActionBarSequence.Join(actionBarOpenAppear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenAppear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenAppear);

        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemOpenTween);
        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemOpenAppear);

        ActionBarSequence.Join(WorkTicketDetailsTweener.Instance.WorkTicketOpenTween);
        ActionBarSequence.Join(WorkTicketDetailsTweener.Instance.WorkTicketOpenAppear);
    }
    public void OperationTicketDetailsCloseAndDisappear()
    {
        TwoTicketSystemUITweener.Instance.TwoTicketSystemUITweenerCloseTween();
        OperationTicketDetailsTweener.Instance.OperationTicketDetailsCloseTween();
        StartOutTweener.Instance.StartOutCloseTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarCloseTween();


        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.interactable = false;
        OperationTicketDetailsTweener.Instance.OperationTicketGroup.interactable = false;

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.blocksRaycasts = false;
        OperationTicketDetailsTweener.Instance.OperationTicketGroup.blocksRaycasts = false;

        StartOutTweener.Instance.startOutGroup.interactable = false;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = false;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = false;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = false;

        ActionBarSequence.Join(actionBarCloseTween);
        ActionBarSequence.Join(actionBarCloseDisappear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutCloseDisappear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchCloseDisappear);

        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemCloseTween);
        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemCloseDisappear);

        ActionBarSequence.Join(OperationTicketDetailsTweener.Instance.OperationTicketCloseTween);
        ActionBarSequence.Join(OperationTicketDetailsTweener.Instance.OperationTicketCloseDisappear);
    }
    public void OperationTicketDetailsRestoreAndAppear()
    {
        TwoTicketSystemUITweener.Instance.TwoTicketSystemUITweenerOpenTween();
        OperationTicketDetailsTweener.Instance.OperationTicketDetailsOpernTween();
        StartOutTweener.Instance.StartOutOpernTween();
        FunctionSwitchBarTweener.Instance.FunctionSwitchBarOpernTween();

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.interactable = true;
        OperationTicketDetailsTweener.Instance.OperationTicketGroup.interactable = true;

        TwoTicketSystemUITweener.Instance.TwoTicketSystemGroup.blocksRaycasts = true;
        OperationTicketDetailsTweener.Instance.OperationTicketGroup.blocksRaycasts = true;

        StartOutTweener.Instance.startOutGroup.interactable = true;
        StartOutTweener.Instance.startOutGroup.blocksRaycasts = true;

        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.interactable = true;
        FunctionSwitchBarTweener.Instance.FunctionSwitchGroup.blocksRaycasts = true;

        ActionBarSequence.Join(actionBarCloseTween);
        ActionBarSequence.Join(actionBarCloseDisappear);

        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenTween);
        ActionBarSequence.Join(StartOutTweener.Instance.startOutOpenAppear);

        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenTween);
        ActionBarSequence.Join(FunctionSwitchBarTweener.Instance.FunctionSwitchOpenAppear);

        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemOpenTween);
        ActionBarSequence.Join(TwoTicketSystemUITweener.Instance.TwoTicketSystemOpenAppear);

        ActionBarSequence.Join(OperationTicketDetailsTweener.Instance.OperationTicketOpenTween);
        ActionBarSequence.Join(OperationTicketDetailsTweener.Instance.OperationTicketOpenAppear);
    }
}
