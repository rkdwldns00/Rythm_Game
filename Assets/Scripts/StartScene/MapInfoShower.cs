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
    }

    public void PlayMap()
    {
        if (savedMapData == null)
        {
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
