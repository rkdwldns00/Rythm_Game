using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoteObject : MonoBehaviour
{

}

public class SavedBasicNoteData : SavedNoteData
{
    public override GameObject NotePrefab => NoteManager.instance.basicNotePrefab;

    public float startX;
    public float endX;
}