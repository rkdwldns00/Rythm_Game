using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEditorHaveXposNote : MapEditorNote
{
    public int startX = 1;
    public int xSize = 3;

    public override void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        startX = MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x;

        int x = Mathf.Min(startX + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
        SetAnchor(x, Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - x));
        base.OnHolding(inputPos, holdingSpaceLocalPosition);
    }

    public override void RefreshPosition()
    {
        base.RefreshPosition();
        SetAnchor(startX, xSize);
    }

    protected void SetAnchor(int xLine, int xSize)
    {
        rectTransform.anchorMin = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(xLine, 0, MapEditManager.LineContourCount - 1)), 0);
        rectTransform.anchorMax = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(xLine + xSize, xLine + 1, MapEditManager.LineContourCount - 1)), 0);
        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
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
