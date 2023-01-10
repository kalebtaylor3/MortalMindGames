using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddingCameraController : MonoBehaviour
{
    [SerializeField] CameraInputData inputData;
    private float revealPercentage = 0f;

    private void LateUpdate()
    {
        revealPercentage = inputData.InputVectorX;
        Debug.Log(revealPercentage);

        if (revealPercentage < 0)
            revealPercentage = 0f;
    }

    public float GetRevealPercentage()
    {
        return revealPercentage;
    }
}
