using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceNoteObject : Note
{
    [SerializeField] protected AudioClip keySound;

    public float score { get; set; }
    protected float startX;
    protected float endX;

    private void Update()
    {
        if (DistanceToHittingChecker < 0)
        {
            int startLine = Mathf.FloorToInt(startX);
            int endLine = Mathf.FloorToInt(endX);

            bool isTouch = false;
            for (int i = Mathf.Max(0, startLine); i <= Mathf.Min(HittingNoteChecker.TOUCH_LINE_COUNT - 1, endLine); i++)
            {
                if (HittingNoteChecker.instance.TouchDatas[i] == TouchMode.Hold)
                {
                    isTouch = true;
                    break;
                }
            }

            if (isTouch)
            {
                ComboManager.ProcessHitResult(HitResult.Perfect, score);
            }
            else
            {
                ComboManager.ProcessHitResult(HitResult.Miss, score);
            }
        }
    }

    public virtual void SetData(SavedNoteData data)
    {
        SavedBasicNoteData basic = (SavedBasicNoteData)data;
        RectTransform rect = GetComponent<RectTransform>();
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (basic.endX - basic.startX) / rect.localScale.x);
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, NoteManager.NOTE_Y_SIZE / rect.localScale.y);
        startX = basic.startX;
        endX = basic.endX;
        score = basic.totalScore;
    }
}

public class SavedTraceNoteData : SavedBasicNoteData
{
    public override string serializedDataTitleName => "TN";
    public override float totalScore => 50;

    public override GameObject GamePlayNotePrefab => NoteManager.instance.traceNotePrefab;

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        GameObject g = summoner.InstantiateNote(GamePlayNotePrefab, (startX + endX) / 2f, summoner.BeatToYpos(Beat));
        TraceNoteObject n = g.GetComponent<TraceNoteObject>();
        n?.SetData(this);

        return n;
    }

    public override MapEditorNote SummonMapEditorNote()
    {
        MapEditorTraceNote note = MapEditManager.Instance.SummonNote(MapEditManager.Instance.basicNotePrefab).GetComponent<MapEditorTraceNote>();
        note.beat = _beat;
        note.standardNoteValue = standardNoteValue;
        note.indexInBeat = indexInBeat;
        note.startX = startX;
        note.xSize = endX - startX;
        note.RefreshPosition();
        return note;
    }
}