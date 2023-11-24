using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    protected TouchMode TouchPhaseToTouchMode(TouchPhase touch)
            => touch switch
            {
                TouchPhase.Began => TouchMode.Start,
                TouchPhase.Moved => TouchMode.Hold,
                TouchPhase.Stationary => TouchMode.Hold,
                TouchPhase.Ended => TouchMode.End,
                _ => throw new ArgumentOutOfRangeException("TouchPhase값이 비정상적입니다.")
            };
    
    public int GetTouchCount()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            return 1;
        }
        else
        {
            return 0;
        }
#else
        return Input.touchCount;
#endif
    }

    public TouchData[] GetTouchPoints()
    {
#if UNITY_EDITOR
        TouchMode touchMode = TouchMode.None;
        if (Input.GetMouseButtonDown(0))
        {
            touchMode = TouchMode.Start;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchMode = TouchMode.End;
        }
        else if (Input.GetMouseButton(0))
        {
            touchMode = TouchMode.Hold;
        }

        if (touchMode != TouchMode.None)
        {
            TouchData[] result = new TouchData[1];
            result[0] = new TouchData()
            {
                position = Input.mousePosition,
                delta = Input.mouseScrollDelta,
                mode = touchMode
            };
            return result;
        }
        else
        {
            return new TouchData[0];
        }
#else
        TouchData[] result = new TouchData[Input.touchCount];
        for (int i = 0; i < Input.touchCount; i++)
        {
            result[i] = new TouchData()
            {
                position = Input.touches[i].position,
                delta = Input.touches[i].deltaPosition,
                mode = TouchPhaseToTouchMode(Input.touches[i].phase)
            };
        }
        return result;
#endif
    }
}

public enum TouchMode
{
    None,
    Start,
    Hold,
    End,
}

public struct TouchData
{
    public Vector2 position;
    public Vector2 delta;
    public TouchMode mode;
}