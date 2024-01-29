using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditManager : MonoBehaviour
{
    public static SavedMapData EditingMap { get; private set; }

    public static MapEditManager Instance { get; private set; }

    public static int LineContourCount => Instance.verticalLine.Length;

    [Header("노트 프리팹")]
    public GameObject basicNotePrefab;
    public GameObject meterChangerNotePrefab;
    [Header("스크롤 관리용 참조")]
    [SerializeField] Transform mapScrollViewContent;
    public float mapScrollViewContentYPos => mapScrollViewContent.localPosition.y;
    [SerializeField] Transform mapScrollInLine;
    [Header("마디선 프리펩")]
    [SerializeField] GameObject beatLinePrefab;
    [SerializeField] GameObject barLinePrefab;
    [Header("노트 정보 설정 UI")]
    [SerializeField] GameObject[] noteInfoUIObjects;
    [Header("기타 설정")]
    public float spacing;
    public float firstBarLineYPos = -1800;
    public RectTransform[] verticalLine;
    private Sprite mapStandardSprite;

    MapEditorNoteInfoUI[] noteInfoUI;

    public NotePosCalculator notePosCalculator;

    List<MapEditorNote> mapEditorNotes = new();
    List<(MapEditorNote note, Vector2Int pos)> holdingNotes = new();

    GameObject[] beatLines;
    GameObject[] barLines;

    MapEditorInputManager input;

    public static void StartMapEditScene()
    {
        StartMapEditScene(null);
    }

    public static void StartMapEditScene(SavedMapData mapData)
    {
        SceneManager.LoadScene("MapEditor");
        EditingMap = mapData;
    }

    private void Awake()
    {
        Instance = this;

        if (EditingMap == null)
        {
            ClearEditingMap();
            Debug.LogWarning("편집할 맵이 존재하지 않습니다!");
        }

        notePosCalculator = new MapEditorNotePosCalculator(spacing, EditingMap, this);
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        input = GetComponent<MapEditorInputManager>();

        foreach (var note in EditingMap.notes)
        {
            note.SummonMapEditorNote();
        }

        beatLines = new GameObject[notePosCalculator.BeatOfBar(100)];
        for (int i = 0; i < beatLines.Length; i++)
        {
            GameObject line = Instantiate(beatLinePrefab, mapScrollInLine);
            RectTransform t = line.GetComponent<RectTransform>();
            beatLines[i] = line;
            //t.localPosition = new Vector3(t.localPosition.x, notePosCalculator.BeatToYpos(i), 0);
        }
        barLines = new GameObject[100];
        for (int i = 0; i < barLines.Length; i++)
        {
            GameObject line = Instantiate(barLinePrefab, mapScrollInLine);
            line.GetComponentInChildren<Text>().text = (i + 1).ToString();
            barLines[i] = line;
            //line.transform.localPosition = new Vector3(line.transform.localPosition.x, notePosCalculator.BeatToYpos(notePosCalculator.BeatOfBar(i)), 0);
        }
        RefreshNotesPosition();

        noteInfoUI = new MapEditorNoteInfoUI[noteInfoUIObjects.Length];
        for (int i = 0; i < noteInfoUIObjects.Length; i++)
        {
            noteInfoUI[i] = noteInfoUIObjects[i].GetComponent<MapEditorNoteInfoUI>();
        }
    }

    public void Update()
    {
        TouchData[] touches = input.GetTouchPoints();
        if (touches.Length > 0)
        {
            Vector2 touchPos = touches[0].position;

            if (touches[0].mode == TouchMode.End)
            {
                holdingNotes.ForEach(x => x.note.OnStopHolding(touchPos, x.pos));
                holdingNotes.Clear();
            }
            else
            {
                holdingNotes.ForEach(x => x.note.OnHolding(touchPos, x.pos));
            }
        }
    }

    public void OnScrollMapScrollView()
    {
        if (mapScrollViewContentYPos >= 0)
        {
            mapScrollViewContent.transform.localPosition = Vector3.zero;
        }
    }

    public void StartHoldNote(MapEditorNote noteObject, Vector2Int pos)
    {
        noteObject.transform.SetParent(mapScrollViewContent.transform);
        holdingNotes.Add((noteObject, pos));

        SelectMapEditorNote(noteObject);
    }

    public void StartHoldNote(MapEditorNote noteObject)
    {
        StartHoldNote(noteObject, Vector2Int.zero);
    }

    public float GetVerticalAnchorX(int index)
    {
        return verticalLine[Mathf.Min(index, verticalLine.Length - 1)].anchorMax.x;
    }

    public float GetVerticalXPos(int index)
    {
        return verticalLine[index].position.x;
    }

    public int GetInputVerticalLine()
    {
        if (input.GetTouchCount() > 0)
        {
            float x = input.GetTouchPoints()[0].position.x;
            return XposToCloseVerticalLineIndex(x);
        }
        return 0;
    }

    public int XposToCloseVerticalLineIndex(float x)
    {
        for (int i = 0; i < LineContourCount - 1; i++)
        {
            if (GetVerticalXPos(i) > x)
            {
                return i;
            }
        }
        return LineContourCount - 1;
    }

    public void RegistEditorNote(MapEditorNote note)
    {
        mapEditorNotes.Add(note);
    }

    public void SaveEditingMap()
    {
        CachingEditingNotes();
        MapFileUtil.SaveMapResource(EditingMap);
    }

    public void CachingEditingNotes()
    {
        mapEditorNotes.Sort((a, b) =>
        {
            if (a.beat == b.beat)
            {
                if (a is MapEditorHaveXposNote ax && b is MapEditorHaveXposNote bx)
                {
                    return ax.startX - bx.startX;
                }
                else
                {
                    return 0;
                }
            }
            return (int)Mathf.Sign(a.beat - b.beat);
        });
        SavedNoteData[] noteDatas = new SavedNoteData[mapEditorNotes.Count];
        for (int i = 0; i < mapEditorNotes.Count; i++)
        {
            noteDatas[i] = mapEditorNotes[i].GetNoteData();
        }
        EditingMap.notes = noteDatas;
    }

    public void ClearEditingMap()
    {
        foreach (var note in mapEditorNotes)
        {
            Destroy(note.gameObject);
        }
        mapEditorNotes.Clear();
        EditingMap = new()
        {
            title = "새로운 맵",
            artistName = "Unknown",
            designerName = MenuManager.NickName,
            bgm = null,
            startBpm = 120,
            startOffset = 0,
            thumnail = mapStandardSprite,
            notes = new SavedNoteData[] { new SavedMeterChangerNoteData() { whenSummonBeat = 0, beatPerBar = 4, beatLengthRate = 1 } }
        };
    }

    public GameObject SummonNote(GameObject notePrefeab)
    {
        return Instantiate(notePrefeab, mapScrollViewContent);
    }

    public void SelectMapBgm()
    {
        FileBrowser.ShowLoadDialog(
            (paths) =>
            {
                if (paths.Length > 0)
                {
                    StartCoroutine(AudioClipUtil.Load(paths[0], (clip) => EditingMap.bgm = clip));
                }
            },
            () => { },
            FileBrowser.PickMode.Files, false, null, null, "음악 파일 선택");
    }

    public void SelectMapEditorNote(MapEditorNote note)
    {
        for (int i = 0; i < noteInfoUI.Length; i++)
        {
            noteInfoUI[i].OnSelectNote(note);
        }
    }

    public void RefreshNotesPosition()
    {
        CachingEditingNotes();
        for (int i = 0; i < EditingMap.notes.Length; i++)
        {
            if (EditingMap.notes[i] is SavedMeterChangerNoteData m)
            {
                Debug.Log("beatPerBar : " + m.beatPerBar + ", rate : " + m.beatLengthRate);
            }
        }
        for (int i = 0; i < mapEditorNotes.Count; i++)
        {
            mapEditorNotes[i].RefreshPosition();
        }
        for (int i = 0; i < beatLines.Length; i++)
        {
            beatLines[i].transform.localPosition = new Vector3(beatLines[i].transform.localPosition.x, notePosCalculator.BeatToYpos(i), 0);
        }
        for (int i = 0; i < barLines.Length; i++)
        {
            barLines[i].transform.localPosition = new Vector3(barLines[i].transform.localPosition.x, notePosCalculator.BeatToYpos(notePosCalculator.BeatOfBar(i)), 0);
        }
    }
}

class MapEditorNotePosCalculator : NotePosCalculator
{
    MapEditManager mapEditManager;

    public MapEditorNotePosCalculator(float spacing, SavedMapData map, MapEditManager manager) : base(spacing, map)
    {
        mapEditManager = manager;
    }

    public override float BeatToYpos(float beat)
    {
        return BeatToSec(beat) * spacing + mapEditManager.firstBarLineYPos;
    }

    public override int YposCloseToBeat(float y)
    {
        return base.YposCloseToBeat(y - 2400 - mapEditManager.mapScrollViewContentYPos);
    }
}