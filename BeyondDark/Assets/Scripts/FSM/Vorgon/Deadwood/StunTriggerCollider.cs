using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

public class StunTriggerCollider : MonoBehaviour
{

    public TrapInteractable trap;
    public float stunTime;
    public bool repeat;


    public List<MeshRenderer> meshMats;

    private void OnEnable()
    {
        this.GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vorgon" && trap.isArmed)
        {
            if (trap.bearTrap)
            {
                trap.animator.SetTrigger("Trigger");
            }
            other.GetComponent<VorgonController>().StunVorgon(stunTime);
            if (!repeat)
            {
                this.GetComponent<Collider>().enabled = false;

                for (int i = 0; i < meshMats.Count; i++)
                {
                    Destroy(meshMats[i].materials[1]);
                }
            }

        }
    }
}
