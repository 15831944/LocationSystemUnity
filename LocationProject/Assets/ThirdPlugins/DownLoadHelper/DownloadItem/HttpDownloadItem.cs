using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
/// <summary>
/// HTTP的方式下载，支持断点续传
/// </summary>
public class HttpDownloadItem : DownloadItem
{
    /// <summary>
    /// 临时文件后缀名
    /// </summary>
    string m_tempFileExt = ".temp";
    /// <summary>
    /// 临时文件全路径
    /// </summary>
    string m_tempSaveFilePath;

    public HttpDownloadItem(string url, string path) : base(url, path)
    {
        m_tempSaveFilePath = string.Format("{0}/{1}{2}", m_savePath, m_fileNameWithoutExt, m_tempFileExt);
    }

    public override void StartDownload(Action callback = null)
    {
        base.StartDownload();
        UICoroutine.instance.StartCoroutine(Download(callback));
        //StartCoroutine(Download(callback));
    }

    IEnumerator Download(Action callback = null)
    {
        if (File.Exists(m_saveFilePath))
        {
            File.Delete(m_saveFilePath);
        }
        if (File.Exists(m_tempSaveFilePath))
        {
            File.Delete(m_tempSaveFilePath);//不用断点续传功能
        }

        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(m_srcUrl);
        request.Method = "GET";

        FileStream fileStream;
        if (File.Exists(m_tempSaveFilePath))
        {
            //若之前已下载了一部分，继续下载
            fileStream = File.OpenWrite(m_tempSaveFilePath);
            m_currentLength = fileStream.Length;
            fileStream.Seek(m_currentLength, SeekOrigin.Current);

            //设置下载的文件读取的起始位置
            request.AddRange((int)m_currentLength);
        }
        else
        {
            //第一次下载
            fileStream = new FileStream(m_tempSaveFilePath, FileMode.Create, FileAccess.Write);
            m_currentLength = 0;
        }

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        //总的文件大小=当前需要下载的+已下载的
        m_fileLength = response.ContentLength + m_currentLength;
        Debug.Log("m_saveFilePath:" + m_saveFilePath);
        Debug.Log("m_tempSaveFilePath:" + m_tempSaveFilePath);
        Debug.Log("ContentLength:" + response.ContentLength);
        Debug.Log("m_currentLength:" + m_currentLength);
        Debug.Log("m_fileLength:"+ m_fileLength);
        m_isStartDownload = true;
        int lengthOnce;
        int bufferMaxLength = 1024 * BufferSize;

        while (m_currentLength < m_fileLength)
        {

            byte[] buffer = new byte[bufferMaxLength];
            if (stream.CanRead)
            {
                //读写操作
                lengthOnce = stream.Read(buffer, 0, buffer.Length);
                m_currentLength += lengthOnce;
                fileStream.Write(buffer, 0, lengthOnce);
            }
            else
            {
                break;
            }
            yield return null;
        }

        m_isStartDownload = false;
        response.Close();
        stream.Close();
        fileStream.Close();
        //临时文件转为最终的下载文件
        File.Move(m_tempSaveFilePath, m_saveFilePath);

        if (callback != null)
        {
            callback();
        }
    }

    public override float GetProcess()
    {
        if (m_fileLength > 0)
        {
            return Mathf.Clamp((float)m_currentLength / m_fileLength, 0, 1);
        }
        return 0;
    }

    public override long GetCurrentLength()
    {
        return m_currentLength;
    }

    public override long GetLength()
    {
        return m_fileLength;
    }

    public override void Destroy()
    {
    }
}
