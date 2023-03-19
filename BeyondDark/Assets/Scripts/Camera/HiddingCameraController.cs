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
    [HideInInspector] public float revealPercentage = 0f;
    public enum clamp { X, Y };
    public clamp cameraClamp;

    [HideInInspector] public float leftRightPercentage = 0f;

    [HideInInspector] public CinemachineRecomposer comp;

    private void Awake()
    {
        comp = GetComponent<CinemachineRecomposer>();
    }

    private void LateUpdate()
    {
        if (cameraClamp == clamp.X)
            revealPercentage = Gamepad.current.rightStick.x.ReadValue();
        else
            revealPercentage = Gamepad.current.rightStick.y.ReadValue();

        leftRightPercentage = Gamepad.current.leftStick.x.ReadValue();

        if (revealPercentage < 0)
            revealPercentage = 0f;
    }

    public float GetRevealPercentage()
    {
        return revealPercentage;
    }
}
