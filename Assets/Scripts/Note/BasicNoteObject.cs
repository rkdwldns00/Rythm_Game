using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoteObject : Note
{
    float startX;
    float endX;

    private void Start()
    {

    }

    public override bool CheckHit(int line)
    {
        return Mathf.Abs(DistanceToHittingChecker) < 0.13f && line + 1 >= startX && line - 1 <= endX - 1;
    }

    public override void Hit()
    {
        float t = Mathf.Abs(DistanceToHittingChecker);
        if (t <= 0.04f)
        {
            Log.text = "PERFECT";
            Log.color = Color.cyan;
        }
        else if (t <= 0.08f)
        {
            Log.text = "GREAT";
            Log.color = Color.magenta;
        }
        else if (t <= 0.1f)
        {
            Log.text = "GOOD";
            Log.color = Color.yellow;
        }
        else
        {
            Log.text = "BAD";
            Log.color = Color.grey;
        }
        Destroy(gameObject);
    }

    public override void SetData(SavedNoteData data)
    {
        SavedBasicNoteData basic = (SavedBasicNoteData)data;
        RectTransform rect = GetComponent<RectTransform>();
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (basic.endX - basic.startX) / rect.localScale.x);
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, NoteManager.NOTE_Y_SIZE / rect.localScale.y);
        startX = basic.startX;
        endX = basic.endX;
    }
}

public class SavedBasicNoteData : SavedNoteData
{
    public override GameObject NotePrefab => NoteManager.instance.basicNotePrefab;

    public float startX;
    public float endX;
}