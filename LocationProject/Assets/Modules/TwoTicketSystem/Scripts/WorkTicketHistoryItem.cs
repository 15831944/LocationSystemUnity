using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class WorkTicketHistoryItem : MonoBehaviour
    {

        public WorkTicketHistory info;//信息

        public Text txtNumber;//编号
        public Text txtResponsiblePerson;//负责人
        public Text txtIssuer;//签发人
        public Text txtStartTime;//工作开始时间
        public Text txtEndTime;//工作结束时间
        public Text txtWorkingConditions;//工作条件
        public Text txtWorkPermitPerson;//工作许可人
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
        public void Init(WorkTicketHistory infoT)
        {
            info = infoT;

            txtNumber.text = info.No;
            txtResponsiblePerson.text = info.PersonInCharge;
            txtIssuer.text = info.Lssuer;
            txtStartTime.text = info.StartTimeOfPlannedWork.ToString("yyyy/MM/dd HH:mm");
            txtEndTime.text = info.EndTimeOfPlannedWork.ToString("yyyy/MM/dd HH:mm");
            txtWorkingConditions.text = info.WorkCondition;
            txtWorkPermitPerson.text = info.Licensor;
            //txtDetails.text = info.No;
        }

        /// <summary>
        /// 详情按钮事件触发
        /// </summary>
        public void DetailBtn_OnClick()
        {
            Debug.Log("DetailBtn_OnClick!");
            TwoTicketHistoryUI_N.Instance.ShowWorkTicketHistoryDetailsUI(info);
        }
    }
}
