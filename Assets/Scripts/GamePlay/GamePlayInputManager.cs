using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayInputManager : InputManager
{
    void Update()
    {
//#if UNITY_EDITOR
//        PcInput();
//#else
MobileInput();
//#endif
    }

    void MobileInput()
    {
        TouchData[] touchDatas = GetTouchPoints();
        foreach (TouchData touch in touchDatas)
        {
            TouchMode touchMode = touch.mode;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider == null) continue;
            HittingNoteChecker.instance.HitLine(((int)MathF.Floor(hit.point.x + 7)), touchMode, touch.delta);
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
