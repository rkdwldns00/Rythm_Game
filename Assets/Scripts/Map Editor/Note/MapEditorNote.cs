using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public abstract class MapEditorNote : MonoBehaviour
{
    public EventTrigger eventTrigger;
    public int beat;

    public int standardNoteValue = 1;
    public int indexInBeat = 0;

    protected RectTransform rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        MapEditManager.Instance.RegistEditorNote(this);
    }

    protected virtual void Start()
    {
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((_) => OnBeginDragNote());
        eventTrigger.triggers.Add(beginDragEntry);

        EventTrigger.Entry PointerDownEntry = new EventTrigger.Entry();
        PointerDownEntry.eventID = EventTriggerType.PointerDown;
        PointerDownEntry.callback.AddListener((_) => OnClick());
        eventTrigger.triggers.Add(PointerDownEntry);
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

    public void OnBeginDragNote()
    {
        MapEditManager.Instance.StartHoldNote(this);
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
