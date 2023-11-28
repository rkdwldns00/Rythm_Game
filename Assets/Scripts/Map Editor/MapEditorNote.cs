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


    public float beat;
    public void SetPos(Vector2 pos)
    {
        float y = MapEditManager.Instance.notePosCalculator.BeatToYpos(MapEditManager.Instance.notePosCalculator.YposCloseToBeat(pos.y));
        beat = MapEditManager.Instance.notePosCalculator.YposCloseToBeat(pos.y);
        rectTransform.localPosition = new Vector3(transform.localPosition.x, y);

        int inputLine = MapEditManager.Instance.GetInputVerticalLine();
        rectTransform.anchorMin = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(inputLine, 0, MapEditManager.LineContourCount - 1)), 0);
        rectTransform.anchorMax = new Vector2(MapEditManager.Instance.GetVerticalAnchorX(Mathf.Clamp(inputLine + xSize, inputLine + 1, MapEditManager.LineContourCount - 1)), 0);
        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
    }
}
