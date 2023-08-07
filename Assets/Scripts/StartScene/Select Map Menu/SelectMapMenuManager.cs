using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SelectMapMenuManager : MonoBehaviour
{
    [SerializeField] Transform mapInfoScrollView;
    [SerializeField] GameObject mapInfoPrefab;

    private void OnEnable()
    {
        RefreshMapList();
    }

    public void LoadNewMap()
    {
        FileBrowser.ShowLoadDialog(
            (paths) =>
            {
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        MapFileUtil.ReadRGM(File.ReadAllText(path));
                    }
                }
                RefreshMapList();
            },
            () => { },
            FileBrowser.PickMode.Files,false, null, null, "맵 파일 선택", "Load .rgm");

    }

    public void RefreshMapList()
    {
        foreach (MapInfoShower shower in mapInfoScrollView.GetComponentsInChildren<MapInfoShower>())
        {
            Destroy(shower.gameObject);
        }

        foreach (SavedMapData map in MapFileUtil.LoadAllMapResource())
        {
            MapInfoShower shower = Instantiate(mapInfoPrefab, mapInfoScrollView).GetComponent<MapInfoShower>();

            shower.SetMapData(map);
        }
    }
}
