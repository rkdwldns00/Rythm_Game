using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditManager : MonoBehaviour
{
    public static SavedMapData EditingMap { get; private set; }

    public static MapEditManager Instance { get; private set; }

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
}
