using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SavedBPMChangeNoteData : SavedNoteData
{
    public float bpm;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        return null;
    }
}
