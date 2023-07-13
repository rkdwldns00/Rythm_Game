using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float userSettingNoteDownSpeed => 10f;
    public GameObject basicNotePrefab;
    public GameObject criticalBasicNotePrefab;
    public GameObject holdStartNotePrefab;
    public GameObject holdNotePrefab;
    public GameObject criticalHoldNotePrefab;
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
        Application.targetFrameRate = 60;

        /*new NoteSummoner(new SavedMapData()
        {
            startBpm = 120,
            notes = new SavedNoteData[]
            {
                new SavedHoldNoteData(){whenSummonBeat=10,curveData=new SavedHoldNoteCurve[]
                {
                    new SavedHoldNoteCurve(){spawnBeat=0,startX=2,endX=5},
                    new SavedHoldNoteCurve(){spawnBeat=100,startX=5,endX=8},
                }},
                new SavedBPMChangeNoteData(){bpm=2400,whenSummonBeat=10}
            }
        }, field, userSettingNoteDownSpeed).SummmonMap();*/
        
        new NoteSummoner(new SavedMapData()
        {
            startBpm = 120,
            notes = new SavedNoteData[]
            {
                new SavedBPMChangeNoteData(){bpm=240,whenSummonBeat=10},
                new SavedBasicNoteData(){startX=3,endX=6,whenSummonBeat=9},
                new SavedBasicNoteData(){startX=3,endX=6,whenSummonBeat=10},
                new SavedBasicNoteData(){startX=3,endX=6,whenSummonBeat=11},
                new SavedBasicNoteData(){startX=3,endX=6,whenSummonBeat=12},
                new SavedBasicNoteData(){startX=3,endX=6,whenSummonBeat=13},
            }
        }, field, userSettingNoteDownSpeed).SummmonMap();


        //new NoteSummoner(SUSConveter.ConvertMapData(SUSConveter.ReadTxt("map")), field, userSettingNoteDownSpeed).SummmonMap();
    }

    void Update()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            Vector2 s = MyUtil.BezierCalCulate(i, Vector2.zero, Vector2.up, Vector2.one, new Vector2(1, 2));
            Vector2 e = MyUtil.BezierCalCulate(i + 0.01f, Vector2.zero, Vector2.up, Vector2.one, new Vector2(1, 2));
            Debug.DrawLine(s, e);
        }
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
    public string title;

    public string artistName;
    public string designerName;

    public float startOffset;
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