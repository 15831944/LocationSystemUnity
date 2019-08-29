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
                //PersonnelTreeManage.Instance.areaDivideTree.Tree.SelectNodeByType(PersonInfoUI.personnel.Id);//

                PersonnelTreeManage.Instance.SelectPerson(PersonInfoUI.personnel);
            }

            AlarmToggle.isOn = true;

        }
        else
        {
            PersonInfoUI.SetContentGridActive(false);
            AlarmToggle.isOn = false;
            PersonnelToggle.enabled = false;
            PersonnelToggle.enabled = true;//这两句代码用于刷新Toggle，不然存在选中恢复到不选中状态时UI图标没恢复过来，但是实际上已经恢复过来，只是需要刷新一下
            //PersonnelToggle.gameObject.SetActive(false);
            //PersonnelToggle.gameObject.SetActive(true);
            //LocationManager.Instance.HideCurrentPersonInfoUI();
            //if (LocationManager.Instance.IsFocus)
            //{
            //    LocationManager.Instance.RecoverBeforeFocusAlign();
            //}
        }
    }
}
