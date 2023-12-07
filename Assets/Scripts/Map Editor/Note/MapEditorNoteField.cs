using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorNoteField : MonoBehaviour
{
    [SerializeField] Sprite fieldIcon;
    public Sprite FieldIcon => fieldIcon;

    [SerializeField] string fieldName;
    public string FieldName => fieldName;

    [SerializeField] float minValue;
    public float MinValue => minValue;

    [SerializeField] float maxValue;
    public float MaxValue => maxValue;

    [SerializeField] float value;
    public float Value => value;

    public void SetValue(float v)
    {
        value = Mathf.Clamp(value, minValue, maxValue);
    }
}
