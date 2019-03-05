using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FVTPhotoUI : MonoBehaviour {
    /// <summary>
    /// 3D框的图片
    /// </summary>
    public GameObject ThreeDAreaImage;
    /// <summary>
    /// 3D框下方横线
    /// </summary>
    public GameObject ThreeDBottomLine;
    /// <summary>
    /// 照片区域背景和图片
    /// </summary>
    public GameObject PhotoAreaBg;
    /// <summary>
    /// 圆环顶部装饰
    /// </summary>
    public GameObject circleTopDecration;
    /// <summary>
    /// 圆环左部装饰
    /// </summary>
    public GameObject circleLeftDecration;
    /// <summary>
    /// 照片顶部装饰
    /// </summary>
    public GameObject photoTopDecration;
    /// <summary>
    /// 照片按钮控制区域
    /// </summary>
    public GameObject PhotoControlPart;

    /// <summary>
    /// 园区统计信息
    /// </summary>
    public GameObject AreaInfo;

    /// <summary>
    /// 进入电厂按钮
    /// </summary>
    public GameObject enterFacotoryBtn;
    /// <summary>
    /// 进入电厂按钮文本框
    /// </summary>
    public Text enterFactoryBtnText;
    /// <summary>
    /// 介绍标题
    /// </summary>
    public Text introduceTitleText;
    /// <summary>
    /// 介绍文字
    /// </summary>
    public Text introduceContentText;
    /// <summary>
    /// 圆环区域，竖直的装饰线
    /// </summary>
    public GameObject ExtensionLine;
    /// <summary>
    /// 按钮组，重左往右的线
    /// </summary>
    public GameObject LeftToRightLine;
    /// <summary>
    /// 按钮区域，重上往下的线
    /// </summary>
    public GameObject TopToBottomLine;
    /// <summary>
    /// 退出程序按钮
    /// </summary>
    public GameObject appExitButton;
	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        RecordObjState();
#endif
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 记录物体初始信息
    /// </summary>
    private void RecordObjState()
    {
        AddState(ThreeDAreaImage.gameObject);
        AddState(ThreeDBottomLine);
        AddState(PhotoAreaBg);
        AddState(circleTopDecration);
        AddState(circleLeftDecration);
        AddState(photoTopDecration);
        AddState(PhotoControlPart);
        //AddState(PosBtn);
        AddState(enterFacotoryBtn);
        AddState(AreaInfo);
        AddState(PhotoControlPart);
        AddState(introduceTitleText.gameObject);
        AddState(introduceContentText.gameObject);
        AddState(ExtensionLine);
        AddState(LeftToRightLine);
        AddState(TopToBottomLine);
        AddState(appExitButton);
    }
    /// <summary>
    /// 恢复初始状态
    /// </summary>
    public void RecoverState()
    {
        //Debug.Log("Recover state.");
        foreach (var item in StateList)
        {
            GameObject obj = item.objs;
            obj.SetActive(item.IsActive);
            obj.transform.localEulerAngles = item.localAngles;
            obj.transform.localPosition = item.localPos;
            obj.transform.localScale = item.localSacles;
            if (item.IsFillAmount)
            {
                obj.GetComponent<Image>().fillAmount = item.fillAmount;
            }
            if (item.IsText)
            {
                //obj.GetComponent<Text>().text = item.TextContent;
                obj.GetComponent<Text>().text = "";
            }
        }
        //HideImage();
    }
    /// <summary>
    /// UI状态列表
    /// </summary>
    private List<ObjectState> StateList = new List<ObjectState>();
    /// <summary>
    /// 保存初始信息
    /// </summary>
    /// <param name="obj"></param>
    private void AddState(GameObject obj)
    {
        ObjectState state = new ObjectState(obj);
        StateList.Add(state);
    }
}
