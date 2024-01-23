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
            meter1Text.text = _meter2.ToString();
        }
    }

    public override SavedNoteData GetNoteData()
    {
        return new SavedMeterChangerNoteData()
        {
            whenSummonBeat = beat,
            beatPerBar = _meter1,
            beatLengthRate = _meter1 / _meter2
        };
    }
}
