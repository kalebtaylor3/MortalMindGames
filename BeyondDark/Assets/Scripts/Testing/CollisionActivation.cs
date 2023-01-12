using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionActivation : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
        {
            Debug.Log("Vorgon Realm TP");
            TpTest.Instance.tpPlayer();

        }
    }
}
