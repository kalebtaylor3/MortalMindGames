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

    private void OnEnable()
    {
        textMessage = TooltipMessage;
        armedTrigger.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if (inEvent)
            {
                //OnFailure();
                FailQTE?.Invoke();
                OnFailure();
                Debug.Log("You walked away so you fail");
                canPass = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        canPass = true;
    }

    public override void OnInteract()
    {
        if (canInteract)
        {
            base.OnInteract();
            qte.TriggerEvent();
            canInteract = false;
            isInteractable = false;
            displayText = "";
            animator.SetBool("Arming", true);
            QuickTimeEventSystem.OnSuccess += OnSuccsess;
            QuickTimeEventSystem.OnFailure += OnFailure;
            inEvent = true;
        }
        
    }

    void HandleEvent()
    {
        if (qte_Completed < NumberOfSucsess)
            qte.TriggerEvent();
        else
            OnComplete();
    }

    void OnSuccsess()
    {
        if (canPass)
        {
            qte_Completed = qte_Completed + 1;
            HandleEvent();
            inEvent = false;
        }
    }

    void OnFailure()
    {
        qte_Completed = 0;
        displayText = textMessage;
        animator.SetBool("Fail", true);
        StartCoroutine(WaitForFail());
        QuickTimeEventSystem.OnSuccess -= OnSuccsess;
        QuickTimeEventSystem.OnFailure -= OnFailure;
    }

    IEnumerator WaitForFail()
    {
        yield return new WaitForSeconds(1);
        canInteract = true;
        isInteractable = true;
        ui.SetDisplayText(textMessage);
        inEvent = false;
    }

    void OnComplete()
    {
        Debug.Log("Trap is armed");
        isInteractable = false;
        displayText = "";
        animator.SetBool("Armed", true);
        StartCoroutine(WaitForArming());
        QuickTimeEventSystem.OnSuccess -= OnSuccsess;
        QuickTimeEventSystem.OnFailure -= OnFailure;
        inEvent = false;
    }

    IEnumerator WaitForArming()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        vfx.SetActive(true);
        armedTrigger.SetActive(true);   
    }
}
