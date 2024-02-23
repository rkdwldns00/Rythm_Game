using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject g = new GameObject("GameManger");
                instance = g.AddComponent<GameManager>();
                DontDestroyOnLoad(g);
            }
            return instance;
        }
    }

    private static GameManager instance;

    public bool showResultUI;
    public float score;
    public int[] hitResultCounts;
    public int maxCombo;
}
