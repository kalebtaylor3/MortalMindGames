using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class DoorInteractable : InteractableBase
{

    public Animator doorAnimator;
    public AudioSource doorSource;
    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip slamClip;

    public override void OnInteract()
    {
        base.OnInteract();
        doorAnimator.ResetTrigger("Close");
        doorAnimator.SetTrigger("Open");
        isInteractable = false;
        StartCoroutine(WaitForClose());
    }

    IEnumerator WaitForClose()
    {
        yield return new WaitForSeconds(4);
        doorAnimator.ResetTrigger("Open");
        doorAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);
        isInteractable = true;
    }

    public void PlayOpen()
    {
        doorSource.PlayOneShot(openClip);
    }

    public void PlayClose()
    {
        doorSource.PlayOneShot(closeClip);
    }

    public void Slam()
    {
        doorSource.PlayOneShot(slamClip);
    }

    public void Stop()
    {
        doorSource.Stop();
    }
}
