using System;
using System.Collections;
using System.Collections.Generic;
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
        MobileInput();
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
            if (touchMode != TouchMode.Start) continue;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider == null) continue;
            noteChecker.HitLine(((int)MathF.Floor(hit.point.x + 6)), touchMode);
        }
    }

    void PcInput()
    {

    }
}
