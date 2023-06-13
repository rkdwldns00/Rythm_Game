using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTest : MonoBehaviour
{
    public Text text;
    public Canvas canvas;
    public GameObject note;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string s = "";
        for (int i = 0; i < Input.touchCount; i++)
        {
            s += Input.GetTouch(i).position;
        }
        text.text = s;

        note.GetComponent<RectTransform>().position = Input.GetTouch(0).position;
    }
}
