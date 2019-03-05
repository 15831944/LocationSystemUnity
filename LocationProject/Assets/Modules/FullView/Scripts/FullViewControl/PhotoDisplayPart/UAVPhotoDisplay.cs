using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UAVPhotoDisplay : MonoBehaviour {
    /// <summary>
    /// 主厂区图片
    /// </summary>
    public List<Sprite> mainFacotrySprites = new List<Sprite>();
    /// <summary>
    /// 锅炉区图片
    /// </summary>
    public List<Sprite> boilerPartSprites = new List<Sprite>();
    /// <summary>
    /// 水处理区图片
    /// </summary>
    public List<Sprite> waterTreatmentSprites = new List<Sprite>();
    /// <summary>
    /// 气能源区图片
    /// </summary>
    public List<Sprite> gasEnergySprites = new List<Sprite>();
    /// <summary>
    /// 生活区图片
    /// </summary>
    public List<Sprite> livingPartSprites = new List<Sprite>();
    /// <summary>
    /// 整厂图片
    /// </summary>
    public List<Sprite> fullFactorySprites = new List<Sprite>();
    /// <summary>
    /// 照片显示载体
    /// </summary>
    public Image ImageContent;
    /// <summary>
    /// 显示上一张照片
    /// </summary>
    public Button showLastButton;
    /// <summary>
    /// 显示下一张照片
    /// </summary>
    public Button showNextButton;
    /// <summary>
    /// 图片下标文本框
    /// </summary>
    public Text imageIndexText;
    ///// <summary>
    ///// 图片表面闪烁
    ///// </summary>
    //public GridTwinkle TwinklePart;
    /// <summary>
    /// 当前显示照片下标
    /// </summary>
    private int currentSpriteIndex;
    /// <summary>
    /// 当前显示中的Sprite
    /// </summary>
    private List<Sprite> currentSprits = new List<Sprite>();
    /// <summary>
    /// 每张照片显示间隔时间
    /// </summary>
    private WaitForSeconds gapTime = new WaitForSeconds(3f);
    /// <summary>
    /// 当前展示区域
    /// </summary>
    private FullViewPart currentPart;

    // Use this for initialization
    void Awake () {
        SceneEvents.FullViewPartChange += OnFullViewPartChange;
        showLastButton.onClick.AddListener(ShowLast);
        showNextButton.onClick.AddListener(ShowNext);

    }
	void OnDestroy()
    {
        SceneEvents.FullViewPartChange -= OnFullViewPartChange;
    }

    private void OnFullViewPartChange(FullViewPart part)
    {
        currentPart = part;
        switch (part)
        {
            case FullViewPart.LivingQuarters:
                DisplayImageAuto(livingPartSprites,ImageContent);
                //Debug.Log(part);
                break;
            case FullViewPart.MainBuilding:
                DisplayImageAuto(mainFacotrySprites, ImageContent);
                //Debug.Log(part);
                break;
            case FullViewPart.BoilerRoom:
                DisplayImageAuto(boilerPartSprites, ImageContent);
                //Debug.Log(part);
                break;
            case FullViewPart.WaterTreatmentArea:
                DisplayImageAuto(waterTreatmentSprites, ImageContent);
                //Debug.Log(part);
                break;
            case FullViewPart.GasEnergyArea:
                DisplayImageAuto(gasEnergySprites, ImageContent);
                //Debug.Log(part);
                break;
            case FullViewPart.FullFactory:
                DisplayImageAuto(fullFactorySprites, ImageContent);
                break;
            default:
                Debug.LogError("Error:UAVPhotoDisplay.OnFullViewPartChange,part not find."+part);
                break;
        }
    }
    /// <summary>
    /// 自动展示区域照片
    /// </summary>
    private void DisplayImageAuto(List<Sprite> sprites, Image displayContent)
    {
        //StopAllCoroutines();
        if (sprites != null && sprites.Count != 0)
        {
            currentSpriteIndex = -1;
            currentSprits = sprites;
            displayNextImage(sprites, displayContent);
        }
        else
        {
            Debug.LogError("Error:UAVPhotoDisplay sprites is null!");
        }
       
    }
    /// <summary>
    /// 显示上一张图片
    /// </summary>
    private void displayLastImage(List<Sprite> MSprites, Image displayContent)
    {
        //DoImageTwinkle();
        currentSpriteIndex--;
        currentSpriteIndex = currentSpriteIndex < 0 ? MSprites.Count-1: currentSpriteIndex;
        //Debug.Log(string.Format("Sprites count:{0}.Current sprites index:{1}.", MSprites.Count, currentSpriteIndex));
        displayContent.overrideSprite = MSprites[currentSpriteIndex];
        int indexShow = currentSpriteIndex + 1;
        ShowImageIndex(indexShow, MSprites.Count);
    }
    /// <summary>
    /// 显示下一张照片
    /// </summary>
    /// <param name="MSprites"></param>
    /// <param name="displayContent"></param>
    private void displayNextImage(List<Sprite> MSprites, Image displayContent)
    {
        //DoImageTwinkle();
        currentSpriteIndex++;
        currentSpriteIndex = currentSpriteIndex > MSprites.Count - 1 ? 0 : currentSpriteIndex;
        //Debug.Log(string.Format("Sprites count:{0}.Current sprites index:{1}.", MSprites.Count, currentSpriteIndex));
        displayContent.overrideSprite = MSprites[currentSpriteIndex];
        //显示下标，不是从0开始
        int indexShow = currentSpriteIndex + 1;
        ShowImageIndex(indexShow, MSprites.Count);
    }
    ///// <summary>
    ///// 图片切换动画
    ///// </summary>
    //private void DoImageTwinkle()
    //{
    //    if (IsInvoking("TwinkleStop")) CancelInvoke("TwinkleStop");
    //    if(!TwinklePart.IsTwinkle)
    //    {
    //        TwinklePart.StartImageTwinkle();
    //    }
    //    Invoke("TwinkleStop",1f);
    //}
    //private void TwinkleStop()
    //{
    //    TwinklePart.StopImageTwinkle();
    //}
    /// <summary>
    /// 显示下一张照片
    /// </summary>
    private void ShowNext()
    {
        //停止当前自动展示流程，重新开始
        if(currentSprits!=null&&currentSprits.Count!=0)
        {
            displayNextImage(currentSprits, ImageContent);
        }      
    }
    /// <summary>
    /// 显示上一张照片
    /// </summary>
    private void ShowLast()
    {     
        if (currentSprits != null && currentSprits.Count != 0)
        {
            displayLastImage(currentSprits,ImageContent);
        }
    }
    private string livingQuatersName = "生活区";
    private string mainBuildingName = "主厂区";
    private string boilerRoomName = "锅炉区";
    private string waterTreatmentName = "水处理区";
    private string gasEnergyName = "气能源区";
    private string fullFactoryName = "整体园区";
    /// <summary>
    /// 显示当前照片下标
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="totalCount"></param>
    private void ShowImageIndex(int currentIndex,int totalCount)
    {
        string indexFormat = "{0}--{1}/{2}";
        switch (currentPart)
        {
            case FullViewPart.LivingQuarters:
                imageIndexText.text = string.Format(indexFormat, livingQuatersName,currentIndex,totalCount);
                break;
            case FullViewPart.MainBuilding:
                imageIndexText.text = string.Format(indexFormat, mainBuildingName, currentIndex, totalCount);
                break;
            case FullViewPart.BoilerRoom:
                imageIndexText.text = string.Format(indexFormat, boilerRoomName, currentIndex, totalCount);
                break;
            case FullViewPart.WaterTreatmentArea:
                imageIndexText.text = string.Format(indexFormat, waterTreatmentName, currentIndex, totalCount);
                break;
            case FullViewPart.GasEnergyArea:
                imageIndexText.text = string.Format(indexFormat, gasEnergyName, currentIndex, totalCount);
                break;
            case FullViewPart.FullFactory:
                imageIndexText.text = string.Format(indexFormat,fullFactoryName,currentIndex,totalCount);
                break;
            default:
                imageIndexText.text = "";
                Debug.LogError("Error:UAVPhotoDisplay.OnFullViewPartChange,part not find." + currentPart);
                break;
        }
    }
}
