using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource mainSource;
    //public AudioSource introSource;
    public AudioSource chaseSource;

    public VorgonController vorgon;

    public float fadeTimer = 3f;

    //public List<AudioClip> clips;

    private void OnEnable()
    {
        ChaseState.onStartChase += StartChaseMusic;
        ChaseState.onEndChase += EndChaseMusic;
        WorldData.OnDeath += EndChaseInstant;
    }

    private void OnDisable()
    {
        ChaseState.onStartChase -= StartChaseMusic;
        ChaseState.onEndChase -= EndChaseMusic;
        WorldData.OnDeath -= EndChaseInstant;
    }

    private void Update()
    {
        if(chaseSource.isPlaying && !vorgon.inChase)
        {
            EndChaseMusic();
        }
    }

    public void StartChaseMusic()
    {
        if(WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
        {
            if (vorgon.inChase && !chaseSource.isPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(FadeSource(mainSource, chaseSource));
            }
        }       
    }

    public void EndChaseMusic()
    {
        if (WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
        {
            if (!vorgon.inChase)
            {
                StartCoroutine(endChaseMusic());
            }
        }             
    }

    public void EndChaseInstant()
    {
        StopAllCoroutines();
        StartCoroutine(FadeSource(chaseSource, mainSource));
    }

    private IEnumerator endChaseMusic()
    {
        yield return new WaitForSeconds(3.0f);

        if(!vorgon.inChase && chaseSource.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(FadeSource(chaseSource, mainSource));
        }
        
    }

    private IEnumerator FadeSource(AudioSource playing, AudioSource notPlaying)
    {

        float clockT = 0f;

        notPlaying.Play();

        while (clockT < fadeTimer)
        {
            notPlaying.volume = Mathf.Lerp(0, 0.5f, clockT / fadeTimer);
            playing.volume = Mathf.Lerp(0.5f, 0, clockT / fadeTimer);

            clockT += Time.deltaTime;
            clockT = Mathf.Clamp(clockT, 0, fadeTimer);
            yield return null;
        }
        playing.Stop();
    }
}
