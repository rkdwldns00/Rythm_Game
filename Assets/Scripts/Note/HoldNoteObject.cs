using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HoldNoteObject : Note
{
    //public Vector3[] noteMesh;
    MeshFilter meshFilter;
    public RuntimeHoldNoteCurve[] curves;
    //SavedHoldNoteCurve[] hitCheckData;
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
    }

    public void Draw(RuntimeHoldNoteCurve[] noteMesh)
    {
        curves = noteMesh;

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

        int[] triangles = new int[(noteMesh.Length - 1) * 3 * 2];

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

        for (int i = 0; i < noteMesh.Length - 1; i++)
        {
            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;
            triangles[i * 6 + 3] = i * 2 + 3;
            triangles[i * 6 + 4] = i * 2 + 2;
            triangles[i * 6 + 5] = i * 2 + 1;
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
        return (startX - 1, endX + 1);
    }
}

public class SavedHoldNoteData : SavedNoteData, ISummonable
{
    public override GameObject NotePrefab => NoteManager.instance.holdNotePrefab;
    public SavedHoldNoteCurve[] curveData;

    public Note Summon(NoteSummoner summoner, SavedNoteData data)
    {
        HoldNoteObject noteObject = null;

        SavedHoldNoteData hold = data as SavedHoldNoteData;
        if (hold != null && hold.curveData.Length > 1)
        {
            float startY = summoner.BeatToYpos(hold.whenSummonBeat);
            GameObject g = summoner.InstantiateNote(data.NotePrefab, 0, startY);
            HoldNoteObject n = g.GetComponent<HoldNoteObject>();
            noteObject = n;

            RuntimeHoldNoteCurve[] curves = new RuntimeHoldNoteCurve[hold.curveData.Length];
            for (int i = 0; i < curves.Length; i++)
            {
                RuntimeHoldNoteCurve newCurve = new RuntimeHoldNoteCurve()
                {
                    startX = hold.curveData[i].startX,
                    endX = hold.curveData[i].endX,
                    yPos = summoner.BeatToYpos(hold.curveData[i].spawnBeat)
                };
                curves[i] = newCurve;
            }

            n.Draw(curves);

            int start = (int)hold.curveData[0].spawnBeat;
            int length = (int)(hold.curveData[hold.curveData.Length - 1].spawnBeat - start);
            if (length > 2)
            {
                float[] hitCheckTiming = new float[length - 2];

                for (int i = 0; i < hitCheckTiming.Length; i++)
                {
                    hitCheckTiming[i] = summoner.beatToSec * (i + 1);
                }

                n.SetHitCheckTiming(hitCheckTiming);
            }
        }

        return noteObject;
    }
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

    public RuntimeHoldNoteCurve(float startX, float endX, float yPos)
    {
        this.startX = startX;
        this.endX = endX;
        this.yPos = yPos;
    }
}