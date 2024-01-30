using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterChangerNoteInfoUI : MapEditorNoteInfoUI<MapEditorMeterChangerNote>
{
    [SerializeField] MeterInputUI meterInputUI;

    protected override void OnActive()
    {
        
    }

    public void OnClick()
    {
        meterInputUI.OpenUI(referencedNote);
    }
}
