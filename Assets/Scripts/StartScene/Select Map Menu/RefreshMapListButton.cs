using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshMapListButton : MonoBehaviour
{
    [SerializeField] Transform mapInfoScrollView;
    [SerializeField] GameObject mapInfoPrefab;

    public void RefreshMapList()
    {
        foreach(MapInfoShower shower in mapInfoScrollView.GetComponentsInChildren<MapInfoShower>())
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
