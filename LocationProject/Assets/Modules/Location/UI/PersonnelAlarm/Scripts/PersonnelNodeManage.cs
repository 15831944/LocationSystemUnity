using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelNodeManage : MonoBehaviour
{

    public List<StateSprite> stateSprites;//相关状态图片集合

    public Text txtLowBattery;//低电量

    ///// <summary>
    ///// 窗口
    ///// </summary>
    //public GameObject window;
    //告警节点
    public PersonnelAlarmNode personnelAlarmNode;
    //正常节点
    public PersonnelNormalNode personnelNormalNode;

    PersonInfoUI infoUi;

    void Start()
    {
        if (infoUi == null)
        {
            infoUi = GetComponentInParent<PersonInfoUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 显示告警界面
    /// </summary>
    [ContextMenu("ShowAlarm")]
    public void ShowAlarm()
    {
        HideNormal();
        InitSprites();
        CanvasGroup canvasGroup = personnelAlarmNode.GetComponent<CanvasGroup>();
        SetCanvasGroup(canvasGroup, true);
    }

    public void HideAlam()
    {
        CanvasGroup canvasGroup = personnelAlarmNode.GetComponent<CanvasGroup>();
        SetCanvasGroup(canvasGroup, false);
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    [ContextMenu("ShowNormal")]
    public void ShowNormal()
    {
        HideAlam();
        InitSprites();
        CanvasGroup canvasGroup = personnelNormalNode.GetComponent<CanvasGroup>();
        SetCanvasGroup(canvasGroup, true);
    }

    public void HideNormal()
    {
        CanvasGroup canvasGroup = personnelNormalNode.GetComponent<CanvasGroup>();
        SetCanvasGroup(canvasGroup, false);
    }

    public void SetCanvasGroup(CanvasGroup canvasGroup, bool b)
    {
        if (b)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// 初始化图标
    /// </summary>
    public void InitSprites()
    {
        if (infoUi == null)
        {
            infoUi = GetComponentInParent<PersonInfoUI>();
        }
        PersonInfoUIState stateT = infoUi.state;

        SwitchStateSprite(stateT);
    }

    /// <summary>
    /// 转换对应状态的图标
    /// </summary>
    /// <param name="stateT"></param>
    private void SwitchStateSprite(PersonInfoUIState stateT)
    {
        StateSprite stateSprite = stateSprites.Find((i) => i.Name == stateT.ToString());
        if (stateSprite != null)
        {
            ChangeNormalSprite(stateSprite);

            ChangeAlarmSprite(stateSprite);
        }
    }

    /// <summary>
    /// 切换正常相关图片
    /// </summary>
    /// <param name="stateSprite"></param>
    private void ChangeNormalSprite(StateSprite stateSprite)
    {
        Toggle normalToggle = personnelNormalNode.GetComponentInChildren<Toggle>(true);
        Image toggleImage1 = normalToggle.GetComponent<Image>();
        toggleImage1.sprite = stateSprite.sprite;
        SpriteState ss1 = new SpriteState();
        ss1.highlightedSprite = stateSprite.highlight;
        normalToggle.spriteState = ss1;
        Image checkImage1 = normalToggle.graphic.GetComponent<Image>();
        checkImage1.sprite = stateSprite.highlight;
    }

    /// <summary>
    /// 切换告警相关图片
    /// </summary>
    /// <param name="stateSprite"></param>
    private void ChangeAlarmSprite(StateSprite stateSprite)
    {
        Toggle alarmToggle = personnelAlarmNode.GetComponentInChildren<Toggle>(true);
        Image toggleImage2 = alarmToggle.GetComponent<Image>();
        toggleImage2.sprite = stateSprite.sprite_alarm;
        SpriteState ss2 = new SpriteState();
        ss2.highlightedSprite = stateSprite.highlight_alarm;
        alarmToggle.spriteState = ss2;
        Image checkImage2 = alarmToggle.graphic.GetComponent<Image>();
        checkImage2.sprite = stateSprite.highlight_alarm;
    }

    public void ChangeState(PersonInfoUIState stateT )
    {
        infoUi.state = stateT;
    }

    /// <summary>
    /// 正常
    /// </summary>
    [ContextMenu("SwitchNormal")]
    public void SwitchNormal()
    {
        ChangeState(PersonInfoUIState.Normal);
        SwitchStateSprite(PersonInfoUIState.Normal);
        //infoUi.l
    }

    /// <summary>
    /// 待机状态
    /// </summary>
    [ContextMenu("SwitchStandby")]
    public void SwitchStandby()
    {
        ChangeState(PersonInfoUIState.Standby);
        SwitchStateSprite(PersonInfoUIState.Standby);
    }

    /// <summary>
    /// 待机长时间不动
    /// </summary>
    [ContextMenu("SwitchStandbyLong")]
    public void SwitchStandbyLong()
    {
        ChangeState(PersonInfoUIState.Standby);
        SwitchStateSprite(PersonInfoUIState.Standby);
    }

    /// <summary>
    /// 离开
    /// </summary>
    [ContextMenu("SwitchLeave")]
    public void SwitchLeave()
    {
        ChangeState(PersonInfoUIState.Leave);
        SwitchStateSprite(PersonInfoUIState.Leave);
    }

    bool b=false;
    
    /// <summary>
    /// 设置为弱电状态
    /// </summary>
    [ContextMenu("SetLOWBATTERY")]
    public void SetLowBattery()
    {
        b = !b;
        //SwitchStateSprite(PersonInfoUIState.LOWBATTERY);
        SetLowBatteryActive(b);
    }
    public void SetLowBatteryActive(bool isActive)
    {
        txtLowBattery.gameObject.SetActive(isActive);
    }
}

/// <summary>
/// 状态图片
/// </summary>
[Serializable]
public class StateSprite
{
    public string Name;//名称
    public Sprite sprite;//图片
    public Sprite highlight;//高亮图片
    public Sprite sprite_alarm;//告警图片
    public Sprite highlight_alarm;//告警高亮图片
}
