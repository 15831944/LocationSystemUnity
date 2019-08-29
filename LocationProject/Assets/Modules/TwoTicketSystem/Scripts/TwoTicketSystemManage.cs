
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwoTicketSystemManage : MonoBehaviour
{
    public static TwoTicketSystemManage Instance;

    public Transform item1;
    public Transform item2;
    public Transform item3;

    public Transform item11;
    public Transform item22;
    public Transform item33;

    [System.NonSerialized] public List<Personnel> personnels;//人员信息列表

    public TwoTicketFollowUI followUI;//移动巡检飘浮UI

    // Use this for initialization
    void Start()
    {
        Instance = this;
        GetPersonnels();
        workTicketHistoryPaths = new List<WorkTicketHistoryPath>();
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

    #region 工作票相关

    public List<WorkTicketHistoryPath> workTicketHistoryPaths;

    /// <summary>
    /// 显示工作票路线
    /// </summary>
    public void ShowWorkTicketPath(WorkTicket workTicketT)
    {
        Hide();
        DepNode depNodeT = RoomFactory.Instance.GetDepNodeById(workTicketT.AreaId);
        if (depNodeT == null) return;
        GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(depNodeT.NodeObject.gameObject, Vector3.zero);
        GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj1, "TwoTicketFollowUI", null, false, false);
        TwoTicketFollowUI follow1 = uiObj1.GetComponent<TwoTicketFollowUI>();
        follow1.Init(true, "1", depNodeT.NodeName);
        //CreateWorkTicketHistoryPath(workTicketT);
    }

    /// <summary>
    /// 创建工作票历史路径
    /// </summary>
    public void CreateWorkTicketHistoryPath(WorkTicket workTicketT)
    {
        GetPersonnels();
        Personnel personnel = personnels.Find((item) => item.Id == workTicketT.PersonInChargePersonelId);
        if (personnel != null)
        {
            DateTime start = new DateTime(2018, 9, 30, 10, 0, 0);
            DateTime end = new DateTime(2018, 9, 30, 11, 0, 0);
            List<int> topoNodeIds = RoomFactory.Instance.GetCurrentDepNodeChildNodeIds(SceneEvents.DepNode);
            //List<Position> ps = new List<Position>();
            //List<Vector3> list = new List<Vector3>();
            //List<DateTime> timelist = new List<DateTime>();

            Loom.StartSingleThread(() =>
            {
                var ps = GetHistoryData(personnel.Id, topoNodeIds, start, end);
                var posInfoList = new PositionInfoList();
                Loom.DispatchToMainThread(() =>
                {
                    Debug.LogError("点数：" + ps.Count);
                    if (ps.Count < 2) return;

                    for (int i = 0; i < ps.Count; i++)
                    {
                        var posInfo = new PositionInfo(ps[i], start);
                        posInfoList.Add(posInfo);
                    }

                    PathInfo pathInfo = new PathInfo();
                    pathInfo.personnelT = personnel;
                    pathInfo.color = Color.green;
                    pathInfo.posList = posInfoList;

                    GameObject o = new GameObject(workTicketT.PersonInCharge + "工作票");
                    WorkTicketHistoryPath path = o.AddComponent<WorkTicketHistoryPath>();
                    path.Init(pathInfo, false);
                    workTicketHistoryPaths.Add(path);
                });
            });
        }
    }

    /// <summary>
    /// 获取历史轨迹数据
    /// </summary>
    /// <param name="ps"></param>
    /// <param name="startT"></param>
    /// <param name="end"></param>
    /// <param name="intervalMinute">每次获取数据的时间长度，不超过改数值</param>
    public List<Position> GetHistoryData(int personnelID, List<int> topoNodeIdsT, DateTime startT, DateTime endT, float intervalMinute = 10f)
    {
        List<Position> ps = new List<Position>();
        double minutes = (endT - startT).TotalMinutes;
        float counts = (float)minutes / intervalMinute;
        float valueT = 0;
        float sum = 0;
        while (sum < counts)
        {
            DateTime startTemp;
            DateTime endTemp;
            if (sum + 1 <= counts)
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = startT.AddMinutes(intervalMinute * (sum + 1));
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPersonnelID(personnelID, startTemp, endTemp);
                List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT, startTemp, endTemp);
                ps.AddRange(listT);
            }
            else
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = endT;
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPersonnelID(personnelID, startTemp, endTemp);
                List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT, startTemp, endTemp);
                ps.AddRange(listT);
            }

            sum += 1;
            valueT = sum / counts;
            print("valueT:" + valueT);
            Loom.DispatchToMainThread(() =>
            {
                ProgressbarLoad.Instance.Show(valueT);
            });
        }
        Loom.DispatchToMainThread(() =>
        {
            ProgressbarLoad.Instance.Hide();
        });
        return ps;
    }

    /// <summary>
    /// 清除工作票历史轨迹
    /// </summary>
    [ContextMenu("ClearWorkTicketHistoryPaths")]
    public void ClearWorkTicketHistoryPaths()
    {
        foreach (WorkTicketHistoryPath path in workTicketHistoryPaths)
        {
            DestroyImmediate(path.transform.parent.gameObject);//
        }
        workTicketHistoryPaths.Clear();
        
    }

    #endregion

    #region 操作票相关


    public List<OperationTicketHistoryPath> operationTicketHistoryPaths;

    /// <summary>
    /// 显示工作票路线
    /// </summary>
    public void ShowOperationTicketPath(OperationTicket operationTicketT)
    {
        Hide();
        if (operationTicketT.OperationItems == null) return;
        for (int i = 0; i < operationTicketT.OperationItems.Length; i++)
        {
            OperationItem item = operationTicketT.OperationItems[i];
            //DevNode devNodeT = RoomFactory.Instance.GetDevById(item.DevId);
            //if (devNodeT == null) continue;
            //GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(devNodeT.gameObject, Vector3.zero);
            //GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj1, "TwoTicketFollowUI", null, false, false);
            //MobileInspectionFollowUI follow1 = uiObj1.GetComponent<MobileInspectionFollowUI>();
            //follow1.Init(true, i.ToString(), devNodeT.name);

            RoomFactory.Instance.GetDevById(item.DevId,
                (devNodeT)=>
                {
                    if (devNodeT == null)
                    {

                        return;
                    }
                    GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(devNodeT.gameObject, Vector3.zero);
                    GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj1, "TwoTicketFollowUI", null, false, false);
                    //MobileInspectionFollowUI follow1 = uiObj1.GetComponent<MobileInspectionFollowUI>();
                    TwoTicketFollowUI follow1 = uiObj1.GetComponent<TwoTicketFollowUI>();
                    List<OperationItem> listT = operationTicketT.OperationItems.ToList();
                    OperationItem operationItemT = listT.Find((itemt) => itemt.DevId == devNodeT.Info.DevID);
                    //index = index + 1;
                    //if (index > 0)
                    //{
                    //    follow1.Init(true, index.ToString(), devNodeT.name);
                    //}
                    follow1.Init(true, operationItemT.OrderNum.ToString(), devNodeT.name);
                });

        }

        //CreateOperationTicketHistoryPath(operationTicketT);

    }

    /// <summary>
    /// 创建操作票历史路径
    /// </summary>
    public void CreateOperationTicketHistoryPath(OperationTicket operationTicketT)
    {
        GetPersonnels();
        Personnel personnel = personnels.Find((item) => item.Id == operationTicketT.OperatorPersonelId);
        if (personnel != null)
        {


            DateTime start = new DateTime(2018, 9, 30, 10, 0, 0);
            DateTime end = new DateTime(2018, 9, 30, 11, 0, 0);
            List<int> topoNodeIds = RoomFactory.Instance.GetCurrentDepNodeChildNodeIds(SceneEvents.DepNode);
            //List<Position> ps = new List<Position>();
            //List<Vector3> list = new List<Vector3>();
            //List<DateTime> timelist = new List<DateTime>();

            Loom.StartSingleThread(() =>
            {
                var ps = GetHistoryData(personnel.Id, topoNodeIds, start, end);
                var posInfoList = new PositionInfoList();
                Loom.DispatchToMainThread(() =>
                {
                    Debug.LogError("点数：" + ps.Count);
                    if (ps.Count < 2) return;

                    for (int i = 0; i < ps.Count; i++)
                    {
                        var posInfo = new PositionInfo(ps[i], start);
                        posInfoList.Add(posInfo);
                    }

                    PathInfo pathInfo = new PathInfo();
                    pathInfo.personnelT = personnel;
                    pathInfo.color = Color.green;
                    pathInfo.posList = posInfoList;
                    //pathInfo.timeLength = timeLength;

                    GameObject o = new GameObject(operationTicketT.Operator + "工作票");
                    OperationTicketHistoryPath path = o.AddComponent<OperationTicketHistoryPath>();
                    path.Init(pathInfo, false);
                    operationTicketHistoryPaths.Add(path);
                });
            });
        }
    }

    /// <summary>
    /// 清除操作票历史轨迹
    /// </summary>
    [ContextMenu("ClearOperationTicketHistoryPaths")]
    public void ClearOperationTicketHistoryPaths()
    {
        foreach (OperationTicketHistoryPath path in operationTicketHistoryPaths)
        {
            DestroyImmediate(path.transform.parent.gameObject);//
        }
        operationTicketHistoryPaths.Clear();

    }

    #endregion

    /// <summary>
    /// 清除两票历史路径
    /// </summary>
    public void ClearTwoTicketHistoryPaths()
    {
        ClearWorkTicketHistoryPaths();
        ClearOperationTicketHistoryPaths();
    }






    public void ShowDemo1()
    {
        Hide();
        GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(item1.gameObject, Vector3.zero);
        GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj1, "TwoTicketFollowUI", null, false, false);
        MobileInspectionFollowUI follow1 = uiObj1.GetComponent<MobileInspectionFollowUI>();
        follow1.Init(true, "1", "工作点1");

        GameObject targetTagObj2 = UGUIFollowTarget.CreateTitleTag(item2.gameObject, Vector3.zero);
        GameObject uiObj2 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj2, "TwoTicketFollowUI", null, false, false);
        MobileInspectionFollowUI follow2 = uiObj2.GetComponent<MobileInspectionFollowUI>();
        follow2.Init(true, "2", "工作点2");

        GameObject targetTagObj3 = UGUIFollowTarget.CreateTitleTag(item3.gameObject, Vector3.zero);
        GameObject uiObj3 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj3, "TwoTicketFollowUI", null, false, false);
        MobileInspectionFollowUI follow3 = uiObj3.GetComponent<MobileInspectionFollowUI>();
        follow3.Init(false, "3", "工作点3");
    }

    public void ShowDemo2()
    {
        Hide();
        GameObject targetTagObj1 = UGUIFollowTarget.CreateTitleTag(item11.gameObject, Vector3.zero);
        GameObject uiObj1 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj1, "TwoTicketFollowUI", null, false, false);
        MobileInspectionFollowUI follow1 = uiObj1.GetComponent<MobileInspectionFollowUI>();
        follow1.Init(true, "1", "操作点1");

        GameObject targetTagObj2 = UGUIFollowTarget.CreateTitleTag(item22.gameObject, Vector3.zero);
        GameObject uiObj2 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj2, "TwoTicketFollowUI", null, false, false);
        MobileInspectionFollowUI follow2 = uiObj2.GetComponent<MobileInspectionFollowUI>();
        follow2.Init(true, "2", "操作点2");

        GameObject targetTagObj3 = UGUIFollowTarget.CreateTitleTag(item33.gameObject, Vector3.zero);
        GameObject uiObj3 = UGUIFollowManage.Instance.CreateItem(followUI.gameObject, targetTagObj3, "TwoTicketFollowUI", null, false, false);
        MobileInspectionFollowUI follow3 = uiObj3.GetComponent<MobileInspectionFollowUI>();
        follow3.Init(true, "3", "操作点3");
    }


    public void Hide()
    {
        UGUIFollowManage.Instance.RemoveGroupUIbyName("TwoTicketFollowUI");
        ClearTwoTicketHistoryPaths();
    }
}
