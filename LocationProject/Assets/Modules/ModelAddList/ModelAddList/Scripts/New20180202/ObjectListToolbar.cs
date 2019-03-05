using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;

public class ObjectListToolbar : MonoBehaviour
{
    public static ObjectListToolbar Instance;
    /// <summary>
    /// 工具栏高亮背景图
    /// </summary>
    public GameObject HighlightImage;
    /// <summary>
    /// 动力设备Toggle
    /// </summary>
    public Toggle   powerDeviceToggle;
    /// <summary>
    /// 安防设备Toggle
    /// </summary>
    public Toggle securityDeviceToggle;
    /// <summary>
    /// IT设备Toggle
    /// </summary>
    public Toggle ITDeviceToggle;
    /// <summary>
    /// 其它设备Toggle
    /// </summary>
    public Toggle otherDeviceToggle;
    /// <summary>
    /// 机柜Toggle
    /// </summary>
    public Toggle cabinetToggle;
    /// <summary>
    /// 门窗Toggle
    /// </summary>
    public Toggle doorAndWindowToggle;
    /// <summary>
    /// 装饰Toggle
    /// </summary>
    public Toggle decorationToggle;
    /// <summary>
    /// 当前大类信息
    /// </summary>
    public ObjectAddList_Type currentType;

    // Use this for initialization
    void Start()
    {
        Instance = this;
     //   EventTriggerListener lis = EventTriggerListener.Get(HighlightImage);
      //  lis.onEnter = HighlightOn;
       // lis.onExit = HighlightOff;

        powerDeviceToggle.onValueChanged.AddListener(PowerDeviceToggle_ValueChanged);
        securityDeviceToggle.onValueChanged.AddListener(SecurityDeviceToggle_ValueChanged);
        ITDeviceToggle.onValueChanged.AddListener(ITDeviceToggle_ValueChanged);
        otherDeviceToggle.onValueChanged.AddListener(OtherDeviceToggle_ValueChanged);
        cabinetToggle.onValueChanged.AddListener(CabinetToggle_ValueChanged);
        doorAndWindowToggle.onValueChanged.AddListener(DoorAndWindowToggle_ValueChanged);
        decorationToggle.onValueChanged.AddListener(DecorationToggle_ValueChanged);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 拉近时添加列表的工具栏
    /// </summary>
    public void SetObjectToolbar(bool b)
    {
        cabinetToggle.transform.parent.gameObject.SetActive(b);
        doorAndWindowToggle.transform.parent.gameObject.SetActive(b);
        decorationToggle.transform.parent.gameObject.SetActive(b);
        if (!b)
        {
            cabinetToggle.isOn = b;
            doorAndWindowToggle.isOn = b;
            decorationToggle.isOn = b;
        }
    }


    /// <summary>
    /// 动力设备Toggle_ValueChanged
    /// </summary>
    public void PowerDeviceToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("动环设备");
            ObjectListController.Instance.Show(currentType);
           // powerDeviceToggle.GetComponentInChildren<Image>(true).color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
          //  powerDeviceToggle.GetComponent<Image>().color = new Color(255/255f,255/255f,255/255f,0/255f);
        }
    }
    /// <summary>
    /// 安防设备Toggle_ValueChanged
    /// </summary>
    public void SecurityDeviceToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("安防设备");
            ObjectListController.Instance.Show(currentType);
        }
    }
    /// <summary>
    /// IT设备Toggle_ValueChanged
    /// </summary>
    public void ITDeviceToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("IT设备");
            ObjectListController.Instance.Show(currentType);
        }
    }
    /// <summary>
    /// 其它设备Toggle_ValueChanged
    /// </summary>
    public void OtherDeviceToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("其他");
            ObjectListController.Instance.Show(currentType);
        }
    }
    /// <summary>
    /// 机柜设备Toggle_ValueChanged
    /// </summary>
    public void CabinetToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("机柜");
            ObjectListController.Instance.Show(currentType);
        }
    }
    /// <summary>
    /// 门窗Toggle_ValueChanged
    /// </summary>
    public void DoorAndWindowToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("门窗");
            ObjectListController.Instance.Show(currentType);
        }
    }
    /// <summary>
    /// 装饰Toggle_ValueChanged
    /// </summary>
    public void DecorationToggle_ValueChanged(bool b)
    {
        if (b)
        {
            currentType = ObjectAddListManage.Instance.GetObjectAddListTypeInfoByName("装饰");
            ObjectListController.Instance.Show(currentType);
        }
    }

    /// <summary>
    /// 高亮操作
    /// </summary>
    public void HighlightOn(GameObject o)
    {
        SetTextsActive(true);
    }

    /// <summary>
    /// 取消高亮操作
    /// </summary>
    public void HighlightOff(GameObject o)
    {
        SetTextsActive(false);
    }

    /// <summary>
    /// 控制工具栏文字的显示
    /// </summary>
    public void SetTextsActive(bool b)
    {
        Text[] txts = HighlightImage.GetComponentsInChildren<Text>(true);
        foreach (Text t in txts)
        {
            //高亮后显示文字
            t.transform.parent.gameObject.SetActive(b);
        }
    }

    public void Test111()
    {
        cabinetToggle.isOn = false;
    }
}
