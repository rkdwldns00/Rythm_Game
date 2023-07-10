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

            barLength[i] = new KeyValuePair<int, float>(barIndex, meter);
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
                backData[j] = Convert.ToInt32(mapStringData[i][middleIndex + j + 1].ToString(), 16);
            }

            lines[i] = new SUSLineData() { bar = bar, frontData = frontData, backData = backData };
        }

        //반환용 맵 데이터 생성
        SavedMapData mapData = new SavedMapData();
        mapData.name = "Downloaded Map";
        mapData.startBpm = bpmData[0];
        List<SavedNoteData> notes = new List<SavedNoteData>();

        //박자 체크
        List<BeatPerBar> beatPerBarDatas = new List<BeatPerBar>();
        int lastAddedBarInBeatPerBar = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            int origin = lines[i].backData.Length / 2;
            int value = origin;
            if (lines[i].frontData[1] != 0)
            {
                if (lines[i].bar != lastAddedBarInBeatPerBar)
                {
                    if ((value % 2 == 0 || value == 1) && value < 16)
                    {
                        value = 16;
                    }
                    beatPerBarDatas.Add(new BeatPerBar(lines[i].bar, value, origin));
                    lastAddedBarInBeatPerBar = lines[i].bar;
                }
                else
                {
                    for (int j = 0; j < beatPerBarDatas.Count; j++)
                    {
                        if (beatPerBarDatas[j].bar == lines[i].bar)
                        {
                            BeatPerBar data = beatPerBarDatas[j];
                            data.beatCount = Mathf.Max(data.beatCount, lines[i].backData.Length / 2);
                            beatPerBarDatas[j] = data;
                            break;
                        }
                    }
                }
            }
        }
        beatPerBarDatas.Sort((a, b) =>
        {
            return a.bar - b.bar;
        });
        int sumBeat = 0;
        for (int i = 0; i < beatPerBarDatas.Count; i++)
        {
            notes.Add(new SavedMeterChangerNoteData() { beatPerBar = beatPerBarDatas[i].beatCount, whenSummonBeat = sumBeat });
            if (i < beatPerBarDatas.Count - 1)
            {
                sumBeat += beatPerBarDatas[i].beatCount * (beatPerBarDatas[i + 1].bar - beatPerBarDatas[i].bar);
            }
        }

        List<(int beat, float startX, float endX, SavedHoldNoteCurveType curveType, bool isCritical, int id)> holdStartDatas = new List<(int beat, float startX, float endX, SavedHoldNoteCurveType curveType, bool isCritical, int id)>();
        List<(int beat, float startX, float endX, int id)> holdEndDatas = new List<(int beat, float startX, float endX, int id)>();
        List<(int beat, float startX, float endX, SavedHoldNoteCurveType curveType, int id)> holdCurveDatas = new List<(int beat, float startX, float endX, SavedHoldNoteCurveType curveType, int id)>();

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
                    if (beatPerBarDatas[j].bar >= e)
                    {
                        b = beatPerBarDatas[j].beatCount;
                        break;
                    }
                }
                barStartBeat += b;
            }

            BeatPerBar beatPerBar = new BeatPerBar(line.bar, 16, 16);
            //마디길이계산 추가필요
            for (int j = 0; j < beatPerBarDatas.Count; j++)
            {
                if (beatPerBarDatas[j].bar >= e)
                {
                    if (beatPerBarDatas[j].bar == e)
                    {
                        beatPerBar = beatPerBarDatas[j];
                    }
                    else
                    {
                        beatPerBar = beatPerBarDatas[Mathf.Max(j - 1, 0)];
                    }
                    break;
                }
            }

            float barLengthRate = 1;
            for (int j = 0; j < barLength.Length; j++)
            {
                if (barLength[j].Key >= e)
                {
                    if (barLength[j].Key == e)
                    {
                        barLengthRate = barLength[j].Value / 4f;
                    }
                    else
                    {
                        barLengthRate = barLength[Mathf.Max(j - 1, 0)].Value / 4f;
                    }
                    break;
                }
            }

            //뒷부분 데이터를 통해 실제 노트 작성
            for (int i = 0; i < line.backData.Length / 2; i++)
            {
                int whenSummonBeat = barStartBeat + i * (int)((beatPerBar.beatCount / (line.backData.Length / 2)));
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
                    case 3: //홀드노트
                        if (line.backData[i * 2] == 1) //홀드시작
                        {
                            bool isCritical = false;
                            bool isHaveNote = false;
                            SavedHoldNoteCurveType curveType = SavedHoldNoteCurveType.Basic;
                            for (int j = 0; j < notes.Count; j++)
                            {
                                SavedBasicNoteData b = notes[j] as SavedBasicNoteData;
                                SavedCurveTypeRgsister curveResister = notes[j] as SavedCurveTypeRgsister;
                                if (b != null)
                                {
                                    if (b.whenSummonBeat == whenSummonBeat && b.startX == startX && b.endX == endX)
                                    {
                                        isHaveNote = true;
                                        isCritical = b is SavedCriticalBasicNoteData;
                                        b.isHoldStartNote = true;
                                    }
                                }
                                else if (curveResister != null)//컷인, 컷아웃 홀드시작
                                {
                                    if (curveResister.whenSummonBeat == whenSummonBeat && curveResister.startX == startX && curveResister.endX == endX)
                                    {
                                        curveType = curveResister.curveType;
                                        isCritical = curveResister.isCritical;
                                        notes.RemoveAt(j);
                                    }
                                }
                            }

                            if (!isHaveNote)
                            {
                                SavedBasicNoteData b = new SavedBasicNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat, isHoldStartNote = true };
                                b.isHoldStartNote = true;
                                notes.Add(b);
                            }

                            holdStartDatas.Add(new(whenSummonBeat, startX, endX, curveType, isCritical, line.frontData[2]));
                        }
                        else if (line.backData[i * 2] == 2) //홀드끝
                        {
                            holdEndDatas.Add(new(whenSummonBeat, startX, endX, line.frontData[2]));
                            bool isHaveDataNote = false;
                            for (int j = 0; j < notes.Count; j++)
                            {
                                SavedBasicNoteData b = notes[j] as SavedBasicNoteData;
                                SavedFlickNoteData f = notes[j] as SavedFlickNoteData;
                                if (b != null)
                                {
                                    if (b.whenSummonBeat == whenSummonBeat && b.startX == startX && b.endX == endX)
                                    {
                                        bool isCritical = b is SavedCriticalBasicNoteData;
                                        SavedHoldEndNoteData holdEnd = null;
                                        if (isCritical)
                                        {
                                            holdEnd = new SavedCriticalHoldEndNoteData();
                                        }
                                        else
                                        {
                                            holdEnd = new SavedHoldEndNoteData();
                                        }
                                        holdEnd.startX = startX;
                                        holdEnd.endX = endX;
                                        holdEnd.whenSummonBeat = whenSummonBeat;
                                        notes[j] = holdEnd;
                                        isHaveDataNote = true;
                                        break;
                                    }
                                }
                                else if (f != null)
                                {
                                    if (f.whenSummonBeat == whenSummonBeat && f.startX == startX && f.endX == endX)
                                    {
                                        f.startX = startX;
                                        f.endX = endX;
                                        f.whenSummonBeat = whenSummonBeat;
                                        f.needTouchStart = false;
                                        isHaveDataNote = true;
                                        break;
                                    }
                                }
                            }
                            if (!isHaveDataNote)
                            {
                                notes.Add(new SavedHoldEndNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                            }
                        }
                        else if (line.backData[i * 2] == 3) //커브
                        {
                            SavedHoldNoteCurveType curveType = SavedHoldNoteCurveType.Basic;
                            for (int j = 0; j < notes.Count; j++)
                            {
                                SavedCurveTypeRgsister curveResister = notes[j] as SavedCurveTypeRgsister;
                                if (curveResister != null)//컷인, 컷아웃 홀드시작
                                {
                                    if (curveResister.whenSummonBeat == whenSummonBeat && curveResister.startX == startX && curveResister.endX == endX)
                                    {
                                        Debug.Log(curveResister.whenSummonBeat + "-" + curveResister.curveType);
                                        curveType = curveResister.curveType;
                                        notes.RemoveAt(j);
                                        break;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            holdCurveDatas.Add(new(whenSummonBeat, startX, endX, curveType, line.frontData[2]));
                        }
                        break;
                    case 5: //플릭노트, 커브타입데이터
                        for (int j = 0; j < notes.Count; j++)
                        {
                            SavedBasicNoteData b = notes[j] as SavedBasicNoteData;
                            if (b == null)
                            {
                                continue;
                            }
                            if (b.whenSummonBeat == whenSummonBeat && b.startX == startX && b.endX == endX)
                            {
                                int flickData = line.backData[i * 2];
                                if (flickData == 1 || flickData == 3 || flickData == 4) //플릭노트일 경우
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
                                else //커브타입데이터일 경우
                                {
                                    SavedCurveTypeRgsister register = new SavedCurveTypeRgsister();
                                    register.startX = startX;
                                    register.endX = endX;
                                    register.whenSummonBeat = whenSummonBeat;
                                    register.isCritical = b is SavedCriticalBasicNoteData;

                                    if (flickData == 6)
                                    {
                                        register.curveType = SavedHoldNoteCurveType.CurveIn;
                                    }
                                    else if (flickData == 2)
                                    {
                                        register.curveType = SavedHoldNoteCurveType.CurveOut;
                                    }

                                    notes[j]=register;
                                }
                            }
                        }
                        break;
                }
            }
        }

        holdStartDatas.Sort((a, b) => a.beat - b.beat);
        holdEndDatas.Sort((a, b) => a.beat - b.beat);
        holdCurveDatas.Sort((a, b) => a.beat - b.beat);
        int len = holdStartDatas.Count;
        for (int i = 0; i < len; i++)
        {
            int id = holdStartDatas[0].id;
            for (int j = 0; j < holdEndDatas.Count; j++)
            {
                if (holdEndDatas[j].id == id)
                {
                    SavedHoldNoteData h = null;
                    if (holdStartDatas[0].isCritical)
                    {
                        //크리티컬 홀드노트 추가시 수정
                        h = new SavedCriticalHoldNoteData();
                        for (int k = 0; k < notes.Count; k++)
                        {
                            SavedHoldEndNoteData endNote = notes[k] as SavedHoldEndNoteData;
                            if (endNote != null && endNote.whenSummonBeat == holdEndDatas[j].beat && endNote.startX == holdEndDatas[j].startX && endNote.endX == holdEndDatas[j].endX)
                            {
                                SavedCriticalHoldEndNoteData newCiriticalEndNote = new SavedCriticalHoldEndNoteData();
                                newCiriticalEndNote.whenSummonBeat = endNote.whenSummonBeat;
                                newCiriticalEndNote.startX = endNote.startX;
                                newCiriticalEndNote.endX = endNote.endX;
                                notes[k] = newCiriticalEndNote;
                                break;
                            }
                        }
                    }
                    else
                    {
                        h = new SavedHoldNoteData();
                    }
                    h.whenSummonBeat = holdStartDatas[0].beat;

                    List<SavedHoldNoteCurve> curveList = new List<SavedHoldNoteCurve>();
                    curveList.Add(new SavedHoldNoteCurve() { spawnBeat = 0, startX = holdStartDatas[0].startX, endX = holdStartDatas[0].endX, curveType = holdStartDatas[0].curveType });

                    for (int k = 0; k < holdCurveDatas.Count; k++)
                    {
                        if (holdCurveDatas[k].beat <= holdStartDatas[0].beat || holdCurveDatas[k].beat > holdEndDatas[j].beat)
                        {
                            continue;
                        }
                        if (holdCurveDatas[k].id == id && holdStartDatas[0].beat < holdCurveDatas[k].beat && holdStartDatas[0].beat < holdEndDatas[j].beat)
                        {
                            curveList.Add(new SavedHoldNoteCurve(holdCurveDatas[k].startX, holdCurveDatas[k].endX, holdCurveDatas[k].beat - holdStartDatas[0].beat, holdCurveDatas[k].curveType));
                            /*if (holdCurveDatas[k].curveType != SavedHoldNoteCurveType.Basic && k < holdCurveDatas.Count)
                            {
                                int endBeat;
                                float endStartX;
                                float endEndX;
                                if (k < holdCurveDatas.Count - 1)
                                {
                                    endBeat = holdCurveDatas[k + 1].beat;
                                    endStartX = holdCurveDatas[k + 1].startX;
                                    endEndX = holdCurveDatas[k + 1].endX;
                                }
                                else
                                {
                                    endBeat = holdEndDatas[j].beat;
                                    endStartX = holdEndDatas[j].startX;
                                    endEndX = holdEndDatas[j].endX;
                                }
                                for (float curveBeat = holdCurveDatas[k].beat + 0.5f; curveBeat < endBeat; curveBeat += 0.5f)
                                {
                                    Vector2 leftStartPos = new Vector2(holdCurveDatas[k].startX, holdCurveDatas[k].beat);
                                    Vector2 leftEndPos = new Vector2(endStartX, endBeat);

                                    Vector2 rightStartPos = new Vector2(holdCurveDatas[k].endX, holdCurveDatas[k].beat);
                                    Vector2 rightEndPos = new Vector2(endEndX, endBeat);

                                    float lerpValue = (curveBeat - holdCurveDatas[k].beat) / ((float)endBeat - holdCurveDatas[k].beat);

                                    Vector2 leftViaPos = Vector2.zero;
                                    Vector2 rightViaPos = Vector2.zero;

                                    if (holdCurveDatas[k].curveType == SavedHoldNoteCurveType.CurveIn)
                                    {
                                        leftViaPos = new Vector2(leftEndPos.x, leftStartPos.y);
                                        rightViaPos = new Vector2(rightEndPos.x, rightStartPos.y);
                                    }
                                    else if (holdCurveDatas[k].curveType == SavedHoldNoteCurveType.CurveOut)
                                    {
                                        leftViaPos = new Vector2(leftStartPos.x, leftEndPos.y);
                                        rightViaPos = new Vector2(rightStartPos.x, rightEndPos.y);
                                    }
                                    curveList.Add(new SavedHoldNoteCurve()
                                    {
                                        startX = MyUtil.BezierCalCulate(lerpValue, leftStartPos, leftViaPos, leftEndPos).x,
                                        endX = MyUtil.BezierCalCulate(lerpValue, rightStartPos, rightViaPos, rightEndPos).x,
                                        spawnBeat = curveBeat - holdStartDatas[0].beat,
                                        curveType = holdCurveDatas[k].curveType,
                                    });
                                }
                            }*/

                            holdCurveDatas.RemoveAt(k);
                            k--;
                        }
                    }

                    curveList.Add(new SavedHoldNoteCurve() { spawnBeat = holdEndDatas[j].beat - holdStartDatas[0].beat, startX = holdEndDatas[j].startX, endX = holdEndDatas[j].endX });

                    h.curveData = curveList.ToArray();

                    notes.Add(h);
                    holdStartDatas.RemoveAt(0);
                    holdEndDatas.RemoveAt(j);
                    break;
                }
            }
        }

        notes.Sort((a, b) => a.whenSummonBeat - b.whenSummonBeat);

        for (int i = 0; i < notes.Count; i++)
        {
            string message = notes[i].whenSummonBeat + "-" + notes[i].GetType() + ":";
            if (notes[i] is SavedMeterChangerNoteData)
            {
                message += ((SavedMeterChangerNoteData)notes[i]).beatPerBar;
            }
            if (notes[i] is SavedBPMChangeNoteData)
            {
                message += ((SavedBPMChangeNoteData)notes[i]).bpm;
            }
            Debug.Log(message);
        }

        mapData.notes = notes.ToArray();
        return mapData;
    }

    public static string ReadTxt(string fileName)
    {

        TextAsset text = Resources.Load<TextAsset>(fileName);
        string value = text.text;

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

    struct BeatPerBar
    {
        public int bar;
        public int beatCount;
        public readonly int originBeatCount;

        public BeatPerBar(int bar, int beatCount, int originBeatCount)
        {
            this.bar = bar;
            this.beatCount = beatCount;
            this.originBeatCount = originBeatCount;
        }
    }
}
