using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SelectExportPath : MonoBehaviour
{
    [SerializeField] InputField inputField;

    void Start()
    {
        inputField.text = MapFileUtil.Export_Path;
    }

    public void SetExportPath(string path)
    {
        MapFileUtil.Export_Path = path;
    }

    public void OpenFileBrowser()
    {
        FileBrowser.ShowLoadDialog(
            (paths) =>
            {
                if (paths != null)
                {
                    SetExportPath(paths[0] + "/");
                    inputField.text = MapFileUtil.Export_Path;
                }
            },
            () => { },
            FileBrowser.PickMode.Folders, false, null, null, "맵 내보내기 경로 선택");

    }
}
