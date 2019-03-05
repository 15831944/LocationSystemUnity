using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCameraItem : MonoBehaviour {

    public Toggle Cameratoggle;
    void Start () {

        Cameratoggle = GetComponent<Toggle>();
       
    }
	
	
	void Update () {
		
	}
    
    public void OnValueChange()
    {
        Transform lineExample = selectedCamera.Instance.CameraGrid.transform;
        if (selectedCamera.Instance.CameraType.Count == 1)
        {
            lineExample.GetChild(0).gameObject.SetActive(true);
            lineExample.GetChild(1).gameObject.SetActive(false);
            lineExample.GetChild(2).gameObject.SetActive(false);
            lineExample.GetChild(3).gameObject.SetActive(false);
            lineExample.GetChild(4).gameObject.SetActive(false);
            gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
            gameObject.GetComponent<Toggle>().isOn = true;
            
        }
        if (gameObject.GetComponent<Toggle>().isOn == true)
        {
            selectedCamera.Instance.AddCameraType(this.gameObject);
            gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
            if (selectedCamera.Instance.CameraType.Count == 1)
            {
                lineExample.GetChild(0).gameObject.SetActive(true);
              
            }
            if (selectedCamera.Instance.CameraType.Count == 2)
            {
              
                lineExample.GetChild(0).gameObject.SetActive(false);
                lineExample.GetChild(1).gameObject.SetActive(true);
                lineExample.GetChild(1).gameObject.transform.localPosition = new Vector2(-202.6f, 131);
                lineExample.GetChild(2).gameObject.SetActive(true);
                lineExample.GetChild(2).gameObject.transform.localPosition = new Vector2(204.5f, 131);
            }
            if (selectedCamera.Instance.CameraType.Count == 3)
            {

                lineExample.transform.GetChild(3).gameObject.SetActive(true);
                lineExample.GetChild(3).gameObject.transform.localPosition = new Vector2(-202.6f, -131);

            }
            if (selectedCamera.Instance.CameraType.Count == 4)
            {

                lineExample.GetChild(4).gameObject.SetActive(true);
                lineExample.GetChild(4).gameObject.transform.localPosition = new Vector2(204.5f, -131);

            }
        }
        else
        {
            selectedCamera.Instance.ClearCameraType(gameObject);
            gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
            if (selectedCamera.Instance.CameraType.Count == 1)
            {

                lineExample.GetChild(0).gameObject.SetActive(true);
                lineExample.GetChild(1).gameObject.SetActive(false);
                lineExample.GetChild(2).gameObject.SetActive(false);
                lineExample.GetChild(3).gameObject.SetActive(false);
                lineExample.GetChild(4).gameObject.SetActive(false);


            }

            if (selectedCamera.Instance.CameraType.Count == 2)
            {
                lineExample.GetChild(0).gameObject.SetActive(false);
                lineExample.GetChild(1).gameObject.SetActive(true);
               
                lineExample.GetChild(2).gameObject.SetActive(true);
               
                lineExample.GetChild(3).gameObject.SetActive(false );
                lineExample.GetChild(4).gameObject.SetActive(false);

            }
            if (selectedCamera.Instance.CameraType.Count == 3)
            {
                lineExample.GetChild(1).gameObject.SetActive(true);
                lineExample.GetChild(2).gameObject.SetActive(true);
                lineExample.GetChild(3).gameObject.SetActive(true );
                lineExample.GetChild(4).gameObject.SetActive(false );
            }
            
        }
       
    }
    
}
