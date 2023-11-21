using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapEditManager : MonoBehaviour
{
    public static SavedMapData EditingMap { get; private set; }

    public static MapEditManager Instance { get; private set; }

    [Header("��ũ�� ������ ����")]
    [SerializeField] Transform mapScrollViewContent;
    [SerializeField] Transform mapScrollInLine;
    [Header("���� ������")]
    [SerializeField] GameObject beatLinePrefab;
    [SerializeField] GameObject barLinePrefab;
    [Header("��Ÿ ����")]
    public float spacing;
    public float firstBarLineYPos = -1800;

    public NotePosCalculator notePosCalculator;

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
                artistName = "�����",
                designerName = "�����",
                title = "�����",
                startBpm = 80,
                startOffset = 0,
                notes = new SavedNoteData[]{
                    new SavedBasicNoteData(){startX = 0,endX = 1,whenSummonBeat=1}
                }
            };
            Debug.LogWarning("������ ���� �������� �ʽ��ϴ�!");
        }
        Screen.orientation = ScreenOrientation.Portrait;

        notePosCalculator = new NotePosCalculator(spacing, EditingMap);

        for (int i = 0; i < notePosCalculator.BeatOfBar(100); i++)
        {
            GameObject bar = Instantiate(beatLinePrefab, mapScrollInLine);
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, firstBarLineYPos + notePosCalculator.BeatToYpos(i), 0);
        }
        for (int i = 0; i < 100; i++)
        {
            GameObject bar = Instantiate(barLinePrefab, mapScrollInLine);
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, firstBarLineYPos + notePosCalculator.BeatToYpos(notePosCalculator.BeatOfBar(i)), 0);
        }
    }

    public void OnScrollMapScrollView()
    {
        if (mapScrollViewContent.transform.localPosition.y >= 0)
        {
            mapScrollViewContent.transform.localPosition = Vector3.zero;
        }
    }
}
