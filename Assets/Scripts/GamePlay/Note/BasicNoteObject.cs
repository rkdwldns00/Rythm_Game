using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoteObject : Note, IHitableNoteObject
{
    [SerializeField] bool isDetectTouchEnd = false;
    [SerializeField] float perfectTiming = 0.04f;
    [SerializeField] float greatTiming = 0.08f;
    [SerializeField] float goodTiming = 0.1f;
    [SerializeField] float badTiming = 0.13f;
    [SerializeField] AudioClip keySound;

    public float score { get; set; }
    float startX;
    float endX;

    private void Start()
    {

    }

    public bool CheckHit(int line)
    {
        TouchMode targetTouchMode = TouchMode.Start;
        if (isDetectTouchEnd) { targetTouchMode = TouchMode.End; }

        return Mathf.Abs(DistanceToHittingChecker) < badTiming
            && line + 1 >= startX && line - 1 <= endX - 1
            && HittingNoteChecker.instance.TouchDatas[line] == targetTouchMode;
    }

    public void Hit()
    {

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

        if (hitResult != HitResult.Miss)
        {
            if (keySound != null) SoundManager.PlaySound(keySound);
        }
        ComboManager.ProcessHitResult(hitResult, score);
        Destroy(gameObject);
    }

    public void SetData(SavedNoteData data)
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

public class SavedBasicNoteData : SavedNoteData, IGamePlaySummonable
{
    public override string serializedDataTitleName => "BN";
    public override float totalScore => 100;

    public virtual GameObject GamePlayNotePrefab
    {
        get
        {
            if (isCriticalNote)
            {
                return NoteManager.instance.criticalBasicNotePrefab;
            }
            else if (isHoldStartNote)
            {
                return NoteManager.instance.holdStartNotePrefab;
            }
            else
            {
                return NoteManager.instance.basicNotePrefab;
            }
        }
    }

    public int startX;
    public int endX;
    public bool isHoldStartNote = false;
    public bool isCriticalNote = false;

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        GameObject g = summoner.InstantiateNote(GamePlayNotePrefab, (startX + endX) / 2f, summoner.BeatToYpos(Beat));
        BasicNoteObject n = g.GetComponent<BasicNoteObject>();
        n?.SetData(this);

        return n;
    }

    public override MapEditorNote SummonMapEditorNote()
    {
        MapEditorBasicNote note = MapEditManager.Instance.SummonNote(MapEditManager.Instance.basicNotePrefab).GetComponent<MapEditorBasicNote>();
        note.beat = _beat;
        note.standardNoteValue = standardNoteValue;
        note.indexInBeat = indexInBeat;
        note.startX = startX;
        note.xSize = endX - startX;
        note.RefreshPosition();
        return note;
    }
}

public class SavedHoldEndNoteData : SavedBasicNoteData
{
    public override string serializedDataTitleName => "EN";

    public override GameObject GamePlayNotePrefab
    {
        get
        {
            if (isCriticalNote)
            {
                return NoteManager.instance.criticalHoldEndNotePrefab;
            }
            else
            {
                return NoteManager.instance.holdEndNotePrefab;
            }
        }
    }
}