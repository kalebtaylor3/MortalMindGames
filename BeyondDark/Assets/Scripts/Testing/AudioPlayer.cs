using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Space, Header("Audio Sources")]
    [SerializeField] AudioSource LeftAudioSource;
    [SerializeField] AudioSource RightAudioSource;
    [SerializeField] AudioSource FrontAudioSource;
    [SerializeField] AudioSource BackAudioSource;

    [Space, Header("Audio Clips")]
    [SerializeField] AudioClip[] m_Clips;
}
