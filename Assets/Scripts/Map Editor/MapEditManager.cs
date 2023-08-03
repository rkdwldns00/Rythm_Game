using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Debug.LogError("편집할 맵이 존재하지 않습니다!");
        }
    }
}
