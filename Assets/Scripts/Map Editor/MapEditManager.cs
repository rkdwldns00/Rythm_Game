using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditManager : MonoBehaviour
{
    public static SavedMapData EditingMap { get; private set; }

    public static MapEditManager Instance { get; private set; }

    public static int LineContourCount => Instance.verticalLine.Length;

    [Header("스크롤 관리용 참조")]
    [SerializeField] Transform mapScrollViewContent;
    public float mapScrollViewContentYPos => mapScrollViewContent.localPosition.y;
    [SerializeField] Transform mapScrollInLine;
    [Header("마디선 프리펩")]
    [SerializeField] GameObject beatLinePrefab;
    [SerializeField] GameObject barLinePrefab;
    [Header("기타 설정")]
    public float spacing;
    public float firstBarLineYPos = -1800;
    public RectTransform[] verticalLine;
    private Sprite mapStandardSprite;

    public NotePosCalculator notePosCalculator;

    List<MapEditorNote> mapEditorNotes = new();
    List<(MapEditorNote note, Vector2Int pos)> holdingNotes = new();

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

    private void Start()
    {
        Instance = this;
        if (EditingMap == null)
        {
            ClearEditingMap();
            Debug.LogWarning("편집할 맵이 존재하지 않습니다!");
        }
        else
        {
            
        }
        Screen.orientation = ScreenOrientation.Portrait;
        input = GetComponent<MapEditorInputManager>();

        notePosCalculator = new MapEditorNotePosCalculator(spacing, EditingMap, this);

        for (int i = 0; i < notePosCalculator.BeatOfBar(100); i++)
        {
            GameObject bar = Instantiate(beatLinePrefab, mapScrollInLine);
            RectTransform t = bar.GetComponent<RectTransform>();
            t.localPosition = new Vector3(t.localPosition.x, notePosCalculator.BeatToYpos(i), 0);
        }
        for (int i = 0; i < 100; i++)
        {
            GameObject bar = Instantiate(barLinePrefab, mapScrollInLine);
            bar.GetComponentInChildren<Text>().text = (i + 1).ToString();
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, notePosCalculator.BeatToYpos(notePosCalculator.BeatOfBar(i)), 0);
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
                return a.startX - b.startX;
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
        foreach(var note in mapEditorNotes)
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
            notes = new SavedNoteData[] {}
        };
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