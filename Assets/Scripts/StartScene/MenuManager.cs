using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    const string MAP_RESOURCE_PATH = "MapDatas/";
    GameObject currentMenu = null;

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject gameResultUI;

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
        Screen.orientation = ScreenOrientation.LandscapeRight;
        currentMenu = startMenu;

        MapFileUtil.MakeMapListFile();
        MapFileUtil.MakeMapDatasFolder();

        if(GameManager.Instance.showResultUI)
        {
            SetMenu(gameResultUI);
        }
        else
        {
            SetMenu(startMenu);
        }
    }

    public void SetMenu(GameObject menu)
    {
        if (currentMenu != null) { currentMenu.SetActive(false); }
        currentMenu = menu;
        menu.SetActive(true);
    }
}
