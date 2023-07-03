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
    public float noteDownSpeed => noteDownSpeedRate * userSettingNoteDownSpeed;
    public float noteDownSpeedRate { private get; set; } = 1f;
    public float userSettingNoteDownSpeed => 30f;
    public GameObject basicNotePrefab;
    public GameObject criticalBasicNotePrefab;
    public GameObject holdNotePrefab;
    public GameObject holdEndNotePrefab;
    public GameObject criticalHoldEndNotePrefab;
    public GameObject flickNotePrefab;
    public GameObject criticalFlickNotePrefab;
    public GameObject speedChangerPrefab;

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
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 4},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 5},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 6},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 7},
                new SavedMeterChangerNoteData{beatPerBar = 1,whenSummonBeat=7},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 8},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 9},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 10},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 11},
                new SavedMeterChangerNoteData{beatPerBar = 4,whenSummonBeat=11},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 12},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 13},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 14},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 15},

                new SavedBPMChangeNoteData(){bpm = 500,whenSummonBeat=18},
                new SavedBPMChangeNoteData(){bpm = 120,whenSummonBeat=100},

                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 104},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 105},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 106},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 107},
                new SavedMeterChangerNoteData{beatPerBar = 1,whenSummonBeat=107},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 108},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 109},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 110},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 111},
                new SavedMeterChangerNoteData{beatPerBar = 4,whenSummonBeat=111},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 112},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 113},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 114},
                new SavedBasicNoteData() {startX = 0, endX = 2,whenSummonBeat = 115},
            }
        };

        new NoteSummoner(map, field, 30).SummmonMap();
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
    readonly float userSettingNoteDownSpeed;

    public NoteSummoner(SavedMapData map, Transform field, float noteDownSpeed)
    {
        this.map = map;
        this.field = field;
        userSettingNoteDownSpeed = noteDownSpeed;
    }

    public void SummmonMap()
    {
        foreach (SavedNoteData note in map.notes)
        {
            Note noteObject = note.Summon(this, note);

            if (noteObject != null)
            {
                noteObject.whenExecuteTime = BeatToSec(note.whenSummonBeat);
            }

        }
    }

    public float BeatToSec(float beat)
    {
        float curruntBpm = map.startBpm;

        List<SavedBPMChangeNoteData> bpmChnagers = new List<SavedBPMChangeNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedBPMChangeNoteData bpm = note as SavedBPMChangeNoteData;
            if (bpm is not null)
            {
                bpmChnagers.Add(bpm);
            }
        }

        float sumTime = 0;
        for (int i = 0; i <= beat; i++)
        {
            if (bpmChnagers.Count > 0 && bpmChnagers[0].whenSummonBeat < i)
            {
                curruntBpm = bpmChnagers[0].bpm;
                bpmChnagers.RemoveAt(0);
            }
            sumTime += 60f / BeatPerBar(i) / curruntBpm;
        }
        return sumTime;
    }

    float BeatPerBar(float beat)
    {
        float curruntBeatPerBar = 4;
        List<SavedMeterChangerNoteData> meters = new List<SavedMeterChangerNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedMeterChangerNoteData meter = note as SavedMeterChangerNoteData;
            if (meter is not null)
            {
                meters.Add(meter);
            }
        }

        if (meters.Count > 0)
        {
            meters.Sort((x, y) => { return x.whenSummonBeat - y.whenSummonBeat; });

            int lastMeterChangerIndex = -1;
            for (int i = 0; i < meters.Count; i++)
            {
                if (meters[i].whenSummonBeat < beat)
                {
                    lastMeterChangerIndex = i;
                }
            }

            if (lastMeterChangerIndex >= 0)
            {
                curruntBeatPerBar = meters[lastMeterChangerIndex].beatPerBar;
            }
        }

        return curruntBeatPerBar;
    }

    public float BeatToYpos(float beat)
    {
        float curruntSpeed = 1f;
        List<SavedSpeedChangerNoteData> speedChangers = new List<SavedSpeedChangerNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedSpeedChangerNoteData changer = note as SavedSpeedChangerNoteData;
            if (changer is not null)
            {
                speedChangers.Add(changer);
            }
        }

        if (speedChangers.Count > 0)
        {
            speedChangers.Sort((x, y) => { return x.whenSummonBeat - y.whenSummonBeat; });

            int lastBpmChangerIndex = -1;
            for (int i = 0; i < speedChangers.Count; i++)
            {
                if (speedChangers[i].whenSummonBeat <= beat)
                {
                    lastBpmChangerIndex = i;
                }
            }

            float beatHis = 0;
            float sumTime = 0;
            for (int i = 0; i < lastBpmChangerIndex + 1; i++)
            {
                sumTime += (BeatToSec(speedChangers[i].whenSummonBeat) - BeatToSec(beatHis)) * curruntSpeed * userSettingNoteDownSpeed;
                beatHis = speedChangers[i].whenSummonBeat;
                curruntSpeed = speedChangers[i].noteDownSpeedRate;
            }
            sumTime += (BeatToSec(beat) - BeatToSec(beatHis)) * curruntSpeed * userSettingNoteDownSpeed;
            return sumTime;
        }
        else
        {
            return BeatToSec(beat) * curruntSpeed * userSettingNoteDownSpeed;
        }
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = UnityEngine.Object.Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 7, yPos + NoteManager.NOTE_CHECK_YPOS, -0.01f);
        NoteManager.instance.AddNoteDownListener(g.transform);
        return g;
    }
}