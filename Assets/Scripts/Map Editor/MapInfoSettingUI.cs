using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapInfoSettingUI : MonoBehaviour
{
    [SerializeField] private GameObject uiLayer;
    [Header("���� ���� ��ư")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button bgmLoadButton;
    [Header("�� ���� ����")]
    [SerializeField] private InputField mapNameInput;
    [SerializeField] private InputField mapArtistNameInput;
    [SerializeField] private InputField mapDesignerNameInput;
    [Header("")]
    [SerializeField] private Slider bgmOffsetSlider;
    [SerializeField] private Text bgmOffsetText;
    [SerializeField] private InputField startBpmInput;
    [Header("�޴� ��ȯ ��ư")]
    [SerializeField] private Button openInfoSettingMenuButton;
    [SerializeField] private Button toMenuButton;
    [SerializeField] private Button backToEditButton;

    private void Awake()
    {
        saveButton.onClick.AddListener(SaveMap);
        bgmLoadButton.onClick.AddListener(LoadBgm);
        mapNameInput.onValueChanged.AddListener(SetMapName);
        mapArtistNameInput.onValueChanged.AddListener(SetArtistName);
        mapDesignerNameInput.onValueChanged.AddListener(SetDesignerName);
        bgmOffsetSlider.onValueChanged.AddListener(SetBgmOffset);
        startBpmInput.onValueChanged.AddListener((t) => SetBPM(int.Parse(t)));
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
        startBpmInput.text = MapEditManager.EditingMap.startBpm.ToString();
    }

    private void SaveMap()
    {
        MapEditManager.Instance.SaveEditingMap();
    }

    private void LoadBgm()
    {
        MapEditManager.Instance.SelectMapBgm();
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

    private void SetBPM(int bpm)
    {
        MapEditManager.EditingMap.startBpm = bpm;
        startBpmInput.text = MapEditManager.EditingMap.startBpm.ToString();
    }

    private void GotoMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
