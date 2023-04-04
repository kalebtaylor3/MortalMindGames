using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBossMusic : MonoBehaviour
{
    public AudioSource bossSong;


    private void OnEnable()
    {
        bossSong.enabled = false;
    }
}
