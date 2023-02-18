using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellFireDamage : MonoBehaviour
{
    public enum PROJECTILE_TYPE { BULLET = 0, ACID = 1 };

    public PROJECTILE_TYPE type = PROJECTILE_TYPE.BULLET;
    public float damage = 5;
    public float lifetime = 20;
    public GameObject impactPrefab;
    private bool Collided;

    private void OnEnable()
    {
        Destroy(gameObject, 20);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Minion" && collision.gameObject.tag != "MinionBullet" && !Collided)
        {
            Collided = true;

            if (collision.gameObject.tag == "VorgonRealmPlayer")
            {
                PlayerHealthSystem.Instance.currentPlayerHealth -= damage;
                PlayerHealthSystem.Instance.TakeDamage();
                //Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);
            }

            var impact = Instantiate(impactPrefab, collision.GetContact(0).point, Quaternion.identity) as GameObject;

            if (type == PROJECTILE_TYPE.BULLET)
            {
                Rumbler.Instance.RumbleConstant(0, 0.1f, 0.2f);
                //Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);
                Destroy(impact, 2.0f);
            }
            else if (type == PROJECTILE_TYPE.ACID)
            {
                //Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);                
                Destroy(impact, 5.0f);
            }


            Destroy(gameObject);


        }
    }
}
