using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpeedSlider : SliderValueShower
{
    void Start()
    {
        SetSliderValue(NoteManager.UserSettingNoteDownSpeed);
    }

    public void SetNoteSpeed(float speed)
    {
        NoteManager.UserSettingNoteDownSpeed = speed;
        SetSliderValue(speed);
    }
}
