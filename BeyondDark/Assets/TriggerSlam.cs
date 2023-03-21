using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSlam : MonoBehaviour
{

    public static event Action OnEnter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OnEnter?.Invoke();
        }
    }
}
