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
            Draw(curves);
        }
    }

    public void Draw(RuntimeHoldNoteCurve[] noteMesh)
    {
        curves = noteMesh;

        List<RuntimeHoldNoteCurve> curveList = noteMesh.ToList();

        if (NoteManager.NOTE_CHECK_YPOS > transform.localPosition.y)
        {
            float startX;
            float endX;
            curveList.RemoveAll((a) => a.yPos < NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y);
            (startX, endX) = FindCanHitArea();
            curveList.Add(new RuntimeHoldNoteCurve() { startX = startX, endX = endX, yPos = NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y });
        }
        curveList.Sort((a, b) => (int)Mathf.Sign(a.yPos - b.yPos));
        noteMesh = curveList.ToArray();

        if (noteMesh == null || noteMesh.Length <= 1)
        {
            return;
        }

        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }


        Mesh mesh = new();
        Vector3[] vertices = new Vector3[noteMesh.Length * 2];

        Vector3[] normals = new Vector3[noteMesh.Length * 2];

        int[] triangles = new int[(noteMesh.Length - 1) * 3 * 2 * 2];

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.back;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i % 2 == 0)
            {
                vertices[i] = new Vector3(noteMesh[i / 2].endX, noteMesh[i / 2].yPos, 0);
            }
            else
            {
                vertices[i] = new Vector3(noteMesh[i / 2].startX, noteMesh[i / 2].yPos, 0);
            }
        }

        for (int i = 0; i < (noteMesh.Length - 1); i++)
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
        Debug.DrawRay(transform.position + transform.rotation * new Vector3(startX, NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y), Vector3.up, Color.magenta);
        Debug.DrawRay(transform.position + transform.rotation * new Vector3(endX, NoteManager.NOTE_CHECK_YPOS - transform.localPosition.y), Vector3.up, Color.magenta);

        return (startX, endX);
    }

    public Vector2 BezierCalCulate(float lerpValue, params Vector2[] points)
    {
        while (points.Length > 1)
        {
            Vector2[] newPoints = new Vector2[points.Length - 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = Vector2.Lerp(points[i], points[i + 1], lerpValue);
            }
            points = newPoints;
        }
        return points[0];
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

            int afterIntBeat = 1;
            List<RuntimeHoldNoteCurve> curves = new List<RuntimeHoldNoteCurve>();
            for (int i = 0; i < hold.curveData.Length; i++)
            {
                while (hold.curveData[i].spawnBeat > afterIntBeat)
                {
                    SavedHoldNoteCurve beforeCurve = hold.curveData[i - 1];
                    SavedHoldNoteCurve afterCurve = hold.curveData[i];
                    float rate = (afterIntBeat - beforeCurve.spawnBeat) / (afterCurve.spawnBeat - beforeCurve.spawnBeat);
                    RuntimeHoldNoteCurve intCurve = new RuntimeHoldNoteCurve()
                    {
                        startX = Mathf.Lerp(beforeCurve.startX, afterCurve.startX, rate),
                        endX = Mathf.Lerp(beforeCurve.endX, afterCurve.endX, rate),
                        yPos = summoner.BeatToYpos(whenSummonBeat + afterIntBeat) - summoner.BeatToYpos(whenSummonBeat)
                    };
                    curves.Add(intCurve);
                    afterIntBeat++;
                }
                RuntimeHoldNoteCurve newCurve = new RuntimeHoldNoteCurve()
                {
                    startX = hold.curveData[i].startX,
                    endX = hold.curveData[i].endX,
                    yPos = summoner.BeatToYpos(whenSummonBeat + hold.curveData[i].spawnBeat) - summoner.BeatToYpos(whenSummonBeat)
                };
                curves.Add(newCurve);
            }

            n.Draw(curves.ToArray());

            int start = (int)hold.curveData[0].spawnBeat;
            int length = (int)(hold.curveData[hold.curveData.Length - 1].spawnBeat - start);
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

    public SavedHoldNoteCurve(float startX, float endX, float spawnBeat)
    {
        this.startX = startX;
        this.endX = endX;
        this.spawnBeat = spawnBeat;
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
    CutIn,
    CutOut
}