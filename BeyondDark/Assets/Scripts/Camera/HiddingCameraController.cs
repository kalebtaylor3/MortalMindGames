using Cinemachine;
using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddingCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    [SerializeField] CameraInputData inputData;
    private float revealPercentage = 0f;
    public enum clamp { X, Y };
    public clamp cameraClamp;

    private void LateUpdate()
    {
        if (cameraClamp == clamp.X)
            revealPercentage = inputData.InputVectorX;
        else
            revealPercentage = inputData.InputVectorY;

        if (revealPercentage < 0)
            revealPercentage = 0f;
    }

    public float GetRevealPercentage()
    {
        return revealPercentage;
    }
}
