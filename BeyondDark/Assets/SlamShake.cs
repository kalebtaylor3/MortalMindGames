using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamShake : MonoBehaviour
{
    public void Shake()
    {
        CameraShake.Instance.ShakeCamera(5, 5, 1);
        Rumbler.Instance.RumbleConstant(0.5f, 2f, 1);
    }
}
