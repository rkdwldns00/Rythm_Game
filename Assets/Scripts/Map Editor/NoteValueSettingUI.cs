using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteValueSettingUI : MonoBehaviour
{
    public InputField noteValueInput;
    
    public void OpenUI()
    {
        noteValueInput.text = MapEditManager.Instance.NoteValue.ToString();
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        MapEditManager.Instance.NoteValue = int.Parse(noteValueInput.text);
        gameObject.SetActive(false);
    }
}
