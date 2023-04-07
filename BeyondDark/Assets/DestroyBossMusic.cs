using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBossMusic : MonoBehaviour
{

    public AudioSource bossMusic;

    private void OnEnable()
    {
        bossMusic.enabled = false;
    }
}
