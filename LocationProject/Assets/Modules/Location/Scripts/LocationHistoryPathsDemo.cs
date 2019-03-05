using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationHistoryPathsDemo : MonoBehaviour {

    public List<GameObject> objList;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void On_Click()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths();
        List<Vector3> list = new List<Vector3>();

        foreach (GameObject o in objList)
        {
            list.Add(o.transform.position);
        }

        //LocationManager.Instance.ShowLocationHistoryPath("aaa", list, 250, Color.green);
        
    }

    public void On_ClickReal0002()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths();
        string code = "0002";
        List<Vector3> list = new List<Vector3>();
        DateTime end = new DateTime(2018, 8, 2, 9, 14, 0);
        DateTime start = end.AddMinutes(-2.5);
        List<Position> ps = CommunicationObject.Instance.GetHistoryPositonsByTime(code, start, end);
        if (ps.Count < 2) return;
        foreach (Position p in ps)
        {
            Vector3 temp = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
            temp= LocationManager.GetRealVector(temp);
            Vector3 offset = LocationManager.Instance.transform.position;
            temp = new Vector3(temp.x + offset.x, temp.y + offset.y, temp.z + offset.z);
            list.Add(temp);

        }

        //LocationManager.Instance.ShowLocationHistoryPath(code,list, list.Count,Color.green, "HistoryPath0002");

    }

    public void On_ClickReal0003()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths();
        string code = "0003";
        List<Vector3> list = new List<Vector3>();
        DateTime end = new DateTime(2018, 7, 17, 18, 40, 0);
        DateTime start = end.AddMinutes(-2);
        List<Position> ps = CommunicationObject.Instance.GetHistoryPositonsByTime(code, start, end);
        if (ps.Count < 2) return;
        foreach (Position p in ps)
        {
            Vector3 temp = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
            temp = LocationManager.GetRealVector(temp);
            Vector3 offset = LocationManager.Instance.transform.position;
            temp = new Vector3(temp.x + offset.x, temp.y + offset.y, temp.z + offset.z);
            list.Add(temp);

        }

        //LocationManager.Instance.ShowLocationHistoryPath(code, list, list.Count, Color.red, "HistoryPath0003");

    }

    public void On_ClickU3DHistory0002()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths();
        string code = "0002";
        List<Vector3> list = new List<Vector3>();
        DateTime end = new DateTime(2018, 7,17, 19, 22, 0);
        DateTime start = end.AddMinutes(-2.5);
        List<U3DPosition> ps = CommunicationObject.Instance.GetHistoryU3DPositonsByTime(code, start, end);
        if (ps.Count < 2) return;
        foreach (U3DPosition p in ps)
        {
            Vector3 temp = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
            //temp = LocationManager.GetRealVector(temp);
            Vector3 offset = LocationManager.Instance.transform.position;
            temp = new Vector3(temp.x + offset.x, temp.y + offset.y, temp.z + offset.z);
            list.Add(temp);

        }

        //LocationManager.Instance.ShowLocationHistoryPath(code,list, list.Count, Color.green, "U3DHistory0002");

    }

    public void On_ClickU3DHistory0003()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths();
        string code = "0003";
        List<Vector3> list = new List<Vector3>();
        DateTime end = new DateTime(2018, 7, 17, 18, 40, 0);
        DateTime start = end.AddMinutes(-2);
        List<U3DPosition> ps = CommunicationObject.Instance.GetHistoryU3DPositonsByTime(code, start, end);
        if (ps.Count < 2) return;
        foreach (U3DPosition p in ps)
        {
            Vector3 temp = new Vector3((float)p.X, (float)p.Y, (float)p.Z);
            //temp = LocationManager.GetRealVector(temp);
            Vector3 offset = LocationManager.Instance.transform.position;
            temp = new Vector3(temp.x + offset.x, temp.y + offset.y, temp.z + offset.z);
            list.Add(temp);

        }

        //LocationManager.Instance.ShowLocationHistoryPath(code, list, list.Count, Color.green, "U3DHistory0003");

    }

    public Toggle locationToggle;
    /// <summary>
    /// 清除定位模式
    /// </summary>
    public void ClearLocationMode()
    {
        //locationToggle.isOn = false;
        //ToggleGroup group; group.
        ActionBarManage.Instance.OnPersonnelToggleChange(false);
        ActionBarManage.Instance.CurrentState = ViewState.None;
    }
}
