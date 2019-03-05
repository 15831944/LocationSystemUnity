using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using Assets.M_Plugins.Helpers.Utils;

public class DeviceEdit : MonoBehaviour
{
    public GameObject window;
    /// <summary>
    /// 名称输入框
    /// </summary>
    public InputField NameField;
    /// <summary>
    /// 标识码输入框
    /// </summary>
    public InputField IDField;

    /// <summary>
    /// X坐标输入框
    /// </summary>
    public InputField XPosField;
    /// <summary>
    /// Y坐标输入框
    /// </summary>
    public InputField YPosField;
    /// <summary>
    /// Z坐标输入框
    /// </summary>
    public InputField ZPosField;

    /// <summary>
    /// 角度输入框
    /// </summary>
    public InputField AngleField;

    /// <summary>
    /// CAD坐标A
    /// </summary>
    public Text CadPosA;
    /// <summary>
    /// CAD坐标B
    /// </summary>
    public Text CadPosB;

    public CameraInfoSetting CameraInfoSetting;//摄像头参数设置

    /// <summary>
    /// 当前显示设备
    /// </summary>
    private DevNode CurrentDev;
    /// <summary>
    /// 保留小数单位
    /// </summary>
    private string DecimalUnit = "f2";
    /// <summary>
    /// Float类型默认值
    /// </summary>
    private float DefaultValue = 0;
	// Use this for initialization
	void Start () {
        NameField.onEndEdit.AddListener(OnInputFieldEndEdit);
	    IDField.onEndEdit.AddListener(OnInputFieldEndEdit);
        XPosField.onEndEdit.AddListener(OnInputFieldEndEdit);
        YPosField.onEndEdit.AddListener(OnInputFieldEndEdit);
        ZPosField.onEndEdit.AddListener(OnInputFieldEndEdit);
	    AngleField.onEndEdit.AddListener(OnInputFieldEndEdit);

        XPosField.onValueChanged.AddListener(OnPosChange);
        YPosField.onValueChanged.AddListener(OnPosChange);
        ZPosField.onValueChanged.AddListener(OnPosChange);

	    //AngleField.onValueChanged.AddListener(OnAngleChange);
	}
    /// <summary>
    /// 设置设备位置信息
    /// </summary>
    /// <param name="pos"></param>
    public void ChangePos(DevNode dev,Vector3 pos)
    {
        if (dev != CurrentDev) return;
        NameField.text = dev.Info.Name;
        XPosField.text = Math.Round(pos.x, 2).ToString(CultureInfo.InvariantCulture);
        YPosField.text = Math.Round(pos.y, 2).ToString(CultureInfo.InvariantCulture);
        ZPosField.text = Math.Round(pos.z, 2).ToString(CultureInfo.InvariantCulture);
        SaveInfo();
    }
    private void OnInputFieldEndEdit(string value)
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("SaveInfo");
            SaveInfo();
        }
        else
        {
            SaveInfo();
            Debug.Log("OnEndEdit"+value);        
        }
    }
    /// <summary>
    /// 角度变化
    /// </summary>
    /// <param name="angleText"></param>
    private void OnAngleChange(string angleText)
    {
        DevPos pos = CurrentDev.Info.Pos;
        if (string.IsNullOrEmpty(angleText))
        {
            AngleField.text = pos.RotationY.ToString();
        }
    }
    /// <summary>
    /// 设置编辑的设备信息
    /// </summary>
    /// <param name="dev"></param>
    public void SetDeviceInfo(DevNode dev)
    {
        SetCameraWindow(dev);
        window.SetActive(true);
        CurrentDev = dev;
        NameField.text = dev.Info.Name;
        IDField.text = dev.Info.KKSCode;
        DevPos devPos = dev.Info.Pos;
        if (devPos != null)
        {
            Vector3 cadPos = new Vector3(devPos.PosX, devPos.PosY, devPos.PosZ);
            bool isLocation = !(CurrentDev.ParentDepNode == FactoryDepManager.Instance || CurrentDev is DepDevController);
            Vector3 pos = LocationManager.CadToUnityPos(cadPos, isLocation);
            XPosField.text = Math.Round(pos.x,2).ToString(CultureInfo.InvariantCulture);
            YPosField.text = Math.Round(pos.y, 2).ToString(CultureInfo.InvariantCulture);
            ZPosField.text = Math.Round(pos.z, 2).ToString(CultureInfo.InvariantCulture);

            AngleField.text = Math.Round(devPos.RotationY, 2).ToString(CultureInfo.InvariantCulture);

            CadPosA.text = Math.Round(devPos.PosZ, 2).ToString(CultureInfo.InvariantCulture);
            CadPosB.text = Math.Round(devPos.PosX, 2).ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            Debug.LogError("DevPos is null:"+dev.Info.Name);
            //ClearValue();
        }
    }
    /// <summary>
    /// 设置摄像头设置界面
    /// </summary>
    /// <param name="dev"></param>
    private void SetCameraWindow(DevNode dev)
    {
        if(dev is CameraDevController)
        {
            CameraInfoSetting.Show(dev as CameraDevController);
        }
        else
        {
            CameraInfoSetting.Close();
        }
    }
    /// <summary>
    /// 关闭窗体
    /// </summary>
    public void Close()
    {
        window.SetActive(false);
    }
    /// <summary>
    /// 保存信息
    /// </summary>
    private void SaveInfo()
    {
        Debug.Log("SaveInfo:" + CurrentDev.Info.Name);
        CheckInputValue();
        DeviceEditUIManager manager = DeviceEditUIManager.Instacne;
        if (CurrentDev != null)
        {
            CurrentDev.Info.Name = NameField.text;
            CurrentDev.Info.KKSCode = IDField.text;
            string typeCode = CurrentDev.Info.TypeCode.ToString();
            DevPos posInfo=null;
            if (!TypeCodeHelper.IsStaticDev(typeCode))
            {
                ChangeDevPosAngle();
                bool isLocation = !(CurrentDev.ParentDepNode == FactoryDepManager.Instance || CurrentDev is DepDevController);
                Vector3 cadPos = LocationManager.UnityToCadPos(TryParsePos(), isLocation);
                posInfo = CurrentDev.Info.Pos;
                if (posInfo != null)
                {
                    posInfo.PosX = cadPos.x;
                    posInfo.PosY = cadPos.y;
                    posInfo.PosZ = cadPos.z;
                    posInfo.RotationY = TryParseFloat(AngleField.text);
                }
                manager.ChangeDevFollowInfo(CurrentDev);
                manager.RefleshGizmoPosition();
            }
            manager.ChangeDevFollowInfo(CurrentDev);
            CommunicationObject service = CommunicationObject.Instance;
            if (service)
            {
                service.ModifyDevInfo(CurrentDev.Info);
                if (!TypeCodeHelper.IsStaticDev(typeCode)) service.ModifyDevPos(posInfo);
            }
        }
    }
    /// <summary>
    /// 检测输入框的值
    /// </summary>
    private void CheckInputValue()
    {
        if (TypeCodeHelper.IsStaticDev(CurrentDev.Info.TypeCode.ToString())) return;
        DevPos devPos = CurrentDev.Info.Pos;
        Vector3 cadPos = new Vector3(devPos.PosX, devPos.PosY, devPos.PosZ);
        bool isRoomDev = !(CurrentDev.ParentDepNode == FactoryDepManager.Instance || CurrentDev is DepDevController);
        Vector3 pos = LocationManager.CadToUnityPos(cadPos, isRoomDev);
        if (string.IsNullOrEmpty(NameField.text)) NameField.text = CurrentDev.Info.Name;
        CheckEmptyField(XPosField,pos.x);
        CheckEmptyField(YPosField, pos.y);
        CheckEmptyField(ZPosField, pos.z);
        CheckEmptyField(AngleField, devPos.RotationY);
    }
    /// <summary>
    /// 检测输入框是否为空
    /// </summary>
    /// <param name="input"></param>
    /// <param name="defaultValue"></param>
    private void CheckEmptyField(InputField input,float defaultValue)
    {
        if (string.IsNullOrEmpty(input.text)) input.text = Math.Round(defaultValue, 2).ToString(CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// 改变设备位置
    /// </summary>
    private void ChangeDevPosAngle()
    {
        bool isFactoryDev = CurrentDev.ParentDepNode == FactoryDepManager.Instance || CurrentDev is DepDevController;
        if (isFactoryDev)
        {
            CurrentDev.transform.position=TryParsePos();
        }
        else
        {
            CurrentDev.transform.localPosition = TryParsePos();
        }
        Vector3 angleTemp = CurrentDev.transform.eulerAngles;
        angleTemp.y= TryParseFloat(AngleField.text);
        CurrentDev.transform.eulerAngles = angleTemp;
    }
    /// <summary>
    /// 输入位置后，显示CAD位置
    /// </summary>
    private void OnPosChange(string value)
    {
        //bool isValueEmpty=string.IsNullOrEmpty(XPosField.text)|| string.IsNullOrEmpty(YPosField.text)|| string.IsNullOrEmpty(ZPosField.text)?true:false;
        //if (isValueEmpty) return;
        bool isLocation = !(CurrentDev.ParentDepNode == FactoryDepManager.Instance || CurrentDev is DepDevController);
        Vector3 cadPos = LocationManager.UnityToCadPos(TryParsePos(), isLocation);
        CadPosA.text = Math.Round(cadPos.z, 2).ToString(CultureInfo.InvariantCulture);
        CadPosB.text = Math.Round(cadPos.x, 2).ToString(CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// 获取输入坐标
    /// </summary>
    /// <returns></returns>
    private Vector3 TryParsePos()
    {
        try
        {
            DevPos devPos = CurrentDev.Info.Pos;
            float xPos = TryParseFloat(XPosField.text);
            float yPos = TryParseFloat(YPosField.text);
            float zPos = TryParseFloat(ZPosField.text);
            Vector3 pos = new Vector3(xPos, yPos, zPos);
            return pos;
        }
        catch (Exception e)
        {
            Debug.LogError("Input Pos Error");
            return Vector3.zero;
        }
    }
    /// <summary>
    /// string转Float
    /// </summary>
    /// <param name="num"></param>
    /// <param name="lastValue"></param>
    /// <returns></returns>
    private float TryParseFloat(string num)
    {
        try
        {
            float value = float.Parse(num);
            return value;
        }
        catch (Exception e)
        {
            return DefaultValue;
        }
    }
    ///// <summary>
    ///// CAD位置转Unity位置
    ///// </summary>
    ///// <param name="devNode"></param>
    ///// <returns></returns>
    //public Vector3 CadToUnityPos(DevNode devNode)
    //{
    //    //LocationManager manager = LocationManager.Instance;
    //    Vector3 pos;
    //    DevPos cadPos = devNode.Info.Pos;
    //    Vector3 posT = new Vector3(cadPos.PosX, cadPos.PosY, cadPos.PosZ);
    //    if (devNode as DepDevController)
    //    {
    //        pos = LocationManager.GetRealVector(posT);
    //    }
    //    else if (devNode as RoomDevController)
    //    {
    //        pos = CadToUnityLocalPos(posT);
    //    }
    //    else
    //    {
    //        Debug.Log("Controller not find..");
    //        pos = Vector3.zero;
    //    }
    //    return pos;
    //}

    ///// <summary>
    ///// unity位置转换cad位置
    ///// </summary>
    ///// <param name="posUnity"></param>
    ///// <param name="devNode"></param>
    ///// <returns></returns>
    //public Vector3 UnityPosToCad(Vector3 posUnity, DevNode devNode)
    //{
    //    Vector3 pos;
    //    if (devNode as DepDevController)
    //    {
    //        pos = LocationManager.GetCadVector(posUnity);
    //    }
    //    else if (devNode as RoomDevController)
    //    {
    //        pos = UnityLocalPosToCad(posUnity);
    //    }
    //    else
    //    {
    //        Debug.Log("Controller not find..");
    //        pos = Vector3.zero;
    //    }
    //    return pos;
    //}
    ///// <summary>
    ///// 获取CAD在3D中的LocalPos
    ///// </summary>
    ///// <param name="p"></param>
    ///// <returns></returns>
    //private Vector3 CadToUnityLocalPos(Vector3 p)
    //{
    //    Vector3 pos;
    //    LocationManager manager = LocationManager.Instance;
    //    if (manager.LocationOffsetScale.y == 0)
    //    {
    //        pos = new Vector3(p.x / manager.LocationOffsetScale.x, p.y / manager.LocationOffsetScale.x, p.z / manager.LocationOffsetScale.z);
    //    }
    //    else
    //    {
    //        pos = new Vector3(p.x / manager.LocationOffsetScale.x, p.y / manager.LocationOffsetScale.y, p.z / manager.LocationOffsetScale.z);
    //    }
    //    return pos;
    //}
    ///// <summary>
    ///// UnityLocalPos转CADPos
    ///// </summary>
    ///// <param name="localPos"></param>
    ///// <returns></returns>
    //private Vector3 UnityLocalPosToCad(Vector3 localPos)
    //{
    //    Vector3 tempPos;
    //    LocationManager manager = LocationManager.Instance;
    //    if (manager.LocationOffsetScale.y == 0)
    //    {
    //        tempPos = new Vector3(localPos.x * manager.LocationOffsetScale.x, localPos.y * manager.LocationOffsetScale.x, localPos.z * manager.LocationOffsetScale.z);
    //    }
    //    else
    //    {
    //        tempPos = new Vector3(localPos.x * manager.LocationOffsetScale.x, localPos.y * manager.LocationOffsetScale.y, localPos.z * manager.LocationOffsetScale.z);
    //    }
    //    return tempPos;
    //}
}
