using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public HittingNoteChecker noteChecker;
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
            noteChecker.HitLine(((int)MathF.Floor(hit.point.x + 7)), touchMode, touch.deltaPosition);
        }
    }

    void PcInput()
    {
        Action<int> hit = (int index) => noteChecker.HitLine(index, TouchMode.Start, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.A))
        {
            hit(1);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            hit(2);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            hit(3);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            hit(4);
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            hit(5);
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            hit(6);
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            hit(7);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            hit(8);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            hit(9);
        }
        if (Input.GetKeyUp(KeyCode.Semicolon))
        {
            hit(10);
        }

    }
}
