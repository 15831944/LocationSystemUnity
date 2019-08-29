using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//using Location.WCFServiceReferences.LocationServices;

public class FileDownLoad : MonoBehaviour {

    //  public AudioSource audioSource;
    string urlPath;//资源网络路径
    string file_SaveUrl;//资源保存路径

    public static FileDownLoad Instance;

    //private WWW downloadOperation;
    private bool isDownLoadStart;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }

    void Update()
    {
        if(isDownLoadStart)
        {
            ////判断异步对象并且异步对象没有加载完毕，显示进度    
            //if (downloadOperation != null && !downloadOperation.isDone)
            //{
            //    //Debug.Log(string.Format("下载进度:{0:F}%", downloadOperation.progress * 100.0));
            //    ShowDownloadProgress(true,downloadOperation.progress,"");
            //}

            
            if (processInterval > 0)
            {
                count++;
                if (count % processInterval == 0)
                {
                    if (m_item != null && m_item.isStartDownload)
                    {
                        Debug.Log("下载进度------" + m_item.GetProcess() + "------已下载大小---" + m_item.GetCurrentLength());
                        ShowDownloadProgress(true, m_item.GetProcess(), "");
                    }
                }
            }
            else
            {
                if (m_item != null && m_item.isStartDownload)
                {
                    Debug.Log("下载进度------" + m_item.GetProcess() + "------已下载大小---" + m_item.GetCurrentLength());
                    ShowDownloadProgress(true, m_item.GetProcess(), "");
                }
            }
        } 
    }

    //private string fileName= "file:///"+ @"C:\Users\Administrator\Desktop\LocationSystem.exe";
    //private string fileName = "http://127.0.0.1:8013/电厂/LocationSystem.exe";
    private void WriteLog(string msg)
    {
        Debug.Log(msg);
        //CommunicationObject.Instance.DebugLog(msg);
    }

    DownloadItem m_item;
    public int bufferSize = 1024;
    public int count = 0;
    public int processInterval = 10;

    public void Download(string serverURL)
    {
        Debug.Log(Application.dataPath);
        //m_item = new WWWDownloadItem(testScrUrl, Application.persistentDataPath);
        //m_item.StartDownload(DownloadFinish);
        isDownLoadStart = true;
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "//..//");
        m_item = new HttpDownloadItem(serverURL, dir.FullName);
        file_SaveUrl = m_item.m_saveFilePath;
        m_item.BufferSize = bufferSize;
        m_item.StartDownload(DownloadFinish);
    }

    void DownloadFinish()
    {
        isDownLoadStart = false;
        Debug.Log("DownloadFinish！！！");
        WriteLog("文件创建完成..." + file_SaveUrl);
        ShowDownloadProgress(true, 1, "");
        Application.OpenURL(file_SaveUrl);
        Invoke("Quit", 3f);
    }

    //    public void Download(string serverURL)
    //    {
    //        string path = serverURL;
    //        if (string.IsNullOrEmpty(path))
    //        {
    //            Debug.Log("路径输入错误");
    //            UGUIMessageBox.Show("Version path is empty,please check!");         
    //            return;
    //        }
    //        urlPath = path;
    //        //file_SaveUrl = @"C:\Users\Administrator\Desktop\LocationSystem.exe";
    //        file_SaveUrl = Application.dataPath + @"\LocationSystem.exe";
    //        WriteLog("urlPath : " + urlPath);
    //        WriteLog("file_SaveUrl : " + file_SaveUrl);
    //        FileInfo file = new FileInfo(file_SaveUrl);    //每次都重新计算
    //        byte[] bytes = new byte[1024];                  //
    //        if (File.Exists(file_SaveUrl))//本地存在，删除重新下载
    //        {
    //            try
    //            {
    //                File.Delete(file_SaveUrl);
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.Log(e.ToString());
    //            }
    //        }
    //        StartCoroutine(DownFile(urlPath, file_SaveUrl, file, bytes));
    //    }
    //    IEnumerator DownFile(string url,string file_SaveUrl, FileInfo file, byte[] bytes)
    //    {
    //        WriteLog("下载文件到..." + url+"->"+ file_SaveUrl);
    //        downloadOperation = new WWW(url);
    //        isDownLoadStart = true;
    //        yield return downloadOperation;
    //        if (downloadOperation.error != null)
    //        {
    //            Debug.Log(downloadOperation.error);
    //            isDownLoadStart = false;
    //            ShowDownloadProgress(false, 0,"");
    //            UGUIMessageBox.Show(downloadOperation.error);
    //            yield break;
    //        }        
    //        if (downloadOperation.isDone)
    //        {
    //            try
    //            {
    //                isDownLoadStart = false;
    //                ShowDownloadProgress(true, 1, "");
    //                WriteLog("下载完成 ");
    //                WriteLog("文件大小1 : " + downloadOperation.bytesDownloaded);
    //#if UNITY_EDITOR
    //                WriteLog("文件大小2 : " + downloadOperation.bytes.Length);//打包后运行到这句话会出现内存溢出的问题
    //                bytes = downloadOperation.bytes;//这句话也会出现内存溢出的问题
    //#endif
    //                WriteLog("BeforeCreatFile");
    //                CreatFile(bytes, file, downloadOperation.bytesDownloaded);
    //                WriteLog("文件创建完成..." + file_SaveUrl);
    //                ShowDownloadProgress(true, 1, "文件创建完成...");
    //                Application.OpenURL(file_SaveUrl);
    //                Invoke("Quit", 3f);
    //            }
    //            catch (Exception ex)
    //            {
    //                WriteLog("[DownFile]" + ex.ToString());
    //            }
    //        }
    //        else
    //        {
    //            Log.Error("下载未完成");
    //        }
    //    }
    private void ShowDownloadProgress(bool isActive,float value,string msg)
    {
        //DownloadProgressBar progress = DownloadProgressBar.Instance;
        //if (progress)
        //{
        //    if (isActive) progress.Show(value, msg);
        //    else progress.Hide();
        //}

        if (ShowProgress != null)
        {
            ShowProgress(isActive, value, msg);
        }
    }

    public event Action<bool, float, string> ShowProgress;

    /// <summary>
    /// 退出程序
    /// </summary>
    private void Quit()
    {
        ShowDownloadProgress(true, 1, "退出程序...");
        WriteLog("退出程序");
        Application.Quit();
    }

    private void OnDisable()
    {
        isDownLoadStart = false;
    }
    /// <summary>
    /// 文件流创建文件
    /// </summary>
    /// <param name="bytes"></param>
    void CreatFile(byte[] bytes, FileInfo file,int count)
    {
        try
        {
            Stream stream = file.Create();
            stream.Write(bytes, 0, count);
            stream.Close();
            stream.Dispose();
        }
        catch (Exception ex)
        {
            WriteLog("[CreateFile]"+ ex.ToString());
        }
    }
}
