using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class selectedCamera : MonoBehaviour
{
    public static selectedCamera Instance;
    public GameObject firstCameraType;
    public List<GameObject> CameraType;
   
    public GameObject  CameraGrid;
    void Start()
    {
        Instance = this;
        CameraType = new List<GameObject>();
        CameraType.Add(firstCameraType);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
       
    
    public void AddCameraType(GameObject  camera)
    {
        if (CameraType.Contains(camera))
        {
            return;
        }else
        {
            if (CameraType.Count < 5)
            {
                CameraType.Add(camera);
                if (CameraType.Count == 5)
                {
                    CameraType[0].transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
                    CameraType.RemoveAt(0);
                }
            }
        }
        
            
        
        }
    public void ClearCameraType(GameObject camera)
    {

        if (CameraType.Count >1)
        {
            CameraType.Remove(camera);
        }
       
    }

}
