using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class OperationTicketHistoryItem : MonoBehaviour
    {

        public OperationTicketHistory info;//信息

        public Text txtNumber;//编号
        public Text txtGuardian;//监护人
        public Text txtOperator;//操作人
        public Text txtStartTime;//工作开始时间
        public Text txtEndTime;//工作结束时间
        public Text txtDutyOfficer;//值班负责人
        public Text txtDispatch;//调度
                                        //public Text txtDetails;//详情
        public Button detailBtn;//详情按钮

        // Use this for initialization
        void Start()
        {
            detailBtn.onClick.AddListener(DetailBtn_OnClick);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void Init(OperationTicketHistory infoT)
        {
            info = infoT;

            txtNumber.text = info.No;
            txtGuardian.text = info.Guardian;
            txtOperator.text = info.Operator;
            txtStartTime.text = info.OperationStartTime.ToString("yyyy/MM/dd HH:mm");
            txtEndTime.text = info.OperationEndTime.ToString("yyyy/MM/dd HH:mm");
            txtDutyOfficer.text = info.DutyOfficer;
            txtDispatch.text = info.Dispatch;
            //txtDetails.text = info.No;
        }

        /// <summary>
        /// 详情按钮事件触发
        /// </summary>
        public void DetailBtn_OnClick()
        {
            Debug.Log("DetailBtn_OnClick!");
            TwoTicketHistoryUI_N.Instance.ShowOperationTicketHistoryDetailsUI(info);
        }
    }
}
