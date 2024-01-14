using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoSettingUI : MonoBehaviour
{
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
    [Header("")]
    [SerializeField] private InputField meterInput1;
    [SerializeField] private InputField meterInput2;
    [Header("메뉴 전환 버튼")]
    [SerializeField] private Button toMenuButton;
    [SerializeField] private Button backToEditButton;

    private MapEditManager mapEditManager => MapEditManager.Instance;
}
