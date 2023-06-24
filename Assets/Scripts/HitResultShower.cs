using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitResultShower : MonoBehaviour
{
    Text t;
    static HitResultShower instance;
    float size;

    static string text
    {
        get { return instance.t.text; }
        set { instance.t.text = value; }
    }

    static Color color
    {
        get { return instance.t.color; }
        set { instance.t.color = value; }
    }

    void Awake()
    {
        t = GetComponent<Text>();
        instance = this;
    }

    private void Update()
    {
        size = Mathf.Lerp(size, 1, Time.deltaTime);
        t.fontSize = (int)(80f * size);
    }

    public static void ShowHitResult(HitResult hitResult)
    {
        instance.size = 1.2f;
        switch (hitResult)
        {
            case HitResult.Perfect:
                text = "PERFECT";
                color = Color.cyan;
                break;
            case HitResult.Great:
                text = "GREAT";
                color = Color.magenta;
                break;
            case HitResult.Good:
                text = "GOOD";
                color = Color.yellow;
                break;
            case HitResult.Bad:
                text = "BAD";
                color = Color.grey;
                break;
            case HitResult.Miss:
                text = "MISS";
                color = Color.grey;
                break;
        }
    }
}

public enum HitResult
{
    Perfect,
    Great,
    Good,
    Bad,
    Miss
}
