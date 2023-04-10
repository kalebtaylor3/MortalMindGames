using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MControlledGate : MonoBehaviour
{
    public ParticleSystem wallParticle;

    private void OnEnable()
    {
        wallParticle.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("VorgonRealmPlayer"))
        {
            if (GetComponentInParent<MSpawner>().CheckMinions())
            {
                StartCoroutine(TurnOffGate());
            }
        }
    }

    public IEnumerator TurnOffGate()
    {
        wallParticle.Stop();
        yield return null;
        this.gameObject.SetActive(false);
    }
}
