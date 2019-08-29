using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAlarmFollowUIItem : MonoBehaviour
{

    public Text AlarmType;
    public Text AlarmTimeText;
    public Button PictureBut;
    public Text IPText;
    public Sprite TransperantBack;
    public Text NameText;
   
    int width = 1600;
    int height = 900;
    string PictureData;
    CameraAlarmInfo CurrentAlarm;
    void Start()
    {
    
        PictureWindow.Instance.ClosePicture.onClick.AddListener(() =>
      {
          ShowPictureWindow(false);
      });
    }
    public void GetCameraAlarmData(CameraAlarmInfo alarm)
    {
        CurrentAlarm = new CameraAlarmInfo();
        CurrentAlarm = alarm;
        if (NameText!=null)
        {
            if (alarm!=null&&!string .IsNullOrEmpty(alarm.DevName))
            {
                NameText.text = alarm.DevName;
            }
           else
            {
                NameText.text = "--";
            }
        }
        if (IPText!=null)
        {
            if (alarm != null && !string .IsNullOrEmpty (alarm.DevIp))
            {
                IPText.text = alarm.DevIp; 
            }
            else
            {
                IPText.text = "--";
            }
        }
    
        if (alarm.AlarmType == 1)
        {
            AlarmType.text = "未戴安全帽";

        }
        else if (alarm .AlarmType == 2)
        {
            AlarmType.text = "火警";
        }  else if (alarm.AlarmType == 3)
        {
            AlarmType.text = "烟雾";
        }
        AlarmTimeText.text = GetDataTime(alarm.time_stamp).ToString("yyyy年MM月dd日 HH时mm分ss秒");
        PictureData = alarm.pic_data;
        PictureBut.onClick.AddListener(() =>
       {
           ShowPictureWindow(true);

       });
    }
   public DateTime GetDataTime(long time_stamp)
    {
        DateTime dtStart = new DateTime(1970, 1, 1);
        long lTime = ((long)time_stamp * 10000000);
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime AlarmTime = dtStart.Add(toNow);
        return AlarmTime;
    }
    public void ShowPictureWindow(bool b)
    {
        PictureWindow.Instance.PictureWindowUI.SetActive(b);
        if (b)
        {
           
            CameraAlarmInfo cameraalarm = CommunicationObject.Instance.GetCameraAlarm(CurrentAlarm.id);
            Texture2D texture = new Texture2D(width, height);
            byte[] Pic = PictureDataInfo(cameraalarm.pic_data);
            
            texture.LoadImage(Pic);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            PictureWindow.Instance.Picture.sprite = sprite;
        }
        else
        {
            PictureWindow.Instance.Picture.sprite = PictureWindow.Instance.TransperantBack;
        }
       
    }
    public byte[] PictureDataInfo(string picture)
    {
        picture = picture.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
        byte[] bytes = Convert.FromBase64String(picture);
        return bytes;
    }
    void Update()
    {

    }
}
