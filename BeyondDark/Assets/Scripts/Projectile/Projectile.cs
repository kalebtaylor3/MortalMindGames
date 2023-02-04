using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject impactPrefab;
    private bool Collided;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Player" && !Collided)
        {
            Collided = true;

            var impact = Instantiate(impactPrefab, collision.contacts[0].point, Quaternion.identity) as GameObject;
            

            Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);


            Destroy(impact,2.0f);
            Destroy(gameObject);

        }
    }

}
