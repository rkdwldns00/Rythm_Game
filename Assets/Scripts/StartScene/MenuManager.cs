using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    static GameObject currentMenu;

    [SerializeField] GameObject startMenu;
    [SerializeField] SliderValueShower volumeSlider;
    [SerializeField] SliderValueShower noteSpeedSlider;
    [SerializeField] SliderValueShower offsetSlider;

    private void Start()
    {
        volumeSlider.SetValue(SoundManager.Volume);
        noteSpeedSlider.SetValue(NoteManager.UserSettingNoteDownSpeed);
        currentMenu = startMenu;
    }

    public void SetMenu(GameObject menu)
    {
        if(currentMenu != null) { currentMenu.SetActive(false); }
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

    }

    public void LoadNewMap()
    {

    }


}
