using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class WorkTicketItem_N : MonoBehaviour
    {

        private Toggle itemToggle;//Item按钮
        public Text txtNumber;//编号
        public Text txtPerson;//负责人

        public WorkTicket info;//工作票信息
        public Text PersonnelNumText;
        public   ChangeTextColor changeTextColor;
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
        public void Init(WorkTicket infoT)
        {
            PersonnelNumText.text = TwoTicketSystemUI_N.Instance.WorkNum.ToString();
            info = infoT;
            UpdateData(info.No, info.PersonInCharge);

        }

        /// <summary>
        /// 刷新数据
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
                WorkTicketDetailsUI_N.Instance.Show(info);
                TwoTicketSystemManage.Instance.ShowWorkTicketPath(info);
                ToggleGroup toggleGroup = TwoTicketSystemUI_N.Instance.toggleGroup;
                FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
                changeTextColor.ClickTextColor();
            }
            else
            {
                changeTextColor.NormalTextColor();
                TwoTicketSystemManage.Instance.Hide();
                FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
                WorkTicketDetailsUI_N.Instance.SetWindowActive(false);
            }
        }
    }
}
