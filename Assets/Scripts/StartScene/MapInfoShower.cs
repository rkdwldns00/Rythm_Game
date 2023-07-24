using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapInfoShower : MonoBehaviour
{
    SavedMapData savedMapData;

    [SerializeField] Text titleText;
    [SerializeField] Text artistText;
    [SerializeField] Image thumnailImage;

    void Start()
    {
        if (savedMapData == null)
        {
            return;
        }
    }

    public void SetMapData(SavedMapData mapData)
    {
        savedMapData = mapData;
        titleText.text = mapData.title;
        artistText.text = mapData.artistName;
        if (mapData.thumnail != null)
        {
            thumnailImage.sprite = mapData.thumnail;
        }
        savedMapData = mapData;
    }

    public void PlayMap()
    {
        if (savedMapData == null)
        {
            Debug.Log("할당된 맵데이터가 존재하지 않습니다.");
            return;
        }
        NoteManager.selectedMap = savedMapData;
        SceneManager.LoadScene("GamePlayScene");
    }

    public void StartEditMap()
    {

    }

    public void DeleteMap()
    {

    }
}
