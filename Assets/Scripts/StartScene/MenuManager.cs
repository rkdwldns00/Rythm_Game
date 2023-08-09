using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    const string MAP_RESOURCE_PATH = "MapDatas/";
    static GameObject currentMenu;

    [SerializeField] GameObject startMenu;

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
        currentMenu = startMenu;
        Screen.orientation = ScreenOrientation.LandscapeRight;
    }

    public void SetMenu(GameObject menu)
    {
        if (currentMenu != null) { currentMenu.SetActive(false); }
        currentMenu = menu;
        menu.SetActive(true);
    }
}
