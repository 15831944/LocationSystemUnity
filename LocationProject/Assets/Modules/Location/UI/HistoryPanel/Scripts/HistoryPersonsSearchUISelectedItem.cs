using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryPersonsSearchUISelectedItem : MonoBehaviour {

    public Text txtName;//名称
    public Button deleteBtn;//删除按钮

    /// <summary>
    /// 人员信息数据
    /// </summary>
    [HideInInspector]
    [System.NonSerialized]
    public Personnel personnel;

    // Use this for initialization
    void Start () {
        deleteBtn.onClick.AddListener(DeleteBtn_OnClick);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(Personnel personnelT)
    {
        personnel = personnelT;
        txtName.text = personnel.Name;
    }

    /// <summary>
    /// 删除按钮触发事件
    /// </summary>
    public void DeleteBtn_OnClick()
    {
        Debug.Log("移除人员！");
        HistoryPersonsSearchUI.Instance.SetSelectPersonnelItemToggle(personnel, false);
    }
}
