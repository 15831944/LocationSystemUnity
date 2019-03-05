using Assets.M_Plugins.Helpers.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DevInfoFollowUI : MonoBehaviour
{
    public GameObject NormalPart;
    public GameObject CopyPart;

    /// <summary>
    /// 设备名称按钮
    /// </summary>
    public Text DevNameCopy;
    /// <summary>
    /// 删除设备按钮
    /// </summary>
    public Button DeleteButtonCopy;
    /// <summary>
    /// 批量复制设备
    /// </summary>
    public Button BatchCopyButton;

    /// <summary>
    /// 设备名称按钮
    /// </summary>
    public Text DevNameNormal;
    /// <summary>
    /// 删除设备按钮
    /// </summary>
    public Button DeleteButtonNoraml;
    /// <summary>
    /// 设备编辑管理脚本
    /// </summary>
    private DeviceEditUIManager Manager;
    /// <summary>
    /// 当前设备
    /// </summary>
    private DevNode currentDev;
    /// <summary>
    /// 是否可以批量复制设备
    /// </summary>
    private bool IsCopyDev;
	// Use this for initialization
	void Start ()
	{
        DeleteButtonNoraml.onClick.AddListener(DeleteDev);
        DeleteButtonCopy.onClick.AddListener(DeleteDev);
        BatchCopyButton.onClick.AddListener(StartCopy);
    }

    public void Show(DevNode dev)
    {
        InitManager();
        currentDev = dev;
        SetWindowState(dev);
        ShowDeleteButton(dev);
        //Window.SetActive(true);
        if (TypeCodeHelper.IsStaticDev(dev.Info.TypeCode.ToString()))
        {
            if(!IsCopyDev) DevNameNormal.text = dev.Info != null ? dev.Info.Name + "   " : "";
            else DevNameCopy.text = dev.Info != null ? dev.Info.Name+"   " : "";
        }
        else
        {
            if (!IsCopyDev) DevNameNormal.text = dev.Info != null ? dev.Info.Name : "";
            else DevNameCopy.text = dev.Info != null ? dev.Info.Name : "";
        }
    }
    /// <summary>
    /// 开启批量复制
    /// </summary>
    private void StartCopy()
    {
        SurroundEditMenu_BatchCopy copyPart = SurroundEditMenu_BatchCopy.Instacne;
        if (ObjectAddListManage.IsEditMode&&currentDev!=null) copyPart.Open(currentDev);
    }
    /// <summary>
    /// 设置窗口显示
    /// </summary>
    /// <param name="dev"></param>
    private void SetWindowState(DevNode dev)
    {
        bool isLoctaionDev = (dev.Info != null && TypeCodeHelper.IsLocationDev(dev.Info.TypeCode.ToString()));
        if ((dev is RoomDevController || dev is DepDevController) && !isLoctaionDev)
        {
            IsCopyDev = true;
            CopyPart.SetActive(true);
            NormalPart.SetActive(false);
        }
        else
        {
            IsCopyDev = false;
            CopyPart.SetActive(false);
            NormalPart.SetActive(true);
        }
    }
    private void InitManager()
    {
        if (Manager == null) Manager = DeviceEditUIManager.Instacne;
    }
    /// <summary>
    /// 是否显示删除按钮
    /// </summary>
    /// <param name="dev"></param>
    private void ShowDeleteButton(DevNode dev)
    {
        if (dev == null || dev.Info == null) return;
        if(TypeCodeHelper.IsStaticDev(dev.Info.TypeCode.ToString()))
        {
            DeleteButtonNoraml.gameObject.SetActive(false);
            DeleteButtonCopy.gameObject.SetActive(false);
        }
        else
        {
            if (IsCopyDev) DeleteButtonCopy.gameObject.SetActive(true);
            else DeleteButtonNoraml.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 删除设备
    /// </summary>
    private void DeleteDev()
    {
        if (Manager != null)
        {
            Manager.DeleteDev(currentDev);
        }
        else
        {
            Debug.LogError("DeviceEditUIManager.Instance is null...");
        }
    }

    void OnDestroy()
    {
        DeviceEditUIManager manager = DeviceEditUIManager.Instacne;
        if (manager)
        {
            manager.RemoveFollowUI(gameObject);
        }
    }
}
