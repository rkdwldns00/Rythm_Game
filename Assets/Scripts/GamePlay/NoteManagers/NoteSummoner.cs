using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSummoner : NotePosCalculator
{
    readonly Transform field;

    public NoteSummoner(SavedMapData map, Transform field, float noteDownSpeed) : base(noteDownSpeed, map)
    {
        this.field = field;
    }

    public void SummmonMap()
    {
        foreach (SavedNoteData note in map.notes)
        {
            Note noteObject = note.Summon(this, note);

            if (noteObject != null)
            {
                noteObject.whenExecuteTime = BeatToSec(note.whenSummonBeat);
            }

        }
    }

    public GameObject InstantiateNote(GameObject prefab, float xPos, float yPos)
    {
        GameObject g = Object.Instantiate(prefab, field);
        g.transform.localPosition = new Vector3(xPos - 7, yPos + NoteManager.NOTE_CHECK_YPOS, -0.01f);
        NoteManager.instance.AddNoteDownListener(g.transform);
        return g;
    }
}

public class NotePosCalculator
{
    public readonly float spacing;
    public readonly SavedMapData map;

    public NotePosCalculator(float spacing, SavedMapData map)
    {
        this.spacing = spacing;
        this.map = map;
    }

    public float BeatToSec(float beat)
    {
        float curruntBpm = map.startBpm;

        List<SavedBPMChangeNoteData> bpmChangers = new List<SavedBPMChangeNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedBPMChangeNoteData bpm = note as SavedBPMChangeNoteData;
            if (bpm is not null)
            {
                bpmChangers.Add(bpm);
            }
        }

        float sumTime = 0;
        int i;
        for (i = 0; i < beat; i++)
        {
            if (bpmChangers.Count > 0 && bpmChangers[0].whenSummonBeat <= i)
            {
                curruntBpm = bpmChangers[0].bpm;
                bpmChangers.RemoveAt(0);
            }
            sumTime += 60f * BeatPerBarLengthRate(i) * 4 / curruntBpm;
        }
        sumTime += 60f * BeatPerBarLengthRate(i) * 4 / curruntBpm * (beat - i);
        return sumTime;
    }

    float BeatPerBarLengthRate(float beat)
    {
        float curruntBeatPerBar = 1f / 4f;
        List<SavedMeterChangerNoteData> meters = new List<SavedMeterChangerNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedMeterChangerNoteData meter = note as SavedMeterChangerNoteData;
            if (meter is not null)
            {
                meters.Add(meter);
            }
        }

        if (meters.Count > 0)
        {
            meters.Sort((x, y) => { return x.whenSummonBeat - y.whenSummonBeat; });

            int lastMeterChangerIndex = -1;
            for (int i = 0; i < meters.Count; i++)
            {
                if (meters[i].whenSummonBeat <= beat)
                {
                    lastMeterChangerIndex = i;
                }
            }

            if (lastMeterChangerIndex >= 0)
            {
                curruntBeatPerBar = meters[lastMeterChangerIndex].beatLengthRate / meters[lastMeterChangerIndex].beatPerBar;
            }
        }
        return curruntBeatPerBar;
    }

    public float BeatToYpos(float beat)
    {
        float curruntSpeed = 1f;
        List<SavedSpeedChangerNoteData> speedChangers = new List<SavedSpeedChangerNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedSpeedChangerNoteData changer = note as SavedSpeedChangerNoteData;
            if (changer is not null)
            {
                speedChangers.Add(changer);
            }
        }

        if (speedChangers.Count > 0)
        {
            speedChangers.Sort((x, y) => { return x.whenSummonBeat - y.whenSummonBeat; });

            int lastBpmChangerIndex = -1;
            for (int i = 0; i < speedChangers.Count; i++)
            {
                if (speedChangers[i].whenSummonBeat <= beat)
                {
                    lastBpmChangerIndex = i;
                }
            }

            float beatHis = 0;
            float sumTime = 0;
            for (int i = 0; i < lastBpmChangerIndex + 1; i++)
            {
                sumTime += (BeatToSec(speedChangers[i].whenSummonBeat) - BeatToSec(beatHis)) * curruntSpeed * spacing;
                beatHis = speedChangers[i].whenSummonBeat;
                curruntSpeed = speedChangers[i].noteDownSpeedRate;
            }
            sumTime += (BeatToSec(beat) - BeatToSec(beatHis)) * curruntSpeed * spacing;
            return sumTime;
        }
        else
        {
            return BeatToSec(beat) * curruntSpeed * spacing;
        }
    }
}