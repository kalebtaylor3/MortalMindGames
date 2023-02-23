using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JumpScare : MonoBehaviour
{
    [SerializeField] Animation scareAnim = null;
    [SerializeField] AudioSource scareAudioSource;
    [SerializeField] float startDelay = 0;
    private VorgonController vorgon;

    public static event Action OnJumpScare;

    private void Start()
    {
        vorgon = FindObjectOfType<VorgonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(StartJumpScare());
            if(StealthDetection.Instance.inRange)
                StealthDetection.Instance.SetDetection(2);
            GetComponent<BoxCollider>().enabled = false;
        }             
    }

    IEnumerator StartJumpScare()
    {
        yield return new WaitForSeconds(startDelay);

        if(scareAnim != null)
        {
            scareAnim.Play();
        }

        scareAudioSource.Play();
        OnJumpScare?.Invoke();

        if (StealthDetection.Instance.inRange)
        {
            vorgon.SetLastDetectedLocation(transform.position, null, VorgonController.EVENT_TYPE.ANIM);
        }
    }

}
