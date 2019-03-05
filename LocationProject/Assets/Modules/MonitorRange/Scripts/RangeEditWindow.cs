using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangeEditWindow : MonoBehaviour {
    public static RangeEditWindow Instance;

    public GameObject window;//窗体

    public InputField NameInput;//名称
    public Text TypeTxt;//区域类型

    public InputField APosInput;//A轴位置, CAD的南北方向
    public InputField BPosInput;//B轴位置，CAD的东西方向
    public InputField HPosInput;//H轴位置，高度
    public InputField AngleInput;//角度

    public InputField LengthInput;//长度
    public InputField WidthInput;//宽度
    public InputField HeightInput;//高度

    public MonitorRangeObject currentMRObj;//当前物体


    // Use this for initialization
    void Start () {
        Instance = this;
        EditorObjectSelection.Instance.SelectionChanged += Instance_SelectionChanged;
        EditorObjectSelection.Instance.GameObjectClicked += Instance_GameObjectClicked;

        NameInput.onEndEdit.AddListener(NameInput_OnEndEdit);
        APosInput.onEndEdit.AddListener(APosInput_OnEndEdit);
        BPosInput.onEndEdit.AddListener(BPosInput_OnEndEdit);
        HPosInput.onEndEdit.AddListener(HPosInput_OnEndEdit);
        AngleInput.onEndEdit.AddListener(AngleInput_OnEndEdit);
        LengthInput.onEndEdit.AddListener(LengthInput_OnEndEdit);
        WidthInput.onEndEdit.AddListener(WidthInput_OnEndEdit);
        HeightInput.onEndEdit.AddListener(HeightInput_OnEndEdit);



    }


    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// 选择切换
    /// </summary>
    /// <param name="selectionChangedEventArgs"></param>
    public void Instance_SelectionChanged(ObjectSelectionChangedEventArgs selectionChangedEventArgs)
    {
        Debug.Log("Instance_SelectionChanged!");
        if (selectionChangedEventArgs.SelectedObjects.Count == 0)
        {
            //EditorObjectSelection.Instance.ClearSelection(false);
            Hide();
        }
    }

    /// <summary>
    /// 点击物体
    /// </summary>
    /// <param name="clickedObject"></param>
    public void Instance_GameObjectClicked(GameObject clickedObject)
    {
        Debug.Log("Instance_GameObjectClicked!");
        currentMRObj = clickedObject.GetComponent<MonitorRangeObject>();
        if (currentMRObj)
        {
            Show(currentMRObj);
        }
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show(MonitorRangeObject mrobj)
    {
        currentMRObj = mrobj;
        SetWindowActive(true);
        NameInput.text = mrobj.info.Name;
        TypeTxt.text = "";
        if (mrobj.info.Transfrom.IsOnLocationArea)
        {
            TypeTxt.text = "定位监控区域";
        }
        else if (mrobj.info.Transfrom.IsOnAlarmArea)
        {
            TypeTxt.text = "告警区域";
        }
        else
        {
            TypeTxt.text = "普通区域";
        }

        //if (mrobj.info.Transfrom.IsRelative)
        //{
            //float posx = mrobj.transform.localPosition.x * mrobj.transform.parent.lossyScale.x;
            //float posy = mrobj.transform.localPosition.y * mrobj.transform.parent.lossyScale.y;
            //float posz = mrobj.transform.localPosition.z * mrobj.transform.parent.lossyScale.z;
            //Vector3 posT = LocationManager.GetDisRealSizeVector(new Vector3(posx, posy, posz));
            APosInput.text = Math.Round(mrobj.info.Transfrom.Z, 2).ToString();
            BPosInput.text = Math.Round(mrobj.info.Transfrom.X, 2).ToString();
            HPosInput.text = Math.Round(mrobj.info.Transfrom.Y, 2).ToString();
        //}
        //else
        //{
        //    Vector3 posT = LocationManager.GetDisRealSizeVector(transform.localPosition);
        //    APosInput.text = Math.Round(posT.z, 2).ToString();
        //    BPosInput.text = Math.Round(posT.x, 2).ToString();
        //    HPosInput.text = Math.Round(posT.y, 2).ToString();
        //}

        AngleInput.text = Math.Round(mrobj.info.Transfrom.RY, 2).ToString();

        //Vector3 sizeT = mrobj.gameObject.GetGlobalSize();
        //sizeT = LocationManager.GetDisRealSizeVector(sizeT);

        LengthInput.text= Math.Round(mrobj.info.Transfrom.SZ, 2).ToString();
        WidthInput.text= Math.Round(mrobj.info.Transfrom.SX, 2).ToString();
        HeightInput.text = Math.Round(mrobj.info.Transfrom.SY, 2).ToString();
    }

    ///// <summary>
    ///// 刷新数据
    ///// </summary>
    //public void RefleshData()
    //{

    //}

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindowActive(false);
    }

    /// <summary>
    /// 设置Window
    /// </summary>
    /// <param name="isActive"></param>
    public void SetWindowActive(bool isActive)
    {
        if (window.activeInHierarchy != isActive)
        {
            window.SetActive(isActive);
        }
    }

    public void SetObj()
    {
        currentMRObj.SaveInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    public void NameInput_OnEndEdit(string txtStr)
    {
        Debug.Log("NameInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void APosInput_OnEndEdit(string txtStr)
    {
        Debug.Log("APosInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void BPosInput_OnEndEdit(string txtStr)
    {
        Debug.Log("BPosInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void HPosInput_OnEndEdit(string txtStr)
    {
        Debug.Log("HPosInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void AngleInput_OnEndEdit(string txtStr)
    {
        Debug.Log("AngleInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void LengthInput_OnEndEdit(string txtStr)
    {
        Debug.Log("LengthInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void WidthInput_OnEndEdit(string txtStr)
    {
        Debug.Log("WidthInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }
    public void HeightInput_OnEndEdit(string txtStr)
    {
        Debug.Log("HeightInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    public void UpdateData()
    {
        float APos = float.Parse(APosInput.text);
        float BPos = float.Parse(BPosInput.text);
        float HPos = float.Parse(HPosInput.text);
        float Angle = float.Parse(AngleInput.text);

        float Length = float.Parse(LengthInput.text);
        float Width = float.Parse(WidthInput.text);
        float Height = float.Parse(HeightInput.text);

        Vector3 realpos = new Vector3(BPos, HPos, APos);
        Vector3 realSize = new Vector3(Width, Height, Length);
        Vector3 realAngle = new Vector3(0, Angle, 0);
        currentMRObj.UpdateData(NameInput.text, realpos, realAngle, realSize);
        RefleshGizmoPosition();
    }

    /// <summary>
    /// 刷新区域编辑Gizmo位置
    /// </summary>
    public void RefleshGizmoPosition()
    {
        Gizmo activeGizmo = EditorGizmoSystem.Instance.ActiveGizmo;
        if (activeGizmo == null)
        {
            Debug.Log("Active Gizmo is null...");
            return;
        }
        activeGizmo.transform.position = EditorObjectSelection.Instance.GetSelectionWorldCenter();
    }
}
