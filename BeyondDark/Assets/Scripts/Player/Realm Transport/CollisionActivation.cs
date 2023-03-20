using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionActivation : MonoBehaviour
{
    enum TRIAL_OUTCOME { NONE = 0, FAIL = 1, SUCCESS = 2, SAFEZONE = 3 }

    [SerializeField] TRIAL_OUTCOME EndType = TRIAL_OUTCOME.NONE;

    public Transform ogTpPos;
    public static event Action endingPath;    

    // Temporary collision to trigger realm tp from vorgons realm to the mortal realm
    private void OnTriggerEnter(Collider other)
    {
        switch (EndType)
        {
            case TRIAL_OUTCOME.FAIL:
                if (other.gameObject.tag == "VorgonRealmPlayer" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
                {
                    // Checkpoint
                    Debug.Log("Vorgon Realm Path Fail");

                    GameObject lastRelic = WorldData.Instance.lastPickUpGO;
                    WorldData.Instance.TriggerCheckpoint();
                    WorldData.Instance.happenOnce = false;

                    if (WorldData.Instance.lastPickUpGO != null)
                    {
                        RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);
                    }
                    else
                    {
                        lastRelic.SetActive(true);
                    }

                    TpTest.Instance.tpPlayer(WorldData.Instance.pickUpCP);
                    WorldData.Instance.VorgonRealmPlayerDeath();
                }
                else if (other.gameObject.tag == "Player" && WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
                {
                    Debug.Log("Out of Bounds");
                    WorldData.Instance.PlayerDeathMortalRealm();                    
                    WorldData.Instance.happenOnce = false;                    
                }
                break;

            case TRIAL_OUTCOME.SUCCESS:
                if (other.gameObject.tag == "VorgonRealmPlayer" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
                {
                    Debug.Log("Vorgon Realm TP SUCCESS");
                    WorldData.Instance.happenOnce = false;
                    // Move this to after a trial is completed or failed

                    RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);

                    WorldData.Instance.MortalRealmController.AddItemToInventory(WorldData.Instance.lastPickUpGO.GetComponent<PickUp>());

                    TpTest.Instance.tpPlayer(WorldData.Instance.pickUpCP);
                    StartCoroutine(WaitForHealth());
                }
                break;


            case TRIAL_OUTCOME.SAFEZONE:
                if (other.gameObject.tag == "Player" && WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
                {
                    Debug.Log("SafeZoneEnter");
                    WorldData.Instance.PlayerSafeZone(true);
                }
                break;

            default:  
                
                break;
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
        {
            Debug.Log("SafeZoneExit");
            WorldData.Instance.PlayerSafeZone(false);
        }
    }

    IEnumerator WaitForHealth()
    {
        yield return new WaitForSeconds(3);
        endingPath?.Invoke();
    }
}
