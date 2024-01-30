using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SavedBPMChangeNoteData : SavedNoteData
{
    public override string serializedDataTitleName => "BD";

    public float bpm;

    public override MapEditorNote SummonMapEditorNote()
    {
        throw new System.NotImplementedException();
    }

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        return null;
    }
}
