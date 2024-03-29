public class SavedMeterChangerNoteData : SavedNoteData
{
    public override string serializedDataTitleName => "MD";
    public override float totalScore => 0;

    public int beatPerBar;
    public int meter2;

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        return null;
    }

    public override MapEditorNote SummonMapEditorNote()
    {
        MapEditorMeterChangerNote note = MapEditManager.Instance.SummonNote(MapEditManager.Instance.meterChangerNotePrefab).GetComponent<MapEditorMeterChangerNote>();
        note.Meter1 = beatPerBar;
        note.Meter2 = meter2;
        note.RefreshPosition();
        return note;
    }
}
