using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SUSConveter
{
    public static SavedMapData Converte(string SUSData)
    {
        SUSData = SUSData.Replace("\r", "");

        //�ٹٲ��� �������� ������ ����
        string[] splitForEnterData = SUSData.Split("\n");

        //������ ���ۺκ� ����
        int requestTickPerBeatIndex = -1;
        for (int i = 0; i < splitForEnterData.Length; i++)
        {
            if (splitForEnterData[i].Contains("#REQUEST "))
            {
                requestTickPerBeatIndex = i;
                break;
            }
        }
        if (requestTickPerBeatIndex == -1)
        {
            Warring(0, "#REQEST \"ticks_per_beat\"�� �������� �ʽ��ϴ�.");
        }

        //�����͸� BPM������, ���ڵ�����, ��Ʈ�����ͷ� ����
        List<string> bpmStringData = new List<string>();
        List<string> meterStringData = new List<string>();
        List<string> mapStringData = new List<string>();
        for (int i = requestTickPerBeatIndex + 1; i < splitForEnterData.Length; i++)
        {
            if (splitForEnterData[i].Length == 0 || splitForEnterData[i][0] != '#') continue;
            if (splitForEnterData[i].Contains("#BPM")) bpmStringData.Add(splitForEnterData[i]);
            else if (splitForEnterData[i].Substring(4, 2) == "02") meterStringData.Add(splitForEnterData[i]);
            else mapStringData.Add(splitForEnterData[i]);
        }

        //bpmData �迭�� ����� BPM����
        float[] bpmData = new float[bpmStringData.Count];
        foreach (string source in bpmStringData)
        {
            int bpmIndex = int.Parse(source.Substring(4, 2));

            int dataStartIndex = source.IndexOf(":") + 1;
            float bpm = float.Parse(source.Substring(dataStartIndex, source.Length - dataStartIndex));

            bpmData[bpmIndex - 1] = bpm;
        }

        //meterData �迭�� ����� ��������
        KeyValuePair<int, float>[] barLength = new KeyValuePair<int, float>[meterStringData.Count];
        for (int i = 0; i < meterStringData.Count; i++)
        {
            string source = meterStringData[i];
            int barIndex = int.Parse(source.Substring(1, 3));
            float meter = float.Parse(source.Substring(source.IndexOf(":") + 1, source.Length - source.IndexOf(":") - 1));

            barLength[i] = new KeyValuePair<int, float>(barIndex, meter / 4);
        }

        //��Ʈ������
        SUSLineData[] lines = new SUSLineData[mapStringData.Count];
        for (int i = 0; i < lines.Length; i++)
        {
            int middleIndex = mapStringData[i].IndexOf(":");

            //�ݷ� ������ ������ �ؼ�
            int bar = int.Parse(mapStringData[i].Substring(1, 3));
            int[] frontData = new int[middleIndex - 4];
            for (int j = 0; j < frontData.Length; j++)
            {
                frontData[j] = int.Parse(mapStringData[i][j + 4].ToString());
            }

            //�ݷ� ������ ������ �ؼ�
            int[] backData = new int[mapStringData[i].Length - middleIndex - 1];
            for (int j = 0; j < backData.Length; j++)
            {
                backData[j] = int.Parse(mapStringData[i][middleIndex + j + 1].ToString());
            }

            lines[i] = new SUSLineData() { bar = bar, frontData = frontData, backData = backData };
        }

        //��ȯ�� �� ������ ����
        SavedMapData mapData = new SavedMapData();
        mapData.name = "Downloaded Map";
        mapData.startBpm = bpmData[0];
        List<SavedNoteData> notes = new List<SavedNoteData>();

        //���� üũ
        List<KeyValuePair<int, int>> beatPerBarDatas = new List<KeyValuePair<int, int>>();
        foreach (SUSLineData line in lines)
        {
            int value = line.backData.Length / 2;
            if ((value % 2 == 0 || value == 1) && value < 16)
            {
                value = 16;
            }
            beatPerBarDatas.Add(new KeyValuePair<int, int>(line.bar, value));
        }
        beatPerBarDatas.Sort((a, b) =>
        {
            return a.Key - b.Key;
        });

        int sumBeat = 0;
        for (int i = 0; i < beatPerBarDatas.Count; i++)
        {
            notes.Add(new SavedMeterChangerNoteData() { beatPerBar = beatPerBarDatas[i].Value, whenSummonBeat = sumBeat });
            if (i < beatPerBarDatas.Count - 1)
            {
                sumBeat += beatPerBarDatas[i].Value * (beatPerBarDatas[i + 1].Key - beatPerBarDatas[i].Key);
            }
        }

        //��Ʈ����Ʈ�� SUS�����͸� �ص��Ͽ� �߰�
        foreach (SUSLineData line in lines)
        {
            //������ ���ۺ�Ʈ�� ���
            int barStartBeat = 0;
            int e = 0;
            for (e = 0; e < line.bar; e++)
            {
                int b = 0;
                for (int j = 0; j < beatPerBarDatas.Count; j++)
                {
                    if (beatPerBarDatas[j].Key >= e)
                    {
                        b = beatPerBarDatas[j].Value;
                        break;
                    }
                }
                barStartBeat += b;
            }

            //�޺κ� �����͸� ���� ���� ��Ʈ �ۼ�
            for (int i = 0; i < line.backData.Length / 2; i++)
            {
                int whenSummonBeat = barStartBeat + i * (beatPerBarDatas[e + 1].Value / (line.backData.Length / 2));
                float startX = line.frontData[1];
                float endX = line.frontData[1] + line.backData[i * 2 + 1] - 1;
                switch (line.frontData[0])
                {
                    case 0:
                        if (line.frontData[1] == 8)
                        {
                            notes.Add(new SavedBPMChangeNoteData() { whenSummonBeat = whenSummonBeat, bpm = bpmData[line.backData[0] * 10 + line.backData[1] - 1] });
                        }
                        break;
                    case 1:
                        if (line.backData[i * 2] == 1)
                        {
                            notes.Add(new SavedBasicNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                        }
                        else if (line.backData[i * 2] == 2)
                        {
                            notes.Add(new SavedCriticalBasicNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                        }
                        break;
                }
            }
        }

        notes.Sort((a, b) => a.whenSummonBeat - b.whenSummonBeat);

        foreach (var note in notes)
        {
            if (note is SavedBasicNoteData)
            {
                Debug.Log(note.whenSummonBeat + " : Basic");
            }
            else if(note is SavedMeterChangerNoteData)
            {
                Debug.Log(note.whenSummonBeat + " : meter=" + ((SavedMeterChangerNoteData)note).beatPerBar);
            }else if (note is SavedBPMChangeNoteData)
            {
                Debug.Log(note.whenSummonBeat + " : bpm=" + ((SavedBPMChangeNoteData)note).bpm);
            }
        }

        mapData.notes = notes.ToArray();
        return mapData;
    }

    static void Warring(int errorCode, params string[] logs)
    {
        string message = "SUS Error-" + errorCode + " : SUS������ �д� �������� ������ �߻��Ͽ����ϴ�.";
        foreach (string log in logs)
        {
            message += "\n" + log;
        }
        Debug.Log(message);
    }

    struct SUSLineData
    {
        public int bar;
        public int[] frontData;
        public int[] backData;
    }
}
