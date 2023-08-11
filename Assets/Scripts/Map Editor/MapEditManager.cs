using System.Collections;
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
    [SerializeField] GameObject barLinePrefab;
    [Header("기타 설정")]
    public float spacing;
    public float firstBarLineYPos = -1800;

    public static void StartMapEditScene(SavedMapData mapData)
    {
        SceneManager.LoadScene("MapEditor");
        EditingMap = mapData;
    }

    private void Start()
    {
        Instance = this;
        if(EditingMap == null)
        {
            Debug.LogError("편집할 맵이 존재하지 않습니다!");
        }
        Screen.orientation = ScreenOrientation.Portrait;

        for(int i = 0; i < 100; i++)
        {
            GameObject bar = Instantiate(barLinePrefab, mapScrollInLine);
            bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, firstBarLineYPos + i * spacing, 0);
        }
    }

    public void OnScrollMapScrollView()
    {
        if(mapScrollViewContent.transform.localPosition.y >= 0)
        {
            mapScrollViewContent.transform.localPosition = Vector3.zero;
        }
    }
}
