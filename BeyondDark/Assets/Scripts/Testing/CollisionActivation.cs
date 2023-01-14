using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionActivation : MonoBehaviour
{
    // Temporary collision to trigger realm tp from vorgons realm to the mortal realm
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "VorgonRealmPlayer" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
        {
            Debug.Log("Vorgon Realm TP");
            TpTest.Instance.tpPlayer();

        }
    }
}
