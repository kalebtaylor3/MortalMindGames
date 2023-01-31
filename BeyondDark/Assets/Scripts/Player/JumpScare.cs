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
            StealthDetection.Instance.SetDetection(1);
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

        OnJumpScare?.Invoke();
        scareAudioSource.Play();
        vorgon.SetLastDetectedLocation(transform.position, Vector3.zero, VorgonController.EVENT_TYPE.ANIM);
    }

}
