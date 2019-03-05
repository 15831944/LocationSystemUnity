using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchorSignSet : MonoBehaviour {
    public InputField InputX;
    public InputField InputY;
    public InputField InputZ;
    public Button ChangePos;

    public bool IsArchorSet;

    private ArchorLocation archorPart;
	// Use this for initialization
	void Start () {
        ChangePos.onClick.AddListener(ChangeSignPos);
        SceneEvents.DepNodeChanged += OnDepNodeChange;
    }
    private void OnDepNodeChange(DepNode last,DepNode newNode)
    {
        IsArchorSet = false;
        InputX.text = "";
        InputY.text = "";
        InputZ.text = "";
    }
    private void GetArchorPart()
    {
        if (archorPart == null) archorPart = ArchorToolManage.Instance.ArchroSetPart;
    }
	/// <summary>
    /// 更换标志位置
    /// </summary>
    private void ChangeSignPos()
    {
        IsArchorSet = true;
        float x = GetValue(InputX);
        float y = GetValue(InputY);
        float z = GetValue(InputZ);
        Vector3 pos = new Vector3(x,y,z);
        Vector3 converSionUnit = LocationManager.Instance.LocationOffsetScale;
        //获取在Unity中的实际距离
        Vector3 relativePos = new Vector3(-pos.x / converSionUnit.x, pos.y, -pos.z / converSionUnit.z);
        GetArchorPart();
        archorPart.SignObject.transform.position = GetLeftBottomPos() + relativePos;       
    }
    /// <summary>
    /// 通过拖拽，修改基准点位置
    /// </summary>
    public void ChangePosByMove(Vector3 signPos)
    {
        IsArchorSet = true;
        Vector3 offset = signPos - GetLeftBottomPos();
        Vector3 converSionUnit = LocationManager.Instance.LocationOffsetScale;
        //获取在Unity中的实际距离
        Vector3 relativePos = new Vector3(-offset.x * converSionUnit.x, offset.y, -offset.z * converSionUnit.z);
        InputX.text = Math.Round(relativePos.x, 3).ToString();
        InputY.text = Math.Round(relativePos.y, 3).ToString();
        InputZ.text = Math.Round(relativePos.z, 3).ToString();
    }
    /// <summary>
    /// 获取输入框值
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private float GetValue(InputField field)
    {
        if (string.IsNullOrEmpty(field.text)) return 0;
        else
        {
            return float.Parse(field.text);
        }
    }
    /// <summary>
    /// 获取左下角位置
    /// </summary>
    /// <returns></returns>
    private Vector3 GetLeftBottomPos()
    {
        DepNode currentDep = FactoryDepManager.currentDep;
        if(currentDep as RoomController)
        {
            RoomController room = currentDep as RoomController;
            return room.RoomDevContainer.transform.position;
        }else if(currentDep as FloorController)
        {
            FloorController floor = currentDep as FloorController;
            return floor.RoomDevContainer.transform.position;
        }
        else
        {
            return LocationManager.Instance.axisZero;
        }
    }

}
