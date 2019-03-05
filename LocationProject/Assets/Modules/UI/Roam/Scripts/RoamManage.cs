using DG.Tweening;
using HighlightingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

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
                    FPSController.ChangeGravityValue(0f);
                    FPSController.ChangeWalkSpeed(25f);
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


            }
            if (Input .GetKeyDown(KeyCode.LeftShift))
            {
                FPSController.ChangeWalkSpeed(5f);
            }else
            {
                FPSController.ChangeWalkSpeed(1.6f);
            }
        }

        if (FPSobj.gameObject.activeInHierarchy)
        {
            Transform flight = PromptWindow.transform.GetChild(3).GetChild(0);

            if (Input.GetKeyDown(KeyCode.F))//进入飞行模式
            {
                if (isIndoor)
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
                            flight.GetComponent<Text>().text = "退出飞行模式";
                            isStart = false;
                        }
                        else
                        {
                          //  is_Fly = true;
                            ////FPSobj.GetComponent<Transform>().DOLocalMoveY(1f, 1.2f).SetEase(Ease.InOutQuint).OnComplete(() =>
                            ////{
                            FPSController.ChangeWalkSpeed(1.6f);
                            FPSController.IsSpaceState = true;
                              //  is_Fly = false;
                         //   });
                            EntranceRoam();
                            flight.GetComponent<Text>().text = "进入飞行模式";
                            FPSController.ChangeGravityValue(30f);
                           
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
        PromptWindow.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "进入飞行模式";
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
        isIndoor = b;
        if (b)
        {
            //SetLight(true);
            EntranceIndoorRoam();
        }
        else
        {
            if(DevSubsystemManage.Instance.IsFPSInBuilding())
            {
                //Debug.LogError("FPS still in building...");
                return;
            }
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
