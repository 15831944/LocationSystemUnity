using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System;

public class HoneyWellVideoPart : MonoBehaviour {

    public bool IsSDKInit;
   
    private string HoneyWellWindowName = "HoneyWellVideoMonitor";

    private string HoneyWellProcessName = "HUSSDKDemo";
    // Use this for initialization
    void Start () {
        //改成点击一个摄像头后，才启动
        //Invoke("InitSDK",5f);//进入电厂后，如果systemSetting.HoneyWellEnable=true,则启动霍尼韦尔后台程序
	}

    /// <summary>
    /// 初始化SDK(程序开启后，后台开程序，先注册SDK)
    /// </summary>
    private void InitSDK()
    {
        if (SystemSettingHelper.honeyWellSetting.EnableHoneyWell)
        {
            UnityEngine.Debug.LogError("CameraVideoManage.InitSDK...");
            ShowVideo("0167b420-0b29-41ff-be49-e2d45a6fb67b");//随便输入一个，注册时不会处理这个GUID
        }       
    }

    /// <summary>
    /// 显示视屏监控
    /// </summary>
    /// <param name="camGuid"></param>
    public void ShowVideo(string camGuid)
    {
        //#if UNITY_EDITOR
        //        camGuid = "0167b420-0b29-41ff-be49-e2d45a6fb67b";
        //#endif

        if (string.IsNullOrEmpty(camGuid))
        {
            UGUIMessageBox.Show("视屏参数未设置,请配置参数!");
            return;
        }
        string Ip = SystemSettingHelper.honeyWellSetting.Ip;
        string userName = SystemSettingHelper.honeyWellSetting.UserName;
        string passWord = SystemSettingHelper.honeyWellSetting.PassWord;
        string cameraGuid = camGuid;
        string argsTransfer = string.Format("{0}|{1}|{2}|{3}", Ip, userName, passWord, cameraGuid);
        if (!IsSDKInit)
        {
            CreateNewProcess(argsTransfer);
        }
        else
        {
            StartProcess(argsTransfer);
        }
    }
    #region codeBackUp
    ///// <summary>
    ///// 开启视屏程序
    ///// </summary>
    ///// <param name="args"></param>
    ///// <returns></returns>
    //private bool StartProcess(string args)
    //{
    //    try
    //    {
    //        string fileName = Application.dataPath+ "\\..\\release\\HUSSDKDemo.exe";
    //        if(!File.Exists(fileName))
    //        {
    //            UnityEngine.Debug.LogErrorFormat("path:{0} not exist!",fileName);
    //            return false;
    //        }
    //        Process myprocess = new Process();
    //        ProcessStartInfo startInfo = new ProcessStartInfo(fileName, args);
    //        myprocess.StartInfo = startInfo;
    //        myprocess.StartInfo.UseShellExecute = false;
    //        myprocess.Start();
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        UnityEngine.Debug.LogError("出错原因：" + ex.Message);
    //    }
    //    return false;
    //}
    #endregion


    Process myprocess;//摄像头程序进程
    public const int WM_COPYDATA = 0x004A;//进程发送信息标识

    /// <summary>
    /// 创建新进程
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="args"></param>
    private void CreateNewProcess(string args, string fileName = "")
    {
        if (string.IsNullOrEmpty(fileName))
        {
            if (IsSDKInit) return;
            fileName = Application.dataPath + "\\..\\release\\HUSSDKDemo.exe";
            if (!File.Exists(fileName))
            {
                UnityEngine.Debug.LogErrorFormat("path:{0} not exist!", fileName);
                return;
            }
            IsSDKInit = true;
        }
        CloseExistProcess();//开启进程之前，关闭已经存在的进程
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
                return;
            }
            if (myprocess == null)
            {
                UnityEngine.Debug.LogError("create myprocess.StartInfo");
                CreateNewProcess(args, fileName);
            }
            else if (myprocess.HasExited)
            {
                UnityEngine.Debug.LogError("Camera:myprocess.HasExited");
                CreateNewProcess(args, fileName);
            }
            else
            {
                IntPtr hWnd = myprocess.MainWindowHandle; //获取Form1.exe主窗口句柄
                int pId = -1;
                if (hWnd.ToInt32() == 0)
                {
                    pId = FindWindow(null, HoneyWellWindowName);
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
                ThreadManager.Run(() =>
                {
                    SendMessage(pId, WM_COPYDATA, 0, ref cds); //SendMessage是同步的，考虑放在线程里，防止卡住主线程

                }, () =>
                {
                    UnityEngine.Debug.LogError("Show camera:" + args);
                }, "");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error : CameraVideoManage.StartProcess：" + ex.Message);
        }
    }
    /// <summary>
    /// 删除多余视频进程
    /// </summary>
    /// <returns></returns>
    private void CloseExistProcess()
    {
        Process[] ps = Process.GetProcessesByName(HoneyWellWindowName);
        bool isExist = ps != null && ps.Length > 0;
        if (isExist)
        {
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].CloseMainWindow();//关闭多余的窗口
            }
        }
    }

    private void OnDestroy()
    {
        if (myprocess != null)
        {
            //myprocess.Kill();
            myprocess.CloseMainWindow();  //程序关闭识，把进程关闭。不然再打开程序，会有多个视频进程
        }
    }
    #region WinAPI

    [DllImport("User32.dll", EntryPoint = "FindWindow")]
    private static extern int FindWindow(string lpClassName, string lpWindowName);

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
