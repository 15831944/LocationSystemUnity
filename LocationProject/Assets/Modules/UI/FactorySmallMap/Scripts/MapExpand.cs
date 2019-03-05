using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MapExpand : MonoBehaviour {
    /// <summary>
    /// 展开按钮
    /// </summary>
    public Button ExpandButton;
    /// <summary>
    /// 正常背景图
    /// </summary>
    public GameObject NormalBg;
    /// <summary>
    /// 关闭背景图
    /// </summary>
    public GameObject CloseBg;
    /// <summary>
    /// 地图展开按钮
    /// </summary>
    public GameObject MapExpandClicker;
    /// <summary>
    /// 地图展开后，按钮角度
    /// </summary>
    private Vector3 ClickerExpandAngle = new Vector3(0, 0, 270);
    /// <summary>
    /// 地图背景展开动画
    /// </summary>
    private Tween ExpandTween;
    private bool IsExpand = true;
	// Use this for initialization
	void Start () {
        InitTween();
        ExpandButton.onClick.AddListener(ExpandMap);
    }
	public void ShowBg(DepNode node)
    {
        DepNode CurrentNode = node;
        MapLoadManage Map = MapLoadManage.Instance;
        if (IsParentFloor(CurrentNode))
        {
            CurrentNode = node.ParentNode;
        }
        if (CurrentNode as FactoryDepManager|| CurrentNode as DepController|| CurrentNode as BuildingController)
        {
            if (Map)
            {
                Map.DisSelectLast();
            }
            ExpandButton.gameObject.SetActive(false);
            CloseBg.SetActive(true);
            NormalBg.SetActive(false);
            ScaleTreeWindow(true);
        }
        else
        {
            if(Map&&Map.ShowBuildingMap(CurrentNode))
            {
                ExpandButton.gameObject.SetActive(true);
                //if (IsExpand) ExpandOut();
                if(IsExpand)
                {
                    ExpandOut();
                    ScaleTreeWindow(false);
                }               
            }
        }
    }
    /// <summary>
    /// 展开小地图
    /// </summary>
    public void ExpandMap()
    {
        if(IsExpand)
        {
            ExpandIn();
            ScaleTreeWindow(true);
        }
        else
        {           
            ExpandOut();
            ScaleTreeWindow(false);
        }
    }
    /// <summary>
    /// 父节点是否楼层
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private bool IsParentFloor(DepNode dep)
    {
        if (dep as RoomController || (dep.ParentNode != null && dep.ParentNode is FloorController))return true;
        else return false;
    }
    /// <summary>
    /// 缩放树窗体
    /// </summary>
    /// <param name="isExpand">树窗体是否扩大</param>
    private void ScaleTreeWindow(bool isTreeExpand)
    {
        FullViewController mainPage = FullViewController.Instance;
        if (mainPage&&mainPage.IsFullView)
        {
            Debug.Log("Is in main page...");
            return;
        }
        TopoTreeManager topoTree = TopoTreeManager.Instance;
        if (topoTree) topoTree.ScaleWindow(isTreeExpand);
        PersonnelTreeManage personalTree = PersonnelTreeManage.Instance;
        if (personalTree) personalTree.ScaleWindow(isTreeExpand);
    }
    /// <summary>
    /// 地图收起
    /// </summary>
    private void ExpandIn()
    {
        IsExpand = false;
        CloseBg.SetActive(true);
        MapExpandClicker.transform.localEulerAngles = Vector3.zero;
        ExpandTween.OnRewind(()=> 
        {
            if(!IsExpand)
            {
                NormalBg.SetActive(false);
            }
        }).PlayBackwards();
    }
    /// <summary>
    /// 地图展开
    /// </summary>
    private void ExpandOut()
    {
        IsExpand = true;
        MapExpandClicker.transform.localEulerAngles = ClickerExpandAngle;
        if (!NormalBg.activeInHierarchy)
        {
            NormalBg.SetActive(true);
            ExpandTween.OnComplete(()=> 
            {
                CloseBg.SetActive(false);
            }).Restart();
        }
        else
        {
            NormalBg.SetActive(true);
            if(ExpandTween.IsComplete())
            {
                Debug.Log("Tween has been complete...");
                //Todo: Change map by DepId;
            }
            else
            {
                ExpandTween.OnComplete(() =>
                {
                    CloseBg.SetActive(false);
                }).PlayForward();
            }
        }       
    }
    /// <summary>
    /// 初始化动画
    /// </summary>
    private void InitTween()
    {
        ExpandTween = NormalBg.transform.DOScaleY(1, 0.3f);
        ExpandTween.SetAutoKill(false);
        ExpandTween.Pause();
    }
}
