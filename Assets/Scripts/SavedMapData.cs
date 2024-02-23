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
        list.Sort((a, b) => (int)Mathf.Sign(a.Beat - b.Beat));
        notes = list.ToArray();
    }
}

[Serializable]
public abstract class SavedNoteData
{
    //JSON¶§¹®¿¡ public
    public int _beat = 0;
    public float Beat
    {
        get
        {
            return _beat + ((float)indexInBeat / ((float)standardNoteValue / 4f));
        }
        set
        {
            _beat = (int)value;
        }
    }

    public int standardNoteValue = 1;
    public int indexInBeat = 0;

    public abstract string serializedDataTitleName { get; }
    public abstract float totalScore { get; }

    public abstract Note SummonGamePlayNote(NoteSummoner summoner);
    public abstract MapEditorNote SummonMapEditorNote();
}