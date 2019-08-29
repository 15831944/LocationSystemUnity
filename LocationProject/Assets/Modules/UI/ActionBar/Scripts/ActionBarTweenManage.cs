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



    //public GameObject actionBarBg;
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
        if (isOn)
        {

            IsTween = true ;   
            if (ActionBarManage.Instance.PersonnelToggle.isOn  == isOn)
            {
                SystemModeTweenManage.Instance.PersonTree_normal();
                SystemModeTweenManage.Instance.TweenSequence.PlayBackwards();
            }
            else if (ActionBarManage.Instance.DevToggle.isOn  == isOn)
            {
                TopoTweenerManage.Instance.TopoTree_normal();               
                TopoTweenerManage.Instance.TopoTweenSequence.PlayForward();
            }
            else if (ActionBarManage.Instance.TwoVotesToggle.isOn  == isOn)
            {
                if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.工作票)
                {
                    TwoTicketSystemUITweenManage.Instance.SetTwoVotes_normal();
                   
                    TwoTicketSystemUITweenManage.Instance.TwoVotesTweenSequence.Restart();

                }
                else if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.操作票)
                {
                    OperationTicketDetailsTweenManage.Instance.SetMobilePatrol_normal();
                   // OperationTicketDetailsCloseAndDisappear();
                    OperationTicketDetailsTweenManage.Instance.MobilePatrolTweenSequence.Restart();
                }

            }
            IsTween = false;
        }
        else
        {
            IsTween = true;
            
           // ActionBarOpernTween();
            if (ActionBarManage.Instance.PersonnelToggle.isOn  == true )
            {
                SystemModeTweenManage.Instance.PersonTree_click();
                SystemModeTweenManage.Instance.TweenSequence.Restart();
            }
            else if (ActionBarManage.Instance.DevToggle.isOn  == true)
            {
                TopoTweenerManage.Instance.TopoTree_click();

               
                TopoTweenerManage.Instance.TopoTweenSequence.PlayBackwards();
            }
            else if (ActionBarManage.Instance.TwoVotesToggle.isOn  == true)
            {
                if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.工作票)
                {
                    TwoTicketSystemUITweenManage.Instance.SetTwoVotes_click();
                    
                    TwoTicketSystemUITweenManage.Instance.TwoVotesTweenSequence.PlayBackwards();
                 
                }
                else if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.操作票)
                {
                    OperationTicketDetailsTweenManage.Instance. SetMobilePatrol_click();
                   // OperationTicketDetailsRestoreAndAppear();
                    OperationTicketDetailsTweenManage.Instance.MobilePatrolTweenSequence.PlayBackwards();
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
        obj.transform.GetChild(0).gameObject.SetActive(true);
        if (TweenToggle.isOn == false)
        {
            obj.transform.GetChild(1).GetComponent<Text>().text = "收起UI";
          
        }
        else
        {
            obj.transform.GetChild(1).GetComponent<Text>().text = "展开UI";
        }
        obj.transform.GetChild(2).GetComponent<Image>().sprite = ClckImage;
    }
    public void Exit_toggle(GameObject obj)
    {
        obj.transform.GetChild(0).gameObject.SetActive(false );
        obj.transform.GetChild(1).GetComponent<Text>().text = "  ";
        obj.transform.GetChild(2).GetComponent<Image>().sprite = ExitImage;
    }
   
    

  
    
    
}
