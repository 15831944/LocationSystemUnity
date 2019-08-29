using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class LocationHistoryUITool
{
    public static bool GetIsNormalMode()
    {
        bool isNormalMode;
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            isNormalMode = MultHistoryPlayUINew.Instance.mode == HistoryMode.Normal;
        }
        else
        {
            isNormalMode = MultHistoryPlayUI.Instance.mode == HistoryMode.Normal;
        }
        return isNormalMode;
    }

    public static bool GetIsDrawingMode()
    {
        bool isDrawingMode;
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            isDrawingMode = MultHistoryPlayUINew.Instance.mode == HistoryMode.Drawing;
        }
        else
        {
            isDrawingMode = MultHistoryPlayUI.Instance.mode == HistoryMode.Drawing;
        }

        return isDrawingMode;
    }

    public static bool GetIsMouseDragSlider()
    {
        bool isMouseDragSliderT;
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            isMouseDragSliderT = MultHistoryPlayUINew.Instance.IsMouseDragSlider;
        }
        else
        {
            isMouseDragSliderT = MultHistoryPlayUI.Instance.IsMouseDragSlider;
        }

        return isMouseDragSliderT;
    }

    /// <summary>
    /// 是否正在播放
    /// </summary>
    /// <returns></returns>
    public static bool GetIsPlaying()
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            return MultHistoryPlayUINew.Instance.isPlay;
        }
        else
        {
            return MultHistoryPlayUI.Instance.isPlay;
        }
    }
    /// <summary>
    /// 停止播放
    /// </summary>
    /// <param name="personnel"></param>
    public static void StopPlay(Personnel personnel)
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            //如果在播放就让它终止
            //ExecuteEvents.Execute<IPointerClickHandler>(MultHistoryPlayUINew.Instance.StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            MultHistoryPlayUINew.Instance.RemovePerson(personnel);
            MultHistoryPlayUINew.Instance.Stop();
        }
        else
        {
            //如果在播放就让它终止
            ExecuteEvents.Execute<IPointerClickHandler>(MultHistoryPlayUI.Instance.StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            MultHistoryPlayUI.Instance.RemovePerson(personnel);
        }
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    public static void Show()
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            MultHistoryPlayUINew.Instance.ShowT();
        }
        else
        {
            MultHistoryPlayUI.Instance.ShowT();
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public static void Hide()
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            MultHistoryPlayUINew.Instance.Hide();
        }
        else
        {
            MultHistoryPlayUI.Instance.Hide();
        }
    }

    public static MultHistoryTimeStamp GetTimeStamp()
    {
        MultHistoryTimeStamp stamp = new MultHistoryTimeStamp();
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            stamp.timeSum = MultHistoryPlayUINew.Instance.timeSum;
            stamp.showPointTime = MultHistoryPlayUINew.Instance.GetStartTime().AddSeconds(stamp.timeSum);
            stamp.currentSpeed = MultHistoryPlayUINew.Instance.CurrentSpeed;
        }
        else
        {
            stamp.timeSum = MultHistoryPlayUI.Instance.timeSum;
            stamp.showPointTime = MultHistoryPlayUI.Instance.GetStartTime().AddSeconds(stamp.timeSum);
            stamp.currentSpeed = MultHistoryPlayUI.Instance.CurrentSpeed;
        }
        return stamp;
    }

    /// <summary>
    /// 获取进度条的值
    /// </summary>
    /// <returns></returns>
    public static float GetProcessSliderValue()
    {
        try
        {
            if (UIManage.Instance.isShowNewHistoryWindow)
            {
                return MultHistoryPlayUINew.Instance.processSlider.value;
            }
            else
            {
                return MultHistoryPlayUI.Instance.processSlider.value;
            }
        }catch(Exception e)
        {
            Log.Error("LocationHistoryUITool.GetProcessSliderValue.Exception:"+e.ToString());
            return 0;
        }
    }

    public static int GetLimitPersonNum()
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            return MultHistoryPlayUINew.Instance.limitPersonNum;
        }
        else
        {
            return MultHistoryPlayUI.Instance.limitPersonNum;
        }
    }
    public static List<Position> GetPositionsByPersonnel(Personnel p)
    {
        List<Position> ps = new List<Position>();
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            ps = MultHistoryPlayUINew.Instance.GetPositionsByPersonnel(p);
        }
        else
        {
            ps = MultHistoryPlayUI.Instance.GetPositionsByPersonnel(p);
        }

        return ps;
    }

    /// <summary>
    /// 获取开始时间
    /// </summary>
    /// <returns></returns>
    public static DateTime GetStartTime()
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            return MultHistoryPlayUINew.Instance.GetStartTime();
        }
        else
        {
            return MultHistoryPlayUI.Instance.GetStartTime();
        }
    }

    /// <summary>
    /// 设置人员所在区域名称
    /// </summary>
    /// <param name="personnel"></param>
    /// <param name="nodeName"></param>
    public static void SetItemArea(Personnel personnel, string nodeName)
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            MultHistoryPlayUINew.Instance.SetItemArea(personnel, nodeName);
        }
        else
        {
            MultHistoryPlayUI.Instance.SetItemArea(personnel, nodeName);
        }
    }


    public static void ShowPersons(List<Personnel> currentSelectPersonnelsT)
    {
        if (UIManage.Instance.isShowNewHistoryWindow)
        {
            MultHistoryPlayUINew.Instance.ShowPersons(currentSelectPersonnelsT);
        }
        else
        {
            MultHistoryPlayUI.Instance.ShowPersons(currentSelectPersonnelsT);
        }
    }

}