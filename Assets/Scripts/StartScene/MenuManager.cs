using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    static GameObject currentMenu;

    public static void SetMenu(GameObject menu)
    {
        if(currentMenu != null) { Destroy(currentMenu); }
        currentMenu = menu;
        menu.SetActive(true);
    }
}
