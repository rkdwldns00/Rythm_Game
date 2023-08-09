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
    [SerializeField] Transform mapScrollViewFirstLine;

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
    }

    public void OnScrollMapScrollView()
    {
        if(mapScrollViewContent.transform.localPosition.y <= 0)
        {
            mapScrollViewContent.transform.localPosition = Vector3.zero;
        }
    }
}
