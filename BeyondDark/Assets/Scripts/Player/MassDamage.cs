using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MassDamage : MonoBehaviour
{
    public float damage = 1;

    public bool canDamagePlayer = true;

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.tag == "VorgonRealmPlayer" && canDamagePlayer)
    //    {
    //        //Debug.Log("MeleeAttacked");
    //        PlayerHealthSystem.Instance.currentPlayerHealth -= damage;
    //        PlayerHealthSystem.Instance.TakeDamage();
    //        Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);
    //    }
    //}

    private void OnParticleCollision(GameObject other)
    {

        //int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Debug.Log(other.name);

        if (other.gameObject.tag == "VorgonRealmPlayer" && canDamagePlayer)
        {
            Debug.Log("Spit hit");
            //Debug.Log("MeleeAttacked");
            PlayerHealthSystem.Instance.currentPlayerHealth -= damage;
            PlayerHealthSystem.Instance.TakeDamage();
            Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);
        }
    }
}
