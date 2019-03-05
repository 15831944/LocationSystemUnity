using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 历史轨迹添加人员界面的UIItem
/// </summary>
public class HistoryPersonsSearchUIItem : MonoBehaviour {

    public Image photo;//头像
    public Text txtName;//名称
    public Text txtWorkNum;//工号
    public Text txtDepartment;//部门
    public Text txtPost;//岗位

    //[HideInInspector]
    public Toggle toggle;//Toggle
    [HideInInspector]
    public Personnel personnel;// 人员信息数据

    // Use this for initialization
    void Start ()
    {
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }
        if (HistoryPersonsSearchUI.Instance.IsAchisevePersonsLimitNum())
        {
            if (!toggle.isOn)
            {
                AddListener_Alarm();
            }
            else
            {
                AddListener();
            }
        }
        else
        {
            AddListener();
        }
    }

    public void AddListener()
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(Toggle_OnValueChanged);
    }

    public void AddListener_Alarm()
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(ToggleAlarm_OnValueChanged);
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitData(Personnel personnelT)
    {
        personnel = personnelT;
        txtName.text = personnel.Name;
        //if (personnel.Tag != null)
        //{
        //    txtWorkNum.text = personnel.Tag.Code;
        //}
        //else
        //{
            txtWorkNum.text = personnel.WorkNumber.ToString();
        //}

        if (personnel.Parent != null)
        {
            txtDepartment.text = personnel.Parent.Name;
        }

        if (personnel.Tag != null)
        {
            txtPost.text = personnel.Pst + personnel.Tag.Code;
        }
        else
        {
            txtPost.text = personnel.Pst;
        }
        LocationUIManage.Instance.SetPhoto(photo, personnel.Sex);
    }

    /// <summary>
    /// Toggle值改变触发事件
    /// </summary>
    public void Toggle_OnValueChanged(bool b)
    {
        if (b)
        {
            HistoryPersonsSearchUI.Instance.AddSelectPersonnelItem(personnel);
        }
        else
        {
            HistoryPersonsSearchUI.Instance.RemoveSelectPersonnelItem(personnel);
        }
    }

    /// <summary>
    /// Toggle值改变触发事件,超过人数限制警告触发
    /// </summary>
    public void ToggleAlarm_OnValueChanged(bool b)
    {
        if (b)
        {
            toggle.isOn = false;
            UGUIMessageBox.Show("显示历史轨迹不能超过" + MultHistoryPlayUI.Instance.limitPersonNum + "人！");
        }

    }

    public void SetToggle(bool b)
    {
        toggle.isOn = b;
    }

}
