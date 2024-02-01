using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Layer;

    public void OpenUI()
    {
        Time.timeScale = 0;
        Layer.SetActive(true);
    }

    public void CloseUI()
    {
        Time.timeScale = 1;
        Layer.SetActive(false);
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }
}
