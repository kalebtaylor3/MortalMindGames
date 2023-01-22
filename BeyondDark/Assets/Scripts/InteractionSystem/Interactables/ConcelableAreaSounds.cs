using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcelableAreaSounds : MonoBehaviour
{
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioSource sounds;

    public void Open()
    {
        sounds.clip = openSound;
        sounds.Play();
    }

    public void Close()
    {
        sounds.clip = closeSound;
        sounds.Play();
    }

    public void Stop()
    {
        sounds.Stop();
    }
}
