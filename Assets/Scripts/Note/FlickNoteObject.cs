using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickNoteObject : Note, IHitableNoteObject
{
    float startX;
    float endX;

    float rotation;

    bool isStartTouch = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CheckHit(int line)
    {
        if (!isStartTouch && HittingNoteChecker.instance.TouchDatas[line] == TouchMode.Start && Mathf.Abs(DistanceToHittingChecker) < 0.13f)
        {
            isStartTouch = true;
        }

        Vector2 flickPower = HittingNoteChecker.instance.GetFlickPower(line);
        float flickPowerRotation = Mathf.Atan2(flickPower.y, flickPower.x) * Mathf.Rad2Deg;
        return
            isStartTouch
            && Mathf.Abs(DistanceToHittingChecker) < 0.13f
            && line + 1 >= startX && line - 1 <= endX - 1
            && HittingNoteChecker.instance.TouchDatas[line] == TouchMode.Hold
            && flickPower.magnitude > 5 && Mathf.Abs(rotation - flickPowerRotation) <= 30f;
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

    public void SetData(SavedFlickNoteData data)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (data.endX - data.startX) / rect.localScale.x);
        rect?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, NoteManager.NOTE_Y_SIZE / rect.localScale.y);
        startX = data.startX;
        endX = data.endX;
        rotation = data.rotation;
    }
}

public class SavedFlickNoteData : SavedNoteData, ISummonable
{
    public override GameObject NotePrefab => NoteManager.instance.flickNotePrefab;

    public float startX;
    public float endX;

    public float rotation;

    public Note Summon(NoteSummoner summoner,SavedNoteData data)
    {
        FlickNoteObject n = null;

        SavedFlickNoteData flick = data as SavedFlickNoteData;
        if (flick != null)
        {
            GameObject g = summoner.InstantiateNote(data.NotePrefab, (flick.startX + flick.endX) / 2f, summoner.BeatToYpos(flick.whenSummonBeat));
            n = g.GetComponent<FlickNoteObject>();
            n?.SetData(flick);
        }

        return n;
    }
}
