using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPersonnelRoleInfo : MonoBehaviour {
    public static EditPersonnelRoleInfo Instance;
    public InputField InputName;
    public InputField InputDetail;
    CardRole EditCardRole;
    public Button SaveBut;
    public Button CancelBut;
    public Text IdText;
    public Button CloseBut;
    public GameObject EditRoleWindow;
    void Start () {
        Instance = this;

        SaveBut. onClick.RemoveAllListeners();
        SaveBut.onClick.AddListener(() =>
        {
            SaverCardRoleData();
        });
        CancelBut. onClick.RemoveAllListeners();
        CancelBut.onClick.AddListener(() =>
        {
            CloseEditPersonnelRoleWindow();
            CloseCurrentWindowAndShowOtherWindow();
        });
        CloseBut. onClick.RemoveAllListeners();
        CloseBut.onClick.AddListener(() =>
        {
            CloseEditPersonnelRoleWindow();
            CloseCurrentWindowAndShowOtherWindow();
        });
    }
    public void CloseCurrentWindowAndShowOtherWindow()
    {
        RoleNameList.Instance.ShowRoleWindow();
        RoleNameList.Instance.GetCardRoleData(()=> {

           RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
          //  RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
        });
        
    }
	public void GetEditPersonnelRoleInfo(CardRole info)
    {
        EditCardRole = new CardRole();
        EditCardRole = info;
        IdText.text = "<color=#60D4E4FF>Id：</color>" + info.Id.ToString();
        InputName.text = info.Name;
        InputDetail.text = info.Description;


    }
    public void SaverCardRoleData()
    {
        EditCardRole.Name = InputName.text;
        EditCardRole.Description = InputDetail.text;
        if (string.IsNullOrEmpty(InputDetail.text)||string .IsNullOrEmpty (InputName.text))
        {
            UGUIMessageBox.Show("角色缺少必填信息，请补充完整在进行提交！", "确定", "",null, null,null );
        }
        else
        {
         bool IsSuccessful =  CommunicationObject.Instance.EditCardRole(EditCardRole);
            if (IsSuccessful)
            {
                UGUIMessageBox.Show("新建角色成功！", "确定", "", ()=>
                {
                    CloseEditPersonnelRoleWindow();
                    CloseCurrentWindowAndShowOtherWindow();
                }, null, null);
            }
            else
            {
                UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
            }
        }
        
    }
    public void ShowEditPersonnelRoleWindow()
    {
        EditRoleWindow.SetActive(true);
    }
    public void CloseEditPersonnelRoleWindow()
    {
        EditRoleWindow.SetActive(false );
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            InputName.GetComponent<InputField>().interactable = false;
            InputDetail.GetComponent<InputField>().interactable = false;
            SaveBut.GetComponent<Button>().interactable = false;
            CancelBut.GetComponent<Button>().interactable = false;

        }
        else
        {
            InputName.GetComponent<InputField>().interactable = true;
            InputDetail.GetComponent<InputField>().interactable = true;
            SaveBut.GetComponent<Button>().interactable = true;
            CancelBut.GetComponent<Button>().interactable = true;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
