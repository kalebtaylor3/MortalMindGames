using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSounds : MonoBehaviour
{
    public TrapInteractable trap;
    public AudioSource source;


    public void StartSound()
    {
        if(!source.isPlaying)   
            source.Play();
    }

    private void Update()
    {
        if (trap.isArmed)
            source.Stop();

        if (trap.isInteractable)
            source.Stop();
    }

    //public void Stop()
    //{
    //    source.enabled = false;
    //    source.Stop();
    //}
}
