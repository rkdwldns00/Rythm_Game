using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HoldNoteObject : Note
{
    public GameObject holdNoteTickPrefab;

    MeshFilter meshFilter;
    public RuntimeHoldNoteCurve[] curves;
    float[] hitCheckTiming = new float[0];
    public Dictionary<float, GameObject> tickObjects { get; set; } = new Dictionary<float, GameObject>();
    int checkIndex = 0;

    private void Start()
    {

    }

    private void Update()
    {
        if (hitCheckTiming.Length > checkIndex && hitCheckTiming[checkIndex] <= DistanceToHittingChecker)
        {
            float start;
            float end;
            (start, end) = FindCanHitArea();
            start -= 1;
            end += 1;

            int startLine = Mathf.FloorToInt(start);
            int endLine = Mathf.CeilToInt(end - 1);

            bool isTouch = false;
            for (int i = Mathf.Max(0, startLine); i <= Mathf.Min(HittingNoteChecker.TOUCH_LINE_COUNT - 1, endLine); i++)
            {
                if (HittingNoteChecker.instance.TouchDatas[i] == TouchMode.Hold)
                {
                    isTouch = true;
                    break;
                }
            }

            if (isTouch)
            {
                ComboManager.ProcessHitResult(HitResult.Perfect);
            }
            else
            {
                ComboManager.ProcessHitResult(HitResult.Miss);
            }

            if (tickObjects.Count > 0 && tickObjects.ContainsKey(hitCheckTiming[checkIndex]))
            {
                tickObjects[hitCheckTiming[checkIndex]].GetComponent<HoldNoteTickObjet>().Execute(isTouch);
                tickObjects.Remove(hitCheckTiming[checkIndex]);
            }
            checkIndex++;
        }
        if (NoteManager.NOTE_CHECK_YPOS > transform.localPosition.y)
        {
            Draw();
        }
    }

    public void Draw()
    {
        List<RuntimeHoldNoteCurve> curveList = curves.ToList();

        if (NoteManager.NOTE_CHECK_YPOS > transform.localPosition.y)
        {
            float startX;
            float endX;
            curveList.RemoveAll((a) => a.yPos < NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y);
            (startX, endX) = FindCanHitArea();
            curveList.Add(new RuntimeHoldNoteCurve() { startX = startX, endX = endX, yPos = NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y });
        }
        curveList.Sort((a, b) => (int)Mathf.Sign(a.yPos - b.yPos));

        List<RuntimeHoldNoteCurve> newCurveList = new List<RuntimeHoldNoteCurve>();
        for (int k = 0; k < curveList.Count; k++)
        {
            if (curveList[k].curveType != SavedHoldNoteCurveType.Basic)
            {
                float endBeat;
                float endStartX;
                float endEndX;

                endBeat = curveList[k + 1].yPos;
                endStartX = curveList[k + 1].startX;
                endEndX = curveList[k + 1].endX;

                for (float curveYpos = curveList[k].yPos + 0.5f; curveYpos < endBeat; curveYpos += 0.5f)
                {
                    Vector2 leftStartPos = new Vector2(curveList[k].startX, curveList[k].yPos);
                    Vector2 leftEndPos = new Vector2(endStartX, endBeat);

                    Vector2 rightStartPos = new Vector2(curveList[k].endX, curveList[k].yPos);
                    Vector2 rightEndPos = new Vector2(endEndX, endBeat);

                    float lerpValue = (curveYpos - curveList[k].yPos) / ((float)endBeat - curveList[k].yPos);

                    Vector2 leftViaPos = Vector2.zero;
                    Vector2 rightViaPos = Vector2.zero;

                    if (curveList[k].curveType == SavedHoldNoteCurveType.CurveIn)
                    {
                        leftViaPos = new Vector2(leftEndPos.x, leftStartPos.y);
                        rightViaPos = new Vector2(rightEndPos.x, rightStartPos.y);
                    }
                    else if (curveList[k].curveType == SavedHoldNoteCurveType.CurveOut)
                    {
                        leftViaPos = new Vector2(leftStartPos.x, leftEndPos.y);
                        rightViaPos = new Vector2(rightStartPos.x, rightEndPos.y);
                    }
                    newCurveList.Add(new RuntimeHoldNoteCurve()
                    {
                        startX = MathUtil.BezierCurveCalculate(lerpValue, leftStartPos, leftViaPos, leftEndPos).x,
                        endX = MathUtil.BezierCurveCalculate(lerpValue, rightStartPos, rightViaPos, rightEndPos).x,
                        yPos = curveYpos,
                    });
                }
            }
        }

        newCurveList.ForEach((a) => curveList.Add(a));
        curveList.Sort((a, b) => (int)Mathf.Sign(a.yPos - b.yPos));
        curves = curveList.ToArray();

        if (curves == null || curves.Length <= 1)
        {
            return;
        }

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        Mesh mesh = new();
        Vector3[] vertices = new Vector3[curves.Length * 2];

        Vector3[] normals = new Vector3[curves.Length * 2];

        int[] triangles = new int[(curves.Length - 1) * 3 * 2 * 2];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.back;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i % 2 == 0)
            {
                vertices[i] = new Vector3(curves[i / 2].endX, curves[i / 2].yPos, 0);
            }
            else
            {
                vertices[i] = new Vector3(curves[i / 2].startX, curves[i / 2].yPos, 0);
            }
        }

        for (int i = 0; i < (curves.Length - 1); i++)
        {
            triangles[i * 12] = i * 2;
            triangles[i * 12 + 1] = i * 2 + 1;
            triangles[i * 12 + 2] = i * 2 + 2;
            triangles[i * 12 + 3] = i * 2 + 3;
            triangles[i * 12 + 4] = i * 2 + 2;
            triangles[i * 12 + 5] = i * 2 + 1;

            triangles[i * 12 + 6] = i * 2;
            triangles[i * 12 + 7] = i * 2 + 2;
            triangles[i * 12 + 8] = i * 2 + 1;
            triangles[i * 12 + 9] = i * 2 + 3;
            triangles[i * 12 + 10] = i * 2 + 1;
            triangles[i * 12 + 11] = i * 2 + 2;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
    }

    public void SetHitCheckTiming(float[] times)
    {
        hitCheckTiming = times;
    }

    public (float startX, float endX) FindCanHitArea(float yPos)
    {
        RuntimeHoldNoteCurve beforeCurve = curves[0];
        RuntimeHoldNoteCurve afterCurve = curves[curves.Length - 1];
        for (int i = 0; i < curves.Length - 1; i++)
        {
            if (curves[i].yPos <= yPos)
            {
                beforeCurve = curves[i];
                afterCurve = curves[i + 1];
            }
        }

        float startX;
        float endX;
        startX = Mathf.Lerp(beforeCurve.startX, afterCurve.startX, (yPos - beforeCurve.yPos) / (afterCurve.yPos - beforeCurve.yPos));
        endX = Mathf.Lerp(beforeCurve.endX, afterCurve.endX, (yPos - beforeCurve.yPos) / (afterCurve.yPos - beforeCurve.yPos));

        return (startX, endX);
    }

    (float startX, float endX) FindCanHitArea()
    {
        return FindCanHitArea(NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y);
    }
}

