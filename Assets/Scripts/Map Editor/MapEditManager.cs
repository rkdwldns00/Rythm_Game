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
            Debug.LogError("������ ���� �������� �ʽ��ϴ�!");
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
