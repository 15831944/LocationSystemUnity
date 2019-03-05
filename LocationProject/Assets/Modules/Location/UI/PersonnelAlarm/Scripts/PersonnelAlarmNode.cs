using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelAlarmNode : MonoBehaviour
{

    
   
    /// <summary>
    /// 非人员告警按钮
    /// </summary>
    public Toggle PersonnelToggle;
    /// <summary>
    /// 人员告警Toggle
    /// </summary>
    public Toggle AlarmToggle;
    private PersonInfoUI personInfoUI;
    
    public PersonInfoUI PersonInfoUI
    {
        get
        {
            if (personInfoUI == null)
            {
                personInfoUI = gameObject.GetComponentInParent<PersonInfoUI>();
            }
            return personInfoUI;
        }

        set
        {
            personInfoUI = value;
        }
    }


    
    /// <summary>
    /// 展示人员告警界面
    /// </summary>
    public void ShowPersonnelAlarm(bool ison)
    {
        if (ison)
        {
            if (PersonInfoUI.personnel != null)
            {
                //PersonInfoUI.SetContentGridActive(true);
                //LocationManager.Instance.FocusPersonAndShowInfo(PersonInfoUI.personnel.Id);
                PersonnelTreeManage.Instance.departmentDivideTree.Tree.SelectNodeByData(PersonInfoUI.personnel.TagId);
                //PersonNode nodeT = PersonnelTreeManage.Instance.PersonnelToPersonNode(PersonInfoUI.personnel);
                PersonnelTreeManage.Instance.areaDivideTree.Tree.SelectNodeByType(PersonInfoUI.personnel.TagId);//PersonNode.Id==Personnel.Id
            }
            // AlarmTweener.PlayForward();
            //window.SetActive(true);
            //   PersonnelToggle.isOn = true;
            //StartOutManage.Instance.SetUpperStoryButtonActive(false);
        }
        else
        {
            PersonInfoUI.SetContentGridActive(false);
           // AlarmTweener.PlayBackwards();
            //window.SetActive(false);
          //  PersonnelToggle.isOn = false;
            //StartOutManage.Instance.SetUpperStoryButtonActive(true);
        }
    }


    void Start()
    {
        PersonInfoUI = gameObject.GetComponentInParent<PersonInfoUI>();

        AlarmToggle.onValueChanged.AddListener(ShowPersonnelAlarm);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
