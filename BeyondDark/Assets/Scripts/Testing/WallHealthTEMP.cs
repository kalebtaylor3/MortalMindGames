using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHealthTEMP : MonoBehaviour
{
    public float health = 15;
    public bool alive = true;

    public Transform ShotPos = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            ReceiveDamage(other.GetComponent<Projectile>().damage);
        }
        else if (other.gameObject.CompareTag("MinionBullet"))
        {
            ReceiveDamage(other.GetComponent<MProjectile>().damage, other.GetComponent<MProjectile>().minionControl);
        }
           
    }


    public void ReceiveDamage(float amount, MinionController minion = null)
    {
        health -= amount;

        if(health <= 0)
        {
            health = 0;
            StartCoroutine(WallDeath(minion));
        }
    }

    IEnumerator WallDeath(MinionController minion)
    {
        alive = false;
        
        
        yield return new WaitForSeconds(1.5f);

        if(minion!= null) 
        {
            minion.WallDeath();
        }
        

        Destroy(gameObject);

    }
}
