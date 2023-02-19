using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTaunt : MonoBehaviour
{

    public VorgonBossController boss;

    public void Taunt()
    {
        boss.Taunt(true);
        boss.healthBar.SetTrigger("Start");
        Rumbler.Instance.RumbleConstant(0.5f, 1f, 1.5f);
        CameraShake.Instance.ShakeCamera(1, 1, 1.5f);
    }

    public void Vibration()
    {
        Rumbler.Instance.RumbleConstant(0.2f, 0.4f, 2.5f);
    }
}
