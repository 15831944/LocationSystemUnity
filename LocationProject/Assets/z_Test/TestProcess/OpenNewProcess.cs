using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class OpenNewProcess : MonoBehaviour {


    public Button OnlyFindWindowButton;

    public Button StartProcessButton;
    [HideInInspector]
    public string HoneyWellWindowName = "HUSSDKDemo";
    // Use this for initialization
    void Start () {

        OnlyFindWindowButton.onClick.AddListener(FindWindow);
        StartProcessButton.onClick.AddListener(StartProcess);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    [ContextMenu("CreateProcess")]
    public void CreateNewProcess()
    {
        string fileName = Application.dataPath + "\\..\\release\\HUSSDKDemo.exe";
        string guid = Guid.NewGuid().ToString();
        string argsTransfer = string.Format("{0}|{1}|{2}|{3}", "127.0.0.1", "Admin", "admin123", guid);
        CreateNewProcess(fileName, argsTransfer);
    }

    public void FindWindow()
    {
        IntPtr findValue = FindWindowTest.FindWindowByName(HoneyWellWindowName);
        if (findValue.ToInt32() != 0)
        {           
            UnityEngine.Debug.LogError("FindWindow success:"+findValue.ToInt32());                       
        }
        else
        {
            UnityEngine.Debug.LogError("FindWindow failed:" + findValue.ToInt32());
        }
    }

    public void StartProcess()
    {
        ShowVideo();
    }

    public void ShowVideo(string cameraGuid="")
    {
        if(string.IsNullOrEmpty(cameraGuid))cameraGuid = Guid.NewGuid().ToString();
        string argsTransfer = string.Format("{0}|{1}|{2}|{3}", "127.0.0.1", "Admin", "admin123", cameraGuid);
        StartProcess(argsTransfer);
    }

    Process myprocess;
    public const int WM_COPYDATA = 0x004A;

    private void CreateNewProcess(string fileName,string args)
    {
        myprocess = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args);
        myprocess.StartInfo = startInfo;
        myprocess.StartInfo.UseShellExecute = false;
        myprocess.Start();
    }

    /// <summary>
    /// 开启视屏程序
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private void StartProcess(string args)
    {
        try
        {
            string fileName = Application.dataPath + "\\..\\release\\HUSSDKDemo.exe";
            if (!File.Exists(fileName))
            {
                UnityEngine.Debug.LogErrorFormat("path:{0} not exist!", fileName);
            }
            if(myprocess==null)
            {
                UnityEngine.Debug.LogError("create myprocess.StartInfo");
                if(!IsProcessExist())
                {
                    CreateNewProcess(fileName, args);
                }
                //CreateNewProcess(fileName,args);
            }
            else if(myprocess.HasExited)
            {
                UnityEngine.Debug.LogError("Camera:myprocess.HasExited");
                CreateNewProcess(fileName, args);
            }
            else
            {
                IntPtr hWnd = myprocess.MainWindowHandle; //获取Form1.exe主窗口句柄
                int pId = -1;
                if (hWnd.ToInt32() == 0)
                {
                    IntPtr findValue = FindWindowTest.FindWindowByName(HoneyWellWindowName);
                    if (findValue.ToInt32() != 0)
                    {
                        //GetWindowThreadProcessId(findValue, out pId);
                        pId = findValue.ToInt32();
                        UnityEngine.Debug.LogError("FindWindow:" + pId);                       
                    }
                    else
                    {
                        GetWindowThreadProcessId(findValue, out pId);
                        //UnityEngine.Debug.LogError("FindWindow2:"+pId);
                    }
                }
                else
                {
                    pId = hWnd.ToInt32();
                }
                string sendString = args;
                byte[] sarr = System.Text.Encoding.Default.GetBytes(sendString);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)0;
                cds.cbData = len + 1;
                cds.lpData = sendString;
                SendMessage(pId, WM_COPYDATA, 0, ref cds);
            }
            UnityEngine.Debug.LogError("Show camera:"+args);          
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("出错原因：" + ex.Message);
        }
    }
    /// <summary>
    /// 视频进程是否存在
    /// </summary>
    /// <returns></returns>
    private bool IsProcessExist()
    {
        Process[] ps = Process.GetProcessesByName("HUSSDKDemo");
        bool isExist = ps != null && ps.Length > 0;
        if (isExist)
        {
            for (int i = 0; i < ps.Length; i++)
            {
                if (i == 0)
                {
                    myprocess = ps[0];
                }
                else
                {
                    ps[i].Kill();//关闭多余的窗口
                }
            }
        }
        return isExist;
    }
    private void OnDestroy()
    {
        if(myprocess!=null)myprocess.CloseMainWindow();
    }

    #region WinAPI

    [DllImport("User32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    /// 根据句柄查找进程ID
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="ID"></param>
    /// <returns></returns>
    [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


    /// <summary>
    /// 使用COPYDATASTRUCT来传递字符串
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

    //消息发送API
    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    public static extern int SendMessage(
        IntPtr hWnd,        // 信息发往的窗口的句柄
        int Msg,            // 消息ID
        int wParam,         // 参数1
        int lParam          //参数2
    );

    //消息发送API
    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    public static extern int SendMessage(
        int hWnd,        // 信息发往的窗口的句柄
        int Msg,            // 消息ID
        int wParam,         // 参数1
        ref COPYDATASTRUCT lParam  //参数2
    );

    //消息发送API
    [DllImport("User32.dll", EntryPoint = "PostMessage")]
    public static extern int PostMessage(
        IntPtr hWnd,        // 信息发往的窗口的句柄
        int Msg,            // 消息ID
        int wParam,         // 参数1
        int lParam            // 参数2
    );


    //异步消息发送API
    [DllImport("User32.dll", EntryPoint = "PostMessage")]
    public static extern int PostMessage(
        int hWnd,        // 信息发往的窗口的句柄
        int Msg,            // 消息ID
        int wParam,         // 参数1
        ref COPYDATASTRUCT lParam  // 参数2
    );


    #endregion

}
