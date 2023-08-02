using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class MapFileUtil
{
    const string MAP_DATA_PATH = "Assets/Resources/MapDatas/";
    const string EXPORTED_MAP_FILE_TYPE = ".rgm";

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

    public static SavedMapData[] LoadAllMapResource()
    {
        TextAsset[] maps = Resources.LoadAll<TextAsset>("MapDatas/");
        List<SavedMapData> result = new List<SavedMapData>();

        foreach (TextAsset map in maps)
        {
            if (map != null)
            {
                SavedMapData loadedMap = LoadMapResource(map.name);
                if (loadedMap != null)
                {
                    result.Add(loadedMap);
                }
            }
        }

        return result.ToArray();
    }

    public static SavedMapData LoadMapResource(string mapName)
    {
        string mapInfoData = Resources.Load<TextAsset>("MapDatas/" + mapName).text;

        mapInfoData.Replace("\r", "\n");
        string[] jsonDatas = mapInfoData.Split("\n");

        SavedMapData data = null;
        if (jsonDatas.Length > 0)
        {
            data = JsonUtility.FromJson<SavedMapData>(jsonDatas[0]);
        }

        if (data == null)
        {
            Debug.LogWarning("유효하지않은 맵 파일을 불러오려고 시도했습니다, 맵 이름 : " + mapName);
            return null;
        }


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

    public static void SaveMapResource(SavedMapData data)
    {
        if (data == null)
        {
            Debug.LogWarning("저장하려는 맵이 null 입니다.");
            return;
        }

        string file = "";
        file += JsonUtility.ToJson(data);
        foreach (var note in data.notes)
        {
            file += "\n" + NoteToTXT(note);
        }

        File.WriteAllText(MAP_DATA_PATH + data.title + ".txt", file);
        SpriteUtil.ExportTextureToPNG(data.thumnail.texture, MAP_DATA_PATH + data.title + ".png");
        AudioClipUtil.ExportAudioClipToWAV(data.bgm, MAP_DATA_PATH + data.title + ".wav");
    }

    static string NoteToTXT(SavedNoteData noteData)
    {
        return noteTypeKey[noteData.GetType()] + JsonUtility.ToJson(noteData);
    }

    public static void DeleteMapResource(string mapTitle)
    {
        string path = "Assets/Resources/MapDatas/" + mapTitle;
        if (File.Exists(path + ".txt"))
        {
            File.Delete(path + ".txt");
        }
        else
        {
            Debug.LogWarning("삭제하려는 맵이 존재하지않습니다.");
            return;
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

    public static void ExportMap(SavedMapData mapData)
    {
        const char parsingText = '|';

        string fileText = "";
        fileText += JsonUtility.ToJson(mapData) + parsingText;
        foreach (var note in mapData.notes)
        {
            fileText += "\n" + NoteToTXT(note);
        }
        fileText += parsingText;
        fileText += SpriteUtil.TextureToJSON(mapData.thumnail.texture) + parsingText;
        fileText += AudioClipUtil.AudioClipToJSON(mapData.bgm);

        string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads/" + mapData.title + EXPORTED_MAP_FILE_TYPE);

        File.WriteAllText(filePath, fileText);
    }
}
