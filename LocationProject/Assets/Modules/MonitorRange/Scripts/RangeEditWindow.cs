using Location.WCFServiceReferences.LocationServices;
using MonitorRange;
using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangeEditWindow : MonoBehaviour
{
    public static RangeEditWindow Instance;

    public GameObject window;//窗体
    public GameObject content;//内容
    public GameObject mask;//没选择任何区域

    public InputField NameInput;//名称
    public Text TypeTxt;//区域类型

    public InputField APosInput;//A轴位置, CAD的南北方向
    public InputField BPosInput;//B轴位置，CAD的东西方向
    public InputField HPosInput;//H轴位置，高度
    public InputField AngleInput;//角度

    public InputField LengthInput;//长度
    public InputField WidthInput;//宽度
    public InputField HeightInput;//高度

    public MonitorRangeObject currentMRObj;//当前物体

    public Button areaAddBtn;//区域添加按钮
    public Button deleteBtn;//区域删除按钮

    public Button Exit;//退出区域编辑按钮

    // Use this for initialization
    void Start()
    {
        Instance = this;
        EditorObjectSelection.Instance.SelectionChanged += Instance_SelectionChanged;
        EditorObjectSelection.Instance.GameObjectClicked += Instance_GameObjectClicked;

        NameInput.onEndEdit.AddListener(NameInput_OnEndEdit);
        APosInput.onEndEdit.AddListener(APosInput_OnEndEdit);
        BPosInput.onEndEdit.AddListener(BPosInput_OnEndEdit);
        HPosInput.onEndEdit.AddListener(HPosInput_OnEndEdit);
        AngleInput.onEndEdit.AddListener(AngleInput_OnEndEdit);
        LengthInput.onEndEdit.AddListener(LengthInput_OnEndEdit);
        WidthInput.onEndEdit.AddListener(WidthInput_OnEndEdit);
        HeightInput.onEndEdit.AddListener(HeightInput_OnEndEdit);

        if(areaAddBtn!=null)
            areaAddBtn.onClick.AddListener(AddAreaBtn_OnClick);
        if(deleteBtn!=null)
            deleteBtn.onClick.AddListener(DeleteAreaBtn_OnClick);
        SetDeletwBtnInteractable(false);

        if(Exit)
            Exit.onClick.AddListener(Exit_OnClick);
    }


    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 选择切换
    /// </summary>
    /// <param name="selectionChangedEventArgs"></param>
    public void Instance_SelectionChanged(ObjectSelectionChangedEventArgs selectionChangedEventArgs)
    {
        Debug.Log("Instance_SelectionChanged!");
        //if (selectionChangedEventArgs.SelectedObjects.Count == 0)
        //{
        //    //EditorObjectSelection.Instance.ClearSelection(false);
        //    //Hide();
        //    HideContent();
        //    SetDeletwBtnInteractable(true);


        //}
        if (EditorObjectSelection.Instance.SelectedGameObjects.Count == 0)
        {
            //EditorObjectSelection.Instance.ClearSelection(false);
            //Hide();
            HideContent();
            SetDeletwBtnInteractable(false);


        }        
        else if (EditorObjectSelection.Instance.SelectedGameObjects.Count > 1)
        {
            HideContent();
            //List<GameObject> gs = new List<GameObject>(EditorObjectSelection.Instance.SelectedGameObjects);
            //for (int i = 0; i < gs.Count; i++)
            //{
            //    GameObject o = gs[i];
            //    MonitorRangeObject objt = o.GetComponent<MonitorRangeObject>();
            //    objt.areaNameUIController.SetImageRaycast(false);
            //}
            SetDeletwBtnInteractable(false);
        }
        else
        {
            SetDeletwBtnInteractable(true);
        }
    }

    /// <summary>
    /// 点击物体
    /// </summary>
    /// <param name="clickedObject"></param>
    public void Instance_GameObjectClicked(GameObject clickedObject)
    {
        Debug.Log("Instance_GameObjectClicked!");
        MonitorRangeObject objT= clickedObject.GetComponent<MonitorRangeObject>();
        //currentMRObj = clickedObject.GetComponent<MonitorRangeObject>();
        if (objT)
        {
            Show(objT);
        }
    }

    public void Show()
    {
        SetWindowActive(true);
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show(MonitorRangeObject mrobj)
    {
        ShowContent();
        SetCurrentMRObj(mrobj);
        SetWindowActive(true);
        NameInput.text = mrobj.info.Name;
        TypeTxt.text = "";
        if (mrobj.info.Transfrom.IsOnLocationArea)
        {
            TypeTxt.text = "定位监控区域";
        }
        else if (mrobj.info.Transfrom.IsOnAlarmArea)
        {
            TypeTxt.text = "告警区域";
        }
        else
        {
            TypeTxt.text = "普通区域";
        }

        //if (mrobj.info.Transfrom.IsRelative)
        //{
        //float posx = mrobj.transform.localPosition.x * mrobj.transform.parent.lossyScale.x;
        //float posy = mrobj.transform.localPosition.y * mrobj.transform.parent.lossyScale.y;
        //float posz = mrobj.transform.localPosition.z * mrobj.transform.parent.lossyScale.z;
        //Vector3 posT = LocationManager.GetDisRealSizeVector(new Vector3(posx, posy, posz));
        APosInput.text = Math.Round(mrobj.info.Transfrom.Z, 2).ToString();
        BPosInput.text = Math.Round(mrobj.info.Transfrom.X, 2).ToString();
        HPosInput.text = Math.Round(mrobj.info.Transfrom.Y, 2).ToString();
        //}
        //else
        //{
        //    Vector3 posT = LocationManager.GetDisRealSizeVector(transform.localPosition);
        //    APosInput.text = Math.Round(posT.z, 2).ToString();
        //    BPosInput.text = Math.Round(posT.x, 2).ToString();
        //    HPosInput.text = Math.Round(posT.y, 2).ToString();
        //}

        AngleInput.text = Math.Round(mrobj.info.Transfrom.RY, 2).ToString();

        //Vector3 sizeT = mrobj.gameObject.GetGlobalSize();
        //sizeT = LocationManager.GetDisRealSizeVector(sizeT);

        LengthInput.text = Math.Round(mrobj.info.Transfrom.SZ, 2).ToString();
        WidthInput.text = Math.Round(mrobj.info.Transfrom.SX, 2).ToString();
        HeightInput.text = Math.Round(mrobj.info.Transfrom.SY, 2).ToString();
    }

    private void SetCurrentMRObj(MonitorRangeObject mrobj)
    {
        if (currentMRObj != mrobj)
        {
            if (currentMRObj != null)
            {
                currentMRObj.areaNameUIController.SetImageRaycast(true);
            }

            if (mrobj != null&& mrobj.areaNameUIController!=null)
            {
                mrobj.areaNameUIController.SetImageRaycast(false);
            }
        }
        currentMRObj = mrobj;
    }

    ///// <summary>
    ///// 刷新数据
    ///// </summary>
    //public void RefleshData()
    //{

    //}

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindowActive(false);
        HideContent();
    }

    /// <summary>
    /// 设置Window
    /// </summary>
    /// <param name="isActive"></param>
    public void SetWindowActive(bool isActive)
    {
        if (window.activeInHierarchy != isActive)
        {
            window.SetActive(isActive);
        }
    }

    /// <summary>
    /// 显示内容
    /// </summary>
    public void ShowContent()
    {
        SetContent(true);
    }

    /// <summary>
    /// 关闭内容
    /// </summary>
    public void HideContent()
    {
        SetCurrentMRObj(null);
        SetContent(false);
    }

    /// <summary>
    /// 设置内容的显示和隐藏
    /// </summary>
    public void SetContent(bool isActive)
    {
        content.SetActive(isActive);
        SetMaskActive(!isActive);
    }

    public void SetObj()
    {
        currentMRObj.SaveInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    public void NameInput_OnEndEdit(string txtStr)
    {
        Debug.Log("NameInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void APosInput_OnEndEdit(string txtStr)
    {
        Debug.Log("APosInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void BPosInput_OnEndEdit(string txtStr)
    {
        Debug.Log("BPosInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void HPosInput_OnEndEdit(string txtStr)
    {
        Debug.Log("HPosInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void AngleInput_OnEndEdit(string txtStr)
    {
        Debug.Log("AngleInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void LengthInput_OnEndEdit(string txtStr)
    {
        Debug.Log("LengthInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    public void WidthInput_OnEndEdit(string txtStr)
    {
        Debug.Log("WidthInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }
    public void HeightInput_OnEndEdit(string txtStr)
    {
        Debug.Log("HeightInput_OnEndEdit!");
        if (currentMRObj != null)
        {
            UpdateData();
        }
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    public void UpdateData()
    {
        float APos = float.Parse(APosInput.text);
        float BPos = float.Parse(BPosInput.text);
        float HPos = float.Parse(HPosInput.text);
        float Angle = float.Parse(AngleInput.text);

        float Length = float.Parse(LengthInput.text);
        float Width = float.Parse(WidthInput.text);
        float Height = float.Parse(HeightInput.text);

        Vector3 realpos = new Vector3(BPos, HPos, APos);
        Vector3 realSize = new Vector3(Width, Height, Length);
        Vector3 realAngle = new Vector3(0, Angle, 0);
        currentMRObj.UpdateData(NameInput.text, realpos, realAngle, realSize);
        RefleshGizmoPosition();
    }

    /// <summary>
    /// 刷新区域编辑Gizmo位置
    /// </summary>
    public void RefleshGizmoPosition()
    {
        Gizmo activeGizmo = EditorGizmoSystem.Instance.ActiveGizmo;
        if (activeGizmo == null)
        {
            Debug.Log("Active Gizmo is null...");
            return;
        }
        activeGizmo.transform.position = EditorObjectSelection.Instance.GetSelectionWorldCenter();
    }

    ///// <summary>
    ///// 设置区域添加按钮的显示隐藏
    ///// </summary>
    //public void SetAreaAddBtnActive(bool isActive)
    //{
    //    areaAddBtn.gameObject.SetActive(isActive);
    //}

    /// <summary>
    /// 添加OnClick
    /// </summary>
    public void AddAreaBtn_OnClick()
    {
        CreateArea();
    }

    private PhysicalTopology CreateDefaultArea()
    {
        PhysicalTopology p = new PhysicalTopology();
        p.Type = AreaTypes.范围;
        p.Transfrom = new TransformM();
        p.Transfrom.IsCreateAreaByData = true;
        p.Name = "区域" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return p;
    }

    private void SetAreaTransform(PhysicalTopology area, Vector3 pos)
    {
        area.Transfrom.X = pos.x;
        area.Transfrom.Y = pos.y;
        area.Transfrom.Z = pos.z;

        area.Transfrom.RX = 0;
        area.Transfrom.RY = 0;
        area.Transfrom.RZ = 0;

        Vector3 realSizeT = LocationManager.GetDisRealSizeVector(Vector3.one * DefaultSize);//一个单位
        area.Transfrom.SX = Mathf.Abs(realSizeT.x);
        area.Transfrom.SY = Mathf.Abs(realSizeT.y);
        area.Transfrom.SZ = Mathf.Abs(realSizeT.z);
    }

    public float DefaultSize = 3f;

    public void CreateArea()
    {
        DepNode parentDep = FactoryDepManager.currentDep;//当前区域，要在当前区域下创建子区域（告警区域）
        if (parentDep == null)
        {
            Log.Error("RangeEditWindow.CreateArea", "depnodeT == null");
            return;
        }
        if (parentDep.TopoNode == null)
        {
            Log.Error("RangeEditWindow.CreateArea", "depnodeT.TopoNode == null");
            return;
        }

        if (parentDep.TopoNode.Type == AreaTypes.范围
            || parentDep.TopoNode.Type == AreaTypes.分组 
            || parentDep.TopoNode.Type == AreaTypes.CAD
            || parentDep.TopoNode.Type == AreaTypes.大楼
            || parentDep.TopoNode.Type == AreaTypes.机房
            )
        {
            UGUIMessageBox.Show("不能在该区域类型下创建子区域,类型:"+ parentDep.TopoNode.Type+",请切换到楼层或者厂区。");
            return;
        }

        Log.Info("RangeEditWindow.CreateArea", string.Format("type:{0},name:{1}", parentDep.TopoNode.Type, parentDep.TopoNode.Name));

        PhysicalTopology p = CreateDefaultArea();

        TransformM tm = parentDep.TopoNode.Transfrom;
        Vector3 centerPos = Vector3.zero;
        if (tm != null)
        {
            Log.Info("RangeEditWindow.CreateArea", string.Format("SX:{0},SY:{1},SZ:{2}", tm.SX, tm.SY, tm.SZ));
            centerPos = new Vector3((float)(tm.SX / 2f), (float)(tm.SY / 2), (float)(tm.SZ / 2));//父物体的尺寸的一半也就是父物体的中心
        }

        //if(RoomFactory.Instance.FactoryType==FactoryTypeEnum.BaoXin)
        //{
        //    pos2D = new Vector3(LocationManager.Instance.axisZero.x,pos2D.y, LocationManager.Instance.axisZero.z);            
        //}

        //Vector3 buildPos = Vector3.zero;
        //buildPos = -LocationManager.GetRealSizeVector(pos2D);

        p.ParentId = parentDep.TopoNode.Id;

        //判断是否是楼层或楼层以下级别的，是的话，设置IsRelative="true"
        bool b = MonitorRangeManager.IsBelongtoFloor(parentDep);

        Log.Info("RangeEditWindow.CreateArea", string.Format("IsBelongtoFloor:{0}", b));
        if (b)//如果是楼层及楼层以下级别就设置成相对的
        {
            p.IsRelative = true;
            p.Transfrom.IsRelative = true;
        }
        else
        {
            p.IsRelative = false;
            p.Transfrom.IsRelative = false;
            //pos2D = new Vector3((float)tm.X, (float)tm.Y, (float)tm.Z);
            centerPos = Vector3.zero;
            GetRealPos(parentDep, ref centerPos);
        }
        //Vector3 pos = parentDep.monitorRangeObject.transform.position;
        Vector3 realpos = centerPos;
        if (parentDep.TopoNode.Type == AreaTypes.楼层)//宝信项目坐标系偏移，应该也是兼容其他项目的
        {
            realpos.x += parentDep.TopoNode.InitBound.MinX;
            realpos.z += parentDep.TopoNode.InitBound.MinY;
        }

        SetAreaTransform(p, realpos);

        Loom.StartSingleThread(() =>
        {
            Log.Info("RangeEditWindow.CreateArea", string.Format("realpos:{0}", realpos));
            PhysicalTopology newArea = CommunicationObject.Instance.AddMonitorRange(p);//发送信息给服务端
            Loom.DispatchToMainThread(() =>
            {
                try
                {
                    if (newArea != null && newArea.Transfrom != null)
                    {
                        var newT = newArea.Transfrom;
                        Log.Info("RangeEditWindow.CreateArea", string.Format("newPos:({0},{1},{2})", newT.X, newT.Y, newT.Z));

                        RangeNode parentRangeNode = parentDep.monitorRangeObject.rangeNode;//区域的根节点
                        RangeNode newNode = parentRangeNode.NewNode();

                        MonitorRangeManager.Instance.CreateRangesByRootNode(newArea, newNode);
                        MonitorRangeObject monitorRangeObject = newNode.rangeObject;
                        monitorRangeObject.SetIsNewAdd(true);
                        monitorRangeObject.SetEditEnable(true);
                        monitorRangeObject.SetRendererEnable(true);
                        //monitorRangeObject.SetSelectedUI(true);
                        newNode.rangeObject.gameObject.layer = LayerMask.NameToLayer(Layers.Range);
                        EditorObjectSelection.Instance.ClearSelection(false);
                        EditorObjectSelection.Instance.SetSelectedObjects(new List<GameObject>() { monitorRangeObject.gameObject }, false);
                        //if (depnodeT.ChildNodes == null)
                        //{
                        //    //depnodeT.ChildNodes.Add()
                        RangeController rangeController = RoomFactory.Instance.AddRange(parentDep, newArea);
                        //}
                        Debug.LogError("CreateArea:成功！");
                        rangeController.monitorRangeObject = monitorRangeObject;
                        Show(monitorRangeObject);
                        MonitorRangeManager.Instance.AddRangeToList(monitorRangeObject);
                        //PersonnelTreeManage.Instance.areaDivideTree.RefreshShowAreaDivideTree();
                        PersonnelTreeManage.Instance.areaDivideTree.AddAreaChild(parentDep.TopoNode, newArea);
                        //monitorRangeObject.Focus();

                    }
                    else//else要有
                    {
                        UGUIMessageBox.Show("服务端创建子区域失败!");
                    }
                }
                catch (Exception e)
                {
                    Log.Error("RangeEditWindow.CreateArea", ""+e);
                }
                
            });
        });


    }

    /// <summary>
    /// 获取实际尺寸，就是CAD尺寸
    /// </summary>
    public void GetRealPos(DepNode depnodeT, ref Vector3 v)
    {
        if (depnodeT == null || depnodeT.TopoNode == null || depnodeT.TopoNode.Transfrom == null) return;
        TransformM tm = depnodeT.TopoNode.Transfrom;
        Vector3 vt = new Vector3((float)tm.X, (float)tm.Y, (float)tm.Z);
        v += vt;
        if (depnodeT.TopoNode.IsRelative)
        {
            GetRealPos(depnodeT.ParentNode, ref v);
        }
        else
        {
            return;
        }
    }

    public void DeleteAreaBtn_OnClick()
    {
        string selectedName = GetSelectedNames();
        UGUIMessageBox.Show("确定删除区域:"+ selectedName, () =>
        {
            Debug.LogError("删除按钮！");
            DeleteAreas();
            SetDeletwBtnInteractable(false);
        }, null);
    }

    private string GetSelectedNames()
    {
        string txt = "";
        List<GameObject> gs = new List<GameObject>(EditorObjectSelection.Instance.SelectedGameObjects);
        for (int i = 0; i < gs.Count; i++)
        {
            var obj = gs[i];
            if (obj == null) continue;
            var monitorRangeObject = obj.GetComponent<MonitorRangeObject>();
            if (monitorRangeObject == null) continue;
            if (monitorRangeObject.info == null) continue;
            txt += monitorRangeObject.info.Name;
            if (gs.Count > 1 && i < gs.Count - 1)
            {
                txt += ",";
            }
        }

        return txt;
    }

    public void DeleteAreas()
    {
        List<GameObject> gs = new List<GameObject>(EditorObjectSelection.Instance.SelectedGameObjects);

        for (int i = 0; i < gs.Count; i++)
        {
            DeleteArea(gs[i]);
        }
    }

    private static void DeleteArea(GameObject objt)
    {
        //GameObject objt = gs[i];
        MonitorRangeObject monitorRangeObject = objt.GetComponent<MonitorRangeObject>();
        PhysicalTopology pt = monitorRangeObject.info;
        //if (gs.Count >= 0)
        //{
        //    objt = gs[gs.Count - 1];
        //    monitorRangeObject = objt.GetComponent<MonitorRangeObject>();
        //    pt = monitorRangeObject.info;
        //}

        //if (pt == null)
        //    return;

        Loom.StartSingleThread(() =>
        {
            bool b = CommunicationObject.Instance.DeleteMonitorRange(pt);
            Loom.DispatchToMainThread(() =>
            {
                if (b)
                {
                    Debug.LogError("删除成功！");
                    UGUIMessageBox.Show("删除成功！");
                }
                else
                {
                    Debug.LogError("删除失败！");
                    UGUIMessageBox.Show("删除失败，区域下存在设备！");
                    return;
                }
                if (pt != null)
                {
                    RangeNode node = monitorRangeObject.rangeNode;
                    RangeNode parentnode = node.parentNode;
                    parentnode.subNodes.Remove(node);

                    DepNode depnode = monitorRangeObject.depNode;
                    DepNode parentdepnode = depnode.ParentNode;
                    parentdepnode.ChildNodes.Remove(depnode);

                    RoomFactory.Instance.NodeDic_Remove(monitorRangeObject.depNode);
                    EditorObjectSelection.Instance.ClearSelection(false);
                    Destroy(monitorRangeObject.gameObject);
                    PersonnelTreeManage.Instance.areaDivideTree.RemoveAreaChild(pt.Id);
                    
                }
            });
        });
    }

    public void SetMaskActive(bool isActive)
    {
        mask.SetActive(isActive);
    }

    public void SetDeletwBtnInteractable(bool isInteractable)
    {
        if (deleteBtn)
        {
            deleteBtn.interactable = isInteractable;
            if (isInteractable)
            {
                deleteBtn.GetComponentInChildren<Text>(true).color = deleteBtn.colors.normalColor;

            }
            else
            {
                deleteBtn.GetComponentInChildren<Text>(true).color = deleteBtn.colors.disabledColor;
            }
        }
    }

    /// <summary>
    /// 退出区域编辑
    /// </summary>
    public void Exit_OnClick()
    {
        PersonSubsystemManage.Instance.SetEditAreaToggle(false);
    }
}
