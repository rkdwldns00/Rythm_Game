using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSummoner : NotePosCalculator
{
    readonly Transform field;

    public NoteSummoner(SavedMapData map, Transform field, float noteDownSpeed) : base(noteDownSpeed, map)
    {
        this.field = field;
    }

    public void SummmonMap()
    {
        foreach (SavedNoteData note in map.notes)
        {
            Note noteObject = note.SummonGamePlayNote(this);

            if (noteObject != null)
            {
                noteObject.whenExecuteTime = BeatToSec(note.Beat);
            }

        }
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = Object.Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 7, yPos + NoteManager.NOTE_CHECK_YPOS, -0.01f);
        NoteManager.instance.AddNoteDownListener(g.transform);
        return g;
    }
}

