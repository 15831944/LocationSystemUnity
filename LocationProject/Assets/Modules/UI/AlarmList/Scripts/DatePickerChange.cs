using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DatePickerChange : UIBehaviour
{

    public Text _dateText = null;
    public CalendarChange _calendar = null;
 
    private DateTime _dateTime = DateTime.Today;
    // get data from this property
    public DateTime DateTime
    {
        get { return _dateTime; }
        set
        {
            _dateTime = value;
            refreshDateText();
        }
    }

    protected override void Awake()
    {
        Init();
    }

    private bool inited = false;

    private void Init()
    {
        if (inited)
        {
            return;
        }
        inited = true;
        if (_dateText == null)
        {
            _dateText = this.transform.Find("DateText").GetComponent<Text>();
            if (_dateText == null)
            {
                Log.Error("DatePickerChange.Awake", "_dateText == null:" + this); ;
            }
        }

        if (_calendar == null)
        {
            _calendar = this.transform.Find("Calendar").GetComponent<CalendarChange>();
            if (_calendar == null)
            {
                Log.Error("DatePickerChange.Awake", "_calendar == null:" + this); ;
            }
        }

        //_calendar.onDayClick.AddListener(dateTime => { DateTime = dateTime; });
        _calendar.onDayClick.AddListener(dateTime
            =>
        {
            if (dateTime > DateTime.Now)
            {
                DateTime = DateTime.Now;
            }
            else
            {
                DateTime = dateTime;
            }
        });
        transform.Find("PickButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!_calendar.gameObject.activeInHierarchy)
            {
                _calendar.gameObject.SetActive(true);
            }
            else
            {
                _calendar.gameObject.SetActive(false);
            }
        });
        refreshDateText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            //Debug.LogError("PointerEventData:" + pointerEventData.pointerPress.name);
            //Debug.LogError("EventSystem:" + EventSystem.current.currentSelectedGameObject.name);

            GameObject ui = EventSystem.current.currentSelectedGameObject;
            if (ui == null)
            {
                _calendar.gameObject.SetActive(false);
                return;
            }
            DatePickerChange datePickerChange = ui.GetComponentInParent<DatePickerChange>();
            if (datePickerChange == null)
            {
                _calendar.gameObject.SetActive(false);
            }
        }


    }

    private void refreshDateText()
    {
        if (_calendar == null)
        {
            Init();
        }
        if (_calendar == null)
        {
            Log.Error("DatePickerChange.refreshDateText", "_calendar == null:" + this); ;
            return;
        }
        if (_calendar.DisplayType == E_DisplayType.Standard)
        {
            switch (_calendar.CalendarType)
            {
                case E_CalendarType.Day:
                    _dateText.text = DateTime.ToShortDateString();
                    break;
                case E_CalendarType.Month:
                    _dateText.text = DateTime.Year + "/" + DateTime.Month;
                    break;
                case E_CalendarType.Year:
                    _dateText.text = DateTime.Year.ToString();
                    break;
            }
        }
        else
        {
            switch (_calendar.CalendarType)
            {
                case E_CalendarType.Day:
                    _dateText.text = DateTime.Year + "年" + DateTime.Month + "月" + DateTime.Day + "日";
                    break;
                case E_CalendarType.Month:
                    _dateText.text = DateTime.Year + "年" + DateTime.Month + "月";
                    break;
                case E_CalendarType.Year:
                    _dateText.text = DateTime.Year + "年";
                    break;
            }
        }
        _calendar.gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        _calendar.gameObject.SetActive(false);
    }
}
