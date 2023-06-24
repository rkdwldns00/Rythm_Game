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
    public const int TOUCH_LINE_COUNT = 14;
    public const int SHOW_LINE_START = 1;
    public const int SHOW_LINE_END = 12;

    public static HittingNoteChecker instance;

    public Canvas canvas;
    public GameObject touchCheck;
    public TouchMode[] TouchDatas { get; private set; } = new TouchMode[TOUCH_LINE_COUNT];
    Queue<Vector2>[] flickDatas = new Queue<Vector2>[TOUCH_LINE_COUNT];
    bool[] holdingDatas = new bool[TOUCH_LINE_COUNT];

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < flickDatas.Length; i++)
        {
            flickDatas[i] = new Queue<Vector2>();
            flickDatas[i].Enqueue(Vector2.zero);
            flickDatas[i].Enqueue(Vector2.zero);
            flickDatas[i].Enqueue(Vector2.zero);
        }
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
        holdingDatas = new bool[TOUCH_LINE_COUNT];

        for (int i = 0; i < flickDatas.Length; i++)
        {
            flickDatas[i].Dequeue();
            if (flickDatas[i].Count < 3)
            {
                flickDatas[i].Enqueue(Vector2.zero);
            }
        }
    }

    public void HitLine(int lineIndex, TouchMode touchMode, Vector2 moveSpeed)
    {
        if (TouchDatas[lineIndex] == TouchMode.None && lineIndex >= SHOW_LINE_START && lineIndex <= SHOW_LINE_END)
        {
            GameObject g = Instantiate(touchCheck, canvas.transform);
            g.transform.localPosition = new Vector3(lineIndex - 6.5f, -5f, 0);
        }

        if (touchMode == TouchMode.Hold)
        {
            holdingDatas[lineIndex] = true;
        }

        if(touchMode == TouchMode.Start)
        {
            NoteManager.instance.HitCheck(lineIndex);
        }

        flickDatas[lineIndex].Enqueue(moveSpeed);
        TouchDatas[lineIndex] = touchMode;
    }

    public Vector2 GetFlickPower(int lineIndex)
    {
        Vector2 sum = Vector2.zero;
        foreach (Vector2 v in flickDatas[lineIndex])
        {
            sum += v;
        }
        sum.y *= -1;
        return sum;
    }
}
