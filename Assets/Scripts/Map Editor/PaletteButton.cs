using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteButton : MonoBehaviour
{
    public GameObject notePrefab;

    public void OnClick()
    {
        GameObject g = Instantiate(notePrefab);
        MapEditManager.Instance.StartHoldNote(g.GetComponent<MapEditorNote>());
    }
}
