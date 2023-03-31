using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    List<GameObject> concelableDialouge;
    List<GameObject> trapDialouge;


    private void OnEnable()
    {
        concelableDialouge = new List<GameObject>();
        trapDialouge = new List<GameObject>();

        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("TrapDialouge"))
        {
            concelableDialouge.Add(fooObj);
        }

        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("ConcelableDialouge"))
        {
            trapDialouge.Add(fooObj);
        }

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
        for(int i = 0; i < trapDialouge.Count; i++)
        {
            Destroy(trapDialouge[i]);
        }
    }

    void DestroyConcelable()
    {
        for (int i = 0; i < concelableDialouge.Count; i++)
        {
            Destroy(concelableDialouge[i]);
        }

    }

}
