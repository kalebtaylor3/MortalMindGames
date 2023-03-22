using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatSystem : MonoBehaviour
{
    public AudioClip[] heartbeatClips;
    public Transform playerTransform;
    public float maxDistance = 10f;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public float beatInterval = 1f;

    private AudioSource audioSource;
    private AudioClip currentClip;
    private float nextBeatTime;

    public VorgonController vorgon;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentClip = heartbeatClips[0];
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= maxDistance)
        {
            if (!audioSource.isPlaying || Time.time >= nextBeatTime)
            {
                if (audioSource.clip == currentClip)
                {
                    currentClip = (currentClip == heartbeatClips[0]) ? heartbeatClips[1] : heartbeatClips[0];
                }
                audioSource.clip = currentClip;
                audioSource.loop = false;
                audioSource.Play();
                nextBeatTime = Time.time + beatInterval;
            }

            if (!vorgon.inChase)
            {
                float t = Mathf.InverseLerp(maxDistance, 0f, distance);
                float pitch = Mathf.Lerp(maxPitch, minPitch, t);
                audioSource.pitch = pitch;
            }
            else
            {
                audioSource.pitch = minPitch;
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}
