using Cinemachine;
using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
            revealPercentage = Gamepad.current.rightStick.x.ReadValue();
        else
            revealPercentage = Gamepad.current.rightStick.y.ReadValue();

        if (revealPercentage < 0)
            revealPercentage = 0f;
    }

    public float GetRevealPercentage()
    {
        return revealPercentage;
    }
}
