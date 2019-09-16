
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmTypeStatistics : MonoBehaviour
{
    public static DevAlarmTypeStatistics Instance;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;

    void Start()
    {

    }
    private void Awake()
    {
        Instance = this;
    }
    public void SetDeviceAlarmTypeInfo(List<AlarmGroupCount> devAlarmList)
    {
       
        for (int i = 0; i < devAlarmList.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            DevAlarmTypeStatisticsDetailInfo item = Obj.GetComponent<DevAlarmTypeStatisticsDetailInfo>();
            item.GetDevAlarmTypeStatisticsDetailData(devAlarmList[i], devAlarmList[0].Count);
        }
    }


    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
       // Debug.LogErrorFormat("ParentName:{0} Name:{1}",o.transform.parent.name,transform.name);
        return o;
    }

    public void DelectItem()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    void Update()
    {

    }
}
