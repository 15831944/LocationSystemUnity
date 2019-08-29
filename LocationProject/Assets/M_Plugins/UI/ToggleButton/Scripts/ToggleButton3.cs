using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton3 : MonoBehaviour
{
    /// <summary>
    /// 按钮
    /// </summary>
    public Button btn;
    /// <summary>
    /// 文字
    /// </summary>
    public Text txt;
    /// <summary>
    /// 图片
    /// </summary>
    [HideInInspector]
    public Image image;
    /// <summary>
    /// 正常图片
    /// </summary>
    public Sprite normalSprite;
    /// <summary>
    /// 选中图片
    /// </summary>
    public Sprite selectSprite;
    /// <summary>
    /// 是否选中
    /// </summary>
    public bool ison;

    /// <summary>
    /// 值改变触发事件
    /// </summary>
    public Action<bool> OnValueChanged;

    // Use this for initialization
    void Start()
    {
        if (btn == null)
        {
            btn = GetComponent<Button>();
        }
        if (image == null)
        {
            image = btn.image;
        }
        btn.onClick.AddListener(Btn_OnClick);
        SetImage(ison);
        EventTriggerListener txtListen = EventTriggerListener.Get(txt.gameObject);
        txtListen.onClick = Txt_OnClick;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 按钮触发事件
    /// </summary>
    public void Btn_OnClick()
    {
        ison = !ison;
        SetToggle(ison);
    }

    public void SetToggle(bool isonT)
    {
        ison = isonT;
        SetImage(isonT);
        ToggleButton3_OnValueChanged(isonT);
    }

    public void ToggleButton3_OnValueChanged(bool isonT)
    {
        if (OnValueChanged != null)
        {
            OnValueChanged(isonT);
        }
    }

    /// <summary>
    /// 设置按钮图片
    /// </summary>
    private void SetImage(bool ison)
    {
        if (image == null)
        {
            image = btn.image;
        }
        if (ison)
        {
            image.sprite = selectSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }

    /// <summary>
    /// 点击文字触发事件
    /// </summary>
    public void Txt_OnClick(GameObject o)
    {
        Btn_OnClick();
    }

    /// <summary>
    /// 恢复状态
    /// </summary>
    public void RReset()
    {
        //ison = false;
        //SetImage(false);
        SetState(false);
    }

    /// <summary>
    /// 设置状态,不触发事件
    /// </summary>
    public void SetState(bool b)
    {
        ison = b;
        SetImage(b);
    }
}
