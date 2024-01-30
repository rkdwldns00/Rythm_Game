using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq;

[Serializable]
public class SavedMapData
{
    public string title;

    public string artistName;
    public string designerName;

    public AudioClip bgm;
    public Sprite thumnail;

    public float startOffset;
    public float startBpm;
    public SavedNoteData[] notes;

    public void Sort()
    {
        List<SavedNoteData> list = notes.ToList();
        list.Sort((a, b) => (a.whenSummonBeat - b.whenSummonBeat));
        notes = list.ToArray();
    }
}

[Serializable]
public abstract class SavedNoteData
{
    public int whenSummonBeat;
    public abstract string serializedDataTitleName { get; }

    public abstract Note SummonGamePlayNote(NoteSummoner summoner);
    public abstract MapEditorNote SummonMapEditorNote();
}