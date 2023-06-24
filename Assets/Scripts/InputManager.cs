using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        PcInput();
#else
MobileInput();
#endif
    }

    TouchMode TouchPhaseToTouchMode(TouchPhase touch)
        => touch switch
        {
            TouchPhase.Began => TouchMode.Start,
            TouchPhase.Moved => TouchMode.Hold,
            TouchPhase.Stationary => TouchMode.Hold,
            TouchPhase.Ended => TouchMode.End,
            _ => throw new ArgumentOutOfRangeException("TouchPhase값이 비정상적입니다.")
        };

    void MobileInput()
    {
        foreach (Touch touch in Input.touches)
        {
            TouchMode touchMode = TouchPhaseToTouchMode(touch.phase);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider == null) continue;
            HittingNoteChecker.instance.HitLine(((int)MathF.Floor(hit.point.x + 7)), touchMode, touch.deltaPosition);
        }
    }

    void PcInput()
    {
        Action<int> hit = (int index) => HittingNoteChecker.instance.HitLine(index, TouchMode.Start, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.A))
        {
            hit(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            hit(2);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            hit(3);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            hit(4);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            hit(5);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            hit(6);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            hit(7);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            hit(8);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            hit(9);
        }
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            hit(10);
        }

    }
}
