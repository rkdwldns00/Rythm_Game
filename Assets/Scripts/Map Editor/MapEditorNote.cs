using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorNote : MonoBehaviour
{
    int xSize;

    private void Awake()
    {
        UpdateScale(3);
    }

    public void UpdateScale(int x)
    {
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    public void SetPos(Vector2 pos)
    {
        //pos.y = MapEditManager.Instance.notePosCalculator.YposCloseToBeat(pos.y).yPos;
        //pos.x = MapEditManager.Instance.notePosCalculator.XposCloseToLine(pos.x);
        transform.localPosition = pos;
    }
}
