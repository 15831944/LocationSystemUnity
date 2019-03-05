using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 历史轨迹人员控制器
/// </summary>
public class HistoryManController : MonoBehaviour
{

    public Color color = Color.green;
    public GameObject followUI;
    //private Button followUIbtn;
    public ToggleButton2 followUIbtn;

    public LocationHistoryPathBase locationHistoryPathBase;

    // Use this for initialization
    void Start()
    {
        HighlightOn();
    }

    // Update is called once per frame
    void Update()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            if (LocationHistoryManager.Instance.CurrentFocusController == this)
            {
                showArchorsTime += Time.deltaTime;
                if (showArchorsTime > 1)
                {
                    showArchorsTime = 0;
                    //StartCoroutine(ShowArchors_Coroutine());
                    ShowArchors_T();
                }
            }
            else
            {
                showArchorsTime = 0;
            }
        }
    }

    public void Init(Color colorT, LocationHistoryPathBase locationHistoryPathBaseT)
    {
        color = colorT;
        locationHistoryPathBase = locationHistoryPathBaseT;
    }

    private void OnEnable()
    {
        HighlightOn();
    }

    private void OnDisable()
    {
        HighlightOff();
        if (LocationHistoryManager.Instance.CurrentFocusController == this)
        {
            LocationHistoryManager.Instance.RecoverBeforeFocusAlign();
        }
    }

    /// <summary>
    /// 开启高亮
    /// </summary>
    public void HighlightOn()
    {
        //Highlighter h = transform.parent.gameObject.AddMissingComponent<Highlighter>();
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOn(color);
    }

    /// <summary>
    /// 关闭高亮
    /// </summary>
    public void HighlightOff()
    {
        //Highlighter h = transform.parent.gameObject.AddMissingComponent<Highlighter>();
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOff();
    }

    public AlignTarget GetAlignTarget()
    {
        Quaternion quaDir = Quaternion.LookRotation(-transform.forward, Vector3.up);
        AlignTarget alignTarget = new AlignTarget(transform, new Vector2(30, quaDir.eulerAngles.y), 5, new Range(5, 90), new Range(1, 40));
        return alignTarget;
    }

    public void SetFollowUI(GameObject o)
    {
        followUI = o;
        followUIbtn = followUI.GetComponent<ToggleButton2>();
        followUIbtn.OnValueChanged = null;
        followUIbtn.OnValueChanged += FollowUIbtn_OnClick;
    }

    /// <summary>
    /// 跟随按钮触发事件
    /// </summary>
    public void FollowUIbtn_OnClick(bool ison)
    {
        if (LocationHistoryManager.Instance)
            LocationHistoryManager.Instance.Focus(this);
    }

    #region 让参与计算的基站显示出来（测试）,及显示人员真实位置的测试小球
    //让参与计算的基站显示出来（测试）
    List<DevNode> archorObjs = new List<DevNode>();
    float showArchorsTime = 0;
    /// <summary>
    /// 显示参与计算的基站，协程
    /// </summary>
    public IEnumerator ShowArchors_Coroutine()
    {
        bool ishasvalue = true;

        if (locationHistoryPathBase.currentPointIndex - 1 >= 0)
        {
            double temp = (locationHistoryPathBase.timelist[locationHistoryPathBase.currentPointIndex] - locationHistoryPathBase.timelist[locationHistoryPathBase.currentPointIndex - 1]).TotalSeconds;
            if (temp > 3f)//如果当前要执行历史点的值，超过播放时间值5秒，就认为这超过5秒时间里，没历史轨迹数据
            {
                ishasvalue = false;
            }
        }

        if (LocationHistoryManager.Instance.CurrentFocusController == this && ishasvalue)
        {
            ShowArchors();
            yield return new WaitForSeconds(1);
        }
        else
        {
            FlashingOffArchors();
            yield return null;
        }

    }

    public void ShowArchors_T()
    {
        bool ishasvalue = true;
        //try
        //{
            if (locationHistoryPathBase.currentPointIndex - 1 >= 0&& locationHistoryPathBase.currentPointIndex< locationHistoryPathBase.timelist.Count)
            {
                double temp = (locationHistoryPathBase.timelist[locationHistoryPathBase.currentPointIndex] - locationHistoryPathBase.timelist[locationHistoryPathBase.currentPointIndex - 1]).TotalSeconds;
                if (temp > 3f)//如果当前要执行历史点的值，超过播放时间值5秒，就认为这超过5秒时间里，没历史轨迹数据
                {
                    ishasvalue = false;
                }
            }
        //}
        //catch
        //{
        //    int i = 0;
        //}
        if (LocationHistoryManager.Instance.CurrentFocusController == this && ishasvalue)
        {
            ShowArchors();
        }
        else
        {
            FlashingOffArchors();
        }

    }

    /// <summary>
    /// 显示参与计算的基站
    /// </summary>
    public void ShowArchors()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            FlashingOnArchors();
        }
        //else
        //{
        //    FashingOffArchors();
        //}
    }

    /// <summary>
    /// 闪烁所有基站
    /// </summary>
    public void FlashingOnArchors()
    {
        FlashingOffArchors();

        archorObjs.Clear();

        Position p = GetPosition();
        if (p == null) return;

        if (p.Archors != null)
        {
            foreach (string astr in p.Archors)
            {
                Archor a = LocationManager.Instance.GetArchorByCode(astr);
                if (a == null) continue;
                int idT = a.DevInfoId;
                RoomFactory.Instance.GetDevByid(idT, (nodeT)
                    =>
                {
                    if (nodeT == null) return;
                    archorObjs.Add(nodeT);
                    nodeT.FlashingOn();
                });
            }
        }
    }

    /// <summary>
    /// 停止闪烁所有基站
    /// </summary>
    public void FlashingOffArchors()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            foreach (DevNode o in archorObjs)
            {
                o.FlashingOff();
            }
        }
    }


    /// <summary>
    /// 计算历史轨迹人员的所在区域
    /// </summary>
    public Position GetPosition()
    {
        List<Position> ps = MultHistoryPlayUI.Instance.GetPositionsByPersonnel(locationHistoryPathBase.personnel);
        if (ps != null)
        {
            if (locationHistoryPathBase.currentPointIndex < ps.Count)
            {
                Position p = ps[locationHistoryPathBase.currentPointIndex];
                return p;
            }
        }

        return null;
    }
    #endregion
}
