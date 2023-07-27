using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueShower : MonoBehaviour
{
    public Slider slider;
    public Text shower;

    public void ShowValue()
    {
        shower.text = (Mathf.Floor(slider.value * 10) / 10).ToString();
    }

    public void SetSliderValue(float value)
    {
        slider.value = Mathf.Floor(value * 10) / 10;
        ShowValue();
    }
}
