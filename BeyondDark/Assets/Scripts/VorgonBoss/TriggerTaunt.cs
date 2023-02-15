using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTaunt : MonoBehaviour
{

    public VorgonBossController boss;

    public void Taunt()
    {
        boss.Taunt();
    }
}
