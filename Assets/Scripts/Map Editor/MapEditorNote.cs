using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorNote : MonoBehaviour
{
    int xLine;
    int xSize = 3;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public float beat;
    public void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        beat = MapEditManager.Instance.notePosCalculator.YposCloseToBeat(inputPos.y) + holdingSpaceLocalPosition.y;
        float y = MapEditManager.Instance.notePosCalculator.BeatToYpos(beat);
        rectTransform.localPosition = new Vector3(transform.localPosition.x, y);

        xLine = MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x;

        int x = Mathf.Min(xLine + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
        SetAnchor(x, Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - x));
    }

    public void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        if (xSize + xLine > MapEditManager.LineContourCount - 1)
        {
            xLine = Mathf.Min(MapEditManager.Instance.XposToCloseVerticalLineIndex(inputPos.x) + holdingSpaceLocalPosition.x + holdingSpaceLocalPosition.x, MapEditManager.LineContourCount - 2);
            xSize = Mathf.Min(xSize, MapEditManager.LineContourCount - 1 - xLine);

            SetAnchor(xLine, xSize);
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
