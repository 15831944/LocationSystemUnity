using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RangeSlider))]
public class RangeSliderShowValues : MonoBehaviour
{

    [SerializeField]
    Text TextMin;
    [SerializeField]
    Text TextMax;

    RangeSlider slider;
    void Start()
    {
        slider = GetComponent<RangeSlider>();
        if (slider != null)
        {
            slider.OnValuesChange.AddListener(SliderChanged);
            SliderChanged(slider.ValueMin, slider.ValueMax);
        }
    }

    void SliderChanged(int min, int max)
    {
        if (TextMin != null)
        {
            //if (slider.WholeNumberOfSteps)
            //{
            //    TextMin.text = string.Format("Range: {0:000} - {1:000}; Step: {2}", min, max, slider.Step);
            //}
            //else
            //{
            TextMin.text = min.ToString();
            //}
        }

        if (TextMax != null)
        {
            TextMax.text = max.ToString();
        }
    }

    void OnDestroy()
    {
        if (slider != null)
        {
            slider.OnValuesChange.RemoveListener(SliderChanged);
        }
    }

}
