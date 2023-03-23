using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionDeath : MonoBehaviour
{
    public static event Action<MinionController> Death;
    public MinionController minion;


    public void MDeath()
    {
        if(minion!= null)
        {
            Death?.Invoke(minion);
        }        
    }
}
