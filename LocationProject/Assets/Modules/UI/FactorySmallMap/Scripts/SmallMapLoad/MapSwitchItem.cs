using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MapSwitchItem : MonoBehaviour,IPointerClickHandler
{
    /// <summary>
    /// 当前楼层
    /// </summary>
    [HideInInspector]
    public MapFloor CurrentFloor;
    /// <summary>
    /// 切页管理
    /// </summary>
    private MapSwitch SwitchManager;
    /// <summary>
    /// 选中背景图
    /// </summary>
    public Image SelectImage;
    /// <summary>
    /// 楼层文本
    /// </summary>
    public Text FloorIndexText;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 设置项目
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="manager"></param>
    public void SetItem(MapFloor floor,MapSwitch manager)
    {
        CurrentFloor = floor;
        SwitchManager = manager;
        FloorIndexText.text = floor.FloorNum.ToString();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClick();
    }
    /// <summary>
    /// 选中当前项目
    /// </summary>
    public void OnItemClick()
    {
        if(CurrentFloor!=null&&CurrentFloor.FloorNode!=null)
        {
            if (FactoryDepManager.currentDep == CurrentFloor.FloorNode) return;
            Selcet();
            SwitchManager.ItemSelectNode = CurrentFloor.FloorNode;
            RoomFactory.Instance.FocusNode(CurrentFloor.FloorNode) ;
        }
    }
    public void Selcet()
    {
        SetAlpha(1, SelectImage);
        CurrentFloor.ShowFloor();
        int index= transform.GetSiblingIndex();
        SwitchManager.SelectItem(index);
    }
    /// <summary>
    /// 取消项目选中
    /// </summary>
    public void DisSelect()
    {
        SetAlpha(0,SelectImage);
        CurrentFloor.HideFloor();
        //SelectImage.enabled = false;
    }
    /// <summary>
    /// 设置图片透明度
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="img"></param>
    private void SetAlpha(float alpha,Image img)
    {
        alpha = alpha > 1 || alpha < 0 ? 1 : alpha;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
