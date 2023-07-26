using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyUtil
{
    public static Vector2 BezierCalCulate(float lerpValue, params Vector2[] points)
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

    public static SavedMapData LoadMapFile(string mapName)
    {
        string mapInfoData = Resources.Load<TextAsset>("MapDatas/" + mapName).text;

        mapInfoData.Replace("\r", "\n");
        string[] jsonDatas = mapInfoData.Split("\n");

        SavedMapData data = JsonUtility.FromJson<SavedMapData>(jsonDatas[0]);
        data.thumnail = Resources.Load<Sprite>("MapDatas/" + mapName);
        data.bgm = Resources.Load<AudioClip>("MapDatas/" + mapName);

        List<SavedNoteData> notes = new List<SavedNoteData>();
        for (int i = 1; i < jsonDatas.Length; i++)
        {
            notes.Add(JsonUtility.FromJson<SavedBasicNoteData>(jsonDatas[i]));
        }
        data.notes = notes.ToArray();
        return data;
    }

    public static void SaveMapFile(SavedMapData data)
    {
        string file = "";
        file += JsonUtility.ToJson(data);
        foreach (var note in data.notes)
        {
            file += "\n" + JsonUtility.ToJson(note);
        }

        File.WriteAllText("Assets/Resources/MapDatas/" + data.title + ".txt", file);

    }
}
