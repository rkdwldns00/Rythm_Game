using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public enum TouchMode
{
    None,
    Start,
    Hold,
    End,
}

public class HittingNoteChecker : MonoBehaviour
{
    public Canvas canvas;
    public GameObject touchCheck;
    public TouchMode[] TouchDatas { get; private set; } = new TouchMode[12];
    public Vector2[] FlickDatas { get; private set; }= new Vector2[12];
    bool[] holdingDatas = new bool[12];

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        for (int i = 0; i < TouchDatas.Length; i++)
        {
            if (!(TouchDatas[i] == TouchMode.Hold && holdingDatas[i]))
            {
                TouchDatas[i] = TouchMode.None;
            }
        }
        holdingDatas = new bool[12];
    }

    public void HitLine(int lineIndex, TouchMode touchMode, Vector2 moveSpeed)
    {
        if (TouchDatas[lineIndex] == TouchMode.None)
        {
            GameObject g = Instantiate(touchCheck, canvas.transform);
            g.transform.localPosition = new Vector3(lineIndex - 5.5f, -5f, 0);
        }

        if (touchMode == TouchMode.Hold)
        {
            holdingDatas[lineIndex] = true;
        }

        FlickDatas[lineIndex] = moveSpeed;
        TouchDatas[lineIndex] = touchMode;
    }
}
