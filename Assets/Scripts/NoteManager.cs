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
    public float mapTimer => Time.time - mapStartTime;

    float mapStartTime;
    List<Transform> noteListeners = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;

        SavedMapData map = new SavedMapData()
        {
            startBpm = 240,
            name = "테스트곡",
            notes = new SavedNoteData[]
            {
                new SavedBasicNoteData() {whenSummonBeat = 54, startX = 1, endX = 3},
                new SavedBasicNoteData() {whenSummonBeat = 58, startX = 3, endX = 5},
                new SavedBasicNoteData() {whenSummonBeat = 62, startX = 5, endX = 7},
                new SavedBasicNoteData() {whenSummonBeat = 66, startX = 7, endX = 9},
            }
        };

        map.notes = new SavedNoteData[1000];

        for (int i = 0; i < map.notes.Length; i++)
        {
            int a = UnityEngine.Random.Range(1, 12);
            int b = -1;
            do
            {
                b = UnityEngine.Random.Range(1, 12);
            } while (a == b);
            map.notes[i] = new SavedBasicNoteData() { whenSummonBeat = i * 2, startX = Mathf.Min(a, b), endX = Mathf.Max(a, b) };
        }

        SummmonMap(map);
    }

    // Update is called once per frame
    void Update()
    {
        noteListeners.RemoveAll((x) => x == null);
        foreach (Transform t in noteListeners)
        {
            t.localPosition += Time.deltaTime * noteDownSpeed * Vector3.down;
        }
    }

    void SummmonMap(SavedMapData map)
    {
        mapStartTime = Time.time;
        float bitToSec = 60f / (float)MAXIMUM_BEAT * 4f / map.startBpm;

        foreach (SavedNoteData note in map.notes)
        {
            SavedBasicNoteData basic = note as SavedBasicNoteData;
            GameObject g = InstantiateNote(basicNotePrefab, (basic.startX + basic.endX) / 2f, basic.whenSummonBeat * bitToSec * noteDownSpeed + NOTE_CHECK_YPOS);
            g.GetComponent<Note>().SetData(note);
            if (basic != null)
            {
                g.GetComponent<Note>().whenExecuteTime = bitToSec * note.whenSummonBeat;
            }
        }
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 7, yPos, 0);
        AddNoteDownListener(g.transform);
        return g;
    }

    public void AddNoteDownListener(Transform listener)
    {
        noteListeners.Add(listener);
    }

    public void HitCheck(int line)
    {
        Note hittedNote = null;

        noteListeners.RemoveAll((x) => x == null);

        foreach (Transform noteTransform in noteListeners)
        {
            Note note = noteTransform.GetComponent<Note>();

            if (note is null)
            {
                continue;
            }

            if (note.CheckHit(line) && (hittedNote is null || note.whenExecuteTime < hittedNote.whenExecuteTime))
            {
                hittedNote = note;
            }
        }

        hittedNote?.Hit();
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

public abstract class Note : MonoBehaviour
{
    public float whenExecuteTime;

    //판정선에 갈때까지 걸리는 시간
    public float DistanceToHittingChecker => NoteManager.instance.mapTimer - whenExecuteTime;

    public abstract void SetData(SavedNoteData data);
    public abstract bool CheckHit(int line);
    public abstract void Hit();
}