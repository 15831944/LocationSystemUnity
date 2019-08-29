using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoricalPathStatisticsManage : MonoBehaviour
{
    public static HistoricalPathStatisticsManage Instance;
    public Toggle TimeBenchmarkTog;
    public Toggle PersonnelBenchmarkTog;
    public Toggle AreaBenchmarkTog;
    public Button CloseBut;
    public GameObject HistoricalPathStatisticsWindow;
    List<PositionList> BenchmarkmanageList;

    void Start()
    {
        TimeBenchmarkTog.onValueChanged.AddListener(TimeBenchmarkTog_Click);
        CloseBut.onClick.AddListener(CloseHistoricalPathStatisticsWindow);
        PersonnelBenchmarkTog.onValueChanged.AddListener(PersonnelBenchmarkTog_Click);
        AreaBenchmarkTog.onValueChanged.AddListener(AreaBenchmarkTog_Click);
    }
    private void Awake()
    {
        Instance = this;
    }

    private int mode = 0;

    public void TimeBenchmarkTog_Click(bool b)
    {
        mode = 1;
        TimeBenchmarkManage.Instance.ShowTimeBenchmarkUI(b);
        if (b)
        {
            //BenchmarkmanageList = CommunicationObject.Instance.GetHistoryPositonStatistics(1, "", "");
            //TimeBenchmarkManage.Instance.GetTimeBenchmarkList(BenchmarkmanageList);
            //TimeBenchmarkManage.Instance.ShowLineChartInfo(BenchmarkmanageList);

            CommunicationObject.Instance.GetHistoryPositonStatistics(1, "", "", "",list =>
            {
                BenchmarkmanageList = list;
                TimeBenchmarkManage.Instance.GetTimeBenchmarkList(BenchmarkmanageList);
                TimeBenchmarkManage.Instance.ShowLineChartInfo(BenchmarkmanageList);
            });
        }

    }
    public void PersonnelBenchmarkTog_Click(bool b)
    {
        mode = 2;
        PersonnelBenchmarkManage.Instance.ShowPersonnelBenchmarkWindow(b);
        if (b)
        {
            CommunicationObject.Instance.GetHistoryPositonStatistics(2, "", "","", list =>
            {
                BenchmarkmanageList = list;
                PersonnelBenchmarkManage.Instance.PersonnelBenchmarkList(BenchmarkmanageList);
            });
        }
    }
    public void AreaBenchmarkTog_Click(bool b)
    {
        mode = 3;
        AreaBenchmarkManage.Instance.ShowAreaBenchmarkWindow(b);
        if (b)
        {
            CommunicationObject.Instance.GetHistoryPositonStatistics(3, "", "", "",list =>
            {
                BenchmarkmanageList = list;
                AreaBenchmarkManage.Instance.GetPieChartData(BenchmarkmanageList);
                BuildingBenchmark.Instance.BuildingBenchmarkList(BenchmarkmanageList);
            });
            
        }
    }
    public void ShowHistoricalPathStatisticsWindow()
    {
        HistoricalPathStatisticsWindow.SetActive(true);

        //处理关闭再打开没有数据显示的问题
        if (mode == 1)
        {
            TimeBenchmarkTog_Click(true);
        }
        else if (mode == 2)
        {
            PersonnelBenchmarkTog_Click(true);
        }
        else if (mode == 3)
        {
            AreaBenchmarkTog_Click(true);
        }
    }
    public void CloseHistoricalPathStatisticsWindow()
    {
        CloseAllWindow();
        HistoricalPathStatisticsWindow.SetActive(false);
    }
    public void CloseAllWindow()
    {
        TimeBenchmarkManage.Instance.ShowTimeBenchmarkUI(false);
        PersonnelBenchmarkManage.Instance.ShowPersonnelBenchmarkWindow(false);
        AreaBenchmarkManage.Instance.ShowAreaBenchmarkWindow(false);

    }
    public void OpenHistoricalPathStatisticsInfo()
    {
        if (TimeBenchmarkTog.isOn == false)
        {
            TimeBenchmarkTog.isOn = true;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 显示当天的所有定位点（测试用）
    /// </summary>
    [ContextMenu("ShowAllLocationPointOfDay")]
    public void ShowAllLocationPointOfDay()
    {
        var item = TimeBenchmarkManage.Instance.GetSelectedItem();
        if (item != null)
        {
            CommunicationObject.Instance.GetHistoryPositonData(1, item.Info.Name, "", "", list =>
                {
                    Debug.Log("Count:"+ list.Count);
                    ShowPosList(list);

                });
        }
        else
        {
            Debug.LogError("item == null");
        }
    }

    private void ShowPosList(List<Pos>  posList)
    {
        //DateTime start = DateTime.Now;
        //List<Pos> result = null;

        //InitPosPointsParent();//删除并重新创建PosParent

        //foreach (Pos pos in posList)
        //{
        //    //CreatePoint(pos);

        //    Vector3 v1 = new Vector3(pos.X, pos.Y, pos.Z);
        //    Vector3 v2 = LocationManager.GetRealVector(v1);
        //    var p1=CreatePoint(v2,Color.blue, posPointParent.transform);

        //    Vector3 v3 = NavMeshHelper.GetClosetPoint(v2);//这部分可能会比较花时间
        //    var p2=CreatePoint(v3,Color.red,p1.transform);
        //    var dis = Vector3.Distance(v2, v3);
        //    p2.name = "d:" + dis;
        //}

        //posPointParent.HighlightOn();

        //Log.Info("ShowPosList:", (DateTime.Now - start).TotalMilliseconds + "ms");

        StartCoroutine(ShowPosListAsync(posList));
    }

    public float RedDistance = 5f;

    public float GreenDistanc = 1f;

    private GameObject GetPrefab(Color color, float transparent)
    {
        GameObject bluePrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bluePrefab.transform.localScale = posScale;
        bluePrefab.SetColor(color);
        bluePrefab.SetTransparent(transparent);
        bluePrefab.name = color.ToString();
        return bluePrefab;
    }

    private IEnumerator ShowPosListAsync(List<Pos> posList)
    {
        DateTime start = DateTime.Now;
        List<Pos> result = null;

        InitPosPointsParent(); //删除并重新创建PosParent

        GameObject bluePrefab = GetPrefab(Color.blue, 0.5f);
        GameObject redPrefab = GetPrefab(Color.red, 0.5f);
        GameObject greenPrefab = GetPrefab(Color.green, 0.5f);
        GameObject blackPrefab = GetPrefab(Color.black, 0.5f);

        for (int i = 0; i < posList.Count && i < LimitCount; i++)
        {
            Pos pos = posList[i];
            //CreatePoint(pos);

            Vector3 v1 = new Vector3(pos.X, pos.Y, pos.Z);//二维坐标
            Vector3 v2 = LocationManager.GetRealVector(v1);//三维坐标
            
            Vector3 v3 = NavMeshHelper.GetClosetPoint(v2); //NavMesh上的坐标，这部分可能会比较花时间
            //Vector3 v4 = new Vector3(v3.x, v2.y, v3.z);//高度相同
            var dis = Vector3.Distance(v2, v3);

            GameObject prefab1 = null;
            GameObject prefab2 = blackPrefab;

            if (dis > RedDistance)//大于一个距离 5
            {
                prefab1 = redPrefab;
            }
            else if(dis < GreenDistanc)//大于一个距离 1
            {
                prefab1 = greenPrefab;
            }
            else
            {
                prefab1 = bluePrefab;
            }
            var p1 = CreatePoint(prefab1, v2, posPointParent.transform);//三维坐标
            p1.name = string.Format("{0}|{1}", i, v2);
            var p2 = CreatePoint(prefab2, v3, p1.transform);//NavMesh上的坐标
            p2.name = "d:" + dis;
            //Gizmos.DrawLine(v2, v3);
            Debug.Log(string.Format("{0}/{1}", i, posList.Count));
            yield return null;
        }

        GameObject.DestroyImmediate(bluePrefab);
        GameObject.DestroyImmediate(redPrefab);
        GameObject.DestroyImmediate(greenPrefab);
        GameObject.DestroyImmediate(blackPrefab);

        posPointParent.HighlightOn();

        Log.Info("ShowPosList:", (DateTime.Now - start).TotalMilliseconds + "ms");
    }

    public int LimitCount = 1000;

    private void InitPosPointsParent()
    {
        if (posPointParent != null)
        {
            GameObject.DestroyImmediate(posPointParent);
            posPointParent = null;
        }

        if (posPointParent == null)
        {
            posPointParent = new GameObject();
            posPointParent.name = "PosParent";
        }
    }

    private GameObject posPointParent;

    //public GameObject posPrefab;

    public Vector3 posScale=new Vector3(1,1,1);

    private GameObject CreatePoint(GameObject prefab, Vector3 v2, Transform parent)
    {
        GameObject p = null;
        if (prefab == null)
        {
            //posPrefab=
            GameObject newPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newPrefab.transform.localScale = posScale;
            //newPrefab.SetColor(color);
            newPrefab.SetTransparent(0.5f);
            prefab = newPrefab;
        }

        p = GameObject.Instantiate(prefab);
        p.transform.position = v2;
        p.transform.parent = parent;
        p.name = v2.ToString();
        return p;
    }

    /// <summary>
    /// 显示当天的所有定位点（测试用）
    /// </summary>
    [ContextMenu("ShowAllLocationPointOfPersonInDay")]
    public void ShowAllLocationPointOfPersonInDay()
    {
        var item1 = TimeBenchmarkManage.Instance.GetSelectedItem();
        if (item1 != null)
        {
            var item2 = PersonnelTimeBenchmark.Instance.GetSelectedItem();
            if (item2 != null)
            {
                CommunicationObject.Instance.GetHistoryPositonData(1, item1.Info.Name, item2.Info.Name, "", list =>
                {
                    Debug.Log("Count:" + list.Count);
                    ShowPosList(list);
                });
            }
            else
            {
                Debug.LogError("item2 == null");
            }
        }
        else
        {
            Debug.LogError("item1 == null");
        }
    }
}
