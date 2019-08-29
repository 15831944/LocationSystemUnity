using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureWindow : MonoBehaviour {
    public static PictureWindow Instance;
    public GameObject PictureWindowUI;
    public Image Picture;
    public Button ClosePicture;
    public Sprite TransperantBack;
    private void Awake()
    {
        Instance = this;
    } 
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
