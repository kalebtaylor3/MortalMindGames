using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // LEFT = 0
    // RIGHT = 1
    // FRONT = 2
    // BACK = 3

    [Space, Header("Audio Sources")]
    [SerializeField] AudioSource LeftAudioSource;
    [SerializeField] AudioSource RightAudioSource;
    [SerializeField] AudioSource FrontAudioSource;
    [SerializeField] AudioSource BackAudioSource;

    [Space, Header("Audio Clips")]
    [SerializeField] AudioClip[] m_Clips;

    [SerializeField] float delay;

    private void Start()
    {
        StartCoroutine(RandomEerieSound(delay));
    }

    IEnumerator RandomEerieSound(float t)
    {
        while(true)
        {
            yield return new WaitForSeconds(t);

            int rand1Source = Random.Range(0, 4);

            int rand2Clip = Random.Range(0, m_Clips.Length - 1);

            switch (rand1Source)
            {
                case 0:
                    LeftAudioSource.PlayOneShot(m_Clips[rand2Clip]);
                    break;
                case 1:
                    RightAudioSource.PlayOneShot(m_Clips[rand2Clip]);
                    break;
                case 2:
                    FrontAudioSource.PlayOneShot(m_Clips[rand2Clip]);
                    break;
                case 3:
                    BackAudioSource.PlayOneShot(m_Clips[rand2Clip]);
                    break;

                default:
                    break;
            }
        }
               
    }
}
