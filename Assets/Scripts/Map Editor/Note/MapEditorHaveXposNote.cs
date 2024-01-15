using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEditorHaveXposNote : MapEditorNote
{
    public override void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        base.OnHolding(inputPos, holdingSpaceLocalPosition);
        startX = MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x;

        int x = Mathf.Min(startX + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
        SetAnchor(x, Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - x));
    }

    public override void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        base.OnStopHolding(inputPos, holdingSpaceLocalPosition);
        if (xSize + startX > MapEditManager.LineContourCount - 1)
        {
            startX = Mathf.Min(MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
            xSize = Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - startX);

            SetAnchor(startX, xSize);
        }
    }
}
