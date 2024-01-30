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
        if (string.IsNullOrEmpty(meter1.text))
        {
            referencedNote.Meter1 = 4;
        }
        else
        {
            int a = int.Parse(meter1.text);
            if (a > 0)
            {
                referencedNote.Meter1 = a;
            }
            else
            {
                referencedNote.Meter1 = 4;
            }
        }

        if (string.IsNullOrEmpty(meter2.text))
        {
            referencedNote.Meter2 = 4;
        }
        else
        {
            int b = int.Parse(meter2.text);
            if (b > 0)
            {
                referencedNote.Meter2 = b;
            }
            else
            {
                referencedNote.Meter2 = 4;
            }
        }

        MapEditManager.Instance.RefreshNotesPosition();
        gameObject.SetActive(false);
    }
}
