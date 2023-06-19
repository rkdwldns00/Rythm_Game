using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoteObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public class SavedBasicNoteData : SavedNoteData
    {
        public override GameObject NotePrefab => NoteManager.instance.basicNotePrefab;

        public float startX;
        public float endX;

        public override Action Bake()
        {
            Action action = () =>
            {
                GameObject g = NoteManager.instance.InstantiateNote(NotePrefab, (startX + endX) / 2f);
                g.transform.localScale = new Vector3(endX - startX, NoteManager.NOTE_Y_SIZE, 1);
                NoteManager.instance.AddNoteDownListener(g.transform);
            };
            return action;
        }
    }
}