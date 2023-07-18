using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Map", menuName = "Scriptable Object/Map Data", order = int.MinValue)]
public class SavedMapData : ScriptableObject
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

[Serializable]
public abstract class SavedNoteData : UnityEngine.Object
{
    public int whenSummonBeat;

    public abstract Note Summon(NoteSummoner summoner, SavedNoteData data);
}