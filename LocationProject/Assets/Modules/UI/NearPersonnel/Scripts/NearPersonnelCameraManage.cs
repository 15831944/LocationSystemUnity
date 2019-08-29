using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NearPersonnelCameraManage : MonoBehaviour
{
    public static NearPersonnelCameraManage Instance;
    [System.NonSerialized] public List<NearbyDev> NearPerCamList;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    public GameObject NearPersonnelCameraWindow;
    public GameObject CameraGrid;
    public GameObject CameraPrefab;
    public Button closeWindow;

    public Image PersonnelRotation;

    public Text Personnel;//监控人员
    public Text CurrentArea;//当前所在区域
    public Text CamNum;


    public Scrollbar vertical;


    public bool isRefresh = false;
    int Id;
    float Distance;
    int NFlag;
    string AreaName;
    string PerName;
    LocationObject LocationObj;
    int CurrentId;
    void Start()
    {
        Instance = this;
        NearPerCamList = new List<NearbyDev>();
        closeWindow.onClick.AddListener(CloseNearPersonnelCameraWindow);

    }


    public void StartRefershNearPersonnelCameraData()
    {
       
        Debug.LogError("进入");
        if (!IsInvoking("RefershNearPersonnelCameraData"))
        {
            Debug.LogError("开始");
           
            InvokeRepeating("RefershNearPersonnelCameraData", 0, CommunicationObject.Instance.RefreshSetting.NearCamera);

        }
       
    }
    public void RefershNearPersonnelCameraData()
    {
        ReshDataAndSelectItem();
        Instance.SaveSelection();
        if (isRefresh) return;
        isRefresh = true;
        var nearPersonnelData = CommunicationObject.Instance.GetNearbyDev_Currency(Id , Distance , NFlag );
       
        if (string .IsNullOrEmpty (AreaName))
        {
            CurrentArea.text = "厂区内";
        }
        else
        {
            CurrentArea.text = AreaName.ToString();
        }
        if (!string.IsNullOrEmpty(PerName))
        {
            Personnel.text = PerName.ToString();
        }
   
        if (nearPersonnelData == null)
        {
            CamNum.text = "0";
            isRefresh = false;
           
            return;
        }
        if (nearPersonnelData != null)
        {
            NearPerCamList = new List<NearbyDev>(nearPersonnelData);
        }

        SetNearPersonnelCamData(NearPerCamList);
        if (NearPerCamList == null)
        {
            CamNum.text = "";
        }
        else
        {
            CamNum.text = NearPerCamList.Count.ToString();
        }

        isRefresh = false;
        ReshCompleteAndSelectItem();
        Vector3 PerRotion = new Vector3(LocationObj.transform.eulerAngles.x, LocationObj.transform.eulerAngles.z, LocationObj.transform.eulerAngles.y);
        PersonnelRotation.GetComponent<RectTransform>().localEulerAngles = PerRotion;

      
        CurrentId = Id;
        Debug.LogError("caixulu");
    }
    public void CloseRefershNearPersonnelCameraData()
    {
        if (IsInvoking("RefershNearPersonnelCameraData"))
        {
            CancelInvoke("RefershNearPersonnelCameraData");
        }
    }
    public void GetNearPerCamData(int id, float distance, int nFlag, string areaName, LocationObject locationObj,string perName)
    {
         Id =id ;
         Distance=distance ;
         NFlag=nFlag ;
        AreaName = areaName;
        PerName = perName;
        LocationObj =locationObj ;
        StartRefershNearPersonnelCameraData();


    }
    string RecordId;
    public void ReshDataAndSelectItem()
    {
        if (grid.transform.childCount == 0) return;
        for (int i =0;i < grid.transform .childCount;i++)
        {
            Toggle selectTog = grid.transform.GetChild(i).GetComponent<Toggle>();
            if (selectTog.isOn ==true)
            {
                RecordId = grid.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Text>().text;
            }
        }
    }
    public void ReshCompleteAndSelectItem()
    {
        if (grid.transform.childCount == 0) return;
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Toggle selectTog = grid.transform.GetChild(i).GetComponent<Toggle>();
             string   CurrentRecordId= grid.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Text>().text;
             string CurrentName = grid.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text;
            if (RecordId == CurrentRecordId)
            {
                selectTog.isOn = true;
               for (int m=0;m < CameraGrid.transform .childCount;m++)
                {
                    string SelectName= grid.transform.GetChild (i ).GetChild(1).GetChild (0).GetComponent<Text>().text;
                    Toggle selectNameTog = CameraGrid.transform.GetChild(i).GetChild(1).GetComponent<Toggle>();
                    if (CurrentName== SelectName)
                    {
                        selectNameTog.isOn = true;
                    }
                }
            } 
           
        }
    }
    public void SetNearPersonnelCamData(List<NearbyDev> devList)
    {
        for (int i = 0; i < devList.Count; i++)
        {

            GameObject Obj = InstantiateLine();
            NearPersonnelCameraInfo item = Obj.GetComponent<NearPersonnelCameraInfo>();
            item.showNearPersonnelCamInfo(devList[i], devList.Count, i);


            GameObject camObj = CreateCameraPrefabs();
            NearPerCameraRotation camItem = camObj.GetComponent<NearPerCameraRotation>();
            camItem.GetNearPersonnelCamInfo(devList[i], devList.Count, i);

            item.nearPerCameraRotation = camItem;
            camItem.nearPersonnelCameraInfo = item;
        }

    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject Obj = Instantiate(TemplateInformation);
        Obj.SetActive(true);
        Obj.transform.parent = grid.transform;
        Obj.transform.localScale = Vector3.one;
        return Obj;
    }
    /// <summary>
    /// 生成摄像头的预设
    /// </summary>
    /// <returns></returns>
    public GameObject CreateCameraPrefabs()
    {
        GameObject camObj = Instantiate(CameraPrefab);
        camObj.SetActive(true);
        camObj.transform.localScale = Vector3.one;
        camObj.transform.localPosition = new Vector3(camObj.transform.localPosition.x, camObj.transform.localPosition.y, 0);
        camObj.transform.parent = CameraGrid.transform;
        return camObj;
    }

    void Update()
    {

    }
    public void ShowNearPersonnelCameraWindow()
    {
        NearPersonnelCameraWindow.SetActive(true);
    }
    public void CloseNearPersonnelCameraWindow()
    {
        CloseRefershNearPersonnelCameraData();
        NearPersonnelCameraWindow.SetActive(false);
        SaveSelection();
        RecordId = "";
    }
    /// <summary>
    /// 删除生成的预设
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
        for (int j = CameraGrid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(CameraGrid.transform.GetChild(j).gameObject);
        }
    }
}
