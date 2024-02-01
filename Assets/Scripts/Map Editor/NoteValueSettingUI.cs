using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteValueSettingUI : MonoBehaviour
{
    public InputField noteValueInput;

    private void Awake()
    {
        noteValueInput.onEndEdit.AddListener((t) =>
        {
            OnEdit(int.Parse(t));
        });
    }

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

    private void OnEdit(int noteValue)
    {
        if (noteValue % 2 != 0)
        {
            int a = 4;
            while (a < noteValue)
            {
                a *= 2;
            }
            noteValueInput.text = a.ToString();
        }
    }
}
