using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadNewMapButton : MonoBehaviour
{
    public void LoadNewMap()
    {
        FileBrowser.ShowLoadDialog(
            (paths) => { 
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        MapFileUtil.ReadRGM(File.ReadAllText(path));
                    }
                }
            },
            () => { },
            FileBrowser.PickMode.Files, false,null,null,"맵 파일 선택","Load .rgm");
        
    }
}
