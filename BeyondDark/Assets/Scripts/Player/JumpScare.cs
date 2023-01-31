using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScare : MonoBehaviour
{
    [SerializeField] Animation scareAnim = null;
    [SerializeField] AudioSource scareAudioSource;
    [SerializeField] float startDelay = 0;
    private VorgonController vorgon;

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

        
        scareAudioSource.Play();
        vorgon.SetLastDetectedLocation(transform.position, Vector3.zero, VorgonController.EVENT_TYPE.ANIM);
    }

}
