using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HoldNoteObject : Note
{
    MeshFilter meshFilter;
    public RuntimeHoldNoteCurve[] curves;
    float[] hitCheckTiming = new float[0];

    int checkIndex = 0;

    private void Start()
    {

    }

    private void Update()
    {
        if (hitCheckTiming.Length > checkIndex && hitCheckTiming[checkIndex] <= DistanceToHittingChecker)
        {
            checkIndex++;
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
                HitResultShower.ShowHitResult(HitResult.Perfect);
            }
            else
            {
                HitResultShower.ShowHitResult(HitResult.Miss);
            }
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
        curves = curveList.ToArray();

        if (curves == null || curves.Length <= 1)
        {
            return;
        }

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        curveList.ForEach((a) => Debug.Log(a.yPos + ":" + a.curveType));

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

    (float startX, float endX) FindCanHitArea()
    {
        RuntimeHoldNoteCurve beforeCurve = curves[0];
        RuntimeHoldNoteCurve afterCurve = curves[curves.Length - 1];
        for (int i = 0; i < curves.Length - 1; i++)
        {
            if (curves[i].yPos <= NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y)
            {
                beforeCurve = curves[i];
                afterCurve = curves[i + 1];
            }
        }

        float startX;
        float endX;
        startX = Mathf.Lerp(beforeCurve.startX, afterCurve.startX, ((NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y) - beforeCurve.yPos) / (afterCurve.yPos - beforeCurve.yPos));
        endX = Mathf.Lerp(beforeCurve.endX, afterCurve.endX, ((NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y) - beforeCurve.yPos) / (afterCurve.yPos - beforeCurve.yPos));

        return (startX, endX);
    }
}

public class SavedHoldNoteData : SavedNoteData, ISummonable
{
    public virtual GameObject NotePrefab => NoteManager.instance.holdNotePrefab;
    public SavedHoldNoteCurve[] curveData;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        HoldNoteObject noteObject = null;

        SavedHoldNoteData hold = data as SavedHoldNoteData;
        if (hold != null && hold.curveData.Length > 1)
        {
            float startY = summoner.BeatToYpos(hold.whenSummonBeat);
            GameObject g = summoner.InstantiateNote(((ISummonable)data).NotePrefab, 0, startY);
            HoldNoteObject n = g.GetComponent<HoldNoteObject>();
            noteObject = n;

            List<RuntimeHoldNoteCurve> curves = new List<RuntimeHoldNoteCurve>();
            for (int i = 0; i < hold.curveData.Length; i++)
            {
                RuntimeHoldNoteCurve newCurve = new RuntimeHoldNoteCurve()
                {
                    startX = hold.curveData[i].startX,
                    endX = hold.curveData[i].endX,
                    yPos = summoner.BeatToYpos((float)whenSummonBeat + hold.curveData[i].spawnBeat) - summoner.BeatToYpos(whenSummonBeat)
                };

                curves.Add(newCurve);
            }
            
            n.curves = curves.ToArray();
            n.Draw();

            int length = (int)(hold.curveData[hold.curveData.Length - 1].spawnBeat);
            if (length > 2)
            {
                float[] hitCheckTiming = new float[length - 2];

                for (int i = 0; i < hitCheckTiming.Length; i++)
                {
                    hitCheckTiming[i] = summoner.BeatToSec(hold.whenSummonBeat + i + 1) - summoner.BeatToSec(whenSummonBeat);
                }

                n.SetHitCheckTiming(hitCheckTiming);
            }
        }

        return noteObject;
    }
}

public class SavedCriticalHoldNoteData : SavedHoldNoteData
{
    public override GameObject NotePrefab => NoteManager.instance.criticalHoldNotePrefab;
}

public struct SavedHoldNoteCurve
{
    public float startX;
    public float endX;

    public float spawnBeat;

    public SavedHoldNoteCurveType curveType;

    public SavedHoldNoteCurve(float startX, float endX, float spawnBeat,SavedHoldNoteCurveType curveType)
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

    public RuntimeHoldNoteCurve(float startX, float endX, float yPos)
    {
        this.startX = startX;
        this.endX = endX;
        this.yPos = yPos;
        curveType = SavedHoldNoteCurveType.Basic;
    }

    public static bool operator ==(RuntimeHoldNoteCurve a,RuntimeHoldNoteCurve b)
    {
        return a.startX == b.startX &&
            a.endX == b.endX &&
            a.yPos == b.yPos &&
            a.curveType == b.curveType;
    }

    public static bool operator !=(RuntimeHoldNoteCurve a,RuntimeHoldNoteCurve b)
    {
        return !(a == b);
    }
}

public class SavedCurveTypeRgsister : SavedNoteData
{
    public SavedHoldNoteCurveType curveType = SavedHoldNoteCurveType.Basic;
    public float startX;
    public float endX;

    public override Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        Debug.LogWarning("SavedCurveTypeResister가 사용되지 않았습니다.");
        return null;
    }
}

public enum SavedHoldNoteCurveType
{
    Basic,
    CurveIn,
    CurveOut
}