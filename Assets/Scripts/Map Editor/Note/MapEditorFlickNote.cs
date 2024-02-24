using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorFlickNote : MapEditorHaveXposNote
{
    public override SavedNoteData GetNoteData()
    {
        return new SavedFlickNoteData()
        {
            startX = startX,
            endX = startX + xSize,
            Beat = beat,
            standardNoteValue = standardNoteValue,
            indexInBeat = indexInBeat,
            needTouchStart = false,
            rotation = 0,
            isCriticalNote = false
        };
    }
}
