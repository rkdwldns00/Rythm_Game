using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorNote : MonoBehaviour
{
    int xSize = 3;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetPos(Vector2 pos)
    {
        pos.y = MapEditManager.Instance.notePosCalculator.YposCloseToBeat(pos.y).yPos;
        transform.position = new Vector3(transform.position.x, pos.y);

        int inputLine = MapEditManager.Instance.GetInputVerticalLine();
        rectTransform.anchorMin = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(inputLine, 0, MapEditManager.LineContourCount - 1)), rectTransform.anchorMin.y);
        rectTransform.anchorMax = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(inputLine + xSize, inputLine + 1, MapEditManager.LineContourCount - 1)), rectTransform.anchorMax.y);
        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
    }
}
