using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonBossSounds : MonoBehaviour
{

    public AudioSource vorgonAudio;
    public AudioClip laughClip;
    public AudioClip slamClip;
    public AudioClip raiseHandClip;
    public AudioClip chosenDeath;
    public AudioClip damageClip;
    public Transform particleSpawn;
    public GameObject particles;

    public void Laugh()
    {
        vorgonAudio.PlayOneShot(laughClip);
    }

    public void Slam()
    {
        vorgonAudio.PlayOneShot(slamClip);
        GameObject obj = Instantiate(particles, particleSpawn.position, Quaternion.identity);
        Destroy(obj, 4);
    }

    public void RaiseHand()
    {
        vorgonAudio.PlayOneShot(raiseHandClip);
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

    public void Die()
    {
        vorgonAudio.PlayOneShot(damageClip);
    }

}
