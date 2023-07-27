using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public const int MAXIMUM_BEAT = 16;
    public const float NOTE_CHECK_YPOS = -4.5f;
    public const float NOTE_Y_SIZE = 1f;

    public static NoteManager instance;
    public static float UserSettingNoteDownSpeed
    {
        get
        {
            return PlayerPrefs.GetFloat("UserSettingNoteDownSpeed");
        }
        set
        {
            PlayerPrefs.SetFloat("UserSettingNoteDownSpeed", value);
        }
    }

    public static float UserSettingOffset
    {
        get
        {
            return PlayerPrefs.GetFloat("UserSettingOffset");
        }
        set
        {
            PlayerPrefs.SetFloat("UserSettingOffset", value);
        }
    }
    public static SavedMapData selectedMap;

    public Transform field;
    public float noteDownSpeedRate { private get; set; } = 1f;
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
    float cachedUserSettingNoteDownSpeed;
    List<Transform> noteListeners = new List<Transform>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 120;
        cachedUserSettingNoteDownSpeed = UserSettingNoteDownSpeed * 3;

        StartCoroutine(StartMap());
    }

    IEnumerator StartMap()
    {
        yield return new WaitForSeconds(2);
        mapStartTime = Time.time;

        new NoteSummoner(selectedMap, field, cachedUserSettingNoteDownSpeed).SummmonMap();
    }

    void Update()
    {
        noteListeners.RemoveAll((x) => x == null);
        foreach (Transform t in noteListeners)
        {
            t.localPosition += Time.deltaTime * noteDownSpeedRate * cachedUserSettingNoteDownSpeed * Vector3.down;
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