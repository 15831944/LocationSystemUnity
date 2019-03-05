using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelAlarmManage : MonoBehaviour {
    /// <summary>
    /// 人员告警时Toggle的图标
    /// </summary>
    public Transform OpenAlarm;
    /// <summary>
    /// 人员告警动画
    /// </summary>
    public Tweener AlarmTweener;
    Vector3 rotation = new Vector3(0, 0, -45);
    /// <summary>
    /// 人员告警的UI界面
    /// </summary>
    public GameObject window;
    /// <summary>
    /// 人员告警Toggle
    /// </summary>
    public Toggle AlarmToggle;
    /// <summary>
    /// 创建人员告警动画
    /// </summary>
    public void CreateSpreadTweener()
    {
        AlarmTweener = OpenAlarm.DORotate(rotation,0.24f);
        AlarmTweener.SetAutoKill(false);
        AlarmTweener.Pause();
        AlarmTweener.SetEase(Ease.InOutBack);
    }
    /// <summary>
    /// 展示人员告警界面
    /// </summary>
    public void ShowPersonnelAlarm()
    {
        if (AlarmToggle .isOn ==true)
        {
            AlarmTweener.PlayForward();
            //window.SetActive(true);
        }
        else
        {
            AlarmTweener.PlayBackwards();
             //window.SetActive(false );
        }
    }
   

    void Start () {
        CreateSpreadTweener();


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
