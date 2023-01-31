using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapInteractable : InteractableBase
{

    public QuickTimeEventSystem qte;
    public InteractionUIPanel ui;
    public Animator animator;
    bool canInteract = true;

    public GameObject armedTrigger;

    public int NumberOfSucsess = 2;
    int qte_Completed = 0;

    string textMessage;

    public GameObject vfx;
    bool inEvent = false;

    public static event Action FailQTE;

    bool canPass = false;
    bool hasFailed = false;

    public AudioClip completeClip;
    public AudioClip startClip;
    public AudioSource completeSource;

    public VorgonController vorgon;

    private void OnEnable()
    {
        textMessage = TooltipMessage;
        armedTrigger.SetActive(false);
        vorgon = FindObjectOfType<VorgonController>();
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if (inEvent && !hasFailed)
            {
                //OnFailure();
                FailQTE?.Invoke();
                OnFailure();
                Debug.Log("You walked away so you fail");
                canPass = false;
                inEvent = false;
            }

            canPass = false;
        }
    }

    //private void Update()
    //{
    //    if (this.canPass)
    //        this.ui.SetDisplayText(textMessage);
    //    else
    //        this.ui.SetDisplayText("");
    //}

    private void OnTriggerEnter(Collider other)
    {
        canPass = true;
    }

    private void OnTriggerStay(Collider other)
    {
        canPass = true;
    }

    //private void Update()
    //{
    //    Debug.Log(canPass);
    //}

    public override void OnInteract()
    {
        if (canPass)
        {
            if (canInteract)
            {
                base.OnInteract();
                hasFailed = false;
                qte.TriggerEvent();
                canInteract = false;
                isInteractable = false;
                displayText = "";
                animator.SetBool("Arming", true);
                QuickTimeEventSystem.OnSuccess += OnSuccsess;
                QuickTimeEventSystem.OnFailure += OnFailure;
                inEvent = true;
                completeSource.PlayOneShot(startClip);
            }
        }
        return;
        
    }

    IEnumerator StartAnotherEvent()
    {
        qte.StopAllCoroutines();
        yield return new WaitForSeconds(1f);
        completeSource.PlayOneShot(startClip);
        qte.TriggerEvent();
    }

    void HandleEvent()
    {
        if (qte_Completed < NumberOfSucsess)
            StartCoroutine(StartAnotherEvent());
        else
            OnComplete();
    }

    void OnSuccsess()
    {
        if (canPass)
        {
            StopAllCoroutines();
            qte_Completed = qte_Completed + 1;
            HandleEvent();
            inEvent = false;            
        }
    }

    void OnFailure()
    {
        hasFailed = true;
        StopAllCoroutines();
        qte_Completed = 0;
        displayText = textMessage;
        animator.SetBool("Fail", true);
        StartCoroutine(WaitForFail());
    }

    IEnumerator WaitForFail()
    {
        yield return new WaitForSeconds(1);
        canInteract = true;
        isInteractable = true;
        ui.SetDisplayText(textMessage);
        inEvent = false;
        QuickTimeEventSystem.OnSuccess -= OnSuccsess;
        QuickTimeEventSystem.OnFailure -= OnFailure;
        vorgon.SetLastDetectedLocation(transform.position, Vector3.zero, VorgonController.EVENT_TYPE.ANIM);
    }

    void OnComplete()
    {
        Debug.Log("Trap is armed");
        isInteractable = false;
        displayText = "";
        animator.SetBool("Armed", true);
        StartCoroutine(WaitForArming());
        inEvent = false;
    }

    IEnumerator WaitForArming()
    {
        animator.SetBool("Armed", true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        vfx.SetActive(true);
        armedTrigger.SetActive(true);
        QuickTimeEventSystem.OnSuccess -= OnSuccsess;
        QuickTimeEventSystem.OnFailure -= OnFailure;
        completeSource.PlayOneShot(completeClip);
        gameObject.GetComponentInChildren<StunTriggerCollider>().gameObject.SetActive(true);        
    }
}
