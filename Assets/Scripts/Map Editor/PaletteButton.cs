using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteButton : MonoBehaviour
{
    public GameObject notePrefab;

    public void OnClick()
    {
        GameObject g = Instantiate(notePrefab);
        MapEditorNote note = g.GetComponent<MapEditorNote>();
        MapEditManager.Instance.StartHoldNote(note);
        MapEditManager.Instance.RegistEditorNote(note);
    }
}
