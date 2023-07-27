using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetSlider : SliderValueShower
{
    void Start()
    {
        SetSliderValue(NoteManager.UserSettingOffset);
    }

    public void SetOffset(float offset)
    {
        NoteManager.UserSettingOffset = offset;
        SetSliderValue(offset);
    }
}
