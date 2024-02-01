using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorMeterChangerNote : MapEditorNote
{
    public Text meter1Text;
    public Text meter2Text;

    [SerializeField] private int _meter1;
    [SerializeField] private int _meter2;

    protected override void Awake()
    {
        base.Awake();
        meter1Text.text = _meter1.ToString();
        meter2Text.text = _meter2.ToString();
    }

    public int Meter1
    {
        get { return _meter1; }
        set
        {
            _meter1 = value;
            meter1Text.text = _meter1.ToString();
        }
    }

    public int Meter2
    {
        get { return _meter2; }
        set
        {
            _meter2 = value;
            meter2Text.text = _meter2.ToString();
        }
    }

    public override SavedNoteData GetNoteData()
    {
        return new SavedMeterChangerNoteData()
        {
            Beat = beat,
            beatPerBar = _meter1,
            meter2 = _meter2
        };
    }

    int beforeBeat = -1;

    public override void OnHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        base.OnHolding(inputPos, holdingSpaceLocalPosition);
        if (beat != beforeBeat)
        {
            beforeBeat = beat;
            MapEditManager.Instance.RefreshNotesPosition();
        }
    }

    public override void OnStopHolding(Vector2 inputPos, Vector2Int holdingSpaceLocalPosition)
    {
        base.OnStopHolding(inputPos, holdingSpaceLocalPosition);
        MapEditManager.Instance.RefreshNotesPosition();
    }

    public override void DeleteNote()
    {
        base.DeleteNote();
        MapEditManager.Instance.RefreshNotesPosition();
    }
}
