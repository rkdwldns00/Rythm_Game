using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public GameObject criticalBasicNotePrefab;
    public GameObject holdNotePrefab;
    public GameObject holdEndNotePrefab;
    public GameObject criticalHoldEndNotePrefab;
    public GameObject flickNotePrefab;
    public GameObject criticalFlickNotePrefab;

    public float mapTimer => Time.time - mapStartTime;

    float mapStartTime;
    List<Transform> noteListeners = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 120;

        SavedMapData map = new SavedMapData()
        {
            startBpm = 60,
            name = "테스트곡",
            notes = new SavedNoteData[]
            {
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=16},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=17},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=18},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=19},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=20},
                new SavedBPMChangeNoteData(){whenSummonBeat=21,bpm=240},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=21},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=22},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=23},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=24},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=25},
                new SavedBPMChangeNoteData(){whenSummonBeat=26,bpm=60},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=26},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=27},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=28},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=29},
                new SavedCriticalHoldEndNoteData() {startX=1, endX=5,whenSummonBeat=30},
            }
        };

        new NoteSummoner(map, field).SummmonMap();
    }

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
        IHitableNoteObject hittedHitableNote = null;

        noteListeners.RemoveAll((x) => x == null);

        foreach (Transform noteTransform in noteListeners)
        {
            Note note = noteTransform.GetComponent<Note>();
            IHitableNoteObject hitableNote = note as IHitableNoteObject;

            if (note is null)
            {
                continue;
            }

            if (hitableNote is not null && hitableNote.CheckHit(line) && (hittedNote is null || note.whenExecuteTime < hittedNote.whenExecuteTime))
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
    public int whenSummonBeat;

    public abstract Note Summon(NoteSummoner summoner, SavedNoteData data);
}

public abstract class Note : MonoBehaviour
{
    public float whenExecuteTime { get; set; }

    //판정선에 갈때까지 걸리는 시간
    public float DistanceToHittingChecker => NoteManager.instance.mapTimer - whenExecuteTime;
}

public interface IHitableNoteObject
{
    //현재 작동해아되는 상태인지 확인
    public bool CheckHit(int line);

    //이 노트를 작동시키는 함수
    public void Hit();
}

public interface ISummonable
{
    public GameObject NotePrefab { get; }

    //public Note Summon(NoteSummoner summoner, SavedNoteData data);
}

public class NoteSummoner
{
    readonly SavedMapData map;
    readonly Transform field;
    public float curruntBpm;
    public float curruntNoteDownSpeed = 30f;

    public float beatToSec(float beat)
    {
        float curruntBpm = map.startBpm;
        List<SavedBPMChangeNoteData> bpmChangers = new List<SavedBPMChangeNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedBPMChangeNoteData bpm = note as SavedBPMChangeNoteData;
            if (bpm is not null)
            {
                bpmChangers.Add(bpm);
            }
        }

        if (bpmChangers.Count > 0)
        {
            bpmChangers.Sort((x, y) => { return x.whenSummonBeat - y.whenSummonBeat; });

            int lastBpmChangerIndex = -1;
            for (int i = 0; i < bpmChangers.Count; i++)
            {
                if (bpmChangers[i].whenSummonBeat <= beat)
                {
                    lastBpmChangerIndex = i;
                }
            }

            float beatHis = 0;
            float sumTime = 0;
            for (int i = 0; i < lastBpmChangerIndex + 1; i++)
            {
                sumTime += 60f / (float)NoteManager.MAXIMUM_BEAT * 4f / curruntBpm * (bpmChangers[i].whenSummonBeat - beatHis);
                beatHis = bpmChangers[i].whenSummonBeat;
                curruntBpm = bpmChangers[i].bpm;
            }
            sumTime += 60f / (float)NoteManager.MAXIMUM_BEAT * 4f / curruntBpm * (beat - beatHis);
            return sumTime;
        }
        else
        {
            return 60f / (float)NoteManager.MAXIMUM_BEAT * 4f / curruntBpm * beat;
        }

    }

    public NoteSummoner(SavedMapData map, Transform field)
    {
        this.map = map;
        curruntBpm = map.startBpm;
        this.field = field;
    }

    public void SummmonMap()
    {
        foreach (SavedNoteData note in map.notes)
        {
            Note noteObject = note.Summon(this, note);

            if (noteObject != null)
            {
                noteObject.whenExecuteTime = beatToSec(note.whenSummonBeat);
            }

        }
    }

    public float BeatToYpos(float beat)
    {
        return beatToSec(beat) * curruntNoteDownSpeed;
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = UnityEngine.Object.Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 7, yPos + NoteManager.NOTE_CHECK_YPOS, -0.01f);
        NoteManager.instance.AddNoteDownListener(g.transform);
        return g;
    }
}