using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventZone : MonoBehaviour
{
    
    public UnityEngine.Events.UnityEvent onTriggerEnterEvent;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "VorgonRealmPlayer")
        {
            onTriggerEnterEvent.Invoke();
        }
    }
}