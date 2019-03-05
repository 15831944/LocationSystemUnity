using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FVTButtonGroupUI : MonoBehaviour {
    /// <summary>
    /// 格子(整体大背景)
    /// </summary>
    public Image BgGrid;
    /// <summary>
    /// 按钮组背景
    /// </summary>
    public Image ButtonGroupBg;
    /// <summary>
    /// 工厂按钮图片
    /// </summary>
    public Image FactoryImage;
    /// <summary>
    /// 工厂文本框
    /// </summary>
    public Text FactoryText;
    /// <summary>
    /// 锅炉区按钮图片
    /// </summary>
    public Image BoilerImage;
    /// <summary>
    /// 锅炉区文本
    /// </summary>
    public Text BoilerText;
    /// <summary>
    /// 水能源区域
    /// </summary>
    public Image WaterImage;
    /// <summary>
    /// 水能源区域文本
    /// </summary>
    public Text WaterText;
    /// <summary>
    /// 气能源图片
    /// </summary>
    public Image GasEnergyImage;
    /// <summary>
    /// 气能源文本
    /// </summary>
    public Text GasEnergyText;
    /// <summary>
    /// 生活区图片
    /// </summary>
    public Image LifeAreaImage;
    /// <summary>
    /// 生活区文本
    /// </summary>
    public Text LifeAreaText;
    /// <summary>
    /// 整厂图片
    /// </summary>
    public Image FullFactoryImage;
    /// <summary>
    /// 整厂文本
    /// </summary>
    public Text FullFactoryText;
	// Use this for initialization
	void Start () {
        //RecordObjState();
    }
	
	// Update is called once per frame
	void Update () {
		
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
    /// 记录物体初始信息
    /// </summary>
    private void RecordObjState()
    {
        AddState(BgGrid.gameObject);
        AddState(ButtonGroupBg.gameObject);
        AddState(FactoryImage.gameObject);
        AddState(FactoryText.gameObject);
        AddState(BoilerImage.gameObject);
        AddState(BoilerText.gameObject);
        AddState(WaterImage.gameObject);
        AddState(WaterText.gameObject);
        AddState(GasEnergyImage.gameObject);
        AddState(GasEnergyText.gameObject);
        AddState(LifeAreaImage.gameObject);
        AddState(LifeAreaText.gameObject);
    }
    /// <summary>
    /// 关闭Toggle的背景
    /// </summary>
    private void HideImage()
    {
        FactoryImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
        BoilerImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
        WaterImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
        GasEnergyImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
        LifeAreaImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
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
