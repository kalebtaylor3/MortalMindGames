using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonProjectile : MonoBehaviour
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
        Physics.IgnoreLayerCollision(13,14);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Minion" && collision.gameObject.tag != "MinionBullet" && !Collided)
        {
            Collided = true;

            var impact = Instantiate(impactPrefab, collision.GetContact(0).point, Quaternion.identity) as GameObject;

            Rumbler.Instance.RumbleConstant(0.5f, 1f, 0.6f);
            CameraShake.Instance.ShakeCamera(2, 2, 0.6f);
            Destroy(impact, 2.0f);


            Destroy(gameObject);


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "VorgonRealmPlayer")
        {
            PlayerHealthSystem.Instance.currentPlayerHealth -= damage;
            PlayerHealthSystem.Instance.TakeDamage();
        }
    }
}
