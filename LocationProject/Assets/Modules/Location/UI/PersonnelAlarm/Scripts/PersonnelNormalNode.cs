using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelNormalNode : MonoBehaviour
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

    ///// <summary>
    ///// 非人员告警界面
    ///// </summary>
    //public GameObject Window;
    //public CircleImage circleImage;
    void Start()
    {
        PersonInfoUI = gameObject.GetComponentInParent<PersonInfoUI>();

       
    }
    


    public void ShowPersonnelWindow()
    {
        if (PersonnelToggle.isOn == true)
        {
            if (PersonInfoUI == null) return;
            if (PersonInfoUI.personnel != null)
            {
                //PersonnelTreeManage.Instance.departmentDivideTree.Tree.SelectNodeByData(PersonInfoUI.personnel.Id);
                //PersonnelTreeManage.Instance.departmentDivideTree.Tree.SelectNodeByData(PersonInfoUI.personnel.TagId);
                //PersonnelTreeManage.Instance.areaDivideTree.Tree.SelectNodeByType(PersonInfoUI.personnel.TagId);//PersonNode.Id==Personnel.Id
                PersonnelTreeManage.Instance.departmentDivideTree.Tree.SelectNodeByData(PersonInfoUI.personnel.Id);
                PersonnelTreeManage.Instance.areaDivideTree.Tree.SelectNodeByType(PersonInfoUI.personnel.Id);//PersonNode.Id==Personnel.Id

            }

            AlarmToggle.isOn = true;

        }
        else
        {
            //ControlMenuController.Instance.SetMultHistoryToggle(true);
            PersonInfoUI.SetContentGridActive(false);
            //Window.SetActive(false);
           
            AlarmToggle.isOn = false;
            //StartOutManage.Instance.SetUpperStoryButtonActive(true);
        }
    }
}
