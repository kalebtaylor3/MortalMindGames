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
    bool happenOnce = false;

    private void OnEnable()
    {
        TriggerSlam.OnEnter += SlamEvent;
    }

    private void OnDisable()
    {
        TriggerSlam.OnEnter -= SlamEvent;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        doorAnimator.ResetTrigger("Close");
        doorAnimator.SetTrigger("Open");
        isInteractable = false;
    }

    IEnumerator WaitForClose()
    {
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
        if (!happenOnce)
        {
            doorSource.PlayOneShot(slamClip);
            StartCoroutine(Wait());
            happenOnce = true;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        happenOnce = false;
    }

    public void Stop()
    {
        doorSource.Stop();
    }

    void SlamEvent()
    {
        StartCoroutine(WaitForClose());
        doorAnimator.ResetTrigger("Open");
        doorAnimator.SetTrigger("Close");
    }
}
