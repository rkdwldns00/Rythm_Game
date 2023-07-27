using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyUtil
{
    static Dictionary<Type, string> noteTypeKey = new Dictionary<Type, string>() {
        {typeof(SavedBasicNoteData), "BN"},
        {typeof(SavedFlickNoteData), "FN"},
        {typeof(SavedHoldNoteData), "HN"},
        {typeof (SavedHoldEndNoteData), "EN"},
        {typeof(SavedBPMChangeNoteData), "BD"},
        {typeof(SavedMeterChangerNoteData), "MD"},
        {typeof(SavedSpeedChangerNoteData), "SD"}
    };

    public static Type NoteKeyToType(string key)
    {
        foreach (var keyValue in noteTypeKey)
        {
            if (keyValue.Value == key)
            {
                return keyValue.Key;
            }
        }
        return null;
    }

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
            Type t = NoteKeyToType(jsonDatas[i][..2]);
            string noteJson = jsonDatas[i][2..];
            if (t == typeof(SavedBasicNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedBasicNoteData>(noteJson));
            }
            else if (t == typeof(SavedFlickNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedFlickNoteData>(noteJson));
            }
            else if (t == typeof(SavedHoldNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedHoldNoteData>(noteJson));
            }
            else if (t == typeof(SavedHoldEndNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedHoldEndNoteData>(noteJson));
            }
            else if (t == typeof(SavedBPMChangeNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedBPMChangeNoteData>(noteJson));
            }
            else if (t == typeof(SavedMeterChangerNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedMeterChangerNoteData>(noteJson));
            }
            else if (t == typeof(SavedSpeedChangerNoteData))
            {
                notes.Add(JsonUtility.FromJson<SavedSpeedChangerNoteData>(noteJson));
            }
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
            file += "\n" + noteTypeKey[note.GetType()] + JsonUtility.ToJson(note);
        }

        File.WriteAllText("Assets/Resources/MapDatas/" + data.title + ".txt", file);
    }

    public static void DeleteMapFile(string mapTitle)
    {
        string path = "Assets/Resources/MapDatas/" + mapTitle;
        if (File.Exists(path + ".txt"))
        {
            File.Delete(path + ".txt");
        }

        if (File.Exists(path + ".png"))
        {
            File.Delete(path + ".png");
        }
        else if (File.Exists(path + ".jpg"))
        {
            File.Delete(path + ".jpg");
        }

        if (File.Exists(path + ".mp3"))
        {
            File.Delete(path + ".mp3");
        }
    }
}
