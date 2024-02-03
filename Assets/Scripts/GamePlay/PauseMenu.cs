using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Layer;

    public void OpenUI()
    {
        NoteManager.instance.SetPause(true);
        Layer.SetActive(true);
    }

    public void CloseUI()
    {
        NoteManager.instance.SetPause(false);
        Layer.SetActive(false);
    }

    public void Exit()
    {
        NoteManager.instance.SetPause(false);
        NoteManager.instance.GameOver();
    }
}
