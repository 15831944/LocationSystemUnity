using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkInfoTypeTween : MonoBehaviour
{
    public GameObject InfoType;//统计数据类型
    public GameObject BaseImage;//底部信息栏图片
    public CanvasGroup BaseImageGroup;
    public GameObject ArrowDot;//箭头中间的点
    public GameObject ArrowUp;//上箭头
    public GameObject ArrowDown;//朝下箭头
    public GameObject ArrowTog;
    Color ArrowDotColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 102 / 255f);

    public static ParkInfoTypeTween Instance;
    public GameObject Person;
    public GameObject Pos;
    public CanvasGroup personGroup;
    public CanvasGroup posGroup;
    public GameObject Dev;
    public CanvasGroup DevGroup;
    public GameObject DevAlarm;
    public CanvasGroup DevAlarmGroup;
    public GameObject Image1;
    public GameObject Image2;
    public GameObject Image3;
    public GameObject image4;
    public GameObject Window;
    public Sequence objInfoCloseSequence;
    public Sequence objInfoAppearSequence;
    public Toggle   devType;
    public Toggle  perType;
   

    // Use this for initialization
    void Start()
    {
        Instance = this;
        SetObjCloseAndDisappearTween();
        ChangeArrowDotColor();
        ObjOpenAndAppearTween();
    }
    /// <summary>
    /// 点击鼠标箭头改变
    /// </summary>
    public void ChangeArrowDotColor()
    {
        EventTriggerListener ArrowDotColor = EventTriggerListener.Get(ArrowTog);
        ArrowDotColor.onEnter = Arrow_Up;
        ArrowDotColor.onExit = Arrow_Exit;


    }
    /// <summary>
    /// 鼠标放上后
    /// </summary>
    /// <param name="ArrowDot"></param>
    public void Arrow_Up(GameObject image)
    {
        ArrowDot.GetComponent<Image>().color = Color.white;
        if (ParkInformationManage.Instance.ArrowTog.isOn == true)
        {
           // ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 51 / 255f);
            ArrowUp.SetActive(true);
            ArrowUp.GetComponent<Image>().color = Color.white;
            ArrowDown.SetActive(false);
        }
        else
        {
            ArrowUp.SetActive(false);
            ArrowDown.SetActive(true);
            ArrowDown.GetComponent<Image>().color = Color.white;
        }


    }
    /// <summary>
    /// 鼠标离开
    /// </summary>
    /// <param name="ArrowDot"></param>
    public void Arrow_Exit(GameObject image)
    {
        ArrowDot.GetComponent<Image>().color = ArrowDotColor;

        ArrowUp.GetComponent<Image>().color = ArrowDotColor;
        ArrowUp.SetActive(false);
        ArrowDown.GetComponent<Image>().color = ArrowDotColor;
    }

    public void Play()
    {
        objInfoCloseSequence.Restart();
        perType.interactable = false;
        devType.interactable = false;
    }
    public void PlayBack()
    {
        objInfoAppearSequence.Restart();

        perType.interactable = true ;
        devType.interactable = true ;
    }
    public void SetObjCloseAndDisappearTween()
    {
        objInfoCloseSequence = DOTween.Sequence();
        ArrowUp.SetActive(false);
        ArrowDown.SetActive(false);

        ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        float DevAlarmY = DevAlarm.transform.GetComponent<RectTransform>().localPosition.y +20;
        Tween DevAlarmOpenTween = DevAlarm.transform.GetComponent<RectTransform>().DOLocalMoveY(DevAlarmY, 1f).SetEase(Ease.OutBack);
        Tween DevAlarmDisappear = DOTween.To(() => DevAlarmGroup.alpha, x => DevAlarmGroup.alpha = x, 0, 0.15f);

        float posY = Pos.transform.GetComponent<RectTransform>().localPosition.y + 20;
        Tween PosOpenTween = Pos.transform.GetComponent<RectTransform>().DOLocalMoveY(posY, 1f).SetEase(Ease.OutBack);
        Tween PosOpenDisappear = DOTween.To(() => posGroup.alpha, x => posGroup.alpha = x, 0, 0.15f);

        float devY = Dev.transform.GetComponent<RectTransform>().localPosition.y + 20;
        Tween DevOpenTween = Dev.transform.GetComponent<RectTransform>().DOLocalMoveY(devY, 1f).SetEase(Ease.OutBack);
        Tween DevOpenDisappear = DOTween.To(() => DevGroup.alpha, x => DevGroup.alpha = x, 0, 0.15f);

        float perY = Person.transform.GetComponent<RectTransform>().localPosition.y + 20;
        Tween PersonOpenTween = Person.transform.GetComponent<RectTransform>().DOLocalMoveY(perY, 1f).SetEase(Ease.OutBack);
        Tween PersonDisappear = DOTween.To(() => personGroup.alpha, x => personGroup.alpha = x, 0, 0.06f);

        float BaseImageY = BaseImage.transform.GetComponent<RectTransform>().localPosition.y + 475;
        Tween BaseImageTween = BaseImage.transform.GetComponent<RectTransform>().DOLocalMoveY(BaseImageY, 0.30f);
        Tween BaseImageDisappear = DOTween.To(() => BaseImageGroup.alpha, x => BaseImageGroup.alpha = x, 0, 0.30f);

        float Image1Y = Image1.transform.GetComponent<RectTransform>().localPosition.y + 490;
        Tween Image1Tween = Image1.transform.GetComponent<RectTransform>().DOLocalMoveY(Image1Y, 0.34f);

        float Image2Y = Image2.transform.GetComponent<RectTransform>().localPosition.y + 490;
        Tween Image2Tween = Image2.transform.GetComponent<RectTransform>().DOLocalMoveY(Image2Y, 0.32f);

        float Image3Y = Image3.transform.GetComponent<RectTransform>().localPosition.y + 490;
        Tween Image3Tween = Image3.transform.GetComponent<RectTransform>().DOLocalMoveY(Image3Y, 0.32f);

        float ArrowTogY = ArrowTog.transform.GetComponent<RectTransform>().localPosition.y + 490;
        Tween ArrowTogTween = ArrowTog.transform.GetComponent<RectTransform>().DOLocalMoveY(ArrowTogY, 0.30f);
        Tween ArrowTogRotate = ArrowTog.transform.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 360), 0.16f).SetLoops(-1, LoopType.Yoyo);

        float image4Y = image4.transform.GetComponent<RectTransform>().localPosition.y + 490;
        Tween image4Tween = image4.transform.GetComponent<RectTransform>().DOLocalMoveY(image4Y, 0.4f);

        Vector2 sizeDelta = Window.transform.GetComponent<RectTransform>().sizeDelta;       
        Tween WindowHighTween = Window.transform.GetComponent<RectTransform>().DOSizeDelta(new Vector2(sizeDelta.x,100),0.46f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            ArrowTogRotate.Rewind();
            ArrowTogRotate.Kill();
            ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 51 / 255f);
            ArrowUp.SetActive(true);
            ArrowDown.SetActive(true);
        });


        objInfoCloseSequence.SetAutoKill(false);
        objInfoCloseSequence.Pause();
        objInfoCloseSequence.Rewind();

        objInfoCloseSequence.Append(DevAlarmOpenTween);
        objInfoCloseSequence.Join(DevAlarmDisappear);
        objInfoCloseSequence.Join(ArrowTogRotate);

        objInfoCloseSequence.Insert(0.05f, PosOpenTween);
        objInfoCloseSequence.Join(PosOpenDisappear);

        objInfoCloseSequence.Insert(0.15f, DevOpenTween);
        objInfoCloseSequence.Join(DevOpenDisappear);

        objInfoCloseSequence.Insert(0.2f, BaseImageTween);
        objInfoCloseSequence.Join(BaseImageDisappear);
        objInfoCloseSequence.Join(Image1Tween);
        objInfoCloseSequence.Join(Image2Tween);
        objInfoCloseSequence.Join(Image3Tween);
        objInfoCloseSequence.Join(image4Tween);
        objInfoCloseSequence.Join(ArrowTogTween);
        objInfoCloseSequence.Join(WindowHighTween);

        objInfoCloseSequence.Insert(0.25f, PersonOpenTween);
        objInfoCloseSequence.Join(PersonDisappear);

        
    }

    public void ObjOpenAndAppearTween()
    {
        objInfoAppearSequence = DOTween.Sequence();


        ArrowUp.SetActive(false);
        ArrowDown.SetActive(false);
        ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255/ 255f);
        float DevAlarmY = DevAlarm.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween devAlarmOpenTwen = DevAlarm.transform.GetComponent<RectTransform>().DOLocalMoveY(DevAlarmY, 1f);
        Tween devAlarmAppear = DOTween.To(() => DevAlarmGroup.alpha, x => DevAlarmGroup.alpha = x, 1, 0.15f);

        float posY = Pos.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween PosOpenTwen = Pos.transform.GetComponent<RectTransform>().DOLocalMoveY(posY, 1f);
        Tween PosAppear = DOTween.To(() => posGroup.alpha, x => posGroup.alpha = x, 1, 0.15f);


        float devY = Dev.transform.GetComponent<RectTransform>().localPosition.y +0;
        Tween DevOpenTwen = Dev.transform.GetComponent<RectTransform>().DOLocalMoveY(devY, 1f);
        Tween DevAppear = DOTween.To(() => DevGroup.alpha, x => DevGroup.alpha = x, 1, 0.15f);

        float perY = Person.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween PersonOpenTwen = Person.transform.GetComponent<RectTransform>().DOLocalMoveY(perY, 1f);
        Tween PersonAppear = DOTween.To(() => personGroup.alpha, x => personGroup.alpha = x, 1, 0.15f);

        float BaseImageY = BaseImage.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween BaseImageOpenTwen = BaseImage.transform.GetComponent<RectTransform>().DOLocalMoveY(BaseImageY, 0.28f);
        Tween BaseImageAppear = DOTween.To(() => BaseImageGroup.alpha, x => BaseImageGroup.alpha = x, 1, 0.28f);

        float Image1Y = Image1.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween Image1OpenTwen = Image1.transform.GetComponent<RectTransform>().DOLocalMoveY(Image1Y, 0.32f);

        float Image2Y = Image2.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween Image2OpenTwen = Image2.transform.GetComponent<RectTransform>().DOLocalMoveY(Image2Y, 0.30f);

        float Image3Y = Image3.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween Image3OpenTwen = Image3.transform.GetComponent<RectTransform>().DOLocalMoveY(Image3Y, 0.30f);

        float ArrowTogY = ArrowTog.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween ArrowTogOpenTwen = ArrowTog.transform.GetComponent<RectTransform>().DOLocalMoveY(ArrowTogY, 0.28f);
        Tween ArrowTogRotate = ArrowTog.transform.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 360), 0.16f).SetLoops(-1, LoopType.Yoyo);

        float image4Y = image4.transform.GetComponent<RectTransform>().localPosition.y + 0;
        Tween image4OpenTwen = image4.transform.GetComponent<RectTransform>().DOLocalMoveY(image4Y, 0.32f);

        Vector2 sizeDelta = Window.transform.GetComponent<RectTransform>().sizeDelta;
        Tween WindowHighTween = Window.transform.GetComponent<RectTransform>().DOSizeDelta(new Vector2(sizeDelta.x, 588), 0.46f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            ArrowTogRotate.Rewind();
            ArrowTogRotate.Kill();
            ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 51 / 255f);
            ArrowUp.SetActive(true);
            ArrowDown.SetActive(true);
        });

        objInfoAppearSequence.Rewind();
        objInfoAppearSequence.SetAutoKill(false);
        objInfoAppearSequence.Pause();
        objInfoAppearSequence.Append(Image1OpenTwen);
        objInfoAppearSequence.Join(Image2OpenTwen);
        objInfoAppearSequence.Join(Image3OpenTwen);
        objInfoAppearSequence.Join(image4OpenTwen);
        objInfoAppearSequence.Join(ArrowTogOpenTwen);
        objInfoAppearSequence.Join(WindowHighTween);
        objInfoAppearSequence.Join(BaseImageOpenTwen);
        objInfoAppearSequence.Join(BaseImageAppear);

        objInfoAppearSequence.Join(devAlarmOpenTwen);
        //objInfoAppearSequence.Insert(0.1f,devAlarmOpenTwen);
        objInfoAppearSequence.Join(devAlarmAppear);

        objInfoAppearSequence.Join(PosOpenTwen);
        //objInfoAppearSequence.Insert(0.2f, PosOpenTwen);
        objInfoAppearSequence.Join(PosAppear);

        objInfoAppearSequence.Join(DevOpenTwen);
        //objInfoAppearSequence.Insert(0.25f, DevOpenTwen);
        objInfoAppearSequence.Join(DevAppear);

        objInfoAppearSequence.Join(PersonOpenTwen);
        //objInfoAppearSequence.Insert(0.3f, PersonOpenTwen);
        objInfoAppearSequence.Join(PersonAppear);

    }
   
}
