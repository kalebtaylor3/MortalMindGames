using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonBossSounds : MonoBehaviour
{

    public AudioSource vorgonAudio;
    public AudioClip laughClip;
    public AudioClip slamClip;
    public AudioClip raiseHandClip;
    public AudioClip shootClip;
    public AudioClip chosenDeath;
    public AudioClip damageClip;

    public void Laugh()
    {
        vorgonAudio.PlayOneShot(laughClip);
    }

    public void Slam()
    {
        vorgonAudio.PlayOneShot(slamClip);
    }

    public void RaiseHand()
    {
        vorgonAudio.PlayOneShot(raiseHandClip);
    }

    public void Shoot()
    {
        vorgonAudio.PlayOneShot(shootClip);
    }

    public void ChoseDeath()
    {
        vorgonAudio.PlayOneShot(chosenDeath);
    }

    public void TakeDamage()
    {
        if (!vorgonAudio.isPlaying)
            vorgonAudio.PlayOneShot(damageClip);
    }

}
