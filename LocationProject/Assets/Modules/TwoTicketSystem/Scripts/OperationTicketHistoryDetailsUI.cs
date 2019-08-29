using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class OperationTicketHistoryDetailsUI : MonoBehaviour {

        /// <summary>
        /// 窗体
        /// </summary>
        public GameObject window;

        public OperationTicketHistory info;//工作票信息

        public Text TxtNumber;//编号
        //T1
        public Text TxtOperationTask;//操作任务
        public Text TxtPlanTimeStart;//计划时间
        public Text TxtPlanTimeEnd;//计划时间
        //T2
        public Text TxtGuardian;//监护人
        public Text TxtOperator;//操作人
        public Text TxtDutyOfficer;//值班负责人
        public Text TxtDispatchingOfficer;//调度负责人

        public GameObject operationItemPrefab;//措施单项
        public VerticalLayoutGroup operationGrid;//措施列表

        public Button closeBtn;//关闭

        // Use this for initialization
        void Start()
        {
            closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show(OperationTicketHistory infoT)
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
            TxtOperationTask.text = info.OperationTask;
            TxtPlanTimeStart.text = info.OperationStartTime.ToString("yyyy/MM/dd HH:mm");
            TxtPlanTimeEnd.text = info.OperationEndTime.ToString("yyyy/MM/dd HH:mm");
            TxtGuardian.text = info.Guardian;
            TxtOperator.text = info.Operator;
            TxtDutyOfficer.text = info.DutyOfficer;
            TxtDispatchingOfficer.text = info.Dispatch;
        }

        /// <summary>
        /// 创建措施列表
        /// </summary>
        public void CreateMeasuresItems()
        {
            ClearMeasuresItems();
            if (info.OperationItems == null || info.OperationItems.Length == 0) return;
            foreach (OperationItemHistory sm in info.OperationItems)
            {
                GameObject itemT = CreateMeasuresItem();
                Text[] ts = itemT.GetComponentsInChildren<Text>();
                if (ts.Length > 0)
                {
                    ts[0].text = sm.OrderNum.ToString();
                }
                if (ts.Length > 1)
                {
                    ts[1].text = sm.Item;
                }
            }
        }

        /// <summary>
        /// 创建措施项
        /// </summary>
        public GameObject CreateMeasuresItem()
        {
            GameObject itemT = Instantiate(operationItemPrefab);
            itemT.transform.SetParent(operationGrid.transform);
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
            int childCount = operationGrid.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(operationGrid.transform.GetChild(i).gameObject);
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
            TwoTicketHistoryUI_N.Instance.SetContentActive(true);
        }
    }
}