public class SavedHoldNoteData : SavedNoteData, IGamePlaySummonable
{
    public override string serializedDataTitleName => "HN";

    public virtual GameObject GamePlayNotePrefab
    {
        get
        {
            if (isCriticalNote)
            {
                return NoteManager.instance.criticalHoldNotePrefab;
            }
            else
            {
                return NoteManager.instance.holdNotePrefab; ;
            }
        }
    }
    public SavedHoldNoteCurve[] curveData;
    public float[] tickBeatData;
    public bool isCriticalNote = false;

    public override MapEditorNote SummonMapEditorNote()
    {
        throw new NotImplementedException();
    }

    public override Note SummonGamePlayNote(NoteSummoner summoner)
    {
        HoldNoteObject noteObject = null;

        if (curveData.Length > 1)
        {
            float startY = summoner.BeatToYpos(whenSummonBeat);
            GameObject newHoldNote = summoner.InstantiateNote(GamePlayNotePrefab, 0, startY);
            HoldNoteObject n = newHoldNote.GetComponent<HoldNoteObject>();
            noteObject = n;

            List<RuntimeHoldNoteCurve> curves = new List<RuntimeHoldNoteCurve>();
            for (int i = 0; i < curveData.Length; i++)
            {
                RuntimeHoldNoteCurve newCurve = new RuntimeHoldNoteCurve(
                    curveData[i].startX,
                    curveData[i].endX,
                    summoner.BeatToYpos((float)whenSummonBeat + curveData[i].spawnBeat) - summoner.BeatToYpos(whenSummonBeat),
                    curveData[i].curveType);

                curves.Add(newCurve);
            }

            n.curves = curves.ToArray();
            n.Draw();

            List<float> hitCheckTiming = new List<float>();
            int length = (int)(curveData[curveData.Length - 1].spawnBeat);
            if (length > 2)
            {
                for (int i = 1; i < length - 1; i++)
                {
                    hitCheckTiming.Add(summoner.BeatToSec(whenSummonBeat + i) - summoner.BeatToSec(whenSummonBeat));
                }

            }
            if (tickBeatData != null && tickBeatData.Length > 0)
            {
                for (int i = 0; i < tickBeatData.Length; i++)
                {
                    GameObject newTick = UnityEngine.Object.Instantiate(n.holdNoteTickPrefab, n.transform);
                    float yPos = summoner.BeatToYpos(whenSummonBeat + tickBeatData[i]) - summoner.BeatToYpos(whenSummonBeat);
                    (float startX, float endX) = n.FindCanHitArea(yPos);
                    newTick.transform.localPosition = new Vector3((startX + endX) / 2, yPos, -0.02f);
                    float sec = summoner.BeatToSec(whenSummonBeat + tickBeatData[i]) - summoner.BeatToSec(whenSummonBeat);
                    n.tickObjects.Add(sec, newTick);
                    hitCheckTiming.Add(sec);
                }
            }

            hitCheckTiming.Sort((a, b) => (int)Mathf.Sign(a - b));
            n.SetHitCheckTiming(hitCheckTiming.ToArray());
        }

        return noteObject;
    }
}

