using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        TextAsset[] maps = Resources.LoadAll<TextAsset>("MapDatas/");
        for (int i = 0; i < mapInfoScrollView.childCount; i++)
        {
            Destroy(mapInfoScrollView.GetChild(0).gameObject);
        }

        foreach (TextAsset map in maps)
        {
            if (map != null)
            {
                MapInfoShower shower = Instantiate(mapInfoPrefab, mapInfoScrollView).GetComponent<MapInfoShower>();
                SavedMapData mapData = SUSConveter.ConvertMapData(map.text);
                Debug.Log("MapDatas/" + map.name + ".jpg");
                Sprite thumnail = Resources.Load<Sprite>("MapDatas/" + map.name + ".jpg");
                if (thumnail != null)
                {
                    mapData.thumnail = thumnail;
                }

                shower.SetMapData(mapData);
            }
        }
    }
}
