using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraVideoRtsp : MonoBehaviour {
    public static CameraVideoRtsp Instance;
    private Vector2 largeSize = new Vector2(1306,648);
    private Vector2 smallSize = new Vector2(400,225);

    private Transform lastParent;
    private GameObject rawImage;
    private int sblingIndex;

    public Transform imangeContainer;
    public GameObject window;
    public Button closeButton;
    public Text titleText;

    private Transform NormalParent;//非漫游状态的父物体

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
        if (closeButton) closeButton.onClick.AddListener(Close);
        NormalParent = transform.parent;
    }
    /// <summary>
    /// 恢复父物体
    /// </summary>
    public void RecoverParent()
    {
        if (NormalParent == null) return;
        gameObject.transform.parent = NormalParent;
    }
    /// <summary>
    /// 设置新的父物体
    /// </summary>
    /// <param name="newParent"></param>
    public void SetNewParent(Transform newParent)
    {
        gameObject.transform.parent = newParent;
    }
    public void Show(string title,GameObject rawImageTemp)
    {
        titleText.text = title;
        window.SetActive(true);
        rawImage = rawImageTemp;
        lastParent = rawImage.transform.parent;
        sblingIndex = rawImage.transform.GetSiblingIndex();
        rawImageTemp.transform.parent = imangeContainer;
        rawImageTemp.transform.localPosition = Vector3.zero;
        RectTransform rect = rawImageTemp.transform.GetComponent<RectTransform>();
        rect.sizeDelta = largeSize;
    }

    public void Close()
    {
        if (rawImage==null||lastParent == null) return; 
        rawImage.transform.parent = lastParent;
        rawImage.transform.localPosition = Vector3.zero;
        RectTransform rect = rawImage.transform.GetComponent<RectTransform>();
        rect.sizeDelta = smallSize;
        rawImage.transform.SetAsFirstSibling();
        window.SetActive(false);
    }
}
