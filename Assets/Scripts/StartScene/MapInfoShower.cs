using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapInfoShower : MonoBehaviour
{
    public SavedMapData SavedMapData { get; set; }

    public Text titleText;
    public Text artistText;
    public Image thumnail;

    void Start()
    {
        if(SavedMapData == null)
        {
            return;
        }

    }

    public void PlayMap()
    {
        if(SavedMapData == null) { 
            return;
        }
        NoteManager.selectedMap = SavedMapData;
        SceneManager.LoadScene("GamePlayScene");
    }

    public void StartEditMap()
    {

    }

    public void DeleteMap()
    {

    }
}
