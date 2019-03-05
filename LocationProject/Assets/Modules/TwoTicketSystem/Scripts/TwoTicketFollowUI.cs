using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoTicketFollowUI : MonoBehaviour {

    public bool isFinished;//是否完成

    public Button flagBtn;//标志按钮
    public Text txtFlagNum;//标志序号

    public Color unfinishedColor;//未完成颜色
    public Color finishedColor;//已完成颜色

    public Sprite unfinished_normal;
    public Sprite unfinished_hovel;
    public Sprite finished_normal;
    public Sprite finished_hovel;

    public GameObject Content;
    public Text txtContent;//详细内容

    private UGUIFollowTarget uguiFollowTarget;//跟随脚本

    // Use this for initialization
    void Start()
    {
        EventTriggerListener lis = EventTriggerListener.Get(flagBtn.gameObject);
        lis.onEnter = FlagBtn_OnEnter;
        lis.onExit = FlagBtn_OnExit;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(bool isFinishedT, string txtFlagNumT, string txtContentT)
    {
        isFinished = isFinishedT;
        InitButtonSprite();
        InitColor();
        SetFlagNum(txtFlagNumT);
        SetTxtContent(txtContentT);
        uguiFollowTarget = GetComponent<UGUIFollowTarget>();
    }

    /// <summary>
    /// 初始化Flag按钮的Sprite
    /// </summary>
    public void InitButtonSprite()
    {
        //if (isFinished)
        //{
        //    //flagBtn.image.sprite = finished_normal;
        //    SetFlagBtnImage(finished_normal);
        //    //flagBtn.spriteState = ChangeButtonSprite(finished_hovel);
        //}
        //else
        //{
        //    //flagBtn.image.sprite = unfinished_normal;
        //    SetFlagBtnImage(unfinished_normal);
        //    //flagBtn.spriteState = ChangeButtonSprite(unfinished_hovel);
        //}

        SetIsFinishednormalBackImage();
    }

    /// <summary>
    /// 设置按钮图片
    /// </summary>
    public void SetIsFinishednormalBackImage()
    {
        if (isFinished)
        {
            //flagBtn.image.sprite = finished_normal;
            flagBtn.image.sprite = finished_normal;
            //flagBtn.spriteState = ChangeButtonSprite(finished_hovel);
        }
        else
        {
            //flagBtn.image.sprite = unfinished_normal;
            flagBtn.image.sprite = unfinished_normal;
            //flagBtn.spriteState = ChangeButtonSprite(unfinished_hovel);
        }
    }


    public void SetButtonSprite(Sprite spriteT)
    {
        flagBtn.image.sprite = spriteT;
    }

    /// <summary>
    /// 改变按钮高亮的图片
    /// </summary>
    /// <param name="highlighted"></param>
    /// <returns></returns>
    public SpriteState ChangeButtonSprite(Sprite highlighted)
    {
        SpriteState ss = new SpriteState();
        ss.highlightedSprite = highlighted;
        return ss;
    }

    /// <summary>
    /// 鼠标进入标志按钮
    /// </summary>
    public void FlagBtn_OnEnter(GameObject o)
    {
        uguiFollowTarget.SetIsUp(true);
        SetFlagNumActive(false);
        if (isFinished)
        {
            SetButtonSprite(finished_hovel);
        }
        else
        {
            SetButtonSprite(unfinished_hovel);
        }
        ShowContent();
    }

    /// <summary>
    /// 鼠标移出标志按钮
    /// </summary>
    public void FlagBtn_OnExit(GameObject o)
    {
        SetFlagNumActive(true);
        HideContent();
        if (isFinished)
        {
            SetButtonSprite(finished_normal);
        }
        else
        {
            SetButtonSprite(unfinished_normal);
        }
        uguiFollowTarget.SetIsUp(false);
    }

    /// <summary>
    /// 显示详细内容
    /// </summary>
    public void ShowContent()
    {
        SetContentActive(true);
    }

    /// <summary>
    /// 关闭详细内容
    /// </summary>
    public void HideContent()
    {
        SetContentActive(false);
    }

    /// <summary>
    /// 设置Content的显示隐藏
    /// </summary>
    public void SetContentActive(bool isActive)
    {
        Content.SetActive(isActive);
    }

    /// <summary>
    /// 设置详细内容信息
    /// </summary>
    public void SetTxtContent(string txt)
    {
        txtContent.text = txt;
    }

    /// <summary>
    /// 设置标志序号是显示
    /// </summary>
    /// <param name="txt"></param>
    public void SetFlagNumActive(bool isActive)
    {
        txtFlagNum.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 设置标志序号
    /// </summary>
    /// <param name="txt"></param>
    public void SetFlagNum(string txt)
    {
        txtFlagNum.text = txt;
    }

    /// <summary>
    /// 初始化颜色
    /// </summary>
    public void InitColor()
    {
        if (isFinished)
        {
            txtFlagNum.color = finishedColor;
        }
        else
        {
            txtFlagNum.color = unfinishedColor;
        }
    }
}
