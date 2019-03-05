using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FullViewTweenCircleUI : MonoBehaviour {
    /// <summary>
    /// 十字线-垂直部分
    /// </summary>
    public GameObject VerticalLine;
    /// <summary>
    /// 十字线-水平部分
    /// </summary>
    public GameObject HorizontalLine;
    /// <summary>
    /// 中间部分X线
    /// </summary>
    public GameObject SmallLine;
    /// <summary>
    /// 左侧小段的线
    /// </summary>
    public GameObject SmallLeftLine;
    /// <summary>
    /// 右侧小段线
    /// </summary>
    public GameObject SmallRightLine;
    /// <summary>
    /// 小线段对应三角形
    /// </summary>
    public GameObject SmallLineTriangle;
    /// <summary>
    /// 菱形线1
    /// </summary>
    public Image DiamondLine1;
    /// <summary>
    /// 菱形线2
    /// </summary>
    public Image DiamondLine2;
    /// <summary>
    /// 菱形线3
    /// </summary>
    public Image DiamondLine3;
    /// <summary>
    /// 菱形线4
    /// </summary>
    public Image DiamondLine4;

    /// <summary>
    /// 圆圈1(最外圈)
    /// </summary>
    public GameObject Circle1;
    public GameObject Circle2;
    public GameObject Circle3;
    public GameObject Circle4;
    public GameObject Circle5;
    public GameObject Circle6;
    // Use this for initialization
    void Start () {
        RecordObjState();
    }
	/// <summary>
    /// 恢复初始状态
    /// </summary>
    public void RecoverState()
    {
        //Debug.Log("Recover state.");
        foreach(var item in StateList)
        {
            GameObject obj = item.objs;
            obj.SetActive(item.IsActive);
            obj.transform.localEulerAngles = item.localAngles;
            obj.transform.localPosition = item.localPos;
            obj.transform.localScale = item.localSacles;
            if(item.IsFillAmount)
            {
                obj.GetComponent<Image>().fillAmount = item.fillAmount;
            }
            if(item.IsText)
            {
                obj.GetComponent<Text>().text = item.TextContent;
            }
            //DOTween.Kill(obj);
        }
    }
    /// <summary>
    /// 记录物体初始信息
    /// </summary>
    private void RecordObjState()
    {
        AddState(VerticalLine);
        AddState(HorizontalLine);
        AddState(SmallLine);
        AddState(SmallLeftLine);
        AddState(SmallRightLine);
        AddState(SmallLineTriangle);
        AddState(DiamondLine1.gameObject);
        AddState(DiamondLine2.gameObject);
        AddState(DiamondLine3.gameObject);
        AddState(DiamondLine4.gameObject);

        AddState(Circle1);
        AddState(Circle2);
        AddState(Circle3);
        AddState(Circle4);
        AddState(Circle5);
        AddState(Circle6);
    }
    private List<ObjectState> StateList = new List<ObjectState>();
    private void AddState(GameObject obj)
    {
        ObjectState state = new ObjectState(obj);
        StateList.Add(state);
    }
}
public class ObjectState
{
    public GameObject objs;
    public Vector3 localPos;
    public Vector3 localAngles;
    public Vector3 localSacles;
    public bool IsActive;
    public bool IsFillAmount;
    public float fillAmount;
    public bool IsText;
    public string TextContent;
    public ObjectState(GameObject obj)
    {
        objs = obj;
        localPos = obj.transform.localPosition;
        localSacles = obj.transform.localScale;
        localAngles = obj.transform.localEulerAngles;
        IsActive = obj.activeInHierarchy;
        Image img = obj.GetComponent<Image>();
        if(img!=null)
        {
            if(img.type==Image.Type.Filled)
            {
                IsFillAmount = true;
                fillAmount = img.fillAmount;
            }
        }
        Text t = obj.GetComponent<Text>();
        if(t!=null)
        {
            IsText = true;
            TextContent = t.text;
        }
    }
}
