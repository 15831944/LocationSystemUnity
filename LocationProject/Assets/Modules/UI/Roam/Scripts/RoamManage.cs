using DG.Tweening;
using HighlightingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using Jacovone.AssetBundleMagic;

public class RoamManage : MonoBehaviour
{
    public static RoamManage Instance;
    public GameObject PromptWindow;//漫游模式的操作提示
  
    public GameObject RoamWindow;//漫游窗口
    public Tweener FPSupTween;//第一人称移动动画
    public Transform FPSobj;
    private Vector3 Lowest = new Vector3(93.24f, 20f, -75.3f);//飞行模式下最低点

    public FirstPersonController FPSController;
    public bool isStart = true;
    private bool m_Jump;
    private bool is_Fly = false;
    public GameObject Light;
    public GameObject PromptBox;//没有飞行模式的提示框
    private bool isIndoor;//判断是否在室内
   

    // Use this for initialization
    void Start()
    {
        Instance = this;
        BindingCursorLockState();

    }

    private void BindingCursorLockState( )
    {
        if(FPSController!=null)
        {
            FPSController.BindingCursorAction(ChangeCursorState);
        }
    }

    public void On_fly()
    {
        is_Fly = false;
        FPSController.ChangeGravityValue(30f);
    }
    /// <summary>
    /// 漫游界面
    /// </summary>
    /// <param name="b"></param>
    public void ShowRoamWindow(bool b)
    {
        ChangeHightRender(b);    
        if (b)
        {
            RoamWindow.SetActive(true);
        }
        else
        {
            RoamWindow.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        flight_Click();
    }

    public void flight_Click()
    {
        if(LoadingIndicatorScript.Instance!=null)
            LoadingIndicatorScript.Instance.IsBuildingAndIsDev();
        if (!FPSController.IsCursorLock) return;
        if (FPSController.gameObject.activeInHierarchy)
        {
            if (!PromptWindow.transform.GetChild(6).gameObject.activeInHierarchy)// Debug.Log("飞行模式下，没有选择入口");
            {
                if (Input.GetKeyDown(KeyCode.R))//进入到返回入口
                {
                   
                    FPSController.gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    EntranceManage.instance.ShowWindow(true);
                    FPSMode.Instance.HideCameras(false);
                    FPSMode.Instance.NoFPSUI.SetActive(false);
                    FPSController.ChangeGravityValue(1f);
                    FPSController.ChangeWalkSpeed(1.6f);
                    FPSMode.Instance.SetRoamFollowUI(false);
                    if(RoamDevInfoUI.Instance)RoamDevInfoUI.Instance.Close();
                    ExitRoam();
                }
            }
        }
        if (!PromptWindow.transform.GetChild(6).gameObject.activeInHierarchy)// Debug.Log("飞行模式下没有跳跃");
        {
            if (Input.GetKey(KeyCode.Space))//跳跃
            {
               
                FPSController.ChangeGravityValue(1f);
                FPSController.IsSpaceState = true;
                is_Fly = true;
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                Invoke("On_fly", 1f);
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {

                    FPSController.ChangeWalkSpeed(5f);
                }
                else
                {
                    FPSController.ChangeWalkSpeed(1.6f);
                    Debug.LogError("cailulu+1.6");
                }

            }
            
        }

        if (FPSobj.gameObject.activeInHierarchy)
        {
            Transform flight = PromptWindow.transform.GetChild(3).GetChild(0);

            if (Input.GetKeyDown(KeyCode.F))//进入飞行模式
            {
                
                if (isIndoor&& !PromptWindow.transform.GetChild(3).gameObject.activeInHierarchy)
                {
                    ShowPromptBox();
                    Invoke("ClosePromptBox", 3f);
                }
                else
                {
                    if (is_Fly)
                    {
                    }
                    else
                    {
                        if (isStart)
                        {
                            is_Fly = true;
                            FPSController.ChangeGravityValue(0f);
                            FPSController.ChangeWalkSpeed(25f);
                            FPSobj.GetComponent<Transform>().DOLocalMoveY(20f, 1.2f).SetEase(Ease.InOutQuint).OnComplete(() =>
                            {
                                FPSController.IsSpaceState = false;
                                is_Fly = false;
                            });
                            EntranceFlight();
                            flight.GetComponent<Text>().text = "退出飞行";
                            isStart = false;
                        }
                        else
                        {
                          //  is_Fly = true;
                            ////FPSobj.GetComponent<Transform>().DOLocalMoveY(1f, 1.2f).SetEase(Ease.InOutQuint).OnComplete(() =>
                            ////{
                           // FPSController.ChangeWalkSpeed(1.6f);
                            Debug.LogError("cailulu+1.6");
                            FPSController.IsSpaceState = true;
                              //  is_Fly = false;
                         //   });
                            EntranceRoam();
                            flight.GetComponent<Text>().text = "进入飞行";
                            FPSController.ChangeGravityValue(30f);
                            FPSController.ChangeWalkSpeed(1.6F);
                           // FPSController.ChangeWalkSpeed(1.6f);
                            isStart = true;
                            is_Fly = false ;


                        }

                    }
                }
            }
        }
        if (PromptWindow.transform.GetChild(6).gameObject.activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.Q))//飞行模式上升
            {
                
                if (FPSobj.transform.GetComponent<Transform>().localPosition.y < 160f)
                {
                    FPSobj.GetComponent<Transform>().localPosition += Vector3.up * 2;
                }
            }
            if (Input.GetKey(KeyCode.E))//飞行模式下降
            {
                if (FPSobj.transform.GetComponent<Transform>().localPosition.y > 20f)
                {

                    FPSobj.GetComponent<Transform>().localPosition += Vector3.down * 2f;
                }

            }
        }
    }
    /// <summary>
    /// 更换相机高亮组件
    /// </summary>
    /// <param name="isRoam"></param>
    private void ChangeHightRender(bool isRoam)
    {
        HighlightingRenderer render = Camera.main.GetComponent<HighlightingRenderer>();
        if (render) render.enabled = !isRoam;
        HighlightingRenderer renderRoam = FPSobj.GetComponentInChildren<HighlightingRenderer>(false);
        if (renderRoam) renderRoam.enabled = isRoam;
    }
    /// <summary>
    /// 进入飞行模式后操作栏的改变
    /// </summary>
    public void EntranceFlight()
    {
        PromptWindow.transform.GetChild(0).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(1).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(2).gameObject.SetActive(false );
        PromptWindow.transform.GetChild(3).gameObject.SetActive(true );
        PromptWindow.transform.GetChild(5).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(6).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(4).gameObject.SetActive(false );
        PromptWindow.transform.GetChild(7).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(8).gameObject.SetActive(true);
    }

    /// <summary>
    /// 进入漫游后的操作栏改变
    /// </summary>
    public void EntranceRoam()
    {
       
        PromptWindow.transform.GetChild(0).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(1).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(2).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(3).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(4).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(5).gameObject.SetActive(true );
        PromptWindow.transform.GetChild(6).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(7).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(8).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "进入飞行";
        OnCursonStateChange(true);
    }
    /// <summary>
    /// 退出漫游时操作栏的改变
    /// </summary>
    public void ExitRoam()
    {
        PromptWindow.transform.GetChild(0).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(1).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(2).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(3).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(5).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(4).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(6).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(7).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(8).gameObject.SetActive(false);
    }

   

    /// <summary>
    /// 进入室内操作栏改变
    /// </summary>
    public void EntranceIndoorRoam()
    {
        PromptWindow.transform.GetChild(0).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(1).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(2).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(3).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(5).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(4).gameObject.SetActive(true);
        PromptWindow.transform.GetChild(6).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(7).gameObject.SetActive(false);
        PromptWindow.transform.GetChild(8).gameObject.SetActive(true);
    }

    private List<bool> promptWindowStateList = new List<bool>();
    /// <summary>
    /// 锁定/解锁鼠标时，提示栏的状态
    /// </summary>
    /// <param name="isLock"></param>
    public void ChangeCursorState(bool isLock)
    {
        Debug.LogError("RoamMange.ChangeCursorState:"+isLock);
        Transform promtWindowT = PromptWindow.transform;
        if (!isLock)
        {
            promptWindowStateList.Clear();
            for (int i=0;i< promtWindowT.childCount;i++)
            {
                Transform child = promtWindowT.GetChild(i);
                if(child!=null)promptWindowStateList.Add(child.gameObject.activeSelf);
            }
        }
        if (promptWindowStateList.Count < 9) return;
        promtWindowT.GetChild(0).gameObject.SetActive(isLock?promptWindowStateList[0]:false);
        promtWindowT.GetChild(1).gameObject.SetActive(isLock ? promptWindowStateList[1] : false);
        promtWindowT.GetChild(2).gameObject.SetActive(isLock ? promptWindowStateList[2] : false);
        promtWindowT.GetChild(3).gameObject.SetActive(isLock ? promptWindowStateList[3] : false);
        promtWindowT.GetChild(4).gameObject.SetActive(isLock ? promptWindowStateList[4] : false);
        promtWindowT.GetChild(5).gameObject.SetActive(true);//ESC       
        promtWindowT.GetChild(6).gameObject.SetActive(isLock ? promptWindowStateList[6] : false);
        promtWindowT.GetChild(7).gameObject.SetActive(isLock ? promptWindowStateList[7] : false);
        promtWindowT.GetChild(8).gameObject.SetActive(true);
        OnCursonStateChange(isLock);
    }
    /// <summary>
    /// 鼠标锁定、解锁
    /// </summary>
    /// <param name="isLock"></param>
    private void OnCursonStateChange(bool isLock)
    {
        if (FPSMode.Instance && FPSMode.Instance.CenterImage != null) FPSMode.Instance.CenterImage.gameObject.SetActive(isLock);
        if (isLock)
        {            
            PromptWindow.transform.GetChild(8).GetComponentInChildren<Text>().text = "解锁鼠标";
        }
        else
        {
            PromptWindow.transform.GetChild(8).GetComponentInChildren<Text>().text = "锁定鼠标";
        }
    }
    /// <summary>
    /// 设置漫游室内灯光
    /// </summary>
    /// <param name="b"></param>
    public void SetLight(bool b )
    {
        if (b)
        {
            Light.SetActive(true);
        }
        else
        {
            Light.SetActive(false);
        }
    }
    /// <summary>
    /// 打开无飞行模式的提示框
    /// </summary>
    public void ShowPromptBox()
    {
       PromptBox.SetActive(true);  
    }
    /// <summary>
    /// 关闭无飞行模式的提示框
    /// </summary>
    public void ClosePromptBox()
    {
        PromptBox.SetActive(false );
    }
    /// <summary>
    /// 进入室内漫游灯光和操作栏的改变
    /// </summary>
    /// <param name="b"></param>
    public void EntranceIndoor(bool b)
    {
        if (b)
        {
            isIndoor = true;
            EntranceIndoorRoam();
        }
        else
        {
            if(DevSubsystemManage.Instance.IsFPSInBuilding())
            {
                return;
            }
            isIndoor = false;
            EntranceRoam();
        }
    }
    public void EntranceRoamShowBox(bool b)
    {
        if (b)
        {
            PromptWindow.SetActive(true);
        
        }
        else
        {
            PromptWindow.SetActive(false );
          
        }
    }
}
