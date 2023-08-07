using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete()]
public class SUSConveter
{
    public static SavedMapData ConvertMapData(string SUSData)
    {
        //반환용 맵 데이터 생성
        SavedMapData newMapData = new();
        Debug.Log("컨버터 결과:" + newMapData);
        SUSData = SUSData.Replace("\r", "");

        //줄바꿈을 기준으로 데이터 분할
        string[] splitForEnterData = SUSData.Split("\n");

        //데이터 시작부분 감지
        int requestTickPerBeatIndex = -1;
        for (int readingLineIndex = 0; readingLineIndex < splitForEnterData.Length; readingLineIndex++)
        {
            string readingString = splitForEnterData[readingLineIndex];
            int nameStartIndex = readingString.IndexOf("\"") + 1;
            int nameEndIndex = readingString.LastIndexOf("\"");
            if (readingString.Contains("#TITLE "))
            {
                if (nameStartIndex == nameEndIndex + 1) Warring("제목 정보가 유효하지 않습니다.");
                newMapData.title = readingString.Substring(nameStartIndex, nameEndIndex - nameStartIndex);
            }
            else if (readingString.Contains("#ARTIST "))
            {
                if (nameStartIndex == nameEndIndex + 1) Warring("작곡가 정보가 유효하지 않습니다.");

                newMapData.artistName = readingString.Substring(nameStartIndex, nameEndIndex - nameStartIndex);
            }
            else if (readingString.Contains("#DESIGNER "))
            {
                if (nameStartIndex == nameEndIndex + 1) Warring("맵 디자이너 정보가 유효하지 않습니다.");

                newMapData.designerName = readingString.Substring(nameStartIndex, nameEndIndex - nameStartIndex);
            }
            else if (readingString.Contains("#WAVEOFFSET "))
            {
                if (!float.TryParse(readingString.Substring(12, readingString.Length - 12), out newMapData.startOffset)) Warring("곡의 시작 오프셋이 실수가 아닙니다.");
            }
            else if (readingString.Contains("#REQUEST "))
            {
                requestTickPerBeatIndex = readingLineIndex;
                break;
            }
        }
        if (requestTickPerBeatIndex == -1)
        {
            Warring("파일에 #REQEST \"ticks_per_beat\"명령이 존재하지 않습니다.");
        }

        //데이터를 BPM데이터, 박자데이터, 노트데이터로 분할
        List<string> bpmStringDatas = new();
        List<string> meterStringData = new();
        List<string> mapStringData = new();
        for (int readingLineIndex = requestTickPerBeatIndex + 1; readingLineIndex < splitForEnterData.Length; readingLineIndex++)
        {
            if (splitForEnterData[readingLineIndex].Length == 0 || splitForEnterData[readingLineIndex][0] != '#') continue;
            if (splitForEnterData[readingLineIndex].Contains("#BPM")) bpmStringDatas.Add(splitForEnterData[readingLineIndex]);
            else if (splitForEnterData[readingLineIndex].Substring(4, 2) == "02") meterStringData.Add(splitForEnterData[readingLineIndex]);
            else mapStringData.Add(splitForEnterData[readingLineIndex]);
        }

        //bpmDatas 배열에 사용할 BPM저장
        float[] bpmDatas = new float[bpmStringDatas.Count];
        foreach (string bpmStringData in bpmStringDatas)
        {
            int bpmIndex = -1;
            if (!int.TryParse(bpmStringData.Substring(4, 2), out bpmIndex) || bpmIndex < 1) Warring("BPM데이터의 인덱스가 유효하지 않습니다.", "오류위치 : " + bpmStringData);

            int dataStartIndex = bpmStringData.IndexOf(":") + 1;
            float bpm = -1;
            if (!float.TryParse(bpmStringData.Substring(dataStartIndex, bpmStringData.Length - dataStartIndex), out bpm)) Warring("BPM데이터의 BPM이 유효하지 않습니다.", "오류위치 : " + bpmStringData);

            bpmDatas[bpmIndex - 1] = bpm;
        }
        newMapData.startBpm = bpmDatas[0];

        //barLengthDatas 배열에 사용할 박자저장
        KeyValuePair<int, float>[] barLengthDatas = new KeyValuePair<int, float>[meterStringData.Count];
        for (int readingMeterDataIndex = 0; readingMeterDataIndex < meterStringData.Count; readingMeterDataIndex++)
        {
            string readingMeterString = meterStringData[readingMeterDataIndex];
            int barIndex = int.Parse(readingMeterString.Substring(1, 3));
            float meter = float.Parse(readingMeterString.Substring(readingMeterString.IndexOf(":") + 1, readingMeterString.Length - readingMeterString.IndexOf(":") - 1)) / 4;

            barLengthDatas[readingMeterDataIndex] = new KeyValuePair<int, float>(barIndex, meter);
        }

        //노트데이터
        SUSLineData[] mapDataLines = new SUSLineData[mapStringData.Count];
        for (int readingMapStringDataIndex = 0; readingMapStringDataIndex < mapDataLines.Length; readingMapStringDataIndex++)
        {
            int middleIndex = mapStringData[readingMapStringDataIndex].IndexOf(":");

            //콜론 앞쪽의 데이터 해석
            int bar = int.Parse(mapStringData[readingMapStringDataIndex].Substring(1, 3));
            int[] frontData = new int[middleIndex - 4];
            for (int writingFrontDataIndex = 0; writingFrontDataIndex < frontData.Length; writingFrontDataIndex++)
            {
                frontData[writingFrontDataIndex] = Convert.ToInt32(mapStringData[readingMapStringDataIndex][writingFrontDataIndex + 4].ToString(), 16);
            }

            //콜론 뒤쪽의 데이터 해석
            int[] backData = new int[mapStringData[readingMapStringDataIndex].Length - middleIndex - 1];
            for (int writingBackDataIndex = 0; writingBackDataIndex < backData.Length; writingBackDataIndex++)
            {
                backData[writingBackDataIndex] = Convert.ToInt32(mapStringData[readingMapStringDataIndex][middleIndex + writingBackDataIndex + 1].ToString(), 16);
            }

            mapDataLines[readingMapStringDataIndex] = new SUSLineData() { bar = bar, frontData = frontData, backData = backData };
        }


        List<SavedNoteData> newNoteDatas = new();

        //박자 체크
        List<BeatPerBar> beatPerBarDatas = new();
        int lastAddedBarInBeatPerBar = 0;
        for (int readingLineIndex = 0; readingLineIndex < mapDataLines.Length; readingLineIndex++)
        {
            int originBarLength = mapDataLines[readingLineIndex].backData.Length / 2;
            int nomalizedBarLength = originBarLength;
            if (mapDataLines[readingLineIndex].frontData[1] != 0)
            {
                if (mapDataLines[readingLineIndex].bar != lastAddedBarInBeatPerBar)
                {
                    /*if ((nomalizedBarLength % 2 == 0 || nomalizedBarLength == 1) && nomalizedBarLength < 16)
                    {
                        nomalizedBarLength = 16;
                    }*/
                    beatPerBarDatas.Add(new BeatPerBar(mapDataLines[readingLineIndex].bar, nomalizedBarLength, originBarLength));
                    lastAddedBarInBeatPerBar = mapDataLines[readingLineIndex].bar;
                }
                else
                {
                    for (int readingBeatPerBarDataIndex = 0; readingBeatPerBarDataIndex < beatPerBarDatas.Count; readingBeatPerBarDataIndex++)
                    {
                        if (beatPerBarDatas[readingBeatPerBarDataIndex].bar == mapDataLines[readingLineIndex].bar)
                        {
                            BeatPerBar data = beatPerBarDatas[readingBeatPerBarDataIndex];
                            data.beatCount = Mathf.Max(data.beatCount, mapDataLines[readingLineIndex].backData.Length / 2);
                            beatPerBarDatas[readingBeatPerBarDataIndex] = data;
                            break;
                        }
                    }
                }
            }
        }
        beatPerBarDatas.Sort((a, b) => a.bar - b.bar);
        int sumBeat = 0;
        for (int readingBeatPerBarDataIndex = 0; readingBeatPerBarDataIndex < beatPerBarDatas.Count; readingBeatPerBarDataIndex++)
        {
            float beatLengthRate = 1f;
            for (int i = barLengthDatas.Length - 1; i >= 0; i--)
            {
                if (barLengthDatas[i].Key < beatPerBarDatas[readingBeatPerBarDataIndex].bar)
                {
                    beatLengthRate = barLengthDatas[i].Value;
                    break;
                }
            }
            newNoteDatas.Add(new SavedMeterChangerNoteData()
            {
                beatPerBar = beatPerBarDatas[readingBeatPerBarDataIndex].beatCount,
                whenSummonBeat = sumBeat,
                beatLengthRate = beatLengthRate
            });
            if (readingBeatPerBarDataIndex < beatPerBarDatas.Count - 1)
            {
                sumBeat += beatPerBarDatas[readingBeatPerBarDataIndex].beatCount * (beatPerBarDatas[readingBeatPerBarDataIndex + 1].bar - beatPerBarDatas[readingBeatPerBarDataIndex].bar);
            }
        }

        List<(int beat, float startX, float endX, SavedHoldNoteCurveType curveType, bool isCritical, int id)> holdStartDatas = new();
        List<(int beat, float startX, float endX, int id)> holdEndDatas = new();
        List<(int beat, float startX, float endX, SavedHoldNoteCurveType curveType, SavedTickType tickType, int id)> holdCurveDatas = new();

        //노트리스트에 SUS데이터를 해독하여 추가
        foreach (SUSLineData readingLine in mapDataLines)
        {
            //마디의 시작비트를 계산
            int barStartBeat = 0;
            int readingBar = 0;
            for (readingBar = 0; readingBar < readingLine.bar; readingBar++)
            {
                for (int i = 0; i < beatPerBarDatas.Count; i++)
                {
                    if (beatPerBarDatas[i].bar >= readingBar)
                    {
                        barStartBeat += beatPerBarDatas[i].beatCount;
                        break;
                    }
                }
            }

            BeatPerBar beatPerBar = new BeatPerBar(readingLine.bar, 16, 16);
            //마디길이계산 추가필요
            for (int readingBeatPerBarDataIndex = 0; readingBeatPerBarDataIndex < beatPerBarDatas.Count; readingBeatPerBarDataIndex++)
            {
                if (beatPerBarDatas[readingBeatPerBarDataIndex].bar >= readingBar)
                {
                    if (beatPerBarDatas[readingBeatPerBarDataIndex].bar == readingBar)
                    {
                        beatPerBar = beatPerBarDatas[readingBeatPerBarDataIndex];
                    }
                    else
                    {
                        beatPerBar = beatPerBarDatas[Mathf.Max(readingBeatPerBarDataIndex - 1, 0)];
                    }
                    break;
                }
            }

            float barLengthRate = 1;
            for (int readingBarLengthDataIndex = 0; readingBarLengthDataIndex < barLengthDatas.Length; readingBarLengthDataIndex++)
            {
                if (barLengthDatas[readingBarLengthDataIndex].Key >= readingBar)
                {
                    if (barLengthDatas[readingBarLengthDataIndex].Key == readingBar)
                    {
                        barLengthRate = barLengthDatas[readingBarLengthDataIndex].Value / 4f;
                    }
                    else
                    {
                        barLengthRate = barLengthDatas[Mathf.Max(readingBarLengthDataIndex - 1, 0)].Value / 4f;
                    }
                    break;
                }
            }

            //뒷부분 데이터를 통해 실제 노트 작성
            for (int indexOfReadingNoteWithBackData = 0; indexOfReadingNoteWithBackData < readingLine.backData.Length / 2; indexOfReadingNoteWithBackData++)
            {
                int whenSummonBeat = barStartBeat + indexOfReadingNoteWithBackData * (int)((beatPerBar.beatCount / (readingLine.backData.Length / 2)));
                float startX = readingLine.frontData[1] - 1;
                float endX = readingLine.frontData[1] + readingLine.backData[indexOfReadingNoteWithBackData * 2 + 1] - 1;
                switch (readingLine.frontData[0])
                {
                    case 0: //BPM 변경
                        if (readingLine.frontData[1] == 8)
                        {
                            newNoteDatas.Add(new SavedBPMChangeNoteData()
                            {
                                whenSummonBeat = whenSummonBeat,
                                bpm = bpmDatas[readingLine.backData[0] * 10 + readingLine.backData[1] - 1]
                            });
                        }
                        break;
                    case 1: //기본노트
                        if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 1)
                        {
                            newNoteDatas.Add(new SavedBasicNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                        }
                        else if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 2)
                        {
                            newNoteDatas.Add(new SavedBasicNoteData()
                            {
                                startX = startX,
                                endX = endX,
                                isCriticalNote = true,
                                whenSummonBeat = whenSummonBeat
                            });
                        }
                        else if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 3)
                        {
                            newNoteDatas.Add(new SavedIgnoreXTickRegister()
                            {
                                startX = startX,
                                endX = endX,
                                whenSummonBeat = whenSummonBeat
                            });
                        }
                        break;
                    case 3: //홀드노트
                        if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 1) //홀드시작
                        {
                            bool isCritical = false;
                            bool isHaveNote = false;
                            SavedHoldNoteCurveType curveType = SavedHoldNoteCurveType.Basic;
                            for (int readingNoteIndex = 0; readingNoteIndex < newNoteDatas.Count; readingNoteIndex++)
                            {
                                SavedBasicNoteData basic = newNoteDatas[readingNoteIndex] as SavedBasicNoteData;
                                SavedCurveTypeRgsister curveResister = newNoteDatas[readingNoteIndex] as SavedCurveTypeRgsister;
                                if (basic != null)
                                {
                                    if (basic.whenSummonBeat == whenSummonBeat && basic.startX == startX && basic.endX == endX)
                                    {
                                        isHaveNote = true;
                                        isCritical = basic.isCriticalNote;
                                        basic.isHoldStartNote = true;
                                    }
                                }
                                else if (curveResister != null)//컷인, 컷아웃 홀드시작
                                {
                                    if (curveResister.whenSummonBeat == whenSummonBeat &&
                                        curveResister.startX == startX &&
                                        curveResister.endX == endX)
                                    {
                                        curveType = curveResister.curveType;
                                        isCritical = curveResister.isCritical;
                                        newNoteDatas.RemoveAt(readingNoteIndex);
                                    }
                                }
                            }

                            if (!isHaveNote)
                            {
                                newNoteDatas.Add(new SavedBasicNoteData()
                                {
                                    startX = startX,
                                    endX = endX,
                                    whenSummonBeat = whenSummonBeat,
                                    isHoldStartNote = true
                                });
                            }

                            holdStartDatas.Add(new(whenSummonBeat, startX, endX, curveType, isCritical, readingLine.frontData[2]));
                        }
                        else if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 2) //홀드끝
                        {
                            holdEndDatas.Add(new(whenSummonBeat, startX, endX, readingLine.frontData[2]));
                            bool isHaveDataNote = false;
                            for (int checkingNoteIndex = 0; checkingNoteIndex < newNoteDatas.Count; checkingNoteIndex++)
                            {
                                SavedBasicNoteData basic = newNoteDatas[checkingNoteIndex] as SavedBasicNoteData;
                                SavedFlickNoteData flick = newNoteDatas[checkingNoteIndex] as SavedFlickNoteData;
                                if (basic != null)
                                {
                                    if (basic.whenSummonBeat == whenSummonBeat && basic.startX == startX && basic.endX == endX)
                                    {
                                        SavedHoldEndNoteData holdEnd = new() { isCriticalNote = basic.isCriticalNote };
                                        holdEnd.startX = startX;
                                        holdEnd.endX = endX;
                                        holdEnd.whenSummonBeat = whenSummonBeat;
                                        newNoteDatas[checkingNoteIndex] = holdEnd;
                                        isHaveDataNote = true;
                                        break;
                                    }
                                }
                                else if (flick != null)
                                {
                                    if (flick.whenSummonBeat == whenSummonBeat && flick.startX == startX && flick.endX == endX)
                                    {
                                        flick.startX = startX;
                                        flick.endX = endX;
                                        flick.whenSummonBeat = whenSummonBeat;
                                        flick.needTouchStart = false;
                                        isHaveDataNote = true;
                                        break;
                                    }
                                }
                            }
                            if (!isHaveDataNote)
                            {
                                newNoteDatas.Add(new SavedHoldEndNoteData() { startX = startX, endX = endX, whenSummonBeat = whenSummonBeat });
                            }
                        }
                        else if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 3 ||
                            readingLine.backData[indexOfReadingNoteWithBackData * 2] == 5) //커브
                        {
                            SavedTickType tickType = SavedTickType.Basic;
                            SavedHoldNoteCurveType curveType = SavedHoldNoteCurveType.Basic;

                            for (int checkingNoteIndex = 0; checkingNoteIndex < newNoteDatas.Count; checkingNoteIndex++)
                            {
                                SavedCurveTypeRgsister curveResister = newNoteDatas[checkingNoteIndex] as SavedCurveTypeRgsister;
                                SavedIgnoreXTickRegister ignoreXTick = newNoteDatas[checkingNoteIndex] as SavedIgnoreXTickRegister;

                                if (curveResister != null)//컷인, 컷아웃 홀드시작
                                {
                                    if (curveResister.whenSummonBeat == whenSummonBeat &&
                                        curveResister.startX == startX &&
                                        curveResister.endX == endX)
                                    {
                                        newNoteDatas.RemoveAt(checkingNoteIndex);
                                        curveType = curveResister.curveType;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                if (readingLine.backData[indexOfReadingNoteWithBackData * 2] == 5)
                                {
                                    tickType = SavedTickType.Invisiable;
                                    break;
                                }
                                else if (ignoreXTick != null)
                                {
                                    if (ignoreXTick.whenSummonBeat == whenSummonBeat &&
                                        ignoreXTick.startX == startX &&
                                        ignoreXTick.endX == endX)
                                    {
                                        tickType = SavedTickType.IgnoreX;
                                        newNoteDatas.RemoveAt(checkingNoteIndex);

                                        break;
                                    }
                                }
                                else
                                {
                                    tickType = SavedTickType.Basic;
                                }
                            }
                            holdCurveDatas.Add(new(whenSummonBeat, startX, endX, curveType, tickType, readingLine.frontData[2]));
                        }
                        break;
                    case 5: //플릭노트, 커브타입데이터
                        for (int checkingNoteIndex = 0; checkingNoteIndex < newNoteDatas.Count; checkingNoteIndex++)
                        {
                            SavedBasicNoteData basic = newNoteDatas[checkingNoteIndex] as SavedBasicNoteData;
                            if (basic == null)
                            {
                                continue;
                            }
                            if (basic.whenSummonBeat == whenSummonBeat && basic.startX == startX && basic.endX == endX)
                            {
                                int flickData = readingLine.backData[indexOfReadingNoteWithBackData * 2];
                                if (flickData == 1 || flickData == 3 || flickData == 4) //플릭노트일 경우
                                {
                                    SavedFlickNoteData flick = new();
                                    flick.isCriticalNote = basic.isCriticalNote;
                                    flick.whenSummonBeat = whenSummonBeat;
                                    flick.startX = startX;
                                    flick.endX = endX;
                                    flick.needTouchStart = true;
                                    switch (readingLine.backData[indexOfReadingNoteWithBackData * 2])
                                    {
                                        case 1:
                                            flick.rotation = 0;
                                            break;
                                        case 3:
                                            flick.rotation = -45f;
                                            break;
                                        case 4:
                                            flick.rotation = 45f;
                                            break;
                                    }

                                    newNoteDatas[checkingNoteIndex] = flick;
                                }
                                else //커브타입데이터일 경우
                                {
                                    SavedCurveTypeRgsister register = new();
                                    register.startX = startX;
                                    register.endX = endX;
                                    register.whenSummonBeat = whenSummonBeat;
                                    register.isCritical = basic.isCriticalNote;

                                    if (flickData == 6)
                                    {
                                        register.curveType = SavedHoldNoteCurveType.CurveIn;
                                    }
                                    else if (flickData == 2)
                                    {
                                        register.curveType = SavedHoldNoteCurveType.CurveOut;
                                    }

                                    newNoteDatas[checkingNoteIndex] = register;
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

        for (int readingHoldStartDataIndex = 0; readingHoldStartDataIndex < holdStartDatas.Count; readingHoldStartDataIndex++)
        {
            int id = holdStartDatas[0].id;
            for (int readingHoldEndDataIndex = 0; readingHoldEndDataIndex < holdEndDatas.Count; readingHoldEndDataIndex++)
            {
                if (holdEndDatas[readingHoldEndDataIndex].id == id)
                {
                    SavedHoldNoteData hold = new();
                    if (holdStartDatas[0].isCritical)
                    {
                        //크리티컬 홀드노트 추가시 수정
                        hold.isCriticalNote = true;
                        for (int k = 0; k < newNoteDatas.Count; k++)
                        {
                            SavedHoldEndNoteData endNote = newNoteDatas[k] as SavedHoldEndNoteData;
                            if (endNote != null && endNote.whenSummonBeat == holdEndDatas[readingHoldEndDataIndex].beat &&
                                endNote.startX == holdEndDatas[readingHoldEndDataIndex].startX &&
                                endNote.endX == holdEndDatas[readingHoldEndDataIndex].endX)
                            {
                                endNote.isCriticalNote = true;
                                break;
                            }
                        }
                    }
                    hold.whenSummonBeat = holdStartDatas[0].beat;

                    List<SavedHoldNoteCurve> newCurveList = new();
                    List<float> newTickBeatList = new();
                    newCurveList.Add(new SavedHoldNoteCurve()
                    {
                        spawnBeat = 0,
                        startX = holdStartDatas[0].startX,
                        endX = holdStartDatas[0].endX,
                        curveType = holdStartDatas[0].curveType
                    });

                    for (int readingCurveDataIndex = 0; readingCurveDataIndex < holdCurveDatas.Count; readingCurveDataIndex++)
                    {
                        if (holdCurveDatas[readingCurveDataIndex].beat <= holdStartDatas[0].beat ||
                            holdCurveDatas[readingCurveDataIndex].beat > holdEndDatas[readingHoldEndDataIndex].beat)
                        {
                            continue;
                        }
                        if (holdCurveDatas[readingCurveDataIndex].id == id &&
                            holdStartDatas[0].beat < holdCurveDatas[readingCurveDataIndex].beat &&
                            holdStartDatas[0].beat < holdEndDatas[readingHoldEndDataIndex].beat)
                        {
                            if (holdCurveDatas[readingCurveDataIndex].tickType != SavedTickType.IgnoreX)
                            {
                                newCurveList.Add(
                                    new SavedHoldNoteCurve(
                                        holdCurveDatas[readingCurveDataIndex].startX,
                                        holdCurveDatas[readingCurveDataIndex].endX,
                                        holdCurveDatas[readingCurveDataIndex].beat - holdStartDatas[0].beat,
                                        holdCurveDatas[readingCurveDataIndex].curveType
                                        ));
                            }

                            if (holdCurveDatas[readingCurveDataIndex].tickType != SavedTickType.Invisiable)
                            {
                                newTickBeatList.Add(holdCurveDatas[readingCurveDataIndex].beat - holdStartDatas[0].beat);
                            }

                            holdCurveDatas.RemoveAt(readingCurveDataIndex);
                            readingCurveDataIndex--;
                        }
                    }

                    newCurveList.Add(new SavedHoldNoteCurve()
                    {
                        spawnBeat = holdEndDatas[readingHoldEndDataIndex].beat - holdStartDatas[0].beat,
                        startX = holdEndDatas[readingHoldEndDataIndex].startX,
                        endX = holdEndDatas[readingHoldEndDataIndex].endX
                    });

                    hold.curveData = newCurveList.ToArray();
                    hold.tickBeatData = newTickBeatList.ToArray();

                    newNoteDatas.Add(hold);
                    holdStartDatas.RemoveAt(0);
                    holdEndDatas.RemoveAt(readingHoldEndDataIndex);

                    readingHoldStartDataIndex--;
                    readingHoldEndDataIndex--;
                    break;
                }
            }
        }

        newNoteDatas.Sort((a, b) => a.whenSummonBeat - b.whenSummonBeat);

        for (int readingNoteIndex = 0; readingNoteIndex < newNoteDatas.Count; readingNoteIndex++)
        {
            string message = newNoteDatas[readingNoteIndex].whenSummonBeat + "-" + newNoteDatas[readingNoteIndex].GetType() + ":";
            if (newNoteDatas[readingNoteIndex] is SavedMeterChangerNoteData)
            {
                message += ((SavedMeterChangerNoteData)newNoteDatas[readingNoteIndex]).beatLengthRate + "/" + ((SavedMeterChangerNoteData)newNoteDatas[readingNoteIndex]).beatPerBar;
            }
            if (newNoteDatas[readingNoteIndex] is SavedBPMChangeNoteData)
            {
                message += ((SavedBPMChangeNoteData)newNoteDatas[readingNoteIndex]).bpm;
            }
            Debug.Log(message);
        }

        newMapData.notes = newNoteDatas.ToArray();
        return newMapData;
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
