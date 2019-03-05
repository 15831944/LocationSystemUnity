using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FulViewAutoDisplay : MonoBehaviour
{
    private FullViewController controller;
    /// <summary>
    /// 是否开启自动展示
    /// </summary>
    public bool IsOn;
    /// <summary>
    /// 自动流程间隔时间
    /// </summary>
    private WaitForSeconds WaitTime = new WaitForSeconds(10f);
    /// <summary>
    /// 记录上一次鼠标点击时间
    /// </summary>
    private DateTime LastMouseClickTime;
    // Use this for initialization
    void Awake()
    {      
        SceneEvents.FullViewStateChange += OnViewChange;
        if (IsOn)
        {           
            SceneEvents.FullViewPartChange += OnFullViewPartChange;
        }
    }
    void OnDestroy()
    {
        SceneEvents.FullViewStateChange -= OnViewChange;
        if (IsOn)
        {
            SceneEvents.FullViewPartChange -= OnFullViewPartChange;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(IsOn)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                if (controller.IsFullView)
                {
                    LastMouseClickTime = DateTime.Now;
                }
            }
        }       
    }
    /// <summary>
    /// 全景模式下，对焦建筑改变触发
    /// </summary>
    /// <param name="part"></param>
    private void OnFullViewPartChange(FullViewPart part)
    {
        double IntervalTime = (DateTime.Now - LastMouseClickTime).TotalSeconds;
        if (IntervalTime < 1)
        {
            //Debug.Log("Mouse Input Detected,Stop Corutine.");
            StopAllCoroutines();
            StartCoroutine(restartDisplayFromCurrentState());
        }
    }
    /// <summary>
    /// 是否进入全景模式
    /// </summary>
    /// <param name="isFullView"></param>
    private void OnViewChange(bool isFullView)
    {
        if (isFullView)
        {
            CloseToggleSwitchOff();
            if(IsOn)
            {
                AutoDisplayStart();
            }
            else
            {
                ShowFullFactory();
            }
            
        }
        else
        {
            StopAllCoroutines();
            setAllToggleOff();
        }
    }
    /// <summary>
    /// 设置toggle组，不能全部关闭
    /// </summary>
    private void CloseToggleSwitchOff()
    {
        FullViewController controller = FullViewController.Instance;
        controller.fullViewUI.livingQuaterToggle.group.allowSwitchOff = false;
    }
    /// <summary>
    /// 退出时，关闭所有Toggle
    /// </summary>
    private void setAllToggleOff()
    {
        FullViewController controller = FullViewController.Instance;
        controller.fullViewUI.livingQuaterToggle.group.allowSwitchOff = true;
        if(controller.fullViewUI.livingQuaterToggle.isOn)controller.fullViewUI.livingQuaterToggle.isOn = false;
        if (controller.fullViewUI.mainBuildingToggle.isOn) controller.fullViewUI.mainBuildingToggle.isOn = false;
        if (controller.fullViewUI.boilerRoomToggle.isOn) controller.fullViewUI.boilerRoomToggle.isOn = false;
        if (controller.fullViewUI.waterTreatmentToggle.isOn) controller.fullViewUI.waterTreatmentToggle.isOn = false;
        if (controller.fullViewUI.gasEnergyToggle.isOn) controller.fullViewUI.gasEnergyToggle.isOn = false;
        if (controller.fullViewUI.fullFactoryToggle.isOn) controller.fullViewUI.fullFactoryToggle.isOn = false;
    }
    /// <summary>
    /// 不开启自动展示，默认对焦整厂
    /// </summary>
    private void ShowFullFactory()
    {
        if (controller == null) controller = FullViewController.Instance;
        controller.fullViewUI.fullFactoryToggle.isOn = true;
    }
    /// <summary>
    /// 自动脚本开始
    /// </summary>
    private void AutoDisplayStart()
    {
        StartCoroutine(fullFactoryCorutine());
    }
    /// <summary>
    /// 自动流程被打断，从当前状态重新开始
    /// </summary>
    /// <returns></returns>
    IEnumerator restartDisplayFromCurrentState()
    {
        Debug.Log("curret part:"+controller.CurrentPart);
        yield return WaitTime;
        switch(controller.CurrentPart)
        {
            case FullViewPart.LivingQuarters:
                StartCoroutine(mainBuildingCorutine());
                break;
            case FullViewPart.MainBuilding:
                StartCoroutine(boilerRoomCorutine());
                break;
            case FullViewPart.BoilerRoom:
                StartCoroutine(waterTreatmentCorutine());
                break;
            case FullViewPart.WaterTreatmentArea:
                StartCoroutine(gasEnergyCorutine());
                break;
            case FullViewPart.GasEnergyArea:
                StartCoroutine(lvingQuaterCorutine());
                break;
            case FullViewPart.FullFactory:
                StartCoroutine(fullFactoryCorutine());
                break;
            default:
                Debug.LogError("Error:UAVPhotoDisplay.OnFullViewPartChange,part not find." + controller.CurrentPart);
                break;
        }
    }
    IEnumerator fullFactoryCorutine()
    {
        controller.fullViewUI.fullFactoryToggle.isOn = true;
        yield return WaitTime;
        StartCoroutine(mainBuildingCorutine());
    }
    /// <summary>
    /// 自动区域展示协程
    /// </summary>
    /// <returns></returns>
    IEnumerator lvingQuaterCorutine()
    {
        Debug.Log("lvingQuaterCorutine");
        controller.fullViewUI.livingQuaterToggle.isOn = true;       
        yield return WaitTime;
        StartCoroutine(fullFactoryCorutine());
    }
    IEnumerator mainBuildingCorutine()
    {       
        controller.fullViewUI.mainBuildingToggle.isOn = true;
        yield return WaitTime;
        StartCoroutine(boilerRoomCorutine());
    }
    IEnumerator boilerRoomCorutine()
    {
        controller.fullViewUI.boilerRoomToggle.isOn = true;
        yield return WaitTime;
        StartCoroutine(waterTreatmentCorutine());
    }
    IEnumerator waterTreatmentCorutine()
    {
        controller.fullViewUI.waterTreatmentToggle.isOn = true;
        yield return WaitTime;
        StartCoroutine(gasEnergyCorutine());
    }
    IEnumerator gasEnergyCorutine()
    {
        controller.fullViewUI.gasEnergyToggle.isOn = true;
        yield return WaitTime;
        StartCoroutine(lvingQuaterCorutine());
    }
}
