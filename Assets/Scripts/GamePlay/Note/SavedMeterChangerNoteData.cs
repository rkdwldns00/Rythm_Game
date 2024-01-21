using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedMeterChangerNoteData : SavedNoteData
{
    public int beatPerBar;
    public float beatLengthRate;

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        return null;
    }
}
