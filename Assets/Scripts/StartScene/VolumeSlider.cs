using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : SliderValueShower
{
    void Start()
    {
        SetSliderValue(SoundManager.Volume);
    }

    public void SetVolume(float volume)
    {
        SoundManager.Volume = volume;
        SetSliderValue(volume);
    }
}
