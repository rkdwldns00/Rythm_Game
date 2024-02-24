using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickNoteObject : Note, IHitableNoteObject
{
    [SerializeField] float perfectTiming = 0.04f;
    [SerializeField] float greatTiming = 0.08f;
    [SerializeField] float goodTiming = 0.1f;
    [SerializeField] float badTiming = 0.13f;
    [SerializeField] AudioClip keySound;

    float startX;
    float endX;
    float score;

    float rotation;

    bool isDetectedTouchStart = false;
    bool needTochStart;

    public bool CheckHit(int line)
    {
        if (
            (!needTochStart || (!isDetectedTouchStart && HittingNoteChecker.instance.TouchDatas[line] == TouchMode.Start))
            && Mathf.Abs(DistanceToHittingChecker) < badTiming)
        {
            isDetectedTouchStart = true;
        }

        Vector2 flickPower = HittingNoteChecker.instance.GetFlickPower(line);
        float flickPowerRotation = Mathf.Atan2(flickPower.y, flickPower.x) * Mathf.Rad2Deg;
        return
            isDetectedTouchStart
            && line + 1 >= startX && line - 1 <= endX - 1
            && HittingNoteChecker.instance.TouchDatas[line] == TouchMode.Hold
            && flickPower.magnitude > 5 && Mathf.Abs(rotation - flickPowerRotation) <= 30f;
    }

    public void Hit()
    {
        if (keySound != null) SoundManager.PlaySound(keySound);

        float t = Mathf.Abs(DistanceToHittingChecker);
        HitResult hitResult = HitResult.Miss;
        if (t <= perfectTiming)
        {
            hitResult = HitResult.Perfect;
        }
        else if (t <= greatTiming)
        {
            hitResult = HitResult.Great;
        }
        else if (t <= goodTiming)
        {
            hitResult = HitResult.Good;
        }
        else if (t <= badTiming)
        {
            hitResult = HitResult.Bad;
        }
        ComboManager.ProcessHitResult(hitResult, score);
        Destroy(gameObject);
    }

    public void SetData(SavedFlickNoteData data)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (data.endX - data.startX) / rect.localScale.x);
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, NoteManager.NOTE_Y_SIZE / rect.localScale.y);
        startX = data.startX;
        endX = data.endX;
        rotation = data.rotation;
        needTochStart = data.needTouchStart;
        score = data.totalScore;
    }
}

public class SavedFlickNoteData : SavedNoteData, IGamePlaySummonable
{
    public override string serializedDataTitleName => "FN";
    public override float totalScore => 0;

    public virtual GameObject GamePlayNotePrefab
    {
        get
        {
            if (isCriticalNote)
            {
                return NoteManager.instance.criticalFlickNotePrefab;
            }
            else
            {
                return NoteManager.instance.flickNotePrefab;
            }
        }
    }

    public int startX;
    public int endX;

    public float rotation;
    public bool needTouchStart = true;

    public bool isCriticalNote = false;

    public override MapEditorNote SummonMapEditorNote()
    {
        MapEditorFlickNote note = MapEditManager.Instance.SummonNote(MapEditManager.Instance.flickNotePrefab).GetComponent<MapEditorFlickNote>();
        note.beat = _beat;
        note.standardNoteValue = standardNoteValue;
        note.indexInBeat = indexInBeat;
        note.startX = startX;
        note.xSize = endX - startX;
        note.RefreshPosition();
        return note;
    }

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        FlickNoteObject n = null;

        GameObject g = summoner.InstantiateNote(GamePlayNotePrefab, (this.startX + this.endX) / 2f, summoner.BeatToYpos(this.Beat));
        n = g.GetComponent<FlickNoteObject>();
        n?.SetData(this);

        return n;
    }
}