using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 多人历史轨迹的人员UI项
/// </summary>
public class HistoryPersonUIItem : MonoBehaviour
{

    public GameObject content;//容器
    public Image photo;//头像
    public Text txtName;//人员名称
    public Text txtDepartmemnt;//部门名称
    public Text txtPlace;//人员所在的区域   
    public Button deleteBtn;//删除   
    private Image deleteBtnImg;//删除按钮图    
    public Color color = Color.green;//颜色    
    public Sprite NormalSprite;//正常图片   
    public Sprite deleteSprite;//删除图片

    [HideInInspector]
    [System.NonSerialized]
    public Personnel personnel;// 人员信息数据

    // Use this for initialization
    void Start()
    {
        EventTriggerListener listener = EventTriggerListener.Get(content);
        listener.onEnter = Content_OnEnter;
        listener.onExit = Content_OnExit;

        EventTriggerListener listener2 = EventTriggerListener.Get(deleteBtn.gameObject);
        listener2.onEnter = DeleteBtn_OnEnter;
        listener2.onExit = DeleteBtn_OnExit;

        deleteBtn.onClick.AddListener(Delete);

        deleteBtnImg = deleteBtn.GetComponent<Image>();
        deleteBtnImg.color = color;
        deleteBtnImg.sprite = NormalSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="personnelT"></param>
    /// <param name="colorT"></param>
    public void Init(Personnel personnelT, Color colorT)
    {
        personnel = personnelT;
        color = colorT;
        txtName.text = personnel.Name;
        //txtDepartmemnt.text = personnel.Parent.Name;
        txtDepartmemnt.text = personnel.Pst;
        LocationUIManage.Instance.SetPhoto(photo, personnel.Sex);
    }

    /// <summary>
    /// 删除
    /// </summary>
    public void Delete()
    {
        print("删除");
        bool isPlayt=LocationHistoryUITool.GetIsPlaying();
        if (isPlayt)
        {
            UGUIMessageBox.Show("是否终止当前人员历史轨迹演示，并删除该人员？", "是", "否", () =>
            {
                ////如果在播放就让它终止
                //ExecuteEvents.Execute<IPointerClickHandler>(MultHistoryPlayUI.Instance.StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                //MultHistoryPlayUI.Instance.RemovePerson(personnel);
                LocationHistoryUITool.StopPlay(personnel);
            }, null, null);
        }
        else
        {
            //MultHistoryPlayUI.Instance.RemovePerson(personnel);
            //ExecuteEvents.Execute<IPointerClickHandler>(MultHistoryPlayUI.Instance.StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            LocationHistoryUITool.StopPlay(personnel);
        }

    }

    /// <summary>
    /// 进入容器——触发事件
    /// </summary>
    public void Content_OnEnter(GameObject o)
    {
        print("Content_OnEnter!");
        SetDeleteBtnSprite(true);
    }

    /// <summary>
    /// 离开容器——触发事件
    /// </summary>
    /// <param name="o"></param>
    public void Content_OnExit(GameObject o)
    {
        print("Content_OnExit!");
        SetDeleteBtnSprite(false);
    }

    /// <summary>
    /// 进入删除按钮——触发事件
    /// </summary>
    /// <param name="o"></param>
    public void DeleteBtn_OnEnter(GameObject o)
    {
        print("DeleteBtn_OnEnter!");
    }

    /// <summary>
    /// 离开删除按钮——触发事件
    /// </summary>
    /// <param name="o"></param>
    public void DeleteBtn_OnExit(GameObject o)
    {
        print("DeleteBtn_OnExit!");
    }

    /// <summary>
    /// 设置删除按钮图片
    /// </summary>
    /// <param name="isDelete"></param>
    public void SetDeleteBtnSprite(bool isDelete)
    {
        if (isDelete)
        {
            deleteBtnImg.sprite = deleteSprite;
        }
        else
        {
            deleteBtnImg.sprite = NormalSprite;
        }
    }

    /// <summary>
    /// 更新位置信息
    /// </summary>
    public void RefleshTxtPlace(string placeT)
    {
        txtPlace.text = placeT;
    }
}
