using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MControlledGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("VorgonRealmPlayer"))
        {
            if(GetComponentInParent<MSpawner>().CheckMinions())
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
