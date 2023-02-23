using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionActivation : MonoBehaviour
{
    enum TRIAL_OUTCOME { NONE = 0, FAIL = 1, SUCCESS = 2 }

    [SerializeField] TRIAL_OUTCOME EndType = TRIAL_OUTCOME.NONE;

    public Transform ogTpPos;

    // Temporary collision to trigger realm tp from vorgons realm to the mortal realm
    private void OnTriggerEnter(Collider other)
    {
        switch (EndType)
        {
            case TRIAL_OUTCOME.FAIL:
                if (other.gameObject.tag == "VorgonRealmPlayer" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
                {
                    // Checkpoint
                    Debug.Log("Vorgon Realm TP FAIL");

                    GameObject lastRelic = WorldData.Instance.lastPickUpGO;
                    WorldData.Instance.TriggerCheckpoint();

                    if (WorldData.Instance.lastPickUpGO != null)
                    {
                        RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);
                    }
                    else
                    {
                        lastRelic.SetActive(true);
                    }

                    TpTest.Instance.tpPlayer(WorldData.Instance.pickUpCP);
                }
                break;

            case TRIAL_OUTCOME.SUCCESS:
                if (other.gameObject.tag == "VorgonRealmPlayer" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
                {
                    Debug.Log("Vorgon Realm TP SUCCESS");
                    // Move this to after a trial is completed or failed
                    
                    RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);

                    WorldData.Instance.MortalRealmController.AddItemToInventory(WorldData.Instance.lastPickUpGO.GetComponent<PickUp>());

                    TpTest.Instance.tpPlayer(WorldData.Instance.pickUpCP);
                }
                break;

            default:                
                break;
        }

        
    }
}
