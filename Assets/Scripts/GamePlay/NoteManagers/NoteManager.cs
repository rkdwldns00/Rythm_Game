using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public float mapStartDelayTime;

    public Transform field;
    public AudioSource audioSource;

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
    public float mapEndTime;
    bool isMapStarted = false;
    bool isMapEnd = false;

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
        Screen.orientation = ScreenOrientation.LandscapeRight;
        cachedUserSettingNoteDownSpeed = UserSettingNoteDownSpeed * 3;

        StartCoroutine(StartMap());
    }

    IEnumerator StartMap()
    {
        yield return new WaitForSeconds(2);

        NoteSummoner noteSummoner = new NoteSummoner(selectedMap, field, cachedUserSettingNoteDownSpeed, -8);

        float delayTime = noteSummoner.BeatToSec(1) * 8 + UserSettingOffset / 100f;
        if (delayTime > 0)
        {
            SummonNote();
            yield return new WaitForSeconds(delayTime);
            PlayBGM();
        }
        else
        {
            PlayBGM();
            yield return new WaitForSeconds(-delayTime);
            SummonNote();
        }

        void SummonNote()
        {
            mapStartTime = Time.time;
            noteSummoner.SummmonMap();

            mapEndTime = noteSummoner.BeatToSec(noteSummoner.map.notes[selectedMap.notes.Length - 1].Beat) + noteSummoner.mapStartBeatSec + 2;
            
            isMapStarted = true;
        }
        void PlayBGM()
        {
            if (selectedMap.bgm != null)
            {
                audioSource.PlayOneShot(selectedMap.bgm);
            }
        }
    }

    void Update()
    {
        if (isMapStarted)
        {
            noteListeners.RemoveAll((x) => x == null);
            foreach (Transform t in noteListeners)
            {
                t.localPosition += Time.deltaTime * noteDownSpeedRate * cachedUserSettingNoteDownSpeed * Vector3.down;
            }

            if (mapTimer > mapEndTime && !isMapEnd)
            {
                isMapEnd = true;
                GameOver();
            }
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

    public void GameOver()
    {
        SceneManager.LoadScene("StartScene");
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

public interface IGamePlaySummonable
{
    public GameObject GamePlayNotePrefab { get; }

    //public Note Summon(NoteSummoner summoner, SavedNoteData data);
}