[Serializable]
public struct SavedHoldNoteCurve
{
    public float startX;
    public float endX;

    public float spawnBeat;

    public SavedHoldNoteCurveType curveType;

    public SavedHoldNoteCurve(float startX, float endX, float spawnBeat, SavedHoldNoteCurveType curveType)
    {
        this.startX = startX;
        this.endX = endX;
        this.spawnBeat = spawnBeat;
        this.curveType = curveType;
    }
}

[Serializable]
public struct RuntimeHoldNoteCurve
{
    public float startX;
    public float endX;

    public float yPos;

    public SavedHoldNoteCurveType curveType;

    public RuntimeHoldNoteCurve(float startX, float endX, float yPos, SavedHoldNoteCurveType curveType)
    {
        this.startX = startX;
        this.endX = endX;
        this.yPos = yPos;
        this.curveType = curveType;
    }
}

/*public class SavedCurveTypeRgsister : SavedNoteData
{
    public SavedHoldNoteCurveType curveType = SavedHoldNoteCurveType.Basic;
    public float startX;
    public float endX;
    public bool isCritical = false;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        Debug.LogWarning(GetType().Name + "가 생성되었지만 사용되지 않았습니다.");
        return null;
    }
}*/

/*public class SavedIgnoreXTickRegister : SavedNoteData
{
    public float startX;
    public float endX;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        Debug.LogWarning(GetType().Name + "가 생성되었지만 사용되지 않았습니다.");
        return null;
    }
}*/

public enum SavedHoldNoteCurveType
{
    Basic,
    CurveIn,
    CurveOut
}

public enum SavedTickType
{
    Basic,
    Invisiable,
    IgnoreX
}