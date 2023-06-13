using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public enum TouchMode
{
    Start,
    Hold,
    End,
}

public class HittingNoteChecker : MonoBehaviour
{
    public Canvas canvas;
    public GameObject touchCheck;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HitLine(int lineIndex, TouchMode touchMode)
    {
        Log.text = lineIndex.ToString();
        switch (touchMode)
        {
            case TouchMode.Start:
                GameObject g = Instantiate(touchCheck, canvas.transform);
                g.transform.localPosition = new Vector3(lineIndex - 5.5f, -5f, 0);
                break;
        }
    }
}
