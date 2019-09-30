using Assets.M_Plugins.Helpers.Utils;
using RTEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceSelections : MonoBehaviour {

    public GameObject Window;

    public Text NormalDevText; //通用设备数量
    public Text CameraDevText;//摄像头数量
    public Text DoorAccessText;//门禁设备数量
    public Text BorderDevText;//边界设备
    public Text FireAlarmText;//消防设备

    public GameObject Selections;//筛选部分

    private List<GameObject> normalDevTemp = new List<GameObject>();//设备缓存
    private List<GameObject> cameraDevTemp = new List<GameObject>();//设备缓存
    private List<GameObject> doorDevTemp = new List<GameObject>();//设备缓存
    private List<GameObject> borderDevTemp = new List<GameObject>();//设备缓存
    private List<GameObject> fireDevTemp = new List<GameObject>();//设备缓存
    /// <summary>
    /// 设备分类
    /// </summary>
    public enum DevType
    {
        noramDev,
        cameraDev,
        doorDev,
        borderDev,
        fireDev
    }
	// Use this for initialization
	void Start () {
        InitButtonMethod();
    }
	/// <summary>
    /// 设置设备信息
    /// </summary>
    /// <param name="devList"></param>
    public void SetDevInfo(List<DevNode>devList)
    {
        Window.SetActive(true);
        ClearDevList();
        int normalDev=0;
        int cameraDev = 0;
        int doorDev = 0;
        int borderDev = 0;
        int fireDev = 0;
        foreach(DevNode dev in devList)
        {
            if(dev!=null&&dev.Info!=null)
            {
                string typeCode = dev.Info.TypeCode.ToString();
                if (TypeCodeHelper.IsCamera(typeCode))
                {
                    cameraDev++;
                    cameraDevTemp.Add(dev.gameObject);
                }
                else if (TypeCodeHelper.IsDoorAccess(typeCode))
                {
                    doorDev++;
                    doorDevTemp.Add(dev.gameObject);
                }
                else if (TypeCodeHelper.IsBorderAlarmDev(typeCode))
                {
                    borderDev++;
                    borderDevTemp.Add(dev.gameObject);
                }
                else if (TypeCodeHelper.IsAlarmDev(typeCode))
                {
                    fireDev++;
                    fireDevTemp.Add(dev.gameObject);
                }
                else
                {
                    normalDev++;
                    normalDevTemp.Add(dev.gameObject);
                }
            }
        }
        SetSelections(normalDev,cameraDev,doorDev,borderDev,fireDev);
        NormalDevText.text = normalDev.ToString();
        CameraDevText.text = cameraDev.ToString();
        DoorAccessText.text = doorDev.ToString();
        BorderDevText.text = borderDev.ToString();
        FireAlarmText.text = fireDev.ToString();
    }
    /// <summary>
    /// 清除设备缓存
    /// </summary>
    private void ClearDevList()
    {
        normalDevTemp.Clear();
        cameraDevTemp.Clear();
        doorDevTemp.Clear();
        borderDevTemp.Clear();
        fireDevTemp.Clear();
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        Window.SetActive(false);
    }
    /// <summary>
    /// 点击移除分类
    /// </summary>
    /// <param name="i"></param>
    public void OnButtonRemove(DevType devType)
    {
        EditorObjectSelection.Instance.ClearSelection(false);
        List<GameObject> objList = new List<GameObject>();
        DeviceEditUIManager manager = DeviceEditUIManager.Instacne;
        if(devType==DevType.noramDev)
        {
            normalDevTemp.Clear();
        }else if(devType==DevType.cameraDev)
        {
            cameraDevTemp.Clear();
        }else if(devType==DevType.doorDev)
        {
            doorDevTemp.Clear();
        }else if(devType == DevType.fireDev)
        {
            fireDevTemp.Clear();
        }else
        {
            borderDevTemp.Clear();
        }
        objList.AddRange(normalDevTemp);
        objList.AddRange(cameraDevTemp);
        objList.AddRange(doorDevTemp);
        objList.AddRange(borderDevTemp);
        objList.AddRange(fireDevTemp);
        EditorObjectSelection.Instance.SetSelectedObjects(objList, false);
    }
    /// <summary>
    /// 设置选项
    /// </summary>
    /// <param name="normalDev"></param>
    /// <param name="cameraDev"></param>
    /// <param name="doorDev"></param>
    /// <param name="borderDev"></param>
    /// <param name="fireDev"></param>
    private void SetSelections(int normalDev,int cameraDev,int doorDev,int borderDev,int fireDev)
    {
        SetButtonState(normalDev, Selections.transform.GetChild(0));
        SetButtonState(cameraDev, Selections.transform.GetChild(1));
        SetButtonState(doorDev, Selections.transform.GetChild(2));
        SetButtonState(borderDev, Selections.transform.GetChild(3));
        SetButtonState(fireDev, Selections.transform.GetChild(4));
    }
    /// <summary>
    /// 设置按钮开关状态
    /// </summary>
    /// <param name="num"></param>
    /// <param name="buttonObj"></param>
    private void SetButtonState(int num,Transform buttonObj)
    {
        bool isOn = num == 0 ? false : true;
        buttonObj.gameObject.SetActive(isOn);
    }
    /// <summary>
    /// 初始化按钮
    /// </summary>
    private void InitButtonMethod()
    {
        InitButton(DevType.noramDev, Selections.transform.GetChild(0));
        InitButton(DevType.cameraDev, Selections.transform.GetChild(1));
        InitButton(DevType.doorDev, Selections.transform.GetChild(2));
        InitButton(DevType.borderDev, Selections.transform.GetChild(3));
        InitButton(DevType.fireDev, Selections.transform.GetChild(4));
    }
    /// <summary>
    /// 初始化按钮
    /// </summary>
    /// <param name="type"></param>
    /// <param name="child"></param>
    private void InitButton(DevType type,Transform child)
    {
        Button button = child.GetComponentInChildren<Button>();
        if(button)
        {
            button.onClick.AddListener(()=> 
            {
                OnButtonRemove(type);
            });
        }
    }
}
