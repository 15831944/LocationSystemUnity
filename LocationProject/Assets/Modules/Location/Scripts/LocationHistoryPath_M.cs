using Location.WCFServiceReferences.LocationServices;
using MonitorRange;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vectrosity;

/// <summary>
/// 历史轨迹
/// </summary>
public class LocationHistoryPath_M : LocationHistoryPathBase
{
    public HistoryPathDrawing historyPathDrawing;
    protected override void Start()
    {

        base.Start();
        LocationHistoryManager.Instance.AddHistoryPath_M(this as LocationHistoryPath_M);

        //timeStart = HistoryPlayUI.Instance.timeStart;
        Hide();

    }
    protected override void Update()
    {
        //if (MultHistoryPlayUINew.Instance.istest)
        //{
        //    int i= 0;
        //}
        base.Update();
        if (MultHistoryPlayUINew.Instance.isPlay)
        {
            if (!isCreatePathComplete) return;
            if (!LocationHistoryUITool.GetIsPlaying()) return;
            if (isNeedHide)
            {
                isNeedHide = false;
                Hide();
                return;
            }
            float valuet = 1;
            valuet = LocationHistoryUITool.GetProcessSliderValue();
            if (valuet < 1)
            {
                //double timesum = MultHistoryPlayUI.Instance.timeSum;
                //DateTime showPointTime = MultHistoryPlayUI.Instance.GetStartTime().AddSeconds(timesum);
                MultHistoryTimeStamp timeStamp = LocationHistoryUITool.GetTimeStamp();
                double timesum = timeStamp.timeSum;
                DateTime showPointTime= timeStamp.showPointTime;
                float currentSpeedT = timeStamp.currentSpeed;
                if (currentPointIndex < PosCount && currentPointIndex > -1)
                {
                    //Debug.LogErrorFormat("timelist[currentPointIndex]:{0},showPointTime:{1}", timelist[currentPointIndex], showPointTime);
                    if (PosInfoList[currentPointIndex].Time < showPointTime)
                    {
                        //double timesum2 = (timelist[currentPointIndex] - HistoryPlayUI.Instance.GetStartTime()).TotalSeconds;
                        //Debug.Log("timesum2:" + timesum2);
                        //progressTargetValue = (double)timesum2 / HistoryPlayUI.Instance.timeLength;
                        //if (MultHistoryPlayUI.Instance.isNewWalkPath)
                        //{
                        if (currentPointIndex - 1 >= 0)
                        {
                            double temp = PosInfoList.GetTimeSpane(currentPointIndex);
                            if (temp > 2f)
                            {
                                //ExcuteHistoryPath(currentPointIndex,1f, false);
                                ExcuteHistoryPath(currentPointIndex, currentSpeedT);
                                currentPointIndex++;
                                return;
                            }
                            else
                            {
                                ExcuteHistoryPath(currentPointIndex, currentSpeedT);
                            }
                        }
                        else
                        {
                            ExcuteHistoryPath(currentPointIndex, currentSpeedT);
                        }
                        //}
                        //else
                        //{
                        //    ExcuteHistoryPath(currentPointIndex, MultHistoryPlayUI.Instance.CurrentSpeed);
                        //}
                        //Debug.LogError("currentPointIndex111:" + currentPointIndex);
                        currentPointIndex++;
                        Show();
                    }
                    else//如果当前点的时间超过了当前进度时间
                    {
                        //if (currentPointIndex - 1 >= 0)
                        //{
                        //    ExcuteHistoryPath(currentPointIndex - 1, MultHistoryPlayUI.Instance.CurrentSpeed);
                        //}

                        ExcuteHistoryPath(currentPointIndex, currentSpeedT);

                        //Debug.LogError("currentPointIndex222:" + currentPointIndex);
                        //if (MultHistoryPlayUI.Instance.isNewWalkPath)
                        //{
                        if (currentPointIndex - 1 >= 0)
                        {
                            double temp = (PosInfoList[currentPointIndex].Time - PosInfoList[currentPointIndex - 1].Time).TotalSeconds;
                            if (temp > 2f)//如果当前要执行历史点的值，超过播放时间值5秒，就认为这超过5秒时间里，没历史轨迹数据，则让人员消失
                            {
                                //Hide();
                            }
                        }
                        else
                        {
                            //Hide();
                        }
                        //}
                        //else
                        //{
                        //    double temp = (timelist[currentPointIndex] - showPointTime).TotalSeconds;
                        //    if (temp > 5f)//如果当前要执行历史点的值，超过播放时间值5秒，就认为这超过5秒时间里，没历史轨迹数据，则让人员消失
                        //    {
                        //        Hide();
                        //    }
                        //}
                    }
                    //progressValue = Mathf.Lerp((float)progressValue, (float)progressTargetValue, 2 * Time.deltaTime);
                    //transform.position = line.GetPoint3D01((float)progressValue);
                    //ExcuteHistoryPath(currentPointIndex);
                }
                else
                {
                    Hide();
                }
            }
            else
            {
                progressValue = 0;
                progressTargetValue = 0;
                currentPointIndex = 0;
            }

        }
        //else
        //{
            ShowArea();
        //}

        if (depnode != null)
        {
            if (LocationHistoryManager.Instance&& LocationHistoryManager.Instance.IsFocus)//对焦状态时才可以加载设备
            {
                BuildingController buillding = depnode.GetParentNode<BuildingController>();
                if (buillding && !buillding.IsDevCreate)//加载设备
                {
                    buillding.IsDevCreate = true;
                    buillding.LoadDevices(() =>
                    {
                        RoomFactory.Instance.CreateDepDev(buillding, true);//todo:是否可以不加载动态设备
                    });
                }
            }
            
        }
    }

