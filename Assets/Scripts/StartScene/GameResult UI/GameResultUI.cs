using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text comboText;
    [SerializeField] Text perfectText;
    [SerializeField] Text greatText;
    [SerializeField] Text goodText;
    [SerializeField] Text badText;
    [SerializeField] Text missText;

    private void OnEnable()
    {
        perfectText.text = GameManager.Instance.hitResultCounts[(int)HitResult.Perfect].ToString();
        greatText.text = GameManager.Instance.hitResultCounts[(int)HitResult.Great].ToString();
        goodText.text = GameManager.Instance.hitResultCounts[(int)HitResult.Good].ToString();
        badText.text = GameManager.Instance.hitResultCounts[(int)HitResult.Bad].ToString();
        missText.text = GameManager.Instance.hitResultCounts[(int)HitResult.Miss].ToString();

        scoreText.text = GameManager.Instance.score.ToString();
        comboText.text = GameManager.Instance.maxCombo.ToString();

        GameManager.Instance.showResultUI = false;
    }
}
