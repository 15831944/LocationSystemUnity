using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// UGUI的提示框
/// 设置信息框panel的Pivot为（0,1）
/// </summary>
public class UGUITooltip : MonoBehaviour {

    public static UGUITooltip Instance;

    /// <summary>
    /// 鼠标移出屏幕内边距
    /// </summary>
    public float padding = 5f;//内边距
    /// <summary>
    /// 提示框
    /// </summary>
    public GameObject panel;
    /// <summary>
    /// 提示框相对鼠标的偏移量
    /// </summary>
    [Header("提示框相对鼠标的偏移量")]
    public Vector2 offset = new Vector2(10, -15);
    /// <summary>
    /// 画布组，可以用于调节整体的透明度
    /// </summary>
    private CanvasGroup canvasGroup;
    /// <summary>
    /// 提示的内容
    /// </summary>
    private Text tip;
    /// <summary>
    /// 延迟多长时间显示提示框
    /// </summary>
    public float delayTime = 1f;
    /// <summary>
    /// 动画持续时间
    /// </summary>
    public float duration = 0.5f;

    public ContentSizeFitter contentSizeFitter;
    private Canvas canvas;
    private CanvasScaler canvasScaler;


    // Use this for initialization
    void Start () {
        Instance = this;
        if (panel)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            tip = panel.GetComponentInChildren<Text>();
            contentSizeFitter= panel.GetComponent<ContentSizeFitter>(); 
        }
        if (canvas == null)
        {
            canvas = gameObject.GetComponentInParent<Canvas>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();
        }
        Hide();
    }
	
	// Update is called once per frame
	void Update () {

	}

    /// <summary>
    /// 显示提示内容
    /// </summary>
    public void ShowTooltip(string txt)
    {
        tip.text = txt;
        SetPanelActive(true);
        SetPosition();
        ShowAnimation();
    }

    /// <summary>
    /// 设置提示框的位置
    /// </summary>
    public void SetPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        //Vector3 panelPos = new Vector3(screenPos.x - (Screen.width / 2) + offset.x, screenPos.y - (Screen.height / 2) + offset.y);
        Vector3 panelPos = UGUIFollowTarget.ScreenToUI(Camera.main, canvasScaler, screenPos);
        Vector3 pos= AdjustOutBorder(panelPos);
        panel.transform.localPosition = pos; 
    }

    /// <summary>
    /// 调整信息框是否越界
    /// </summary>
    public Vector3 AdjustOutBorder(Vector3 pos)
    {
        Vector3 screenPos = Input.mousePosition;
        HorizontalLayoutGroup group = panel.GetComponent<HorizontalLayoutGroup>();
        Vector2 panelSize = new Vector2(tip.preferredWidth + group.padding.left + group.padding.right, 
            tip.preferredHeight + group.padding.top + group.padding.bottom);

        RectTransform rect = panel.GetComponent<RectTransform>();
        //如果字体宽度小于原本文本框宽度
        if (panelSize.x < rect.sizeDelta.x)
        {
            panelSize = new Vector2(rect.sizeDelta.x, panelSize.y);
        }
        //如果字体高度于原本文本框高度
        if (panelSize.y < rect.sizeDelta.y)
        {
            panelSize = new Vector2(panelSize.x, rect.sizeDelta.y);
        }
        //Debug.LogError(panelSize);
        //Debug.LogError(tip.preferredWidth);
        //Debug.LogError(tip.preferredHeight);
        //Debug.LogError("preferred" + tip.preferredWidth);
        //Debug.LogError("preferred" + tip.preferredHeight);
        //Debug.LogError("flexible" + tip.flexibleWidth);
        //Debug.LogError("flexible" + tip.flexibleHeight);
        //Debug.LogError("min" + tip.minWidth);
        //Debug.LogError("min" + tip.minHeight);



        float X = screenPos.x + panelSize.x - Screen.width;
        float Y = screenPos.y - panelSize.y;
        //Vector3 pos = panel.transform.localPosition;
        //如果信息框显示位置的右边超出屏幕，将信息框往左挪
        if (X > 0)
        {
            pos = new Vector3(pos.x - offset.x, pos.y, pos.z);//先减掉x偏移量
            pos = new Vector3(pos.x - X, pos.y, pos.z);
        }
        //如果信息框显示位置的底部超出屏幕，将信息框往上挪
        if (Y < 0)
        {
            if (X > 0)//说明x偏移量已经减掉
            {
                pos = new Vector3(pos.x, pos.y - offset.y, pos.z);//减掉y偏移量
            }
            else
            {
                pos = new Vector3(pos.x - offset.x, pos.y - offset.y, pos.z);//先减掉偏移量
            }

            pos = new Vector3(pos.x, pos.y + panelSize.y, pos.z);//显示在鼠标的上边
        }
        return pos;
    }

    /// <summary>
    /// 隐藏提示框
    /// </summary>
    public void Hide()
    {
        //HideAnimation();
        SetPanelActive(false);
    }

    /// <summary>
    /// 隐藏提示框
    /// </summary>
    public void SetPanelActive(bool b)
    {
        panel.SetActive(b);
    }

    /// <summary>
    /// 执行显示时的动画
    /// </summary>
    public void ShowAnimation()
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, duration);
    }

    /// <summary>
    /// 执行隐藏时的动画
    /// </summary>
    public void HideAnimation()
    {
        canvasGroup.DOFade(0, duration).OnComplete(() => { SetPanelActive(false); });
    }
}
