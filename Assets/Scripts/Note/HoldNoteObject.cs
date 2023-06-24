using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HoldNoteObject : MonoBehaviour
{
    public Vector3[] noteMesh;
    MeshFilter meshFilter;

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void Draw(HoldNoteCurve[] noteMesh)
    {
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
                vertices[i] = new Vector3(noteMesh[i / 2].endX, noteMesh[i / 2].spawnBeat, 0);
            }
            else
            {
                vertices[i] = new Vector3(noteMesh[i / 2].endX, noteMesh[i / 2].spawnBeat, 0);
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

    /*public override void SetData(SavedNoteData data)
    {
        SavedHoldNoteData holdData = data as SavedHoldNoteData;
        if (holdData != null)
        {
            Draw(holdData.curveData);
        }
    }*/
}

public class SavedHoldNoteData : SavedNoteData
{
    public override GameObject NotePrefab => NoteManager.instance.holdNotePrefab;
    public HoldNoteCurve[] curveData;
}

public struct HoldNoteCurve
{
    public float startX;
    public float endX;

    public float spawnBeat;
}