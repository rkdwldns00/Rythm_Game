public class SavedMeterChangerNoteData : SavedNoteData
{
    public override string serializedDataTitleName => "MD";

    public int beatPerBar;
    public float beatLengthRate;

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        return null;
    }

    public override MapEditorNote SummonMapEditorNote()
    {
        MapEditorMeterChangerNote note = MapEditManager.Instance.SummonNote(MapEditManager.Instance.meterChangerNotePrefab).GetComponent<MapEditorMeterChangerNote>();
        note.Meter1 = beatPerBar;
        note.Meter2 = (int)(beatPerBar * beatLengthRate);
        note.RefreshPosition();
        return note;
    }
}
