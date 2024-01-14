using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapInfoSettingUI : MonoBehaviour
{
    [Header("���� ���� ��ư")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [Header("�� ���� ����")]
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
    [Header("�޴� ��ȯ ��ư")]
    [SerializeField] private Button toMenuButton;
    [SerializeField] private Button backToEditButton;

    private MapEditManager mapEditManager => MapEditManager.Instance;
}
