using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapEditorHaveXposNote : MapEditorNote
{
    [SerializeField] RectTransform leftSideSensor;
    [SerializeField] RectTransform rightSideSensor;

    public int startX = 1;
    public int xSize = 3;

    public override void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        startX = Mathf.Min(MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
        int newXSize = Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - startX);

        SetAnchor(startX, newXSize);
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

        float y = MapEditManager.Instance.notePosCalculator.BeatToYpos(beat);
        leftSideSensor.anchorMin = new Vector2(0, 0.5f);
        leftSideSensor.anchorMax = new Vector2(0, 0.5f);
        rightSideSensor.anchorMin = new Vector2(1, 0.5f);
        rightSideSensor.anchorMax = new Vector2(1, 0.5f);

    }

    public override void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        base.OnStopHolding(inputPos, holdingSpaceLocalPosition);
        if (xSize + startX > MapEditManager.LineContourCount - 1)
        {
            startX = Mathf.Min(MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
            xSize = Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - startX);

            SetAnchor(startX, xSize);
        }
    }

    private void SetSidePos(int startX, int xSize)
    {
        this.startX = Mathf.Clamp(startX, 0, Mathf.Min(this.startX + this.xSize - 1, MapEditManager.LineContourCount - 2));
        this.xSize = Mathf.Clamp(xSize, 1, MapEditManager.LineContourCount - 1 - this.startX);

        SetAnchor(this.startX, this.xSize);
    }

    public void SetLeftSide()
    {
        int x = MapEditManager.Instance.GetInputVerticalLine();
        int xDelta = x - startX;
        SetSidePos(startX + xDelta, xSize - xDelta);
    }

    public void SetRightSide()
    {
        int x = MapEditManager.Instance.GetInputVerticalLine();
        int xDelta = x - (startX + xSize);
        SetSidePos(startX, xSize + xDelta);
    }
}
