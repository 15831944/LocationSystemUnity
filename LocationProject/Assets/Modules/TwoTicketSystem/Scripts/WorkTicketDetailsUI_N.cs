using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    /// <summary>
    /// 工作票详情
    /// </summary>
    public class WorkTicketDetailsUI_N : MonoBehaviour
    {
        public static WorkTicketDetailsUI_N Instance;
        /// <summary>
        /// 窗体
        /// </summary>
        public GameObject window;

        public WorkTicket info;//工作票信息
        ///// <summary>
        ///// 关闭按钮
        ///// </summary>
        //public Button closeBtn;

        public Text TxtNumber;//编号
        //T1
        public Text TxtJobLeader;//工作负责人
        public Text TxtMember;//班组成员
        public Text TxtWorkPlace;//工作地点
        public Text TxtWorkContent;//工作内容
        public Text TxtWorkCondition;//工作条件
        public Text TxtPlanTime;//计划时间
        //T2
        public Text TxtIssuer;//工作签发人
        public Text TxtLicensor;//工作许可人
        public Text TxtApprover;//工作审批人

        public GameObject measuresItemPrefab;//措施单项
        public VerticalLayoutGroup measuresGrid;//措施列表

        // Use this for initialization
        void Start()
        {
            Instance = this;
            //closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show(WorkTicket infoT)
        {
            info = infoT;
            UpdateData();
            CreateMeasuresItems();
            SetWindowActive(true);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void UpdateData()
        {
            TxtNumber.text = info.No;
            TxtJobLeader.text = info.PersonInCharge;
            TxtMember.text = "";
            TxtWorkPlace.text = info.WorkPlace;
            TxtWorkContent.text = info.JobContent;
            TxtWorkCondition.text = info.WorkCondition;
            TxtPlanTime.text = info.StartTimeOfPlannedWork.ToString("yyyy/MM/dd HH:mm") + "-" + info.EndTimeOfPlannedWork.ToString("yyyy/MM/dd HH:mm");
            TxtIssuer.text = info.Lssuer;
            TxtLicensor.text = info.Licensor;
            TxtApprover.text = info.Approver;
        }

        /// <summary>
        /// 创建措施列表
        /// </summary>
        public void CreateMeasuresItems()
        {
            ClearMeasuresItems();
            if (info.SafetyMeasuress == null || info.SafetyMeasuress.Length == 0) return;
            foreach (SafetyMeasures sm in info.SafetyMeasuress)
            {
                GameObject itemT = CreateMeasuresItem();
                Text[] ts = itemT.GetComponentsInChildren<Text>();
                if (ts.Length > 0)
                {
                    ts[0].text = sm.LssuerContent;
                }
                if (ts.Length > 1)
                {
                    ts[1].text = sm.LicensorContent;
                }
            }
        }

        /// <summary>
        /// 创建措施项
        /// </summary>
        public GameObject CreateMeasuresItem()
        {
            GameObject itemT = Instantiate(measuresItemPrefab);
            itemT.transform.SetParent(measuresGrid.transform);
            itemT.transform.localPosition = Vector3.zero;
            itemT.transform.localScale = Vector3.one;
            LayoutElement layoutElement = itemT.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = false;
            itemT.SetActive(true);
            return itemT;
        }

        /// <summary>
        /// 清空措施列表
        /// </summary>
        public void ClearMeasuresItems()
        {
            int childCount = measuresGrid.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(measuresGrid.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 是否显示传统
        /// </summary>
        public void SetWindowActive(bool isActive)
        {
            window.SetActive(isActive);
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void CloseBtn_OnClick()
        {
            SetWindowActive(false);
        }
    }
}
