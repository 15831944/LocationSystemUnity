using Location.WCFServiceReferences.LocationServices;
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
    void Start()
    {

    }
    public void ShowPersonnelDetailInfo(Personnel per, List<Personnel> peraonnelList, List<LocationObject> listT)
    {
        LocationObject locationObjectT = listT.Find((item) => item.personnel.TagId == per.TagId);
        nameT.text = per.Name;
        sex.text = per.Sex;

        if (string.IsNullOrEmpty(per.WorkNumber))
        {
            workNumber.text = "--";
        }
        else
        {
            workNumber.text = per.WorkNumber;
        }
        if (string.IsNullOrEmpty(per.Pst))
        {
            post.text = "--";
        }
        else
        {
            post.text = per.Pst;
        }
        if (per.Parent != null)
        {

            department.text = per.Parent.Name;
        }
        else
        {
            Debug.Log(per);
            department.text = "--";

        }

        if (per.Tag != null)
        {
            tagName.text = per.Tag.Name;
            tagNum.text = per.Tag.Code;//编号不是Id是code
        }
        else
        {
            tagName.text = "--";
            tagNum.text = "--";
        }
        if (per.AreaName != null)
        {
            area.text = per.AreaName.ToString();
        }
        else
        {
            area.text = "--";
        }

        if (string.IsNullOrEmpty(per.PhoneNumber))
        {
            phone.text = "--";
        }
        else
        {
            phone.text = per.PhoneNumber.ToString();
        }
        if (locationObjectT == null)
        {
            standbyTime.text = "--";
        }
        else
        {
            standbyTime.text = locationObjectT.personInfoUI.infoStandbyTime.text;
        }

        LocationBut.onClick.AddListener(() =>
       {
           if (per.Tag == null)
           {
               DataPaging.Instance.IsGetPersonData = false;
               return;
           }
           PersonnelBut_Click(per.Tag.Id);
       });
        DetailBut.onClick.AddListener(() =>
      {
          EditPersonnelInfo(per.Id, peraonnelList);

      });
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
            DeleteBut.onClick.AddListener(()=> 
            {
                DeletePersonnelInfo(per.Id );
            });
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
        DataPaging.Instance.SaveSelection();
        DataPaging.Instance.IsGetPersonData = false;
        EditPersonnelInformation.Instance.GetPersonnelInformation(perID, peraonnelList);
        EditPersonnelInformation.Instance.ShowAndCloseEditPersonnelInfo(true);
        DataPaging.Instance.personnelSearchUI.SetActive(false);
    }
    /// <summary>
    /// 删除人员
    /// </summary>
    /// <param name="id"></param>
    public void DeletePersonnelInfo(int id)
    {
        UGUIMessageBox.Show("确定删除该人员？",
  () =>
  {
      bool IsSuccessful = CommunicationObject.Instance.DeletePerson(id);
      if (IsSuccessful)
      {
          DataPaging.Instance.StartPerSearchUI();
          DataPaging.Instance.ShowpersonnelSearchWindow();
      }
      else
      {
          UGUIMessageBox.Show("删除人员失败！", null, null);
      }

  }, null);
    }
    void Update()
    {

    }
}
