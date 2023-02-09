using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MProjectile : MonoBehaviour
{
    public float damage = 5;
    public GameObject impactPrefab;
    public MinionController minionControl = null;
    private bool Collided;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Minion" && !Collided)
        {
            Collided = true;

            var impact = Instantiate(impactPrefab, collision.contacts[0].point, Quaternion.identity) as GameObject;


            Rumbler.Instance.RumbleConstant(0, 0.2f, 0.4f);


            Destroy(impact, 2.0f);
            Destroy(gameObject);

            if (collision.gameObject.CompareTag("WallOfSouls"))
            {
                minionControl.FoundWallOfSouls(collision.gameObject);
            }
        }
    }
}
