using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    const float MAX_SCORE = 1000000f;

    public Text comboText;
    public Text hitResultText;

    static ComboManager instance;
    public int comboCount { get; private set; }
    public float score { get; private set; }
    float totalScore;
    float size = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        comboText.gameObject.SetActive(false);
        hitResultText.gameObject.SetActive(false);
    }

    private void Update()
    {
        size = Mathf.Lerp(size, 1, Time.deltaTime);
        hitResultText.fontSize = (int)(80f * size);
        comboText.fontSize = (int)(80f * size);

        if (size <= 1.01f)
        {
            hitResultText.gameObject.SetActive(false);
        }
    }

    public static void SetMapData(SavedMapData mapData)
    {
        float totalScore = 0f;
        foreach (SavedNoteData note in mapData.notes)
        {
            totalScore += note.totalScore;
        }
        instance.totalScore = totalScore;
    }

    public static void ProcessHitResult(HitResult hitResult, float originScore)
    {
        instance.ProcessCombo(hitResult >= HitResult.Great);
        instance.ShowHitResult(hitResult);

        float noteScore = MAX_SCORE / instance.totalScore * originScore;
        float rate = (float)hitResult / 4f;
        instance.score += noteScore * rate;
    }

    private void ShowHitResult(HitResult hitResult)
    {
        switch (hitResult)
        {
            case HitResult.Perfect:
                SetHitResultText("Perfect", Color.cyan);
                break;
            case HitResult.Great:
                SetHitResultText("Great", Color.magenta);
                break;
            case HitResult.Good:
                SetHitResultText("Good", Color.yellow);
                break;
            case HitResult.Bad:
                SetHitResultText("Bad", Color.blue);
                break;
            case HitResult.Miss:
                SetHitResultText("Miss", Color.gray);
                break;
        }
    }

    private void SetHitResultText(string text, Color color)
    {
        size = 1.2f;
        hitResultText.text = text;
        hitResultText.color = color;
        hitResultText.gameObject.SetActive(true);
    }

    private void ProcessCombo(bool isAddCombo)
    {
        if (isAddCombo)
        {
            comboCount++;
            comboText.text = comboCount.ToString();
            comboText.gameObject.SetActive(true);
        }
        else
        {
            comboCount = 0;
            comboText.gameObject.SetActive(false);
        }
    }
}

public enum HitResult
{
    Perfect = 4,
    Great = 3,
    Good = 2,
    Bad = 1,
    Miss = 0
}
