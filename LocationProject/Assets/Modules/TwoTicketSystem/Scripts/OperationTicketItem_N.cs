using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using TwoTicketSystem;
using UnityEngine;
using UnityEngine.UI;

public class OperationTicketItem_N : MonoBehaviour {
    private Toggle itemToggle;//Item按钮
    public Text txtNumber;//编号
    public Text txtPerson;//负责人

    public OperationTicket info;//操作票信息

    public Text personnelNumText;
    public ChangeTextColor changeTextColor;
    // Use this for initialization
    void Start()
    {
        itemToggle = GetComponent<Toggle>();
        if (itemToggle != null)
        {
            TwoTicketSystemUI_N.Instance.ToggleGroupAdd(itemToggle);
            itemToggle.onValueChanged.AddListener(ItemToggle_OnValueChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="numberStr"></param>
    /// <param name="personStr"></param>
    public void Init(OperationTicket infoT)
    {
        personnelNumText.text = TwoTicketSystemUI_N.Instance.OperationNum.ToString();
        info = infoT;
        UpdateData(info.No, info.Guardian);
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="numberStr"></param>
    /// <param name="personStr"></param>
    public void UpdateData(string numberStr, string personStr)
    {
        txtNumber.text = numberStr;
        txtPerson.text = personStr;
    }

    /// <summary>
    /// Item按钮触发
    /// </summary>
    public void ItemToggle_OnValueChanged(bool ison)
    {
        if (ison)
        {
            print("ItemBtn_OnClick!");
            OperationTicketDetailsUI_N.Instance.Show(info);
            TwoTicketSystemManage.Instance.ShowOperationTicketPath(info);
            ToggleGroup toggleGroup = TwoTicketSystemUI_N.Instance.toggleGroup;
            FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
            changeTextColor.ClickTextColor();
        }
        else
        {
            changeTextColor.NormalTextColor();
            TwoTicketSystemManage.Instance.Hide();
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            OperationTicketDetailsUI_N.Instance.SetWindowActive(false);
        }
    }
}
