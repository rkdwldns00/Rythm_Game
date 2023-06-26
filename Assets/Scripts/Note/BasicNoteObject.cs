using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoteObject : Note, IHitableNoteObject
{
    float startX;
    float endX;

    private void Start()
    {

    }

    public bool CheckHit(int line)
    {
        return Mathf.Abs(DistanceToHittingChecker) < 0.13f
            && line + 1 >= startX && line - 1 <= endX - 1
            && HittingNoteChecker.instance.TouchDatas[line] == TouchMode.Start;
    }

    public void Hit()
    {
        float t = Mathf.Abs(DistanceToHittingChecker);
        if (t <= 0.04f)
        {
            HitResultShower.ShowHitResult(HitResult.Perfect);
        }
        else if (t <= 0.08f)
        {
            HitResultShower.ShowHitResult(HitResult.Great);
        }
        else if (t <= 0.1f)
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
    public override GameObject NotePrefab => NoteManager.instance.basicNotePrefab;

    public float startX;
    public float endX;

    public Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        BasicNoteObject n = null;

        SavedBasicNoteData basic = data as SavedBasicNoteData;
        if (basic != null)
        {
            GameObject g = summoner.InstantiateNote(data.NotePrefab, (basic.startX + basic.endX) / 2f, summoner.BeatToYpos(basic.whenSummonBeat));
            n = g.GetComponent<BasicNoteObject>();
            n?.SetData(data);
        }

        return n;
    }
}