    public void SetCurrentPointIndex(int indexT)
    {
        currentPointIndex = indexT;
    }

    public HistoryManController historyManController;

    protected override void StartInit()
    {
        lines = new List<VectorLine>();
        dottedlines = new List<VectorLine>();
        CreatePathParent();
        //LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        transform.SetParent(pathParent);
        if (PosCount <= 1) return;
        render = gameObject.GetComponent<Renderer>();
        renders = gameObject.GetComponentsInChildren<Renderer>();
        collider = gameObject.GetComponent<Collider>();

        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, new Vector3(0, 0.1f, 0));
        followUI = UGUIFollowManage.Instance.CreateItem(LocationHistoryManager.Instance.NameUIPrefab, targetTagObj, "LocationNameUI", null, true);
        Text nametxt = followUI.GetComponentInChildren<Text>();
        nametxt.text = name;
        if (historyManController)
        {
            historyManController.SetFollowUI(followUI);
        }

        GroupingLine();


        historyPathDrawing = gameObject.AddMissingComponent<HistoryPathDrawing>();
        historyPathDrawing.Init(pathParent, color);
        historyPathDrawing.PauseDraw();
    }

    /// <summary>
    /// 获取离它最近的下一个播放点
    /// </summary>
    public override int GetNextPoint(float value)
    {
        double f = timeLength * value;
        //DateTime startTimeT = MultHistoryPlayUI.Instance.GetStartTime();
        DateTime startTimeT=LocationHistoryUITool.GetStartTime();
        //相匹配的第一个元素,结果为-1表示没找到
        return PosInfoList.FindIndex((item) =>
        {
            double timeT = (item.Time - startTimeT).TotalSeconds;
            if (timeT > f)
            {
                return true;
            }
            else
            {
                return false;
            }
        });

    }


    /// <summary>
    /// 根据进度时间值，获取当前需要执行的点的索引
    /// </summary>
    /// <param name="f"></param>
    /// <param name="accuracy">精确度：时间相差accuracy秒</param>
    public override int GetCompareTime(double f, float accuracy = 0.1f)
    {
        DateTime startTimeT = LocationHistoryUITool.GetStartTime();
        ////相匹配的第一个元素,结果为-1表示没找到
        //return PosInfoList.FindIndex((item) =>
        //{
        //    double timeT = (item.Time - startTimeT).TotalSeconds;
        //    if (Math.Abs(f - timeT) < accuracy)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //});

        return PosInfoList.FindIndexByTime(startTimeT, f, accuracy);
    }

    /// <summary>
    /// 计算历史轨迹人员的所在区域
    /// </summary>
    public void ShowArea()
    {
        //List<Position> ps = MultHistoryPlayUI.Instance.GetPositionsByPersonnel(personnel);

        List<Position> ps = LocationHistoryUITool.GetPositionsByPersonnel(personnel);

        if (ps != null)
        {
            if (currentPointIndex < ps.Count && currentPointIndex > -1)
            {
                Position p = ps[currentPointIndex];
                DepNode depnodeT = RoomFactory.Instance.GetDepNodeById((int)p.TopoNodeId);
                DepNode depnodePri = MonitorRangeManager.GetDepNodeBuild(depnode);
                DepNode depnodeNow = MonitorRangeManager.GetDepNodeBuild(depnodeT);

                if (depnodeNow != null && depnodePri != depnodeNow && LocationHistoryManager.Instance.CurrentFocusController == historyManController)
                {

                    BuildingBox box = depnodeNow.GetComponent<BuildingBox>();
                    if (box)
                    {
                        box.LoadBuilding((nNode) =>
                        {
                            FactoryDepManager.Instance.SetAllColliderIgnoreRaycastOP(true);
                        }, false);
                        //LocationManager.Instance.TransparentPark();
                        //return;
                    }
                }

                depnode = depnodeT;
                if (depnode)
                {
                    LocationHistoryUITool.SetItemArea(personnel,depnode.NodeName);
                }
            }
        }
    }

    public override void Hide()
    {
        if (LocationHistoryManager.Instance.CurrentFocusController == historyManController)
        {
            //LocationHistoryManager.Instance.RecoverBeforeFocusAlign();
            if (historyManController.followUIbtn.ison)
            {
                historyManController.followUIbtn.CodeToClick();
            }
        }
        historyManController.FlashingOffArchors();
        base.Hide();


    }

    public override void SetRenderIsEnable(bool isEnable)
    {
        //base.SetRenderIsEnable(isEnable);
        if (IsShowRenderer != isEnable)
        {
            foreach (Renderer render in renders)
            {
                render.enabled = isEnable;
            }
            bool isMouseDragSliderT = LocationHistoryUITool.GetIsMouseDragSlider();
            if (!isMouseDragSliderT)
            {
                bool isDrawingMode = LocationHistoryUITool.GetIsDrawingMode();

                //if (MultHistoryPlayUI.Instance.mode == MultHistoryPlayUI.Mode.Drawing)
                if (isDrawingMode)
                {
                    if (isEnable)
                    {
                        historyPathDrawing.Drawing();
                        historyPathDrawing.AddLine();
                    }
                    else
                    {
                        historyPathDrawing.PauseDraw();
                    }
                }
            }
            IsShowRenderer = isEnable;
        }
    }

    //public void SetRenderIsEnableOP(bool isEnable)
    //{
    //    //base.SetRenderIsEnable(isEnable);
    //    if (IsShowRenderer != isEnable)
    //    {
    //        foreach (Renderer render in renders)
    //        {
    //            render.enabled = isEnable;
    //        }

    //        IsShowRenderer = isEnable;
    //    }
    //}

    private void OnDisable()
    {
        SetFollowUI(false);
    }

    /// <summary>
    /// 设置某条路径的是否启用
    /// </summary>
    /// <param name="patht"></param>
    public void SetPathEnable(bool isEnabled,bool isShow)
    {
        this.historyPathDrawing.lineContent.gameObject.SetActive(isEnabled);
        this.lineParent.gameObject.SetActive(isEnabled);
        this.SetIsCanShowPerson(isEnabled);
        if (isEnabled && isShow)
        {
            this.Show();
        }
        else
        {
            this.Hide();
        }
    }
}
