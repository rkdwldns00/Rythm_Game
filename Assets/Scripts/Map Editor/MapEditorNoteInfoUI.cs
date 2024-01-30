using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MapEditorNoteInfoUI
{
    public void OnSelectNote(MapEditorNote note);
}

public abstract class MapEditorNoteInfoUI<T> : MonoBehaviour, MapEditorNoteInfoUI where T : MapEditorNote
{
    protected T referencedNote { get; private set; }

    public void OnSelectNote(MapEditorNote note)
    {
        if (note is T n)
        {
            referencedNote = n;
            gameObject.SetActive(true);
        }
        else
        {
            OnUnselectNote();
        }
    }

    protected abstract void OnActive();

    public void OnUnselectNote()
    {
        gameObject.SetActive(false);
    }
}
