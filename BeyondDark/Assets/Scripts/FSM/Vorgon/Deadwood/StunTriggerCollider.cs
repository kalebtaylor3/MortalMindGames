using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTriggerCollider : MonoBehaviour
{

    public TrapInteractable trap;

    private void OnEnable()
    {
        this.GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vorgon")
        {
            if (trap.bearTrap)
            {
                trap.animator.SetTrigger("Trigger");
            }
            other.GetComponent<VorgonController>().StunVorgon();
            this.GetComponent<Collider>().enabled = false;
        }
    }
}
