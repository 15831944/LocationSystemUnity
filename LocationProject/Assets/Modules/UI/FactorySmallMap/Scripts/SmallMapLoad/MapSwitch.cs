using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapSwitch : MonoBehaviour {
    /// <summary>
    /// 存放所有的按钮组
    /// </summary>
    public GameObject FloorButtonGroup;
    /// <summary>
    /// 楼层按钮预设
    /// </summary>
    public GameObject FloorButtonPrefab;
    /// <summary>
    /// 按钮栏遮罩
    /// </summary>
    public GameObject ButtonMask;
    /// <summary>
    /// 上一层楼
    /// </summary>
    public Button FrontButton;
    /// <summary>
    /// 下一层楼
    /// </summary>
    public Button NextButton;
    /// <summary>
    /// 切页窗体
    /// </summary>
    public GameObject Window;

    /// <summary>
    /// 自动布局组件
    /// </summary>
    private VerticalLayoutGroup LayoutGroup;
    /// <summary>
    /// 两个按钮之间的距离  35+20
    /// </summary>
    private float ButtonOffset = 55;
    /// <summary>
    /// 子物体选中的节点
    /// </summary>
    [HideInInspector]
    public DepNode ItemSelectNode;
    /// <summary>
    /// 当前选中下标
    /// </summary>
    private int CurrentSelcetIndex=-1;
    /// <summary>
    /// 界面开启时，是否刷新
    /// </summary>
    private bool IsRefreshEnable = false;
    // Use this for initialization
    void Awake () {
        LayoutGroup = FloorButtonGroup.GetComponent<VerticalLayoutGroup>();
        FrontButton.onClick.AddListener(SelectLast);
        NextButton.onClick.AddListener(SelectNext);
    }
    void OnEnable()
    {
        //Debug.LogError("MapSwith.OnEnbale.");
        if(IsRefreshEnable)
        {
            SelectItem(CurrentSelcetIndex);
        }
    }
    /// <summary>
    /// 关闭界面时，清除信息
    /// </summary>
    public void Hide()
    {
        Window.SetActive(false);
        ItemList.Clear();
        DeslectLast();
        CurrentSelcetIndex = -1;
        CurrentLayoutHeight = 0;
        ItemSelectNode = null;
    }
    //private void Hide
    /// <summary>
    /// 按钮列表
    /// </summary>
    [HideInInspector]
    public List<MapSwitchItem> ItemList = new List<MapSwitchItem>();
    /// <summary>
    /// 当前自动布局高度
    /// </summary>
    private float CurrentLayoutHeight;
    public void ShowMapSwitch(MapFloor currentFloor,List<MapFloor>FloorList)
    {        
        if (ItemSelectNode == currentFloor.FloorNode) return;
        CurrentLayoutHeight = LayoutGroup.preferredHeight;
        Debug.Log("LayoutHeight:"+CurrentLayoutHeight);
        if (FloorList==null||FloorList.Count==1)
        {
            Hide();
            return;
        }
        else
        {
            Window.SetActive(true);
            ItemList.Clear();
            InitItem(FloorList);
            SelectCurrentFloor(currentFloor);
        }
    }
    /// <summary>
    /// 取消上一个选中
    /// </summary>
    private void DeslectLast()
    {
        if (CurrentSelcetIndex == -1||FloorButtonGroup.transform.childCount==0) return;
        GameObject obj = FloorButtonGroup.transform.GetChild(CurrentSelcetIndex).gameObject;
        if(obj!=null)
        {
            MapSwitchItem item = obj.GetComponent<MapSwitchItem>();
            if (item) item.DisSelect();
        }
    }
    #region 切页按钮生成部分
    /// <summary>
    /// 选中当前楼层
    /// </summary>
    /// <param name="currentFloor"></param>
    private void SelectCurrentFloor(MapFloor currentFloor)
    {
        foreach(var item in ItemList)
        {
            if(item.CurrentFloor==currentFloor)
            {
                item.Selcet();
            }
        }
    }
    /// <summary>
    /// 生成按钮组
    /// </summary>
    /// <param name="FloorList"></param>
    private void InitItem(List<MapFloor> FloorList)
    {
        int ListCount = FloorList.Count;
        int ChildCount = FloorButtonGroup.transform.childCount;
        if(ChildCount> ListCount)
        {
            for(int i=0;i<ChildCount;i++)
            {
                Transform SwitchButton = FloorButtonGroup.transform.GetChild(i);
                if(i<=ListCount-1)
                {
                    AddSwitchInfo(SwitchButton, FloorList[i]);
                }
                else
                {
                    SwitchButton.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for(int i=0;i<ListCount;i++)
            {               
                if (i <= ChildCount-1)
                {
                    Transform SwitchButton = FloorButtonGroup.transform.GetChild(i);
                    AddSwitchInfo(SwitchButton, FloorList[i]);
                }
                else
                {
                    GameObject obj = Instantiate(FloorButtonPrefab, FloorButtonGroup.transform) as GameObject;
                    obj.SetActive(true);
                    obj.transform.localScale = Vector3.one;
                    MapSwitchItem item = obj.GetComponent<MapSwitchItem>();
                    ItemList.Add(item);
                    item.SetItem(FloorList[i],this);
                }
            }
        }
    }
    /// <summary>
    /// 添加按钮信息
    /// </summary>
    /// <param name="switchButton"></param>
    /// <param name="floorInfo"></param>
    private void AddSwitchInfo(Transform switchButton,MapFloor floorInfo)
    {
        switchButton.gameObject.SetActive(true);
        MapSwitchItem item = switchButton.GetComponent<MapSwitchItem>();
        ItemList.Add(item);
        item.SetItem(floorInfo, this);
    }
    #endregion
    /// <summary>
    /// 选择下一个
    /// </summary>
    private void SelectNext()
    {
        if (CurrentSelcetIndex ==-1) return;
        int value = CurrentSelcetIndex + 1;
        if (value >= ItemList.Count) return;
        ItemList[value].OnItemClick();
    }
    /// <summary>
    /// 选择上一个
    /// </summary>
    private void SelectLast()
    {
        if (CurrentSelcetIndex == -1) return;
        int value = CurrentSelcetIndex - 1;
        if (value < 0) return;
        ItemList[value].OnItemClick();
    }
    /// <summary>
    /// 选中子物体
    /// </summary>
    /// <param name="ChildIndex"></param>
    public void SelectItem(int ChildIndex)
    {
        if(CurrentSelcetIndex!=ChildIndex)DeslectLast();
        CurrentSelcetIndex = ChildIndex;
        //LayoutRebuilder.ForceRebuildLayoutImmediate(FloorButtonGroup.GetComponent<RectTransform>());
        RebuildLayout(()=> 
        {
            float LayoutHeight = LayoutGroup.preferredHeight;
            float MaskHeight = ButtonMask.GetComponent<RectTransform>().sizeDelta.y;
            float posY;
            if (ChildIndex == 0)
            {
                posY = -(LayoutHeight / 2 - MaskHeight / 2);

            }
            else if (ChildIndex == ItemList.Count - 1)
            {
                int newIndex = ChildIndex - 2;
                posY = -(LayoutHeight / 2 - MaskHeight / 2) + (ButtonOffset * newIndex);
            }
            else
            {
                int newIndex = ChildIndex - 1;
                posY = -(LayoutHeight / 2 - MaskHeight / 2) + (ButtonOffset * newIndex);
            }
            FloorButtonGroup.transform.localPosition = new Vector3(0, posY, 0);
        });       
    }
    /// <summary>
    /// 强制刷新自动布局组件
    /// </summary>
    /// <param name="onComplete"></param>
    private void RebuildLayout(Action onComplete)
    {       
        if(!gameObject.activeInHierarchy)
        {
            IsRefreshEnable = true;
            return;
        }
        StopCoroutine("RebuildLayoutCorutine");
        StartCoroutine(RebuildLayoutCorutine(onComplete));
    }
    /// <summary>
    /// 强制刷新自动布局组件
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    IEnumerator RebuildLayoutCorutine(Action onComplete)
    {
        RectTransform buttonGroup = FloorButtonGroup.GetComponent<RectTransform>();
        float preferredHeight = LayoutGroup.preferredHeight;
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonGroup);
        yield return new WaitForEndOfFrame();
        float EndValue = ButtonOffset * ItemList.Count - 20;
        int WaitTime = 0;
        while (Math.Abs(preferredHeight - EndValue) > 5)
        {
            Debug.Log("Layout Height:" + LayoutGroup.preferredHeight);
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonGroup);
            yield return new WaitForEndOfFrame();
            WaitTime++;
            if (WaitTime == 5) break;
        }
        if (onComplete != null) onComplete();
    }
}
