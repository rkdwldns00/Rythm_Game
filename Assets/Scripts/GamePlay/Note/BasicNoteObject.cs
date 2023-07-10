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
        if (t <= perfectTiming)
        {
            HitResultShower.ShowHitResult(HitResult.Perfect);
        }
        else if (t <= greatTiming)
        {
            HitResultShower.ShowHitResult(HitResult.Great);
        }
        else if (t <= goodTiming)
        {
            HitResultShower.ShowHitResult(HitResult.Good);
        }
        else
        {
            HitResultShower.ShowHitResult(HitResult.Bad);
        }
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
    }
}

public class SavedBasicNoteData : SavedNoteData, ISummonable
{
    public virtual GameObject NotePrefab
    {
        get
        {
            if(isCriticalNote)
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

    public float startX;
    public float endX;
    public bool isHoldStartNote = false;
    public bool isCriticalNote = false;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        BasicNoteObject n = null;

        SavedBasicNoteData basic = data as SavedBasicNoteData;
        if (basic != null)
        {
            GameObject g = summoner.InstantiateNote(((ISummonable)data).NotePrefab, (basic.startX + basic.endX) / 2f, summoner.BeatToYpos(basic.whenSummonBeat));
            n = g.GetComponent<BasicNoteObject>();
            n?.SetData(data);
        }

        return n;
    }
}

public class SavedHoldEndNoteData : SavedBasicNoteData
{
    public override GameObject NotePrefab
    {
        get
        {
            if(isCriticalNote)
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