using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public const int MAXIMUM_BEAT = 16;
    public const float NOTE_CHECK_YPOS = -4.5f;
    public const float NOTE_Y_SIZE = 1f;

    public static NoteManager instance;

    public Transform field;
    float noteDownSpeed { get; set; } = 30f;
    public GameObject basicNotePrefab;
    public GameObject holdNotePrefab;

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
            startBpm = 60,
            name = "테스트곡",
            notes = new SavedNoteData[]
            {
                new SavedBasicNoteData() {whenSummonBeat = 4, startX = 1, endX = 3},
                new SavedBasicNoteData() {whenSummonBeat = 8, startX = 3, endX = 5},
                new SavedBasicNoteData() {whenSummonBeat = 12, startX = 5, endX = 7},
                new SavedBasicNoteData() {whenSummonBeat = 16, startX = 7, endX = 9},
                new SavedHoldNoteData() {whenSummonBeat = 17,curveData = new SavedHoldNoteCurve[]{
                    new SavedHoldNoteCurve(0,14,0),
                    new SavedHoldNoteCurve(5,9,5),
                    new SavedHoldNoteCurve(2,12,10),
                }},
            }
        };

        /*map.notes = new SavedNoteData[1000];

        for (int i = 0; i < map.notes.Length; i++)
        {
            int a = UnityEngine.Random.Range(1, 12);
            int b = -1;
            do
            {
                b = UnityEngine.Random.Range(1, 12);
            } while (a == b);
            map.notes[i] = new SavedBasicNoteData() { whenSummonBeat = i, startX = Mathf.Min(a, b), endX = Mathf.Max(a, b) };
        }*/

        new NoteSummoner(map, field).SummmonMap(map);
        //SummmonMap(map);
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

    public void AddNoteDownListener(Transform listener)
    {
        noteListeners.Add(listener);
    }

    public void HitCheck(int line)
    {
        Note hittedNote = null;
        HitableNote hittedHitableNote = null;

        noteListeners.RemoveAll((x) => x == null);

        foreach (Transform noteTransform in noteListeners)
        {
            Note note = noteTransform.GetComponent<Note>();
            HitableNote hitableNote = note as HitableNote;

            if (note is null)
            {
                continue;
            }

            if (hitableNote.CheckHit(line) && (hittedNote is null || note.whenExecuteTime < hittedNote.whenExecuteTime))
            {
                hittedNote = note;
                hittedHitableNote = hitableNote;
            }
        }

        hittedHitableNote?.Hit();
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
}

public interface HitableNote
{
    public bool CheckHit(int line);
    public void Hit();
}

class NoteSummoner
{
    Transform field;
    float curruntBpm;
    float curruntNoteDownSpeed = 30f;

    float beatToSec => 60f / (float)NoteManager.MAXIMUM_BEAT * 4f / curruntBpm;

    public NoteSummoner(SavedMapData map, Transform field)
    {
        curruntBpm = map.startBpm;
        this.field = field;
    }

    public void SummmonMap(SavedMapData map)
    {
        foreach (SavedNoteData note in map.notes)
        {
            Note noteObject = null;

            SavedBasicNoteData basic = note as SavedBasicNoteData;
            if (basic != null)
            {
                GameObject g = InstantiateNote(note.NotePrefab, (basic.startX + basic.endX) / 2f, BeatToYpos(basic.whenSummonBeat));
                BasicNoteObject n = g.GetComponent<BasicNoteObject>();
                noteObject = n;
                n.SetData(note);
            }

            SavedHoldNoteData hold = note as SavedHoldNoteData;
            if (hold != null && hold.curveData.Length > 1)
            {
                float startY = BeatToYpos(hold.whenSummonBeat);
                GameObject g = InstantiateNote(note.NotePrefab, 0, startY);
                HoldNoteObject n = g.GetComponent<HoldNoteObject>();
                noteObject = n;

                RuntimeHoldNoteCurve[] curves = new RuntimeHoldNoteCurve[hold.curveData.Length];
                for (int i = 0; i < curves.Length; i++)
                {
                    RuntimeHoldNoteCurve newCurve = new RuntimeHoldNoteCurve()
                    {
                        startX = hold.curveData[i].startX,
                        endX = hold.curveData[i].endX,
                        yPos = BeatToYpos(hold.curveData[i].spawnBeat)
                    };
                    curves[i] = newCurve;
                }

                n.Draw(curves);

                int start = (int)hold.curveData[0].spawnBeat;
                int length = (int)(hold.curveData[hold.curveData.Length - 1].spawnBeat - start);
                if (length > 2)
                {
                    float[] hitCheckTiming = new float[length - 2];

                    for (int i = 0; i < hitCheckTiming.Length; i++)
                    {
                        hitCheckTiming[i] = beatToSec * (i + 1);
                    }

                    n.SetHitCheckTiming(hitCheckTiming);
                }
            }

            if (noteObject != null)
            {
                noteObject.whenExecuteTime = beatToSec * note.whenSummonBeat;
            }
        }
    }

    public float BeatToYpos(float beat)
    {
        return beat * beatToSec * curruntNoteDownSpeed;
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = UnityEngine.Object.Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 7, yPos + NoteManager.NOTE_CHECK_YPOS, -0.01f);
        NoteManager.instance.AddNoteDownListener(g.transform);
        return g;
    }
}