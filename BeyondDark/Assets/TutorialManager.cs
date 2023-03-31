using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    GameObject[] concelableDialouge;
    GameObject[] trapDialouge;


    private void OnEnable()
    {

        concelableDialouge = GameObject.FindGameObjectsWithTag("ConcelableDialogue");
        trapDialouge = GameObject.FindGameObjectsWithTag("TrapDialogue");


        DialogueTrigger.OnConcelable += DestroyConcelable;
        DialogueTrigger.OnTrap += DestroyTraps;

    }

    private void OnDisable()
    {
        DialogueTrigger.OnConcelable -= DestroyConcelable;
        DialogueTrigger.OnTrap -= DestroyTraps;
    }

    void DestroyTraps()
    {
        for(int i = 0; i < trapDialouge.Length; i++)
        {
            Destroy(trapDialouge[i]);
        }
    }

    void DestroyConcelable()
    {
        for (int i = 0; i < concelableDialouge.Length; i++)
        {
            Destroy(concelableDialouge[i]);
        }

    }

}
