using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class MapEditorNote : MonoBehaviour
{
    public int beat;

    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        MapEditManager.Instance.RegistEditorNote(this);
    }

    public virtual void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        beat = MapEditManager.Instance.notePosCalculator.YposCloseToBeat(inputPos.y) + holdingSpaceLocalPosition.y;
        RefreshPosition();
    }

    public virtual void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {

    }

    public virtual void RefreshPosition()
    {
        float y = MapEditManager.Instance.notePosCalculator.BeatToYpos(beat);
        rectTransform.localPosition = new Vector3(transform.localPosition.x, y);
        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
    }

    public void OnDragNote()
    {
        MapEditManager.Instance.StartHoldNote(this);
        MapEditManager.Instance.SelectMapEditorNote(this);
    }

    public void OnClick()
    {
        MapEditManager.Instance.SelectMapEditorNote(this);
    }

    public virtual void DeleteNote()
    {
        MapEditManager.Instance.UnRegistEditorNote(this);
        Destroy(gameObject);
    }

    public abstract SavedNoteData GetNoteData();
}
