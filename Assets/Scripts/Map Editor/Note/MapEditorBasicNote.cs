using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorBasicNote : MapEditorHaveXposNote
{
    public override SavedNoteData GetNoteData()
    {
        return new SavedBasicNoteData()
        {
            startX = startX,
            endX = startX + xSize,
            Beat = beat,
            standardNoteValue = standardNoteValue,
            indexInBeat = indexInBeat,
            isCriticalNote = false
        };
    }

    public override void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        base.OnStopHolding(inputPos, holdingSpaceLocalPosition);
        Debug.Log(indexInBeat + "/" + standardNoteValue);
    }
}
