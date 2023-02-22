using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonWalkSounds : MonoBehaviour
{

    public AudioSource stepSource;
    public AudioClip[] steps;

    public void PlayStep()
    {
        stepSource.PlayOneShot(steps[Random.Range(0, steps.Length)]);
    }
}
