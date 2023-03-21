using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatSystem : MonoBehaviour
{
    public AudioSource heartBeatSource;
    float distance;
    float minDistance = 10;
    float maxDistance = 2;
    public GameObject vorgon;

    private void Update()
    {

        Debug.Log(distance);

        if(vorgon != null)
        {
            distance = Vector3.Distance(transform.position, vorgon.transform.position);
        }

        
        heartBeatSource.pitch = (1 - Mathf.Clamp01(distance / minDistance)) * maxDistance;
        
    }
}
