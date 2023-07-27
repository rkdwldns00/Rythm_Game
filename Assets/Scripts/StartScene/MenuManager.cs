using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    const string MAP_RESOURCE_PATH = "MapDatas/";
    static GameObject currentMenu;

    [SerializeField] GameObject mapInfoPrefab;
    [SerializeField] Transform mapInfoScrollView;

    [SerializeField] GameObject startMenu;
    [SerializeField] InputField nickNameInputField;

    public static string NickName
    {
        get
        {
            return PlayerPrefs.GetString("NickName");
        }
        set
        {
            PlayerPrefs.SetString("NickName", value);
        }
    }

    private void Start()
    {
        nickNameInputField.text = NickName;

        currentMenu = startMenu;

        /*MyUtil.SaveMapFile(new SavedMapData()
        {
            title = "map",
            artistName = "강지운",
            designerName = "강지",
            startOffset = 0,
            startBpm = 120,
            notes = new SavedNoteData[]
            {
                new SavedBasicNoteData() {startX = 3,endX=6,whenSummonBeat=3},
                new SavedBasicNoteData() {startX = 3,endX=6,whenSummonBeat=4},
                new SavedBasicNoteData() {startX = 3,endX=6,whenSummonBeat=5},
                new SavedBasicNoteData() {startX = 3,endX=6,whenSummonBeat=6},
            }
        });*/
    }

    public void SetMenu(GameObject menu)
    {
        if (currentMenu != null) { currentMenu.SetActive(false); }
        currentMenu = menu;
        menu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetNickName(string nickName)
    {
        NickName = nickName;
    }

    public void LoadNewMap()
    {
        TextAsset[] maps = Resources.LoadAll<TextAsset>(MAP_RESOURCE_PATH);
        for (int i = 0; i < mapInfoScrollView.childCount; i++)
        {
            Destroy(mapInfoScrollView.GetChild(0).gameObject);
        }

        foreach (TextAsset map in maps)
        {
            if (map != null)
            {
                MapInfoShower shower = Instantiate(mapInfoPrefab, mapInfoScrollView).GetComponent<MapInfoShower>();

                shower.SetMapData(MyUtil.LoadMapFile(map.name));
            }
        }
    }
}
