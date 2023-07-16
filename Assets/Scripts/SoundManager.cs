using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static float Volume
    {
        get
        {
            return PlayerPrefs.GetFloat("playVolume");
        }
        set
        {
            PlayerPrefs.SetFloat("playVolume", value);
            if(instance != null)
            {
                instance.source.volume = value;
            }
        }
    }

    static SoundManager instance;
    AudioSource source;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        source.volume = Volume;
    }

    public static void PlaySound(AudioClip clip)
    {
        instance.source.PlayOneShot(clip);
    }
}
