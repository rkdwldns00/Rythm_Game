using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SUSConveter
{
    public static SavedMapData ConvertMapData(string SUSData)
    {
        SUSData = SUSData.Replace("\r", "");

        //줄바꿈을 기준으로 데이터 분할
        string[] splitForEnterData = SUSData.Split("\n");

        //데이터 시작부분 감지
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
            Warring("파일에 #REQEST \"ticks_per_beat\"명령이 존재하지 않습니다.");
        }

        //데이터를 BPM데이터, 박자데이터, 노트데이터로 분할
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

        //bpmData 배열에 사용할 BPM저장
        float[] bpmData = new float[bpmStringData.Count];
        foreach (string source in bpmStringData)
        {
            int bpmIndex = int.Parse(source.Substring(4, 2));

            int dataStartIndex = source.IndexOf(":") + 1;
            float bpm = float.Parse(source.Substring(dataStartIndex, source.Length - dataStartIndex));

            bpmData[bpmIndex - 1] = bpm;
        }

        //meterData 배열에 사용할 박자저장
        KeyValuePair<int, float>[] barLength = new KeyValuePair<int, float>[meterStringData.Count];
        for (int i = 0; i < meterStringData.Count; i++)
        {
            string source = meterStringData[i];
            int barIndex = int.Parse(source.Substring(1, 3));
            float meter = float.Parse(source.Substring(source.IndexOf(":") + 1, source.Length - source.IndexOf(":") - 1));

            barLength[i] = new KeyValuePair<int, float>(barIndex, meter / 4);
        }

        //노트데이터
        SUSLineData[] lines = new SUSLineData[mapStringData.Count];
        for (int i = 0; i < lines.Length; i++)
        {
            int middleIndex = mapStringData[i].IndexOf(":");

            //콜론 앞쪽의 데이터 해석
            int bar = int.Parse(mapStringData[i].Substring(1, 3));
            int[] frontData = new int[middleIndex - 4];
            for (int j = 0; j < frontData.Length; j++)
            {
                frontData[j] = Convert.ToInt32(mapStringData[i][j + 4].ToString(), 16);
            }

            //콜론 뒤쪽의 데이터 해석
            int[] backData = new int[mapStringData[i].Length - middleIndex - 1];
            for (int j = 0; j < backData.Length; j++)
            {
                backData[j] = int.Parse(mapStringData[i][middleIndex + j + 1].ToString());
            }

            lines[i] = new SUSLineData() { bar = bar, frontData = frontData, backData = backData };
        }

        //반환용 맵 데이터 생성
        SavedMapData mapData = new SavedMapData();
        mapData.name = "Downloaded Map";
        mapData.startBpm = bpmData[0];
        List<SavedNoteData> notes = new List<SavedNoteData>();

        //박자 체크
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

        //노트리스트에 SUS데이터를 해독하여 추가
        foreach (SUSLineData line in lines)
        {
            //마디의 시작비트를 계산
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

            //뒷부분 데이터를 통해 실제 노트 작성
            for (int i = 0; i < line.backData.Length / 2; i++)
            {
                int whenSummonBeat = barStartBeat + i * (beatPerBarDatas[e + 1].Value / (line.backData.Length / 2));
                float startX = line.frontData[1] - 1;
                float endX = line.frontData[1] + line.backData[i * 2 + 1] - 1;
                switch (line.frontData[0])
                {
                    case 0: //BPM 변경
                        if (line.frontData[1] == 8)
                        {
                            notes.Add(new SavedBPMChangeNoteData() { whenSummonBeat = whenSummonBeat, bpm = bpmData[line.backData[0] * 10 + line.backData[1] - 1] });
                        }
                        break;
                    case 1: //기본노트
                        if (line.backData[i * 2] == 1)
                        {
                            notes.Add(new SavedBasicNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                        }
                        else if (line.backData[i * 2] == 2)
                        {
                            notes.Add(new SavedCriticalBasicNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                        }
                        break;
                    case 5: //플릭노트
                        notes.ForEach((x) => Debug.Log(x));
                        for(int j=0; j<notes.Count; j++)
                        {
                            SavedBasicNoteData b = notes[j] as SavedBasicNoteData;
                            if (b == null)
                            {
                                continue;
                            }
                            if (b.whenSummonBeat == whenSummonBeat && b.startX == startX && b.endX == endX)
                            {
                                bool isCritical = b is SavedCriticalBasicNoteData;

                                SavedFlickNoteData f = null;
                                if (isCritical)
                                {
                                    f = new SavedCriticalFlickNoteData();
                                }
                                else
                                {
                                    f = new SavedFlickNoteData();
                                }

                                f.whenSummonBeat = whenSummonBeat;
                                f.startX = startX;
                                f.endX = endX;
                                f.needTouchStart = true;
                                switch (line.backData[i * 2])
                                {
                                    case 1:
                                        f.rotation = 0;
                                        break;
                                    case 3:
                                        f.rotation = -45f;
                                        break;
                                    case 4:
                                        f.rotation = 45f;
                                        break;
                                }

                                notes[j] = f;
                            }
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
            else if (note is SavedMeterChangerNoteData)
            {
                Debug.Log(note.whenSummonBeat + " : meter=" + ((SavedMeterChangerNoteData)note).beatPerBar);
            }
            else if (note is SavedBPMChangeNoteData)
            {
                Debug.Log(note.whenSummonBeat + " : bpm=" + ((SavedBPMChangeNoteData)note).bpm);
            }
        }

        mapData.notes = notes.ToArray();
        return mapData;
    }

    public static string ReadTxt(string fileName)
    {
        string filePath = Path.Combine("Assets/", fileName);
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            value = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            Warring("경로에 유효한 파일이 존재하지 않습니다.");
        }
        return value;
    }

    static void Warring(params string[] logs)
    {
        string message = "SUS Error:SUS파일을 읽는 과정에서 오류가 발생하였습니다.";
        foreach (string log in logs)
        {
            message += "\n" + log;
        }
        Debug.LogWarning(message);
    }

    struct SUSLineData
    {
        public int bar;
        public int[] frontData;
        public int[] backData;
    }
}
