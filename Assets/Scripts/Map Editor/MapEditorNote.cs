using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorNote : MonoBehaviour
{
    public int startX;
    public int xSize = 3;
    public float beat;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        beat = MapEditManager.Instance.notePosCalculator.YposCloseToBeat(inputPos.y) + holdingSpaceLocalPosition.y;
        float y = MapEditManager.Instance.notePosCalculator.BeatToYpos(beat);
        rectTransform.localPosition = new Vector3(transform.localPosition.x, y);

        startX = MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x;

        int x = Mathf.Min(startX + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
        SetAnchor(x, Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - x));
    }

    public void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        if (xSize + startX > MapEditManager.LineContourCount - 1)
        {
            startX = Mathf.Min(MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
            xSize = Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - startX);

            SetAnchor(startX, xSize);
        }
    }

    void SetAnchor(int xLine, int xSize)
    {
        rectTransform.anchorMin = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(xLine, 0, MapEditManager.LineContourCount - 1)), 0);
        rectTransform.anchorMax = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(xLine + xSize, xLine + 1, MapEditManager.LineContourCount - 1)), 0);
        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
    }

    public void OnClickNote()
    {
        MapEditManager.Instance.StartHoldNote(this);
    }
}
