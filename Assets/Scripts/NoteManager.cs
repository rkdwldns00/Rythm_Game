using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    const int MAXIMUM_BEAT = 16;
    const float NOTE_SPAWN_YPOS = 30f;
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
        RuntimeMapData map = RuntimeMapData.BakeMap(new SavedMapData()
        {
            startBpm = 240,
            name = "Å×½ºÆ®°î",
            notes = new SavedNoteData[]
            {
                new BasicNoteObject.SavedBasicNoteData() {whenSummonBeat = 16, startX = 0, endX = 3},
                new BasicNoteObject.SavedBasicNoteData() {whenSummonBeat = 18, startX = 0, endX = 3},
                new BasicNoteObject.SavedBasicNoteData() {whenSummonBeat = 20, startX = 0, endX = 3},
                new BasicNoteObject.SavedBasicNoteData() {whenSummonBeat = 20, startX = 8, endX = 10},
            }
        });
        StartCoroutine(SummmonMap(map));
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

    IEnumerator SummmonMap(RuntimeMapData map)
    {
        float bitToSec = 60f / (float)MAXIMUM_BEAT * 4f / map.startBpm;

        foreach (RuntimeNoteData node in map.notes)
        {
            if (node.afterDelayBeat > 0)
            {
                yield return new WaitForSeconds(bitToSec * node.afterDelayBeat);
            }

            if (node.note != null) node.note();
        }
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos)
    {
        GameObject g = Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 6, NOTE_SPAWN_YPOS, 0);
        return g;
    }

    public void AddNoteDownListener(Transform listener)
    {
        noteDownListeners.Add(listener);
    }

    class RuntimeMapData
    {
        public readonly float startBpm;
        public readonly RuntimeNoteData[] notes;

        RuntimeMapData(float startBpm, RuntimeNoteData[] notes)
        {
            this.startBpm = startBpm;
            this.notes = notes;
        }

        public static RuntimeMapData BakeMap(SavedMapData saveData)
        {
            List<RuntimeNoteData> bakedNotes = new List<RuntimeNoteData>();

            List<SavedNoteData> notes = new List<SavedNoteData>();
            foreach (SavedNoteData note in saveData.notes)
            {
                notes.Add(note);
            }

            notes.Sort((x, y) => x.whenSummonBeat - y.whenSummonBeat);

            int his = 0;

            foreach (SavedNoteData note in notes)
            {
                bakedNotes.Add(new RuntimeNoteData(note.Bake(), note.whenSummonBeat - his));
                his = note.whenSummonBeat;
            }

            return new(saveData.startBpm, bakedNotes.ToArray());
        }
    }

    public class RuntimeNoteData
    {
        public readonly int afterDelayBeat;
        public readonly Action note;

        public RuntimeNoteData(Action note, int afterDelayBeat)
        {
            this.afterDelayBeat = afterDelayBeat;
            this.note = note;
        }
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

    public abstract Action Bake();
}