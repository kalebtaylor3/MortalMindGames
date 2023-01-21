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
                // Checkpoint
                break;

            case TRIAL_OUTCOME.SUCCESS:
                if (other.gameObject.tag == "VorgonRealmPlayer" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
                {
                    Debug.Log("Vorgon Realm TP");
                    // Move this to after a trial is completed or failed
                    RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);
                    TpTest.Instance.tpPlayer(Vector3.zero);
                }
                break;

            default:
                WorldData.Instance.TriggerCheckpoint();
                RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);
                TpTest.Instance.tpPlayer(Vector3.zero);
                break;
        }

        
    }
}
