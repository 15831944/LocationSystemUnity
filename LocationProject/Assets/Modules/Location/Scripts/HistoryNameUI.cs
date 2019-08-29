using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryNameUI : MonoBehaviour {

    public ToggleButton3 CameraFollowToggleButton;//摄像机跟随控制

    // Use this for initialization
    void Start () {
        
        CameraFollowToggleButton.OnValueChanged = CameraFollowToggleButton_OnValueChanged;
        SetCameraFollowToggleButtonActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// CameraFollowToggleButton点击切换镜头跟随效果
    /// </summary>
    public void CameraFollowToggleButton_OnValueChanged(bool b)
    {
        if (b)
        {
            CameraSceneManager.Instance.SetTheThirdPersonCameraTrue();
        }
        else
        {
            CameraSceneManager.Instance.SetTheThirdPersonCameraFalse();
        }
    }

    public void SetCameraFollowToggleButtonActive(bool isActive)
    {
        CameraFollowToggleButton.gameObject.SetActive(isActive);

        //if (isActive)
        {
            CameraFollowToggleButton.SetToggle(isActive);//->CameraFollowToggleButton_OnValueChanged(true)->Cinemachine On 这里会切换摄像机控制权
        }
    }
}
