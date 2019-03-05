using Assets.M_Plugins.Helpers.Utils;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonitorDeviceEdit : MonoBehaviour {

    public GameObject window;//窗体
    public InputField NameInput;//名称
    public InputField IDField;//设备编码

    public InputField XPosInput;//X轴位置
    public InputField YPosInput;//Y轴位置
    public InputField ZPosInput;//Z轴位置
    public InputField AngleInput;//角度

    public InputField ScaleXInput;//ScaleX
    public InputField ScaleYInput;//ScaleY
    public InputField ScaleZInput;//ScaleZ

    /// <summary>
    /// Float类型默认值
    /// </summary>
    private float DefaultValue = 0;
    /// <summary>
    /// 保留小数单位
    /// </summary>
    private string DecimalUnit = "f2";
    /// <summary>
    /// 当前显示设备
    /// </summary>
    private DevNode CurrentDev;

    void Start () {
        NameInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        IDField.onEndEdit.AddListener(OnInputFieldEndEdit);
        XPosInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        YPosInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        ZPosInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        AngleInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        ScaleXInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        ScaleYInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        ScaleZInput.onEndEdit.AddListener(OnInputFieldEndEdit);
    }
    /// <summary>
    /// 设置编辑的设备信息
    /// </summary>
    /// <param name="dev"></param>
    public void SetDeviceInfo(DevNode dev)
    {
        window.SetActive(true);
        CurrentDev = dev;
        NameInput.text = dev.Info.Name;
        IDField.text = dev.Info.KKSCode;
        DevPos devPos = dev.Info.Pos;
        if (devPos != null)
        {
            Vector3 cadPos = new Vector3(devPos.PosX, devPos.PosY, devPos.PosZ);
            bool isLocation = !(CurrentDev.ParentDepNode == FactoryDepManager.Instance);
            Vector3 pos = LocationManager.CadToUnityPos(cadPos, isLocation);
            XPosInput.text = Math.Round(pos.x, 2).ToString();
            YPosInput.text = Math.Round(pos.y, 2).ToString();
            ZPosInput.text = Math.Round(pos.z, 2).ToString();

            AngleInput.text = Math.Round(devPos.RotationY, 2).ToString();

            ScaleXInput.text = Math.Round(devPos.ScaleX, 2).ToString();
            ScaleYInput.text = Math.Round(devPos.ScaleY, 2).ToString();
            ScaleZInput.text = Math.Round(devPos.ScaleZ, 2).ToString();
        }
        else
        {
            Debug.LogError("DevPos is null:" + dev.Info.Name);
            //ClearValue();
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
    /// 输入框结束编辑
    /// </summary>
    /// <param name="value"></param>
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
            Debug.Log("OnEndEdit" + value);
        }
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
            CurrentDev.Info.Name = NameInput.text;
            CurrentDev.Info.KKSCode = IDField.text;
            string typeCode = CurrentDev.Info.TypeCode.ToString();
            DevPos posInfo = null;
            if (!TypeCodeHelper.IsStaticDev(typeCode))
            {
                ChangeDevPosAngle();
                bool isLocation = !(CurrentDev.ParentDepNode == FactoryDepManager.Instance);
                Vector3 cadPos = LocationManager.UnityToCadPos(TryParsePos(), isLocation);
                posInfo = CurrentDev.Info.Pos;
                if (posInfo != null)
                {
                    posInfo.PosX = cadPos.x;
                    posInfo.PosY = cadPos.y;
                    posInfo.PosZ = cadPos.z;
                    posInfo.RotationY = TryParseFloat(AngleInput.text);
                    posInfo.ScaleX = TryParseFloat(ScaleXInput.text);
                    posInfo.ScaleY = TryParseFloat(ScaleYInput.text);
                    posInfo.ScaleZ = TryParseFloat(ScaleZInput.text);
                }                
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
    /// 改变设备位置
    /// </summary>
    private void ChangeDevPosAngle()
    {
        if (CurrentDev as DepDevController)
        {
            CurrentDev.transform.position = TryParsePos();
        }
        else
        {
            CurrentDev.transform.localPosition = TryParsePos();
        }
        CurrentDev.transform.localScale = TryParseScale();
        Vector3 angleTemp = CurrentDev.transform.eulerAngles;
        angleTemp.y = TryParseFloat(AngleInput.text);
        CurrentDev.transform.eulerAngles = angleTemp;
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
            float xPos = TryParseFloat(XPosInput.text);
            float yPos = TryParseFloat(YPosInput.text);
            float zPos = TryParseFloat(ZPosInput.text);
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
    /// 获取物体的缩放
    /// </summary>
    /// <returns></returns>
    private Vector3 TryParseScale()
    {
        try
        {
            DevPos devPos = CurrentDev.Info.Pos;
            float scaleX = TryParseFloat(ScaleXInput.text);
            float scaleY = TryParseFloat(ScaleYInput.text);
            float scaleZ = TryParseFloat(ScaleZInput.text);
            Vector3 scaleTemp = new Vector3(scaleX, scaleY, scaleZ);
            return scaleTemp;
        }
        catch (Exception e)
        {
            Debug.LogError("Input Pos Error");
            return Vector3.one;
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
            value = (float)Math.Round(value,2);
            return value;
        }
        catch (Exception e)
        {
            return DefaultValue;
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
        bool isLocation = !(CurrentDev as DepDevController);
        Vector3 pos = LocationManager.CadToUnityPos(cadPos, isLocation);
        if (string.IsNullOrEmpty(NameInput.text)) NameInput.text = CurrentDev.Info.Name;
        CheckEmptyField(XPosInput, pos.x);
        CheckEmptyField(YPosInput, pos.y);
        CheckEmptyField(ZPosInput, pos.z);
        CheckEmptyField(AngleInput, devPos.RotationY);
        CheckEmptyField(ScaleXInput,1);
        CheckEmptyField(ScaleYInput,1);
        CheckEmptyField(ScaleZInput,1);
    }
    /// <summary>
    /// 检测输入框是否为空
    /// </summary>
    /// <param name="input"></param>
    /// <param name="defaultValue"></param>
    private void CheckEmptyField(InputField input, float defaultValue)
    {
        if (string.IsNullOrEmpty(input.text)) input.text = Math.Round(defaultValue, 2).ToString();
    }
   
}
