using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapInfoSettingUI : MonoBehaviour
{
    [SerializeField] private GameObject uiLayer;
    [Header("파일 설정 버튼")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [Header("맵 정보 설정")]
    [SerializeField] private InputField mapNameInput;
    [SerializeField] private InputField mapArtistNameInput;
    [SerializeField] private InputField mapDesignerNameInput;
    [Header("")]
    [SerializeField] private Slider bgmOffsetSlider;
    [SerializeField] private Text bgmOffsetText;
    [SerializeField] private Slider startBpmSlider;
    [SerializeField] private Text startBpmText;
    [Header("메뉴 전환 버튼")]
    [SerializeField] private Button openInfoSettingMenuButton;
    [SerializeField] private Button toMenuButton;
    [SerializeField] private Button backToEditButton;

    private void Awake()
    {
        saveButton.onClick.AddListener(SaveMap);
        mapNameInput.onValueChanged.AddListener(SetMapName);
        mapArtistNameInput.onValueChanged.AddListener(SetArtistName);
        mapDesignerNameInput.onValueChanged.AddListener(SetDesignerName);
        bgmOffsetSlider.onValueChanged.AddListener(SetBgmOffset);
        startBpmSlider.onValueChanged.AddListener(SetBgmOffset);
        openInfoSettingMenuButton.onClick.AddListener(() => SetUIEnable(true));
        toMenuButton.onClick.AddListener(GotoMenu);
        backToEditButton.onClick.AddListener(() => SetUIEnable(false));
    }

    public void SetUIEnable(bool enable)
    {
        uiLayer.SetActive(enable);
        if (enable)
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        mapNameInput.text = MapEditManager.EditingMap.title;
        mapArtistNameInput.text = MapEditManager.EditingMap.artistName;
        mapDesignerNameInput.text = MapEditManager.EditingMap.designerName;
        bgmOffsetSlider.value = MapEditManager.EditingMap.startOffset;
        bgmOffsetText.text = MapEditManager.EditingMap.startOffset.ToString();
        startBpmSlider.value = MapEditManager.EditingMap.startBpm;
        startBpmText.text = MapEditManager.EditingMap.startBpm.ToString();
    }

    private void SaveMap()
    {
        MapEditManager.Instance.SaveEditingMap();
    }

    private void SetMapName(string name)
    {
        MapEditManager.EditingMap.title = name;
    }

    private void SetArtistName(string name)
    {
        MapEditManager.EditingMap.artistName = name;
    }

    private void SetDesignerName(string name)
    {
        MapEditManager.EditingMap.designerName = name;
    }

    private void SetBgmOffset(float offset)
    {
        MapEditManager.EditingMap.startOffset = offset;
        bgmOffsetText.text = offset.ToString();
    }

    private void SetBPM(float bpm)
    {
        MapEditManager.EditingMap.startBpm = bpm;
        startBpmText.text = bpm.ToString();
    }

    private void GotoMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
