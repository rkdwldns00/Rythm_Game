using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    Text t;
    static Log instance;

    public static string text
    {
        get { return instance.t.text; }
        set { instance.t.text = value; }
    }

    void Awake()
    {
        t = GetComponent<Text>();
        instance = this;
    }
}
