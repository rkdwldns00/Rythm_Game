using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapEditManager : MonoBehaviour
{
    public static SavedMapData EditingMap { get; private set; }

    public static MapEditManager Instance { get; private set; }

    [Header("스크롤 관리용 참조")]
    [SerializeField] Transform mapScrollViewContent;
    [SerializeField] Transform mapScrollInLine;
    [Header("마디선 프리펩")]
    [SerializeField] GameObject beatLinePrefab;
    [SerializeField] GameObject barLinePrefab;
    [Header("기타 설정")]
    public float spacing;
    public float firstBarLineYPos = -1800;

    public NotePosCalculator notePosCalculator;
    List<(MapEditorNote note, Vector2 pos)> holdingNotes = new();

    MapEditorInputManager input;

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
            EditingMap = new()
            {
                artistName = "디버거",
                designerName = "디버거",
                title = "디버그",
                startBpm = 80,
                startOffset = 0,
                notes = new SavedNoteData[]{
                    new SavedBasicNoteData(){startX = 0,endX = 1,whenSummonBeat=1}
                }
            };
            Debug.LogWarning("편집할 맵이 존재하지 않습니다!");
        }
        Screen.orientation = ScreenOrientation.Portrait;
        input = GetComponent<MapEditorInputManager>();

        notePosCalculator = new NotePosCalculator(spacing, EditingMap);

        for (int i = 0; i < notePosCalculator.BeatOfBar(100); i++)
        {
            GameObject bar = Instantiate(beatLinePrefab, mapScrollInLine);
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, firstBarLineYPos + notePosCalculator.BeatToYpos(i), 0);
        }
        for (int i = 0; i < 100; i++)
        {
            GameObject bar = Instantiate(barLinePrefab, mapScrollInLine);
            bar.GetComponentInChildren<Text>().text = (i + 1).ToString();
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, firstBarLineYPos + notePosCalculator.BeatToYpos(notePosCalculator.BeatOfBar(i)), 0);
        }
    }

    public void Update()
    {
        Debug.Log(holdingNotes.Count);
        TouchData[] touches = input.GetTouchPoints();
        if (touches.Length > 0)
        {
            Vector2 touchPos = touches[0].position;
            for (int i = 0; i < holdingNotes.Count; i++)
            {
                holdingNotes[i].note.SetPos(touchPos + holdingNotes[i].pos);
            }

            if (touches[0].mode == TouchMode.End)
            {
                holdingNotes.Clear();
            }
        }
    }

    public void OnScrollMapScrollView()
    {
        if (mapScrollViewContent.transform.localPosition.y >= 0)
        {
            mapScrollViewContent.transform.localPosition = Vector3.zero;
        }
    }

    public bool StartHoldNote(MapEditorNote noteObject, Vector2 pos)
    {
        if (Input.touchCount == 0)
        {
            holdingNotes.Add((noteObject, pos));
            return true;
        }
        return false;
    }

    public void StartHoldNote(MapEditorNote noteObject)
    {
        StartHoldNote(noteObject, Vector2.zero);
        noteObject.transform.parent = mapScrollViewContent.transform;
    }
}
