using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class NotePosCalculator
{
    const int STARTING_BEAT_PER_BAR = 4;
    const int STARTING_METER2 = 4;

    public readonly float spacing;
    public readonly SavedMapData map;

    public NotePosCalculator(float spacing, SavedMapData map)
    {
        this.spacing = spacing;
        this.map = map;
    }

    public virtual float BeatToSec(float beat)
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
            if (bpmChangers.Count > 0 && bpmChangers[0].Beat <= i)
            {
                curruntBpm = bpmChangers[0].bpm;
                bpmChangers.RemoveAt(0);
            }
            sumTime += 60f * BeatPerBarLengthRate(i) / curruntBpm;
        }
        sumTime += 60f * BeatPerBarLengthRate(i) / curruntBpm * (beat - i);
        return sumTime;
    }

    public virtual int BeatOfBar(int barIndex)
    {
        if (barIndex == 0) { return 0; }

        List<SavedMeterChangerNoteData> meters = new List<SavedMeterChangerNoteData>();
        foreach (SavedNoteData note in map.notes)
        {
            SavedMeterChangerNoteData meter = note as SavedMeterChangerNoteData;
            if (meter is not null)
            {
                meters.Add(meter);
            }
        }
        if (meters.Count == 0 || meters[0].Beat != 0)
        {
            meters.Insert(0, new SavedMeterChangerNoteData() { beatPerBar = STARTING_BEAT_PER_BAR, meter2 = STARTING_METER2, Beat = 0 });
        }

        int bar = 0;
        for (int i = 0; i < meters.Count - 1; i++)
        {
            for (int j = meters[i]._beat; j < meters[i + 1].Beat; j = Mathf.Min(j + meters[i].beatPerBar, meters[i + 1]._beat))
            {
                if (bar == barIndex)
                {
                    return j;
                }
                bar++;
            }
        }

        return meters[meters.Count - 1]._beat + meters[meters.Count - 1].beatPerBar * (barIndex - bar);
    }

    float BeatPerBarLengthRate(float beat)
    {
        float curruntBeatPerBarLength = 1f / STARTING_METER2 * STARTING_BEAT_PER_BAR;
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
            meters.Sort((x, y) => (int)Mathf.Sign(x.Beat - y.Beat));

            int lastMeterChangerIndex = -1;
            for (int i = 0; i < meters.Count; i++)
            {
                if (meters[i].Beat <= beat)
                {
                    lastMeterChangerIndex = i;
                }
            }

            if (lastMeterChangerIndex >= 0)
            {
                curruntBeatPerBarLength = 1f / meters[lastMeterChangerIndex].meter2 * meters[lastMeterChangerIndex].beatPerBar;
            }
        }
        return curruntBeatPerBarLength;
    }

    public SavedMeterChangerNoteData FindLastMeterChanger(float beat)
    {
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
            meters.Sort((x, y) => (int)Mathf.Sign(x.Beat - y.Beat));

            int lastMeterChangerIndex = -1;
            for (int i = 0; i < meters.Count; i++)
            {
                if (meters[i].Beat <= beat)
                {
                    lastMeterChangerIndex = i;
                }
            }

            if (lastMeterChangerIndex >= 0)
            {
                return meters[lastMeterChangerIndex];
            }
        }
        return null;
    }

    public virtual float BeatToYpos(float beat)
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
            speedChangers.Sort((x, y) => { return (int)Mathf.Sign(x.Beat - y.Beat); });

            int lastBpmChangerIndex = -1;
            for (int i = 0; i < speedChangers.Count; i++)
            {
                if (speedChangers[i].Beat <= beat)
                {
                    lastBpmChangerIndex = i;
                }
            }

            float beatHis = 0;
            float sumTime = 0;
            for (int i = 0; i < lastBpmChangerIndex + 1; i++)
            {
                sumTime += (BeatToSec(speedChangers[i].Beat) - BeatToSec(beatHis)) * curruntSpeed * spacing;
                beatHis = speedChangers[i].Beat;
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

    public virtual int YposCloseToBeat(float y)
    {
        float h;

        int index = 0;
        do
        {
            h = BeatToYpos(index++);
        } while (h < y);
        return Mathf.Max(index - 2, 0);
    }

    public virtual (int beat, int indexInBeat) YposCloseToBeatWithNoteValue(float y, int standardNoteValue)
    {
        int beat = YposCloseToBeat(y);

        float h;
        int index = 0;
        do
        {
            h = BeatToYpos((float)beat + (float)index / ((float)standardNoteValue / 4f));
            index++;
        } while (h < y);

        return (beat, index - 1);
    }
}