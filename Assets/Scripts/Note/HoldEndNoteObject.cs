using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldEndNoteObject : Note, HitableNote
{
    float startX;
    float endX;

    private void Start()
    {
        print("º“»Øµ ");
    }

    public bool CheckHit(int line)
    {
        return Mathf.Abs(DistanceToHittingChecker) < 0.13f
            && line + 1 >= startX && line - 1 <= endX - 1
            && HittingNoteChecker.instance.TouchDatas[line] == TouchMode.End;
    }

    public void Hit()
    {
        float t = Mathf.Abs(DistanceToHittingChecker);
        if (t <= 0.04f)
        {
            HitResultShower.ShowHitResult(HitResult.Perfect);
        }
        else if (t <= 0.08f)
        {
            HitResultShower.ShowHitResult(HitResult.Great);
        }
        else if (t <= 0.1f)
        {
            HitResultShower.ShowHitResult(HitResult.Good);
        }
        else
        {
            HitResultShower.ShowHitResult(HitResult.Bad);
        }
        Destroy(gameObject);
    }

    public void SetData(SavedHoldEndNoteData data)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (data.endX - data.startX) / rect.localScale.x);
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, NoteManager.NOTE_Y_SIZE / rect.localScale.y);
        startX = data.startX;
        endX = data.endX;
    }
}

public class SavedHoldEndNoteData : SavedNoteData
{
    public override GameObject NotePrefab => NoteManager.instance.holdEndNotePrefab;

    public float startX;
    public float endX;
}