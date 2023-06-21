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

    public override void CheckHit(int line)
    {
        float t = NoteManager.instance.mapTimer - executeDelay;
        if (Mathf.Abs(t) < 0.1f && line + 1 >= startX && line - 1 <= endX - 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Log.text = line + ", " + startX + ", " + endX;
        }
    }

    public override void SetData(SavedNoteData data)
    {
        SavedBasicNoteData basic = (SavedBasicNoteData)data;
        transform.localScale = new Vector3(basic.endX - basic.startX, NoteManager.NOTE_Y_SIZE, 1);
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