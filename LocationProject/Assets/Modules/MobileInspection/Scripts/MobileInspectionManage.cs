using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobileInspectionManage : MonoBehaviour
{

    public static MobileInspectionManage Instance;

    public Transform item1;
    public Transform item2;
    public Transform item3;
    public Transform item4;
    public Transform item5;
    public Transform item6;
    public Transform item7;
    public Transform item8;
    public Transform item9;
    public Transform item10;
    public Transform item11;
    public Transform item12;
    public Transform item13;

    [System.NonSerialized]
    public List<Personnel> personnels;//人员信息列表
    public MobileInspectionFollowUI followUI;//移动巡检飘浮UI

    // Use this for initialization
    void Start()
    {
        Instance = this;
        Instance = this;
        GetPersonnels();
        //workTicketHistoryPaths = new List<WorkTicketHistoryPath>();
    }

    private void GetPersonnels()
    {
        if (personnels == null && PersonnelTreeManage.Instance)
        {
            personnels = PersonnelTreeManage.Instance.GetAllPersonnels();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDemo()
    {
        HideDemo();
       
        NewMethod(item1, true, "1", "巡检点1");
        NewMethod(item2, true, "2", "巡检点2");
        NewMethod(item3, true, "3", "巡检点3");
        NewMethod(item4, true, "4", "巡检点4");
        NewMethod(item5, true, "5", "巡检点5");
        NewMethod(item6, true, "6", "巡检点6");
        NewMethod(item7, false, "7", "巡检点7");
        NewMethod(item8, false, "8", "巡检点8");
        NewMethod(item9, false, "9", "巡检点9");
        NewMethod(item10, false, "10", "巡检点10");
        NewMethod(item11, false, "11", "巡检点11");
        NewMethod(item12, false, "12", "巡检点12");
        NewMethod(item13, false, "13", "巡检点13");
    }

    private void NewMethod(Transform tranT, bool isFinishedT, string numStr, string contentStr)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(tranT.gameObject, Vector3.zero);
        GameObject uiObj = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj, "MobileInspectionUI", null, false, false);
        MobileInspectionFollowUI follow3 = uiObj.GetComponent<MobileInspectionFollowUI>();
        follow3.Init(isFinishedT, numStr, contentStr);
    }

    public void HideDemo()
    {
        UGUIFollowManage.Instance.RemoveGroupUIbyName("MobileInspectionUI");
    }




    /// <summary>
    /// 显示工作票路线
    /// </summary>
    public void ShowMobileInspectionPath(PersonnelMobileInspection personnelMobileInspectionT)
    {
        Hide();
        if (personnelMobileInspectionT.list == null) return;
        for (int i = 0; i < personnelMobileInspectionT.list.Length; i++)
        {
            PersonnelMobileInspectionItem item = personnelMobileInspectionT.list[i];

            RoomFactory.Instance.GetDevByid(item.DevId,
                (devNodeT) =>
                {
                    if (devNodeT == null)
                    {

                        return;
                    }
                    GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(devNodeT.gameObject, Vector3.zero);
                    GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj1, "MobileInspectionUI", null, false, false);
                    MobileInspectionFollowUI follow1 = uiObj1.GetComponent<MobileInspectionFollowUI>();

                    List<PersonnelMobileInspectionItem> listT = personnelMobileInspectionT.list.ToList();
                    PersonnelMobileInspectionItem operationItemT = listT.Find((itemt) => itemt.DevId == devNodeT.Info.Id);
                    try
                    {
                        follow1.Init(true, operationItemT.nOrder.ToString(), devNodeT.name);
                    }
                    catch
                    {
                        int j = 0;
                    }
                });

        }


    }

 
    public void Hide()
    {
        UGUIFollowManage.Instance.RemoveGroupUIbyName("MobileInspectionUI");
        //ClearTwoTicketHistoryPaths();
    }
}
