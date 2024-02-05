using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorSpacingSlider : MapEditorNoteInfoUI<MapEditorNote>
{
    public Slider slider;

    private void Start()
    {
        slider.onValueChanged.AddListener(OnChangeValue);
    }

    public void OnChangeValue(float value)
    {
        MapEditManager.Instance.SetSpacing(value);
    }

    public override void OnSelectNote(MapEditorNote note)
    {
        if (note is null)
        {
            OnActive();
            gameObject.SetActive(true);
        }
        else
        {
            OnUnselectNote();
        }
    }

    protected override void OnActive()
    {
        slider.value = MapEditManager.Instance.notePosCalculator.spacing;
    }
}
