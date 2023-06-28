using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChangerNoteObject : Note
{
    float timer = 0;
    public float noteDownSpeedRate;

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > whenExecuteTime) {
            NoteManager.instance.noteDownSpeedRate = noteDownSpeedRate;
            Destroy(gameObject);
        }
    }
}

class SavedSpeedChangerNoteData : SavedNoteData, ISummonable
{
    public GameObject NotePrefab => NoteManager.instance.speedChangerPrefab;
    public float noteDownSpeedRate = 1f;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        SpeedChangerNoteObject note = summoner.InstantiateNote(NotePrefab, 0, summoner.BeatToYpos(whenSummonBeat)).GetComponent<SpeedChangerNoteObject>();
        note.noteDownSpeedRate = noteDownSpeedRate;
        return note;
    }
}
