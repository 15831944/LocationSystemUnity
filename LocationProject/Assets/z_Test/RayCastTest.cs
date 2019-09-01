//using Assets.z_Test.BackUpDevInfo;
using Base.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RayTest();
        //loadXML();
    }
    private void loadXML()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    string filePath = Application.dataPath + "/DevInfoBackup.xml";
        //    DevInfoBackupList devInfo = SerializeHelper.LoadFromFile<DevInfoBackupList>(filePath);
        //    Debug.Log(devInfo.DevList.Count);
        //}           
    }

    public Camera RoamCamera;
    private void RayTest()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray;
            if(RoamCamera==null)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            else
            {
                ray = RoamCamera.ScreenPointToRay(Input.mousePosition);
            }
            
            //Ray ray = new Ray(transform.position, Vector3.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                Debug.LogError(hitInfo.transform.name + LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            }
            //if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            //{
            //    Debug.Log(hitInfo.transform.name + LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            //}
        }
    }
}
