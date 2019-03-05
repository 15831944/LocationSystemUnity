using Location.WCFServiceReferences.LocationServices;
using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ArchorLocation : MonoBehaviour {

    /// <summary>
    /// 当前基站信息
    /// </summary>
    private Archor currentArchor;
    /// <summary>
    /// 当前调整设备
    /// </summary>
    private DevNode currentDev;
    /// <summary>
    /// 标志设置是否开启
    /// </summary>
    private bool isSignOpen;
    /// <summary>
    /// 地板的Layer
    /// </summary>
    LayerMask mask;
    /// <summary>
    /// 地板layer是否获取
    /// </summary>
    private bool isLayerGet;

    [HideInInspector]
    public GameObject SignObject;
    /// <summary>
    /// 标志设置
    /// </summary>
    public Button SignSetButton;
    public GameObject SignPrefab;

    /// <summary>
    /// 基站名称
    /// </summary>
    public InputField DevName;
    /// <summary>
    /// 基站编码
    /// </summary>
    public InputField DevCode;
    /// <summary>
    /// 基站类型
    /// </summary>
    public Dropdown DevTypeDropdown;

    public InputField XPosInput;
    public InputField YPosInput;
    public InputField ZPosInput;

    public Text valueText;
    /// <summary>
    /// 计算位置按钮
    /// </summary>
    public Button ConfirmButton;
    /// <summary>
    /// 保存信息按钮
    /// </summary>
    public Button SaveInfoButton;
    /// <summary>
    /// 保存信息结果
    /// </summary>
    public Text SaveInfoResult;
	// Use this for initialization
	void Start () {

        SignSetButton.onClick.AddListener(() =>
        {
            OpenArchorSign(true);
        });
        ConfirmButton.onClick.AddListener(ConfirmPos);
        SaveInfoButton.onClick.AddListener(SaveInfo);
    }
	private void InitDropDown()
    {
       
    }
	// Update is called once per frame
	void Update () {
        SetSignPos();
    }
    /// <summary>
    /// 标志初始化
    /// </summary>
    private void InitSignObject()
    {
        if(SignObject==null)
        {
            SignObject = Instantiate(SignPrefab)as GameObject;
            SignObject.transform.localScale = Vector3.one;
            SignObject.transform.localEulerAngles = Vector3.zero;
        }
    }
    public void Open(DevNode dev)
    {
        currentDev = dev;
        ClearArchorValue(dev);
        currentArchor = null;
        InitSignObject();
        SignObject.SetActive(true);
    }
    public void Close()
    {
        OpenArchorSign(false);
        SignObject.SetActive(false);
        currentDev = null;
    }
    /// <summary>
    /// 清除输入框值
    /// </summary>
    /// <param name="dev"></param>
    private void ClearArchorValue(DevNode dev)
    {
        CommunicationObject service = CommunicationObject.Instance;
        Archor archor = service.GetArchorByDevId(dev.Info.Id);
        if(archor!=null&&!string.IsNullOrEmpty(archor.Code))
        {
            try
            {
                DevTypeDropdown.value = (int)archor.Type;
            }catch(Exception e)
            {
                DevTypeDropdown.value = 0;
            }
            DevCode.text = archor.Code.ToString();
            XPosInput.text = archor.X.ToString();
            YPosInput.text = archor.Y.ToString();
            ZPosInput.text = archor.Z.ToString();
        }
        else
        {           
            DevTypeDropdown.value = 0;
            DevCode.text = "";
            XPosInput.text = "";
            YPosInput.text = "";
            ZPosInput.text = "";
        }
        DevName.text = dev.Info.Name;
        valueText.text = "";
        SaveInfoResult.text = "";
    }
    private void GetFloorLayer()
    {
        if (isLayerGet) return;
        isLayerGet = true;
        mask = LayerMask.GetMask("Floor");
        Debug.LogError("FloorLayer:"+ mask.value);
    }
    /// <summary>
    /// 打开/关闭 标志设置
    /// </summary>
    /// <param name="isOpen"></param>
    private void OpenArchorSign(bool isOpen)
    {
        Debug.LogError("IsOpen:" + isOpen);
        isSignOpen = isOpen;
    }
    /// <summary>
    /// 设置标志位置
    /// </summary>
    private void SetSignPos()
    {
        if (!isSignOpen) return;
        if (IsClickUGUIorNGUI.Instance.isOverUI)
        {
            Debug.LogError("IsOverUI...");
            if(SignObject!=null)SignObject.SetActive(false);
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GetFloorLayer();
        if (Physics.Raycast(ray, out hit, float.MaxValue, mask.value))
        {
            //Debug.LogError("Ray success...");
            if (SignObject != null)
            {
                if(!SignObject.activeInHierarchy) SignObject.SetActive(true);
                SignObject.transform.position = hit.point;
            }
        }
        else
        {
            //Debug.LogError("Ray failed...");
            if (SignObject != null)
            {
                if (SignObject.activeInHierarchy) SignObject.SetActive(false);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            OpenArchorSign(false);
            ArchorToolManage.Instance.SignSetPart.ChangePosByMove(SignObject.transform.position);
        }
    }
    /// <summary>
    /// 修改位置
    /// </summary>
    private void ConfirmPos()
    {
        if (!ArchorToolManage.Instance.SignSetPart.IsArchorSet)
        {
            valueText.text = "基准点未设置。";
            return;
        }
        if(currentDev==null)
        {
            valueText.text = "当前选中设备为空，位置计算失败。";
            return;
        }
        float x = float.Parse(XPosInput.text);
        float y = float.Parse(YPosInput.text);
        float z = float.Parse(ZPosInput.text);
        LocationManager manager = LocationManager.Instance;
        if(manager == null)
        {
            Debug.LogError("LocationManager.Instance==null");
            return;
        }
        currentArchor = new Archor();
        Vector3 converSionUnit = manager.LocationOffsetScale;
        //根据测量值，获取相对于标记的位置
        Vector3 relativePos = new Vector3(-x / converSionUnit.x, y, -z / converSionUnit.z);

        //获取测量值，在3D中的位置
        Vector3 realPos = SignObject.transform.position + relativePos;
        currentDev.transform.position = new Vector3(realPos.x,currentDev.transform.position.y,realPos.z);

        //转换厂区CAD位置
        Vector3 FactoryPos = LocationManager.GetCadVector(realPos);
        FactoryPos.y = y;
        valueText.text = string.Format("CAD位置为:({0},{1},{2})",FactoryPos.x,FactoryPos.y,FactoryPos.z);
        currentArchor.X = FactoryPos.x;
        currentArchor.Y = FactoryPos.y;
        currentArchor.Z = FactoryPos.z;
        ChangeDevPos();
        RefleshGizmoPosition();
        //Debug.LogError("Archor CadPos:"+FactoryPos);
    }
    /// <summary>
    /// 保存设备信息
    /// </summary>
    private void SaveInfo()
    {
        if (!ArchorToolManage.Instance.SignSetPart.IsArchorSet)
        {
            SaveInfoResult.text = "基准点未设置。";
            return;
        }
        if (currentDev == null)
        {
            SaveInfoResult.text = "当前选中设备为空，位置计算失败。";
            return;
        }
        if (currentArchor==null)
        {
            SaveInfoResult.text="测量值未输入";
            return;
        }
        //Todo:保存基站信息，修改DevInfo信息
        CommunicationObject service = CommunicationObject.Instance;
        if(service)
        {
            Archor archor = service.GetArchorByDevId(currentDev.Info.Id);
            string archorName = DevName.text;
            if (!string.IsNullOrEmpty(archorName)) archor.Name = archorName;
            else archor.Name = currentDev.Info.Name;
            if (DevTypeDropdown.value == 0) archor.Type = ArchorTypes.副基站;
            else archor.Type = ArchorTypes.主基站;
            archor.Code = DevCode.text;
            archor.X = currentArchor.X;
            archor.Y = currentArchor.Y;
            archor.Z = currentArchor.Z;
            bool value = service.EditArchor(archor,(int)currentDev.Info.ParentId);
            Debug.Log("Edit Archor value:"+value);
            ShowSaveResult(value);
        }
    }
    /// <summary>
    /// 显示保存信息结果
    /// </summary>
    /// <param name="value"></param>
    private void ShowSaveResult(bool value)
    {
        if(value) SaveInfoResult.text = "保存信息成功。";
        else SaveInfoResult.text = "保存信息失败。";
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    private void ChangeDevPos()
    {
        Vector3 pos;
        if (currentDev as DepDevController)
        {
            pos = currentDev.transform.position;
        }
        else
        {
            pos = currentDev.transform.localPosition;
        }
        string archorName = DevName.text;
        if (!string.IsNullOrEmpty(archorName)) currentDev.Info.Name = archorName;
        DeviceEditUIManager.Instacne.EditPart.SingleEditPart.ChangePos(currentDev,pos);
    }
    /// <summary>
    /// 刷新设备编辑Gizmo位置
    /// </summary>
    private void RefleshGizmoPosition()
    {
        Gizmo activeGizmo = EditorGizmoSystem.Instance.ActiveGizmo;
        if (activeGizmo == null)
        {
            Debug.Log("Active Gizmo is null...");
            return;
        }
        activeGizmo.transform.position = EditorObjectSelection.Instance.GetSelectionWorldCenter();
    }
}
