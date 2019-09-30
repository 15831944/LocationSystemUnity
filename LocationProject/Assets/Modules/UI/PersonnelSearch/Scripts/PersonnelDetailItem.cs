using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelDetailItem : MonoBehaviour
{

    public Text nameT;
    public Text sex;
    public Text workNumber;
    public Text post;
    public Text department;
    public Text area;
    public Text tagNum;
    public Text tagName;
    public Text standbyTime;//待机时间
    public Text phone;
    public Button LocationBut;
    public Button DetailBut;
    public Button DeleteBut;
    string InputKey;
    int Value = 0;
    Personnel CurrentPer;
    List<Personnel> PeraonnelList;
    GridLayoutGroup Grid;
    string pagePer;
    void Start()
    {
        LocationBut.onClick.AddListener(() =>
        {
            if (CurrentPer.Tag == null)
            {
                DataPaging.Instance.IsGetPersonData = false;
                return;
            }
            PersonnelBut_Click(CurrentPer.Tag.Id);
        });
        DetailBut.onClick.AddListener(() =>
        {
            EditPersonnelInfo(CurrentPer.Id, PeraonnelList);

        });
        DeleteBut.onClick.AddListener(() =>
        {
            DeletePersonnelInfo(CurrentPer.Id, CurrentPer);
        });

    }
    public void ShowPersonnelDetailInfo(Personnel per, List<Personnel> peraonnelList, List<LocationObject> listT, GridLayoutGroup grid)
    {
        Grid = grid;
        PersonnelType();
        CurrentPer = new Personnel();
        CurrentPer = per;
        PeraonnelList = new List<Personnel>();
        PeraonnelList.AddRange(peraonnelList);
        LocationObject locationObjectT = listT.Find((item) => item.personnel.TagId == per.TagId);
        nameT.text = per.Name.Trim();
        sex.text = tempNames[per.TargetType];
        if (string.IsNullOrEmpty(per.WorkNumber))
        {
            workNumber.text = "--";
        }
        else
        {
            workNumber.text = per.WorkNumber.Trim();
        }
        if (string.IsNullOrEmpty(per.Pst))
        {
            post.text = "--";
        }
        else
        {
            post.text = per.Pst.Trim();
        }
        if (per.Parent != null)
        {

            department.text = per.Parent.Name.Trim();
        }
        else
        {
            Debug.Log(per);
            department.text = "--";

        }

        if (per.Tag != null)
        {
            tagName.text = per.Tag.Name.Trim();
            tagNum.text = per.Tag.Code.Trim();//编号不是Id是code
        }
        else
        {
            tagName.text = "--";
            tagNum.text = "--";
        }
        if (per.AreaName != null)
        {
            area.text = per.AreaName.ToString().Trim();
        }
        else
        {
            area.text = "--";
        }
        string phoneNum = string.IsNullOrEmpty(per.PhoneNumber) ? per.Mobile : per.PhoneNumber;
        phone.text = string.IsNullOrEmpty(phoneNum) ? "--" : phoneNum;
        if (locationObjectT == null || per.Tag == null)
        {
            standbyTime.text = "--";
        }
        else
        {
            if (locationObjectT.personInfoUI.infoStandbyTime.gameObject.activeSelf)
            {
                standbyTime.text = locationObjectT.personInfoUI.infoStandbyTime.text.Trim();
            }
            else
            {
                standbyTime.text = "--";
            }
        }

        JudgePersonnelLocation(per, locationObjectT);
    }
    public void JudgePersonnelLocation(Personnel per, LocationObject Location)
    {
        if (per.Tag == null || Location == null)
        {

            LocationBut.GetComponent<Button>().interactable = false;
            Color noTag = new Color(109 / 255f, 236 / 255f, 254 / 255f, 52 / 255f);
            LocationBut.GetComponent<Image>().color = noTag;
            DeleteBut.gameObject.SetActive(true);
            DeleteBut.GetComponent<Button>().interactable = true;

        }
        else
        {
            if (per.Tag != null && Location != null)
            {
                Color NormalTag = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
                LocationBut.GetComponent<Image>().color = NormalTag;
                LocationBut.GetComponent<Button>().interactable = true;
                DeleteBut.gameObject.SetActive(false);
                DeleteBut.GetComponent<Button>().interactable = false;
            }

        }
    }
    /// <summary>
    /// 人员定位
    /// </summary>
    public void PersonnelBut_Click(int tagId)
    {
        DataPaging.Instance.SaveSelection();
        ParkInformationManage.Instance.ShowParkInfoUI(false);
        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
        LocationManager.Instance.FocusPersonAndShowInfo(tagId);
        DataPaging.Instance.personnelSearchUI.SetActive(false);
        PersonSubsystemManage.Instance.SearchToggle.isOn = false;
    }
    public void EditPersonnelInfo(int perID, List<Personnel> peraonnelList)
    {
        RecordScreeningCondition();
        //  DataPaging.Instance.SaveSelection();
        //  DataPaging.Instance.IsGetPersonData = false;
        EditPersonnelInformation.Instance.GetPersonnelInformation(perID, peraonnelList, InputKey, Value, Grid, this.gameObject);
        EditPersonnelInformation.Instance.ShowAndCloseEditPersonnelInfo(true);
        DataPaging.Instance.ShowAndClosePersonnelWindow(false);
    }
    public void RecordScreeningCondition()
    {
        InputKey = DataPaging.Instance.Key;
        Value = DataPaging.Instance.Level;
        pagePer = DataPaging.Instance.pegeNumText.text;
    }
    /// <summary>
    /// 删除人员
    /// </summary>
    /// <param name="id"></param>
    public void DeletePersonnelInfo(int id, Personnel per)
    {
        RecordScreeningCondition();
        UGUIMessageBox.Show("确定删除该人员？",
  () =>
  {
      bool IsSuccessful = CommunicationObject.Instance.DeletePerson(id);
      if (IsSuccessful)
      {
          DataPaging.Instance.peraonnelData.RemoveAll(item => item.Id == per .Id);
          DataPaging.Instance.selectedItem.RemoveAll(item => item.Id == per.Id);
          DataPaging.Instance.ShowPersonnelInfo();
          DataPaging.Instance.ShowpersonnelSearchWindow();
          DataPaging.Instance.PerSelected.text = InputKey;
          DataPaging.Instance.Level = Value;
          DataPaging.Instance.personnelDropdown.PerDropdown.value = Value;
          DataPaging.Instance.personnelDropdown.PerDropdown.captionText.text = DataPaging.Instance.personnelDropdown.devTyprList[Value];

          double pageNum = Math.Ceiling((double)(DataPaging.Instance.selectedItem.Count) / 10);
          if (int.Parse(pagePer)>pageNum  && DataPaging.Instance.selectedItem.Count != 0)
          {
              DataPaging.Instance.pegeNumText.text = pageNum.ToString();
          }
          else if (DataPaging.Instance.selectedItem.Count == 0)
          {

              DataPaging.Instance.pegeNumText.text = "1";

          }
          else
          {
              DataPaging.Instance.pegeNumText.text = pagePer;
          }

      }
      else
      {
          UGUIMessageBox.Show("删除人员失败！", null, null);
      }

  }, null);
    }
    public List<string> tempNames;
    public void PersonnelType()
    {
        tempNames = new List<string>();
        tempNames.Add("男性");
        tempNames.Add("女性");
        tempNames.Add("机器人");
        tempNames.Add("车辆");
        tempNames.Add("物资");
    }
    void Update()
    {

    }
}
