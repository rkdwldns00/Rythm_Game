using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    static GameObject currentMenu;

    [SerializeField] GameObject mapInfoPrefab;
    [SerializeField] Transform mapInfoScrollView;

    [SerializeField] GameObject startMenu;
    [SerializeField] InputField nickNameInputField;
    [SerializeField] SliderValueShower volumeSlider;
    [SerializeField] SliderValueShower noteSpeedSlider;
    [SerializeField] SliderValueShower offsetSlider;

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
        volumeSlider.SetValue(SoundManager.Volume);
        noteSpeedSlider.SetValue(NoteManager.UserSettingNoteDownSpeed);
        nickNameInputField.text = NickName;

        currentMenu = startMenu;
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

    public void SetVolume(float volume)
    {
        SoundManager.Volume = volume;
    }

    public void SetNoteSpeed(float speed)
    {
        NoteManager.UserSettingNoteDownSpeed = speed;
    }

    public void SetOffset(float offset)
    {
        NoteManager.UserSettingOffset = offset;
    }

    public void SetNickName(string nickName)
    {
        NickName = nickName;
    }

    public void LoadNewMap()
    {
        SavedMapData[] maps = Resources.LoadAll<SavedMapData>("MapDatas/");
        for(int i=0;i<mapInfoScrollView.childCount;i++)
        {
            Destroy(mapInfoScrollView.GetChild(0).gameObject);
        }

        foreach (SavedMapData map in maps)
        {
            if (map != null)
            {
                MapInfoShower shower = Instantiate(mapInfoPrefab, mapInfoScrollView).GetComponent<MapInfoShower>();
                shower.SetMapData(map);
            }
        }
    }
}
