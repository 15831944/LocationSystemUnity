using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 历史轨迹集合
/// </summary>
public class LocationHistoryPathList
{
    /// <summary>
    /// LocationHistoryPath后续要去掉
    /// </summary>
    public List<LocationHistoryPath> ItemsOld = new List<LocationHistoryPath>();

    public List<LocationHistoryPath_M> Items= new List<LocationHistoryPath_M>();

    public LocationHistoryPath_M this[int index]
    {
        get { return Items[index]; }
        set { Items[index] = value; }
    }

    public int Count
    {
        get { return Items.Count; }
    }

    public LocationHistoryPathList()
    {
        
    }

    public void SetRateChanged(bool isChanged)
    {
        foreach (var path in Items)
        {
            path.SetRateChanged(isChanged);
        }
    }

    /// <summary>
    /// 刷新轨迹
    /// </summary>
    public void RefleshDrawLine(MonoBehaviour behaviour)
    {
        foreach (LocationHistoryPath path in ItemsOld)
        {
            //path.RefleshDrawLineOP();
            behaviour.StartCoroutine(path.RefleshDrawLineOP());
        }

        foreach (LocationHistoryPath_M path in Items)
        {
            //path.RefleshDrawLineOP();
            behaviour.StartCoroutine(path.RefleshDrawLineOP());
        }
    }

    /// <summary>
    /// 设置显示隐藏
    /// </summary>
    public void SetLinesActive(bool isActive)
    {
        foreach (LocationHistoryPath path in ItemsOld)
        {
            path.SetLinesActive(isActive);
        }

        foreach (LocationHistoryPath_M path in Items)
        {
            path.SetLinesActive(isActive);
        }
    }

    /// <summary>
    /// 清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths()
    {
        foreach (LocationHistoryPath path in ItemsOld)
        {
            GameObject.DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
            //DestroyImmediate(path.gameObject);
        }

        ItemsOld.Clear();
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath(float v)
    {
        foreach (LocationHistoryPath hispath in ItemsOld)
        {
            hispath.Set(v);
        }
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void Add(LocationHistoryPath path)
    {
        ItemsOld.Add(path);
    }

    public PositionInfo SetCurrentIndex(int index)
    {
        PositionInfo posInfo = null;
        if (Items != null)
        {
            Items.ForEach(path => posInfo = path.MoveToCurrentIndex(index));
        }
        return posInfo;
    }

    /// <summary>
    /// 是否不存在任何路径
    /// </summary>
    /// <returns></returns>
    public bool IsPathEmpty()
    {
        return Items == null || Items.Count == 0;
    }

    public string GetCurrentPercent()
    {
        if (IsPathEmpty())
        {
            return "--";
        }
        else
        {
            var index = Items[0].currentPointIndex;
            var count = Items[0].PosCount;
            return string.Format("{0}({1}/{2})", (index + 0.0f) / count, index, count);
        }
    }
    public int GetCurrentIndex()
    {
        if (IsPathEmpty())
        {
            return -1;
        }
        else
        {
            return Items[0].currentPointIndex;
        }
    }

    public LocationHistoryPath_M GetCurrentPath()
    {
        if (IsPathEmpty())
        {
            return null;
        }
        else
        {
            return Items[0];
        }
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void Add(LocationHistoryPath_M path)
    {
        if (!Items.Contains(path))
        {
            Items.Add(path);
        }
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath_M(float v)
    {
        foreach (LocationHistoryPath_M hispath in Items)
        {
            hispath.Set(v);
        }
    }

    /// <summary>
    /// 多人：清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths_M()
    {
        foreach (LocationHistoryPath_M path in Items)
        {
            GameObject.DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
        }

        Items.Clear();
    }

    /// <summary>
    /// 是否开启历史轨迹实时绘制
    /// </summary>
    public void SetHistoryLineDrawing(HistoryMode mode,bool isDrawing, bool isAddLine = false)
    {
        //Log.Info("SetHistoryLineDrawing", string.Format(""));
        foreach (LocationHistoryPath_M h in Items)
        {
            if (isAddLine)
            {
                h.historyPathDrawing.AddLine();
            }
            if (isDrawing)
            {
                if (mode == HistoryMode.Drawing)
                {
                    h.historyPathDrawing.Drawing();
                    //if (isAddLine)
                    //{
                    //    h.historyPathDrawing.AddLine();
                    //}
                }
            }
            else
            {

                h.historyPathDrawing.PauseDraw();
            }
        }

    }

    public void Hide()
    {
        foreach (LocationHistoryPath_M patht in Items)
        {
            patht.Hide();
        }
    }

    /// <summary>
    /// 设置历史轨迹显示隐藏
    /// </summary>
    public void SetHistoryPathLines(bool b)
    {
        foreach (LocationHistoryPath_M patht in Items)
        {
            Transform t = patht.transform.parent.Find("HistoryLines");
            int count = t.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                t.transform.GetChild(i).gameObject.SetActive(b);
            }
            if (patht.PosCount > 0)
            {
                patht.SetFisrtPos();
                patht.SetCurrentPointIndex(0);
            }

            patht.ClearPreviousInfo();
        }
    }

    /// <summary>
    /// 清除实时绘制的轨迹
    /// </summary>
    public void ClearDrawingLines()
    {
        foreach (LocationHistoryPath_M patht in Items)
        {
            Transform t = patht.transform.parent.Find("DrawingLines");
            int count = t.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(t.transform.GetChild(i).gameObject);
            }
            patht.historyPathDrawing.lines.Clear();
            //if (patht.splinePoints.Count > 0)
            //{
            //    patht.transform.position = patht.splinePoints[0];
            //    patht.SetCurrentPointIndex(0);
            //}

            //patht.ClearPreviousInfo();
        }
    }

    public void SetPathEnable(bool isEnable, bool isShow)
    {
        foreach (LocationHistoryPath_M patht in Items)
        {
            patht.SetPathEnable(isEnable, isShow);
        }
    }

    public void SetPathEnable(int ptId, bool isShow)
    {
        foreach (LocationHistoryPath_M patht in Items)
        {
            if (ptId == patht.personnel.Id)
            {
                patht.SetPathEnable(true, isShow);
            }
            else
            {
                patht.SetPathEnable(false, isShow);
            }
        }
    }


}