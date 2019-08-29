using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionInfoFollow : MonoBehaviour
{
    public static MobileInspectionInfoFollow Instance;
    public List<PatrolPoint> PatrolPointList;
    public List<InspectionTrack> InspectionTrackRote;
    public GameObject PatrolPointFollowPrafeb;


    void Start()
    {
        Instance = this;

    }
    public void DateUpdate(InspectionTrack rote)
    {
        PatrolPointList.AddRange(rote.Route);
      	//CreatRouteFollowUI();

    }

    public void CreatRouteFollowUI()
    {
        for (int i = 0; i < PatrolPointList.Count; i++)
        {
            int Num = i + 1;
            ShowMobileInspectionPath(PatrolPointList[i], Num);
        }
    }

    public void ShowMobileInspectionPath(PatrolPoint date, int num)
    {
        RoomFactory.Instance.GetDevByid((int)date.DevId,
            (devNodeT) =>
            {
                if (devNodeT == null)
                {
                    return;
                }
                GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(devNodeT.gameObject, Vector3.zero);
                GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(PatrolPointFollowPrafeb.gameObject, targetTagObj1, "MobileInspectionUI", null, false, false);
                MobileInspectionFollowUI follow1 = uiObj1.GetComponent<MobileInspectionFollowUI>();
                try
                {
                    follow1.MobileInspectionPathFollowUI(num.ToString(), date);
                }
                catch
                {
                    int j = 0;
                }
            });

    }
    public void Hide()
    {
        UGUIFollowManage.Instance.RemoveGroupUIbyName("MobileInspectionUI");
        if (PatrolPointList.Count != 0)
        {
            PatrolPointList.Clear();
        }
       
    }
    /// <summary>
    /// 清除列表项
    /// </summary>
    public void ClearItems()
    {


    }


}
