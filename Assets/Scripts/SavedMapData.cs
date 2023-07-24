using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

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
}

public abstract class SavedNoteData
{
    public int whenSummonBeat;

    public abstract Note Summon(NoteSummoner summoner, SavedNoteData data);
}