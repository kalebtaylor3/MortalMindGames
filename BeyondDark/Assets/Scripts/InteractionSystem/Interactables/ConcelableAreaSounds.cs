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
        if (!sounds.isPlaying)
        {
            sounds.enabled = true;
            sounds.clip = openSound;
            sounds.PlayOneShot(openSound);
        }
    }

    public void Close()
    {
        sounds.clip = closeSound;
        sounds.Play();
    }

    public void Stop()
    {
        sounds.enabled = false;
        sounds.Stop();
    }
}
