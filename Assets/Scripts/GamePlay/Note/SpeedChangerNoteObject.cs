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

class SavedSpeedChangerNoteData : SavedNoteData, IGamePlaySummonable
{
    public override string serializedDataTitleName => "SD";
    public override float totalScore => 0;

    public GameObject GamePlayNotePrefab => NoteManager.instance.speedChangerPrefab;
    public float noteDownSpeedRate = 1f;

    public override MapEditorNote SummonMapEditorNote()
    {
        throw new System.NotImplementedException();
    }

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        SpeedChangerNoteObject note = summoner.InstantiateNote(GamePlayNotePrefab, 0, summoner.BeatToYpos(Beat)).GetComponent<SpeedChangerNoteObject>();
        note.noteDownSpeedRate = noteDownSpeedRate;
        return note;
    }
}
