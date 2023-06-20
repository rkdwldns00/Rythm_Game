using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    const int MAXIMUM_BEAT = 16;
    const float NOTE_CHECK_YPOS = -4.5f;
    public const float NOTE_Y_SIZE = 1f;

    public static NoteManager instance;

    public Transform field;
    float noteDownSpeed { get; set; } = 30f;
    public GameObject basicNotePrefab;

    int currentBit = 0;
    List<Transform> noteDownListeners = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SavedMapData map = new SavedMapData()
        {
            startBpm = 240,
            name = "Å×½ºÆ®°î",
            notes = new SavedNoteData[]
            {
                new SavedBasicNoteData() {whenSummonBeat = 4, startX = 0, endX = 3},
                new SavedBasicNoteData() {whenSummonBeat = 8, startX = 0, endX = 3},
                new SavedBasicNoteData() {whenSummonBeat = 12, startX = 0, endX = 3},
                new SavedBasicNoteData() {whenSummonBeat = 16, startX = 8, endX = 10},
            }
        };
        SummmonMap(map);
    }

    // Update is called once per frame
    void Update()
    {
        noteDownListeners.RemoveAll((x) => x == null);
        foreach (Transform t in noteDownListeners)
        {
            t.localPosition += Time.deltaTime * noteDownSpeed * Vector3.down;
        }
    }

    void SummmonMap(SavedMapData map)
    {
        float bitToSec = 60f / (float)MAXIMUM_BEAT * 4f / map.startBpm;

        foreach (SavedNoteData note in map.notes)
        {
            SavedBasicNoteData basic = note as SavedBasicNoteData;
            if (basic != null)
            {
                GameObject g = InstantiateNote(basicNotePrefab, (basic.startX + basic.endX) / 2f, basic.whenSummonBeat * bitToSec * noteDownSpeed + NOTE_CHECK_YPOS);
                g.transform.localScale = new Vector3(basic.endX - basic.startX, NOTE_Y_SIZE, 1);
            }
        }
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 6, yPos, 0);
        AddNoteDownListener(g.transform);
        return g;
    }

    public void AddNoteDownListener(Transform listener)
    {
        noteDownListeners.Add(listener);
    }
}

public class SavedMapData
{
    public string name;
    public float startBpm;
    public SavedNoteData[] notes;
}

public abstract class SavedNoteData
{
    public abstract GameObject NotePrefab { get; }

    public int whenSummonBeat;
}