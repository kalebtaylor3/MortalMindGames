using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAudio : MonoBehaviour
{
    public Animator animator;

    public AudioSource stepSource;
    public AudioSource shootingSource;
    public AudioClip[] steps;
    public AudioClip[] shot;
    public AudioClip[] acid;
    public AudioClip death;
    public AudioClip mAttack;
    public AudioClip spawn;

    public GameObject deathPrefab;


    public void PlayStep()
    {
        if (!shootingSource.isPlaying)
        {
            stepSource.PlayOneShot(steps[Random.Range(0, steps.Length)]);
        }
    }

    public void PlayShooting()
    {
        if (!shootingSource.isPlaying)
        {
            shootingSource.PlayOneShot(shot[Random.Range(0, steps.Length)]);
        }        
    }

    public void PlayMAttack()
    {
        if (!shootingSource.isPlaying)
        {
            shootingSource.PlayOneShot(mAttack);
        }
    }

    public void PlayAcid()
    {
        if (!shootingSource.isPlaying)
        {
            shootingSource.PlayOneShot(shot[Random.Range(0, steps.Length)]);
        }
    }
    public void PlayDeath()
    {
        if (animator.GetBool("Dead"))
        {
            GameObject d = Instantiate(deathPrefab, this.transform.position, Quaternion.identity);
            Destroy(d, 2f);

            //stepSource.PlayOneShot(death);
        }        
    }

    public void PlaySpawn()
    {
       // if (animator.GetBool("Spawning"))
        //{
            //stepSource.volume = 0.2f;
            stepSource.PlayOneShot(spawn);
       //}
    }
}
