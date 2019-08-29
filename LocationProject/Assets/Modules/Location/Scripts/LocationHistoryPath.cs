using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

public class LocationHistoryPath : LocationHistoryPathBase
{


    protected override void Start()    {
        base.Start();
        LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        //timeStart = HistoryPlayUI.Instance.timeStart;
        Hide();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void Update()
    {
        base.Update();
        if (!isCreatePathComplete) return;
        //if (isSetHistoryPath) return;
        //if (line.drawEnd <= 0) return;
        if (HistoryPlayUI.Instance.isPlay == false) return;        if (currentPointIndex < 0) return;        if (HistoryPlayUI.Instance.processSlider.value < 1)
        {
            double timesum = HistoryPlayUI.Instance.timeSum;
            DateTime showPointTime = HistoryPlayUI.Instance.GetStartTime().AddSeconds(timesum);

            if (currentPointIndex < PosCount)
            {
                if (PosInfoList[currentPointIndex].Time < showPointTime)
                {
                    //double timesum2 = (timelist[currentPointIndex] - HistoryPlayUI.Instance.GetStartTime()).TotalSeconds;
                    //Debug.Log("timesum2:" + timesum2);
                    //progressTargetValue = (double)timesum2 / HistoryPlayUI.Instance.timeLength;
                    if (currentPointIndex - 1 >= 0)
                    {
                        double temp = (PosInfoList[currentPointIndex].Time - PosInfoList[currentPointIndex - 1].Time).TotalSeconds;
                        if (temp > 2f)//如果当前点的时间超过了上个点时间2秒，这中间就存在卡信号丢失问题，人立马移动到当前点，不需要缓动
                        {
                            //ExcuteHistoryPath(currentPointIndex,1f, false);
                            ExcuteHistoryPath(currentPointIndex, HistoryPlayUI.Instance.CurrentSpeed);
                            currentPointIndex++;
                            return;
                        }
                        else
                        {
                            ExcuteHistoryPath(currentPointIndex, HistoryPlayUI.Instance.CurrentSpeed);
                        }
                    }
                    else
                    {
                        ExcuteHistoryPath(currentPointIndex, HistoryPlayUI.Instance.CurrentSpeed);
                    }
                    //ExcuteHistoryPath(currentPointIndex);
                    currentPointIndex++;
                    Show();
                }
                else//如果当前点的时间超过了当前进度时间
                {
                    //if (currentPointIndex - 1 >= 0)
                    //{
                    //    ExcuteHistoryPath(currentPointIndex - 1);
                    //}

                    ExcuteHistoryPath(currentPointIndex, HistoryPlayUI.Instance.CurrentSpeed);

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

                    //double temp = (timelist[currentPointIndex] - showPointTime).TotalSeconds;
                    //if (temp > 5f)//如果当前要执行历史点的值，超过播放时间值5秒，就认为这超过5秒时间里，没历史轨迹数据，则让人员消失
                    //{
                    //    Hide();
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

        ShowArea();
    }


    public HistoryManController historyManController;

    protected override void StartInit()
    {
        lines = new List<VectorLine>();
        dottedlines = new List<VectorLine>();        CreatePathParent();        //LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        transform.SetParent(pathParent);        if (PosCount <= 1) return;
        render = gameObject.GetComponent<Renderer>();
        renders = gameObject.GetComponentsInChildren<Renderer>();
        collider = gameObject.GetComponent<Collider>();

        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, new Vector3(0, 0.1f, 0));
        followUI = UGUIFollowManage.Instance.CreateItem(LocationHistoryManager.Instance.NameUIPrefab, targetTagObj, "LocationNameUI",null,true);
        Text nametxt = followUI.GetComponentInChildren<Text>();
        nametxt.text = name;
        if (historyManController)
        {
            historyManController.SetFollowUI(followUI);
        }

        GroupingLine();
    }

    /// <summary>
    /// 获取离它最近的下一个播放点
    /// </summary>
    public override int GetNextPoint(float value)
    {
        DateTime startTimeT = HistoryPlayUI.Instance.GetStartTime();
        double f = timeLength * value;
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
    /// 根据进度值，获取当前需要执行的点的索引
    /// </summary>
    /// <param name="f"></param>
    /// <param name="accuracy">精确度：时间相差accuracy秒</param>
    public override int GetCompareTime(double f, float accuracy = 0.1f)
    {
        DateTime startTimeT = HistoryPlayUI.Instance.GetStartTime();
        //相匹配的第一个元素,结果为-1表示没找到
        return PosInfoList.FindIndexByTime(startTimeT, f, accuracy);
    }


    /// <summary>
    /// 计算历史轨迹人员的所在区域
    /// </summary>
    public void ShowArea()
    {
        List<Position> ps = HistoryPlayUI.Instance.GetPositions();
        if (ps != null)
        {
            int num = currentPointIndex;
            if (num < ps.Count)
            {
                Position p = ps[num];
                DepNode depnode = RoomFactory.Instance.GetDepNodeById((int)p.TopoNodeId);
                if (depnode != null)
                {
                    HistoryPlayUI.Instance.SetItemArea(depnode.NodeName);
                }
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
        historyManController.FlashingOffArchors();
    }
}
