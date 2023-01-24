using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapInteractable : InteractableBase
{

    public QuickTimeEventSystem qte;
    public InteractionUIPanel ui;
    public Animator animator;
    bool canInteract = true;

    public int NumberOfSucsess = 2;
    int qte_Completed = 0;

    string textMessage;

    public GameObject vfx;

    private void OnEnable()
    {
        textMessage = TooltipMessage;
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
        qte_Completed = qte_Completed + 1;
        HandleEvent();
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
    }

    IEnumerator WaitForArming()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        vfx.SetActive(true);
    }
}
