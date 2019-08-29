using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGetTags : MonoBehaviour {

    public InputField Interval;

    public CommunicationObject comm;

    // Use this for initialization
    void Start () {
        if (comm == null)
        {
            comm = CommunicationObject.Instance;
            if (comm == null)
            {
                comm = gameObject.AddComponent<CommunicationObject>();
            }
            //if (comm.clientWebApi == null)
            //{
            //    comm.clientWebApi = gameObject.AddComponent<WebApiClient>();
            //}
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private float GetTime()
    {
        string time = Interval.text;
        float t = float.Parse(time);
        return t;
    }

    public void StopInvoke()
    {
        Log.Info("StopInvoke");
        CancelInvoke("GetTags1_WCF_Sync");
        CancelInvoke("GetTags1_WCF_Async");
        CancelInvoke("GetTags1_WCF_Thread");
        CancelInvoke("GetTags1_WCF_WebApi");
    }

    public void StartGetTags1_WCF_Sync()
    {
        var t = GetTime();
        if (t == 0)
        {
            GetTags1_WCF_Sync();
        }
        else
        {
            InvokeRepeating("GetTags1_WCF_Sync", 0, t);
        }
    }

    private void GetTags1_WCF_Sync()
    {
        Log.Info("GetTags1_WCF_Sync");
        GetTags(CommunicationMode.Sync);
    }

    private void GetTags(CommunicationMode mode)
    {
        comm.Mode = mode;
        comm.GetTags(tags =>
        {
            Log.Info("Tags", tags);
        });
    }

    public void StartGetTags1_WCF_Async()
    {
        //GetTags1_WCF_Async();
        var t = GetTime();
        InvokeRepeating("GetTags1_WCF_Async", 0, t);
    }

    private void GetTags1_WCF_Async()
    {
        Log.Info("GetTags1_WCF_Sync");
        GetTags(CommunicationMode.Async);
    }

    public void StartGetTags1_WCF_Thread()
    {
        var t = GetTime();
        InvokeRepeating("GetTags1_WCF_Thread", 0, t);
    }

    private void GetTags1_WCF_Thread()
    {
        Log.Info("GetTags1_WCF_Thread");
        GetTags(CommunicationMode.Thread);
    }

    public void StartGetTags1_WCF_WebApi()
    {
        isBusy = false;
        var t = GetTime();
        if (t == 0)
        {
            GetTags1_WCF_WebApi();
        }
        else
        {
            InvokeRepeating("GetTags1_WCF_WebApi", 0, t);
        }
        
    }

    private void GetTags1_WCF_WebApi()
    {
        Log.Info("GetTags1_WCF_WebApi");
        //GetTags(CommunicationMode.WebApi);

        //comm.Mode = CommunicationMode.WebApi;
        //comm.GetTagsAsync(tags =>
        //{
        //    Log.Info("Tags", tags);
        //});

        ShowTagsPosition();
    }

    bool isBusy;

    /// <summary>
    /// 显示定位卡的位置信息
    /// </summary>
    public void ShowTagsPosition()
    {
        if (!isBusy)
        {
            isBusy = true;
            CommunicationObject.Instance.Mode = CommunicationMode.WebApi;
            CommunicationObject.Instance.GetTags((tagsT) =>
            {
                Log.Info("ShowTagsPosition End!!!!!");
                isBusy = false;
            });
        }
        else
        {
            Log.Alarm("ShowTagsPosition", "isBusy:" + isBusy);
        }
    }
}
