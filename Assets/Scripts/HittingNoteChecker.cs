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
    Queue<Vector2>[] flickDatas = new Queue<Vector2>[12];
    bool[] holdingDatas = new bool[12];

    void Start()
    {
        for(int i=0;i<flickDatas.Length;i++)
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
        Log.text = GetFlickPower(0).ToString();
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
        if (TouchDatas[lineIndex] == TouchMode.None)
        {
            GameObject g = Instantiate(touchCheck, canvas.transform);
            g.transform.localPosition = new Vector3(lineIndex - 5.5f, -5f, 0);
        }

        if (touchMode == TouchMode.Hold)
        {
            holdingDatas[lineIndex] = true;
        }

        flickDatas[lineIndex].Enqueue(moveSpeed);
        TouchDatas[lineIndex] = touchMode;
    }

    public Vector2 GetFlickPower(int lineIndex)
    {
        Vector2 sum = Vector2.zero;
        foreach(Vector2 v in flickDatas[lineIndex])
        {
            sum += v;
        }
        sum.y *= -1;
        return sum;
    }
}
