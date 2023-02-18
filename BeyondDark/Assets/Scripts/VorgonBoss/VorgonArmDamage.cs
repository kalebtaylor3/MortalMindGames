using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonArmDamage : MonoBehaviour
{

    public float damage = 50;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "VorgonRealmPlayer")
        {
            PlayerHealthSystem.Instance.currentPlayerHealth -= damage;
            PlayerHealthSystem.Instance.TakeDamage();
        }
    }
}
