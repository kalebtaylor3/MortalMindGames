using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBossMusic : MonoBehaviour
{

    public AudioSource bossMusic;
    public GameObject sword;

    private void OnEnable()
    {
        Destroy(bossMusic);
        Destroy(sword);
    }
}
