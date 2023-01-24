using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTriggerCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vorgon")
        {
            other.GetComponent<VorgonController>().StunVorgon();
            this.GetComponent<Collider>().enabled = false;
        }
    }
}
