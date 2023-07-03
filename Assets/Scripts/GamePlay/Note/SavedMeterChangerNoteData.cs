using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedMeterChangerNoteData : SavedNoteData
{
    public float beatPerBar;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        return null;
    }
}
