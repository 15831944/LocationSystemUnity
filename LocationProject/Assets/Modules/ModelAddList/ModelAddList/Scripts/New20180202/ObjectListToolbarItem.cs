using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectListToolbarItem : MonoBehaviour {
    /// <summary>
    /// Toggle
    /// </summary>
    private Toggle toggle;
    /// <summary>
    /// 图标
    /// </summary>
    private Image icon;
    /// <summary>
    /// 文字
    /// </summary>
    private Text txt;
    /// <summary>
    /// 高亮的颜色
    /// </summary>
    public Color HighlightColor;
    /// <summary>
    /// 正常颜色
    /// </summary>
    public Color NormalColor;
    /// <summary>
    /// 高亮背景图片
    /// </summary>
    private GameObject HighlightImage;

    // Use this for initialization
    void Start () {
      //  toggle = GetComponent<Toggle>();
       // toggle.onValueChanged.AddListener(Toggle_OnValueChanged);
       // EventTriggerListener lis = EventTriggerListener.Get(gameObject);
       // lis.onEnter = HighlightOn;
      //  lis.onExit = HighlightOff;
       // HighlightImage = transform.GetChild(0).gameObject;
     //   if (HighlightImage.name != "Highlighter")
     //   {
     //       HighlightImage = null;
     //   }

     ////   icon = GetComponent<ToggleButton>().NormalImage;
     //   txt = GetComponentInChildren<Text>(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle_OnValueChanged(bool b)
    {
        Debug.LogError("Toggle_OnValueChanged:" + transform.parent.name);
        
        if (b)
        {
            Graphic graphic = toggle.graphic;
            txt.color = graphic.color;
        }
        else
        {
          //  SetIconAndTxtColor(NormalColor);
           // SetHighlightImage(false);
        }
    }

    /// <summary>
    /// 高亮操作
    /// </summary>
    public void HighlightOn(GameObject o)
    {
        if (toggle.isOn) return;
      //  SetIconAndTxtColor(HighlightColor);
       // SetHighlightImage(true);
    }

    /// <summary>
    /// 取消高亮操作
    /// </summary>
    public void HighlightOff(GameObject o)
    {
        if (toggle.isOn) return;
      //  SetIconAndTxtColor(NormalColor);
       // SetHighlightImage(false);
    }

    /// <summary>
    /// 设置字体和图标的颜色
    /// </summary>
    public void SetIconAndTxtColor(Color color)
    {
        icon.color = color;
        txt.color = color;
    }

    /// <summary>
    /// 设置高亮背景图片
    /// </summary>
    /// <param name="b"></param>
    public void SetHighlightImage(bool b)
    {
        if (HighlightImage == null) return;
        HighlightImage.SetActive(b);
    }
}
