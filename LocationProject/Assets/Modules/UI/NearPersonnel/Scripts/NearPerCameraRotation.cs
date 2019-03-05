using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NearPerCameraRotation : MonoBehaviour {
    int devId;
    public GameObject EnterObj;
  //  public GameObject ExitObj;

    public GameObject CameraExample;
    public NearPersonnelCameraInfo nearPersonnelCameraInfo;

    public Toggle  CaneraPoint;
     public GameObject selectCam;

    string camName;
    Color  EnterColor =new  Color(0, 0, 0, 0);
    Color ExitColor= new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
    
    void Start () {
        ClickChangeImage(CameraExample);
        CaneraPoint.onValueChanged.AddListener(CamClick_Toggle);
    }
   public void ClickChangeImage(GameObject obj)
    {
        EventTriggerListener objColor = EventTriggerListener.Get(obj);
        objColor.onEnter = EnterBut;
        objColor.onExit = ExitBut;
    }
    public void EnterBut(GameObject obj)
    {
        obj.transform .GetChild (0).GetComponent <Image >().color = ExitColor;
        obj.transform.GetChild(1).GetComponent<Image>().color = EnterColor;
        obj.transform.GetChild(2).gameObject.SetActive(true);

        obj.transform.GetChild(2).GetChild (0).GetComponent<Text>().text = camName;
    }
    public void ExitBut(GameObject obj)
    {
        obj.transform.GetChild(0).GetComponent<Image>().color = EnterColor;
        obj.transform.GetChild(1).GetComponent<Image>().color = ExitColor;
        obj.transform.GetChild(2).gameObject.SetActive(false );

        
    }
    public void GetNearPersonnelCamInfo(NearbyDev devList, int total, int i)
    {
       
        camName = devList.TypeName .ToString();
        
        this.transform.GetComponent<RectTransform >().anchoredPosition3D  = new Vector3(devList.X, devList.Z , devList.Y );
        devId = devList.id;
        RoomFactory.Instance.GetDevByid(devId, (devNodeT) =>
        {
            GameObject Obj = devNodeT.gameObject;
            EnterObj.transform.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0 , Obj.transform.eulerAngles.y +180);
            selectCam.transform.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, Obj.transform.eulerAngles.y+180f);


            if (devNodeT == null)
            {

                return;
            }

        });
     
    }
    public void  CameraPiontClick()
    {
        CameraExample.transform.GetChild(0).GetComponent<Image>().color = ExitColor;
        CameraExample.transform.GetChild(1).GetComponent<Image>().color = EnterColor;
        CameraExample.transform.GetChild(2).gameObject.SetActive(true);

        CameraExample.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = camName;
    }
    public void CameraPointExit()
    {
        CameraExample.transform.GetChild(0).GetComponent<Image>().color = EnterColor;
        CameraExample.transform.GetChild(1).GetComponent<Image>().color = ExitColor;
        CameraExample.transform.GetChild(2).gameObject.SetActive(false);
    }
    public void CamClick_Toggle(bool ison)
    {
        if (ison)
        {
            nearPersonnelCameraInfo.camTog.isOn = true;
         //   ChangeScrollbarValue();
        }
        else
        {
            nearPersonnelCameraInfo.camTog.isOn = false ;
        }
    }
    /// <summary>
    /// 改便滑动条的数值
    /// </summary>
    public void ChangeScrollbarValue()
    {
    

    }
    // Update is called once per frame
    void Update () {
		
	}
}
