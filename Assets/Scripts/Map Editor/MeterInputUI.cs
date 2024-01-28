using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterInputUI : MonoBehaviour
{
    public InputField meter1;
    public InputField meter2;

    private MapEditorMeterChangerNote referencedNote;

    public void OpenUI(MapEditorMeterChangerNote note)
    {
        referencedNote = note;
        meter1.text = referencedNote.Meter1.ToString();
        meter2.text = referencedNote.Meter2.ToString();
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        referencedNote.Meter1 = int.Parse(meter1.text);
        referencedNote.Meter2 = int.Parse(meter2.text);
        gameObject.SetActive(false);
    }
}
