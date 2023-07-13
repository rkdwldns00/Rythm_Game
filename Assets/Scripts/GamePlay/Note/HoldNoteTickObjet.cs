using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNoteTickObjet : MonoBehaviour
{
    public AudioClip keySound;

    public void Execute(bool isClicked)
    {
        if(isClicked && keySound != null) {
            SoundManager.PlaySound(keySound);
        }
        Destroy(gameObject);
    }
}
