using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class MapFileUtil
{
    public static readonly string MAP_DATA_PATH = Application.persistentDataPath + "/MapDatas/";
    static readonly string MAP_LIST_PATH = Application.persistentDataPath + "/MapList.txt";
    const string EXPORTED_MAP_FILE_TYPE = ".rgm";
    const char RGM_PARSING_TEXT = '|';
    const char MAP_LIST_PARSING_TEXT = '\n';

    static Dictionary<Type, string> noteTypeKey = new Dictionary<Type, string>() {
        {typeof(SavedBasicNoteData), "BN"},
        {typeof(SavedFlickNoteData), "FN"},
        {typeof(SavedHoldNoteData), "HN"},
        {typeof(SavedHoldEndNoteData), "EN"},
        {typeof(SavedBPMChangeNoteData), "BD"},
        {typeof(SavedMeterChangerNoteData), "MD"},
        {typeof(SavedSpeedChangerNoteData), "SD"},
        {typeof(SavedTraceNoteData), "TN"}
    };

    public static string Export_Path
    {
        get { return PlayerPrefs.GetString("Export_Path"); }
        set { PlayerPrefs.SetString("Export_Path", value); }
    }

    static Type NoteKeyToType(string key)
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

    public static void MakeMapListFile()
    {
        if (!File.Exists(MAP_LIST_PATH))
        {
            File.WriteAllText(MAP_LIST_PATH, "");
        }
    }

    public static void MakeMapDatasFolder()
    {
        if (!Directory.Exists(MAP_DATA_PATH))
        {
            Directory.CreateDirectory(MAP_DATA_PATH);
        }
    }

    public static SavedMapData[] LoadAllMapResource()
    {
        string[] mapTitles = File.ReadAllText(MAP_LIST_PATH).Split(MAP_LIST_PARSING_TEXT);

        List<SavedMapData> result = new List<SavedMapData>();

        foreach (string title in mapTitles)
        {
            if (!string.IsNullOrEmpty(title))
            {
                SavedMapData loadedMap = LoadMapResource(title);
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
        if (!File.Exists(MAP_DATA_PATH + mapName + ".txt")) return null;
        string mapInfoData = File.ReadAllText(MAP_DATA_PATH + mapName + ".txt");

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


        data.thumnail = SpriteUtil.LoadSprite(MAP_DATA_PATH + mapName + ".png");
        AudioClipUtil.LoadAudioClip(data);

        List<SavedNoteData> notes = new List<SavedNoteData>();
        for (int i = 1; i < jsonDatas.Length; i++)
        {
            notes.Add(NoteJSONToSavedNoteData(jsonDatas[i]));
        }
        data.notes = notes.ToArray();
        return data;
    }

    static SavedNoteData NoteJSONToSavedNoteData(string text)
    {
        Type t = NoteKeyToType(text[..2]);
        string noteJson = text[2..];
        if (t == typeof(SavedBasicNoteData))
        {
            return JsonUtility.FromJson<SavedBasicNoteData>(noteJson);
        }
        else if (t == typeof(SavedFlickNoteData))
        {
            return JsonUtility.FromJson<SavedFlickNoteData>(noteJson);
        }
        else if (t == typeof(SavedHoldNoteData))
        {
            return JsonUtility.FromJson<SavedHoldNoteData>(noteJson);
        }
        else if (t == typeof(SavedHoldEndNoteData))
        {
            return JsonUtility.FromJson<SavedHoldEndNoteData>(noteJson);
        }
        else if (t == typeof(SavedBPMChangeNoteData))
        {
            return JsonUtility.FromJson<SavedBPMChangeNoteData>(noteJson);
        }
        else if (t == typeof(SavedMeterChangerNoteData))
        {
            return JsonUtility.FromJson<SavedMeterChangerNoteData>(noteJson);
        }
        else if (t == typeof(SavedSpeedChangerNoteData))
        {
            return JsonUtility.FromJson<SavedSpeedChangerNoteData>(noteJson);
        }
        else if (t == typeof(SavedTraceNoteData))
        {
            return JsonUtility.FromJson<SavedTraceNoteData>(noteJson);
        }
        return null;
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
        if (data.thumnail != null)
        {
            SpriteUtil.ExportTextureToPNG(data.thumnail.texture, MAP_DATA_PATH + data.title + ".png");
        }
        if (data.bgm != null)
        {
            AudioClipUtil.ExportAudioClipToWAV(data.bgm, MAP_DATA_PATH + data.title + ".wav");
        }

        string[] mapList = File.ReadAllText(MAP_LIST_PATH).Split(MAP_LIST_PARSING_TEXT);
        if (!mapList.Contains(data.title))
        {
            File.AppendAllText(MAP_LIST_PATH, "\n" + data.title);
        }
    }

    static string NoteToTXT(SavedNoteData noteData)
    {
        return noteData.serializedDataTitleName + JsonUtility.ToJson(noteData);
    }

    public static void DeleteMapResource(string mapTitle)
    {
        string path = MAP_DATA_PATH + mapTitle;
        if (File.Exists(path + ".txt"))
        {
            File.Delete(path + ".txt");
        }
        else
        {
            Debug.LogWarning("삭제하려는 맵이 존재하지않습니다.");
            return;
        }

        foreach (var textureType in SpriteUtil.TEXTURE_FILE_TYPE)
        {
            if (File.Exists(path + textureType))
            {
                File.Delete(path + textureType);
            }
        }

        foreach (var audioType in AudioClipUtil.AUDIO_CLIP_FILE_TYPE)
        {
            if (File.Exists(path + audioType))
            {
                File.Delete(path + audioType);
            }
        }

        string[] mapList = File.ReadAllText(MAP_LIST_PATH).Split(MAP_LIST_PARSING_TEXT);
        string newMapList = "";
        foreach (string listTitle in mapList)
        {
            if (listTitle != mapTitle)
            {
                newMapList += listTitle + "\n";
            }
        }
        File.WriteAllText(MAP_LIST_PATH, newMapList);
    }

    public static void ExportMap(SavedMapData mapData)
    {
        string fileText = "";
        fileText += JsonUtility.ToJson(mapData);
        foreach (var note in mapData.notes)
        {
            fileText += "\n" + NoteToTXT(note);
        }
        fileText += RGM_PARSING_TEXT;
        fileText += SpriteUtil.TextureToJSON(mapData.thumnail.texture) + RGM_PARSING_TEXT;
        fileText += AudioClipUtil.AudioClipToJSON(mapData.bgm);

        string filePath = Export_Path + mapData.title + EXPORTED_MAP_FILE_TYPE;
        Debug.Log(filePath);
        if (Directory.Exists(Export_Path))
        {
            File.WriteAllText(filePath, fileText);
        }
    }

    public static void ReadRGM(string fileText)
    {
        string[] parsingDatas = fileText.Split(RGM_PARSING_TEXT);
        string[] mapInfo = parsingDatas[0].Split("\n");
        SavedMapData mapData = JsonUtility.FromJson<SavedMapData>(mapInfo[0]);
        List<SavedNoteData> notes = new List<SavedNoteData>();
        for (int i = 1; i < mapInfo.Length; i++)
        {
            notes.Add(NoteJSONToSavedNoteData(mapInfo[i]));
        }
        mapData.notes = notes.ToArray();

        SaveMapResource(mapData);
        SpriteUtil.ExportJSONToPNG(parsingDatas[1], MAP_DATA_PATH + mapData.title + ".png");
        AudioClipUtil.ExportJSONToAudioClip(parsingDatas[2], MAP_DATA_PATH + mapData.title + ".wav");
    }
}
