using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    GameObject[] barrelConcelableDialouge;
    GameObject[] oilTrapDialouge;

    GameObject[] chestlConcelableDialouge;
    GameObject[] springTrapDialouge;


    private void OnEnable()
    {

        barrelConcelableDialouge = GameObject.FindGameObjectsWithTag("BarrelConcelableDialogue");
        chestlConcelableDialouge = GameObject.FindGameObjectsWithTag("ChestConcelableDialogue");
        oilTrapDialouge = GameObject.FindGameObjectsWithTag("OilTrapDialogue");
        springTrapDialouge = GameObject.FindGameObjectsWithTag("SpringTrapDialogue");


        DialogueTrigger.OnBarrelConcelable += DestroyBarrelConcelable;
        DialogueTrigger.OnChestConcelable += DestroyChestConcelable;
        DialogueTrigger.OnSpringTrap += DestroySpringTraps;
        DialogueTrigger.OnOIlTrap += DestroyOilTraps;

    }

    private void OnDisable()
    {
        DialogueTrigger.OnBarrelConcelable -= DestroyBarrelConcelable;
        DialogueTrigger.OnOIlTrap -= DestroyOilTraps;
        DialogueTrigger.OnChestConcelable -= DestroyChestConcelable;
        DialogueTrigger.OnSpringTrap -= DestroySpringTraps;
    }

    void DestroyOilTraps()
    {
        for(int i = 0; i < oilTrapDialouge.Length; i++)
        {
            Destroy(oilTrapDialouge[i]);
        }
    }

    void DestroySpringTraps()
    {
        for (int i = 0; i < springTrapDialouge.Length; i++)
        {
            Destroy(springTrapDialouge[i]);
        }
    }

    void DestroyBarrelConcelable()
    {
        for (int i = 0; i < barrelConcelableDialouge.Length; i++)
        {
            Destroy(barrelConcelableDialouge[i]);
        }

    }

    void DestroyChestConcelable()
    {
        for (int i = 0; i < chestlConcelableDialouge.Length; i++)
        {
            Destroy(chestlConcelableDialouge[i]);
        }

    }

}
