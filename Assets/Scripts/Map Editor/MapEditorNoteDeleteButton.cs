using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorNoteDeleteButton : MapEditorNoteInfoUI<MapEditorNote>
{
    protected override void OnActive()
    {

    }

    public void OnClick()
    {
        MapEditManager.Instance.SelectMapEditorNote(null);
        referencedNote.DeleteNote();
    }
}
