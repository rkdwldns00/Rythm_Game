using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshMapListButton : MonoBehaviour
{
    [SerializeField] Transform mapInfoScrollView;
    [SerializeField] GameObject mapInfoPrefab;

    public void RefreshMapList()
    {
        for (int i = 0; i < mapInfoScrollView.childCount; i++)
        {
            Destroy(mapInfoScrollView.GetChild(0).gameObject);
        }

        foreach (SavedMapData map in MapFileUtil.LoadAllMapResource())
        {
            MapInfoShower shower = Instantiate(mapInfoPrefab, mapInfoScrollView).GetComponent<MapInfoShower>();

            shower.SetMapData(map);
        }
    }
}
