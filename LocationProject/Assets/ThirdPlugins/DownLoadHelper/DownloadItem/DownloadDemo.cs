using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadDemo : MonoBehaviour
{

    DownloadItem m_item;
    public int bufferSize = 1024;
    public string scrUrl = "http://localhost:53506/Exe/LocationSystem.exe";
    public int count = 0;
   


    void Start()
    {
        Debug.Log(Application.dataPath);
        //m_item = new WWWDownloadItem(testScrUrl, Application.persistentDataPath);
        //m_item.StartDownload(DownloadFinish);
        m_item = new HttpDownloadItem(scrUrl, Application.dataPath+"//..//");
        m_item.BufferSize = bufferSize;
        m_item.StartDownload(DownloadFinish);
    }

    void Update()
    {
        count++;
        if (count % 20 == 0)
        {
            if (m_item != null && m_item.isStartDownload)
            {
                Debug.Log("下载进度------" + m_item.GetProcess() + "------已下载大小---" + m_item.GetCurrentLength());
            }
        }
    }

    void DownloadFinish()
    {
        Debug.Log("DownloadFinish！！！");
    }
}
