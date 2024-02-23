using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorTraceNote : MapEditorHaveXposNote
{
    public override SavedNoteData GetNoteData()
    {
        return new SavedTraceNoteData()
        {
            startX = startX,
            endX = startX + xSize,
            Beat = beat,
            standardNoteValue = standardNoteValue,
            indexInBeat = indexInBeat,
            isCriticalNote = false
        };
    }
}
