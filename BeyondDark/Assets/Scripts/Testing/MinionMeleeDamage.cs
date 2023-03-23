using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMeleeDamage : MonoBehaviour
{
    private bool attacked = false;
    private float damage = 5f;

    private void OnEnable()
    {
        attacked = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "VorgonRealmPlayer" && !attacked)
        {
            //Debug.Log("MeleeAttacked");
            attacked = true;
            PlayerHealthSystem.Instance.currentPlayerHealth -= damage;
            PlayerHealthSystem.Instance.TakeDamage();
            Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);
        }
    }
}
