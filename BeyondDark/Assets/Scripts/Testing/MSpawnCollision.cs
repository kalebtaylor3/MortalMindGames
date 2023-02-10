using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSpawnCollision : MonoBehaviour
{
    [SerializeField] GameObject minionsGO;

    private bool collided = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("VorgonRealmPlayer"))
        {
            minionsGO.SetActive(true);
        }
    }

}
