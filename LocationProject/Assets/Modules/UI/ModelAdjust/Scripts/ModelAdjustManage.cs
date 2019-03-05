using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ModelAdjustManage : MonoBehaviour {
    public static ModelAdjustManage Instance;
    public GameObject Window;
    /// <summary>
    /// 比例变化X
    /// </summary>
    public InputField PercentageX;
    /// <summary>
    /// 比例变化Y
    /// </summary>
    public InputField PercentageY;
    /// <summary>
    /// 比例变化Z
    /// </summary>
    public InputField PercentageZ;
    /// <summary>
    /// 位置偏移X
    /// </summary>
    public InputField LocationX;
    /// <summary>
    /// 位置偏移Y
    /// </summary>
    public InputField LocationY;
    /// <summary>
    /// 位置偏移Z
    /// </summary>
    public InputField LocationZ;
    /// <summary>
    /// 旋转角度X
    /// </summary>
    public InputField AngleX;
    /// <summary>
    /// 旋转角度Y
    /// </summary>
    public InputField AngleY;
    /// <summary>
    /// 旋转角度Z
    /// </summary>
    public InputField AngleZ;
    /// <summary>
    /// 初始化数据
    /// </summary>
    public GameObject InitalizeButton;
    /// <summary>
    /// 保存数据
    /// </summary>
    public GameObject SaveButton;
    /// <summary>
    /// 操作提示
    /// </summary>
    public GameObject PromptBox;
    /// <summary>
    /// 数据库中保存的坐标系转换配置信息
    /// </summary>
    private LocationManager LocationManagerData;
    private TransferOfAxesConfig cofig;
   

    // Use this for initialization
    void Start () {
        Instance = this;
         cofig = new TransferOfAxesConfig();

         LocationManagerData = new LocationManager();
       
        GetPercentageData();
        GetLocationData();
        GetAngleData();
        ChangeTextColor(InitalizeButton);
        ChangeTextColor(SaveButton);
        InitalizeButton.GetComponent<Button>().onClick.AddListener(InitializeBut);
        SaveButton.GetComponent<Button>().onClick.AddListener(SaveBut);
    }
	/// <summary>
    ///获取 比例
    /// </summary>
	public void GetPercentageData()
    {
        PercentageX.text = LocationManagerData.LocationOffsetScale.x.ToString();
        PercentageY.text = LocationManagerData.LocationOffsetScale.y.ToString();
        PercentageZ.text = LocationManagerData.LocationOffsetScale.z.ToString();

    }
    /// <summary>
    /// 获取位置偏移
    /// </summary>
    public void GetLocationData()
    {
        LocationX.text = LocationManagerData.axisZero.x.ToString();
        LocationY.text = LocationManagerData.axisZero.y.ToString();
        LocationZ.text = LocationManagerData.axisZero.z.ToString();
    }
    /// <summary>
    /// 获取旋转角度
    /// </summary>
    public void GetAngleData()
    {
        AngleX.text = LocationManagerData.direction.x.ToString();
        AngleY.text = LocationManagerData.direction.y.ToString();
        AngleZ.text = LocationManagerData.direction.z.ToString();
    }
    /// <summary>
    /// 设置比例数据
    /// </summary>
    public string  SetPercentageData()
    {
       
        string PercentageV3X = PercentageX.text;
        string PercentageV3Y = PercentageY.text;
        string PercentageV3Z = PercentageZ.text;
        string value = string.Format("{0},{1},{2}",PercentageV3X, PercentageV3Y, PercentageV3Z);
        return value;
    }
    /// <summary>
    /// 设置位置偏移数据
    /// </summary>
    public string  SetLocationData()
    {
       
        string LocationV3X = LocationX.text;
        string LocationV3Y = LocationY.text;
        string LocationV3Z = LocationZ.text;
        string value = string.Format("{0},{1},{2}", LocationV3X, LocationV3Y, LocationV3Z);
        return value;
    }
    /// <summary>
    /// 设置旋转角度
    /// </summary>
    public string  SetAngleData()
    {
        string AngleV3X = AngleX.text;
        string AngleV3Y = AngleY.text;
        string AngleV3Z = AngleZ.text;
        string value = string.Format("{0},{1},{2}", AngleV3X, AngleV3Y, AngleV3Z);
        return value;
    }
    /// <summary>
    /// 初始坐标数据化
    /// </summary>
    public void InitializeBut()
    {
        GetPercentageData();
        GetLocationData();
        GetAngleData();
    }
    /// <summary>
    /// 保存坐标
    /// </summary>
    public void SaveBut()
    {
        SaveLocationData();
    }
    
    /// <summary>
    /// 保存输入坐标的数据
    /// </summary>
    public void   SaveLocationData()
    {
        
        string StringPercentage = SetPercentageData();
        string StringLocation = SetLocationData();
        string StringAngle = SetAngleData();

        cofig.Zero = new ConfigArg();
        cofig.Direction = new ConfigArg();
        cofig.Scale = new ConfigArg();
        cofig.Zero.Value = StringLocation;
        cofig.Direction.Value = StringAngle;
        cofig.Scale.Value = StringPercentage;

        bool value = CommunicationObject.Instance .SetTransferOfAxesConfig(cofig);
        if (value)
        {
            PromptBox.SetActive(true);
            PromptBox.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
            PromptBox.gameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(0,0,0,0);
            PromptBox.gameObject.transform.GetChild(2).GetComponent<Text>().text = "保存成功";
            Invoke("ClosePromptBox", 1.4f);
        }
       else
        {
            PromptBox.SetActive(true);
            PromptBox.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            PromptBox.gameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
            PromptBox.gameObject.transform.GetChild(2).GetComponent<Text>().text = "保存失败";
            Invoke("ClosePromptBox", 1.4f);
        }
       
         
    }
    /// <summary>
    /// 改变字体颜色
    /// </summary>
    /// <param name="obj"></param>
   public  void ChangeTextColor(GameObject obj)
    {
        EventTriggerListener TextColor = EventTriggerListener.Get(obj);
        TextColor.onEnter = CheckTextColor;
        TextColor.onExit = NormalTextColor;
        TextColor .onClick = NormalTextColor;
    }
    /// <summary>
    /// 点击时的颜色
    /// </summary>
    /// <param name="obj"></param>
    public void CheckTextColor(GameObject obj)
    {
        obj.GetComponentInChildren  <Text>(true ).color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
    }
    /// <summary>
    /// 正常字体颜色
    /// </summary>
    
    public void NormalTextColor(GameObject obj)
    {
        obj.GetComponentInChildren<Text>(true).color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
    }
    
    /// <summary>
    /// 坐标系设置界面
    /// </summary>
    /// <param name="b"></param>
    public void ShowWindow(bool b)
    {
        if (b)
        {
            Window.SetActive(true);
        }
        else
        {
            Window.SetActive(false);
        }
    }
   
    /// <summary>
    /// 关闭提示操作栏
    /// </summary>
    public void ClosePromptBox()
    {
        PromptBox.SetActive(false );
    }
}
