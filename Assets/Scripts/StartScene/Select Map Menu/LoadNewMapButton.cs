using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadNewMapButton : MonoBehaviour
{
    public void LoadNewMap()
    {
        string selectedPath = FileUtil.OpenFileBrowser("불러올 맵 파일", "", "");
        if (selectedPath != null)
        {
            MapFileUtil.ReadRGM(File.ReadAllText(selectedPath));
        }
    }
}
